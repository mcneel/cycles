<#
.SYNOPSIS
  Build Cycles only if the Cycles sources differ from the ones the prebuilt
  payload was made from. Called by ccycles.vcxproj on every build.

.DESCRIPTION
  The whole decision lives here so that nobody has to know a switch exists.
  Change Cycles, build Rhino with either RhinoBuilder or Visual Studio, and
  ccycles and its kernels are regenerated. Change nothing and nothing happens -
  exactly as in Rhino 9.x, where Cycles was never built from a normal solution
  build at all.

  How the comparison works, and why not the obvious way:

    Timestamps cannot be used. git sets mtime to checkout time, so on a fresh
    clone the sources and the payload get the same mtime in an arbitrary order,
    and "is the payload stale" becomes a coin flip. Walking the tree is also not
    free - 891 source files.

    So the payload carries a fingerprint of the source it was built from, in
    ccycles.stamp beside ccycles.dll, committed with the payload. The fingerprint
    is content-derived: the cycles commit, plus a hash of any local
    modifications, so it is identical on every machine at the same revision and
    changes the moment anyone edits Cycles.

  Outcomes, in order:

    fingerprint matches       do nothing, say so once
    no CMake or Ninja         say the payload is stale and carry on. A developer
                              who cannot build Cycles is never blocked by it, and
                              this is the case that means somebody changed Cycles
                              without committing a rebuilt payload
    otherwise                 build, install, and write the new stamp

  RHINOCYCLESDEV is still honoured as an escape hatch - 1 forces a build, 0
  forbids one - but nothing in normal use needs it.

  -DecideOnly reports the decision and builds nothing, which is how the decision
  can be inspected without waiting for an hour of kernels.

  Exit code is 0 unless a build was actually attempted and failed, so an
  undecidable situation never breaks someone else's build.
#>
param(
  [Parameter(Mandatory = $true)][string]$Configuration,
  [Parameter(Mandatory = $true)][string]$InstallDir,
  [string]$BuildDir,
  [switch]$DecideOnly,
  [switch]$PrintFingerprint
)

$ErrorActionPreference = 'Stop'

$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$cyclesDir = Split-Path -Parent $toolsDir
$stampFile = Join-Path $InstallDir 'ccycles.stamp'

function Say($msg) { Write-Host "ccycles: $msg" }

function Test-Tool($name) {
  $cmd = Get-Command $name -ErrorAction SilentlyContinue
  return $null -ne $cmd
}

# Everything that can change what ccycles.dll or a kernel comes out as. Kept
# deliberately narrow: docs and tools must not trigger an hour of kernels.
$sourcePaths = @('src', 'CMakeLists.txt', 'build_cycles.ps1', 'cmake')

function Get-SourceFingerprint {
  # Returns a string, or $null when it cannot be determined - in which case the
  # caller must assume nothing and leave the payload alone.
  if (-not (Test-Tool 'git')) { return $null }

  # git writes warnings to stderr - "LF will be replaced by CRLF" on any dirty
  # file here - and with ErrorActionPreference Stop, PowerShell turns native
  # stderr into a terminating error. That made a dirty tree, the one case this
  # exists to detect, report itself as undeterminable.
  $previousPreference = $ErrorActionPreference
  $ErrorActionPreference = 'SilentlyContinue'

  Push-Location $cyclesDir
  try {
    $head = (& git rev-parse HEAD 2>$null)
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($head)) { return $null }

    # Local modifications, so a developer's own edits count without needing a
    # commit. The diff is hashed rather than stored; untracked files are listed
    # by status and hashed individually, because a new .cpp is a real change.
    $existing = @($sourcePaths | Where-Object { Test-Path (Join-Path $cyclesDir $_) })
    $parts = New-Object System.Collections.Generic.List[string]
    $parts.Add($head.Trim())

    if ($existing.Count -gt 0) {
      $diff = (& git diff HEAD -- $existing 2>$null) -join "`n"
      $parts.Add($diff)

      $status = @(& git status --porcelain --untracked-files=all -- $existing 2>$null)
      foreach ($line in ($status | Sort-Object)) {
        $parts.Add($line)
        if ($line -match '^\?\?\s+(.*)$') {
          $untracked = Join-Path $cyclesDir $Matches[1].Trim('"')
          if (Test-Path -PathType Leaf $untracked) {
            $parts.Add((& git hash-object $untracked 2>$null))
          }
        }
      }
    }

    $bytes = [System.Text.Encoding]::UTF8.GetBytes(($parts -join "`n"))
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
      return ($sha.ComputeHash($bytes) | ForEach-Object { $_.ToString('x2') }) -join ''
    } finally {
      $sha.Dispose()
    }
  } catch {
    # Never swallow the reason. A silent "cannot tell" sent me looking for a
    # missing git once when the real fault was here.
    Say "could not fingerprint the Cycles sources: $($_.Exception.Message)"
    return $null
  } finally {
    Pop-Location
    $ErrorActionPreference = $previousPreference
  }
}

