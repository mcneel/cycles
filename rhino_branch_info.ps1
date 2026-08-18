function Get-RhinoBranchNameCandidate {
    param([AllowNull()][string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $trimmedValue = $Value.Trim()
    $match = [System.Text.RegularExpressions.Regex]::Match($trimmedValue, '(^|[\\/])(?<branch>\d+\.x)($|[\\/])')
    if (-not $match.Success) {
        return $null
    }

    return $match.Groups["branch"].Value
}

function Get-RhinoRepoRoot {
    param([Parameter(Mandatory = $true)][string]$StartPath)

    $resolvedStartPath = [System.IO.Path]::GetFullPath($StartPath)
    $match = [System.Text.RegularExpressions.Regex]::Match(
        $resolvedStartPath,
        '^(?<root>.+?)[\\/]src4[\\/]rhino4[\\/]Plug-ins[\\/]RDK[\\/]cycles(?:[\\/]|$)',
        [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
    )
    if ($match.Success) {
        return [System.IO.Path]::GetFullPath($match.Groups["root"].Value)
    }

    return $null
}

function Invoke-GitString {
    param(
        [Parameter(Mandatory = $true)][string]$WorkingDirectory,
        [Parameter(Mandatory = $true)][string[]]$Arguments
    )

    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        return $null
    }

    if (-not (Test-Path $WorkingDirectory)) {
        return $null
    }

    $output = & git -C $WorkingDirectory @Arguments 2>$null
    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    return (($output | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) -join [System.Environment]::NewLine).Trim()
}

function Resolve-RhinoBranchInfo {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)][string]$StartPath,
        [string]$RhinoBranchName
    )

    $resolvedStartPath = [System.IO.Path]::GetFullPath($StartPath)
    $rhinoRepoRoot = Get-RhinoRepoRoot -StartPath $resolvedStartPath
    $gitWorkingDirectory = if ($rhinoRepoRoot) { $rhinoRepoRoot } else { $resolvedStartPath }
    $gitRoot = Invoke-GitString -WorkingDirectory $gitWorkingDirectory -Arguments @("rev-parse", "--show-toplevel")
    if ($gitRoot) {
        $gitRoot = [System.IO.Path]::GetFullPath($gitRoot)
    }

    $resolvedBranchName = $null
    $source = $null

    if ($PSBoundParameters.ContainsKey("RhinoBranchName") -and -not [string]::IsNullOrWhiteSpace($RhinoBranchName)) {
        $resolvedBranchName = Get-RhinoBranchNameCandidate -Value $RhinoBranchName
        if (-not $resolvedBranchName) {
            throw "Invalid Rhino branch override '$RhinoBranchName'. Expected something like '8.x' or '9.x', or a branch path containing it."
        }
        $source = "parameter"
    }

    if (-not $resolvedBranchName -and $rhinoRepoRoot) {
        $resolvedBranchName = Get-RhinoBranchNameCandidate -Value (Split-Path -Leaf $rhinoRepoRoot)
        $source = "path"
    }

    if (-not $resolvedBranchName) {
        $gitBranch = Invoke-GitString -WorkingDirectory $gitWorkingDirectory -Arguments @("branch", "--show-current")
        $resolvedBranchName = Get-RhinoBranchNameCandidate -Value $gitBranch
        if ($resolvedBranchName) {
            $source = "git"
        }
    }

    if (-not $resolvedBranchName) {
        $envBranchName = Get-RhinoBranchNameCandidate -Value $env:RHINO_BRANCH_NAME
        if ($envBranchName) {
            $resolvedBranchName = $envBranchName
            $source = "env:RHINO_BRANCH_NAME"
        }
    }

    if (-not $resolvedBranchName) {
        $startLeaf = Split-Path -Leaf $resolvedStartPath
        throw "Could not determine Rhino branch major version from '$startLeaf'. Expected an ancestor folder or git branch containing '8.x' or '9.x'. You can pass -RhinoBranchName 8.x to override."
    }

    $majorVersionMatch = [System.Text.RegularExpressions.Regex]::Match($resolvedBranchName, '^(?<major>\d+)\.x$')
    if (-not $majorVersionMatch.Success) {
        throw "Resolved Rhino branch '$resolvedBranchName' is invalid. Expected format like '8.x' or '9.x'."
    }

    $resolvedBranchRoot = if ($rhinoRepoRoot) { $rhinoRepoRoot } elseif ($gitRoot) { $gitRoot } else { $resolvedStartPath }

    [PSCustomObject]@{
        BranchName   = $resolvedBranchName
        BranchRoot   = $resolvedBranchRoot
        MajorVersion = $majorVersionMatch.Groups["major"].Value
        Source       = $source
        GitRoot      = $gitRoot
    }
}