if ($PrintFingerprint) {
  $fp = Get-SourceFingerprint
  if ($null -eq $fp) { Write-Host '<undeterminable>'; exit 1 }
  Write-Host $fp
  exit 0
}

# ---------------------------------------------------------------- escape hatches
$force = $false
if ($env:RHINOCYCLESDEV -match '^\s*(0|false|no|off)\s*$') {
  Say 'source builds disabled by RHINOCYCLESDEV, using the prebuilt payload.'
  exit 0
} elseif (-not [string]::IsNullOrEmpty($env:RHINOCYCLESDEV)) {
  Say "RHINOCYCLESDEV is set, building Cycles from source whatever the stamp says."
  $force = $true
}

# ------------------------------------------------------------------- the decision
$fingerprint = Get-SourceFingerprint

if (-not $force) {
  if ($null -eq $fingerprint) {
    # No git, no answer. Saying nothing would be worse than a single line.
    Say 'cannot tell whether Cycles changed, so using the prebuilt payload.'
    exit 0
  }

  $stamped = if (Test-Path $stampFile) { (Get-Content -Raw $stampFile).Trim() } else { '' }
  if ($stamped -eq $fingerprint) {
    Say 'Cycles is unchanged, using the prebuilt payload.'
    exit 0
  }

  if (-not (Test-Path $stampFile)) {
    Say 'the prebuilt payload carries no stamp, so it cannot be compared. Treating it as current.'
    Say 'A Cycles developer will produce one the next time the payload is rebuilt.'
    exit 0
  }

  $missing = @('cmake', 'ninja') | Where-Object { -not (Test-Tool $_) }
  if ($missing.Count -gt 0) {
    Say "Cycles sources differ from the prebuilt payload, but $($missing -join ' and ') is not installed here."
    Say 'Using the payload as it is. Whoever changed Cycles needs to commit a rebuilt one.'
    exit 0
  }

  Say 'Cycles sources changed, rebuilding. Only what changed is recompiled; new kernels take a while.'
}

if ($DecideOnly) {
  Say 'would build (decide-only, nothing done).'
  exit 0
}

# ---------------------------------------------------------------------- the build
$script = Join-Path $cyclesDir 'build_cycles.ps1'
$buildArgs = @('-Configuration', $Configuration, '-InstallDir', $InstallDir)
if (-not [string]::IsNullOrEmpty($BuildDir)) { $buildArgs += @('-BuildDir', $BuildDir) }

& $script @buildArgs
if ($LASTEXITCODE -ne 0) {
  Say "build_cycles.ps1 failed with exit code $LASTEXITCODE."
  exit $LASTEXITCODE
}

# Stamp last, and only on success, so a failed build stays stale and is retried.
if ($null -ne $fingerprint) {
  Set-Content -Path $stampFile -Value $fingerprint -NoNewline
  Say "stamped the payload with $($fingerprint.Substring(0, 12))."
} else {
  Say 'built, but could not compute a fingerprint, so no stamp was written.'
}

exit 0
