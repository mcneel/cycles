#requires -Version 5.1
<#
.SYNOPSIS
    Checks that lib\windows_x64 is the library bundle this repository pins, with real
    files rather than Git LFS pointers.

.DESCRIPTION
    Another bundle can silently link Cycles against the wrong library versions.
    Recognises the submodule itself (lib\windows_x64\.git, as make_update.py makes it)
    and junctions into a shared sibling lib-windows_x64 clone, whose HEAD is compared.

    Exit codes, so build_cycles.ps1 and run_checks.ps1 can act on them:
      0  bundle matches the pin
      1  bundle is a different commit than the pin           (the LOUD case)
      2  bundle files are LFS pointers that were never pulled
      3  cannot tell (not a git checkout, unknown layout) - reported, not failed
      4  lib\windows_x64 is missing or empty - the caller fetches it

.PARAMETER CyclesRoot
    The cycles-core checkout. Defaults to the parent of this script's folder.

.PARAMETER Quiet
    Print only on a problem.

.EXAMPLE
    powershell -File tools\check_lib_bundle.ps1
#>
[CmdletBinding()]
param(
    [string]$CyclesRoot,
    [switch]$Quiet
)

# Continue, not Stop: under 5.1, Stop turns git's redirected stderr into a terminating
# error. Every result is checked explicitly instead.
$ErrorActionPreference = 'Continue'
if (-not $CyclesRoot) {
    $here = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
    $CyclesRoot = (Resolve-Path (Join-Path $here '..')).Path
}
$libDir = Join-Path $CyclesRoot 'lib\windows_x64'

# Loud on purpose: the one case where the build is silently wrong rather than broken.
function Write-Loud([string[]]$lines) {
    $width = 78
    Write-Host ''
    Write-Host (' ' * $width) -BackgroundColor DarkRed
    Write-Host ('  LIBRARY BUNDLE MISMATCH' + (' ' * ($width - 25))) -ForegroundColor White -BackgroundColor DarkRed
    Write-Host (' ' * $width) -BackgroundColor DarkRed
    foreach ($l in $lines) { Write-Host "  $l" -ForegroundColor Red }
    Write-Host ''
}

function Write-Note([string]$msg) { if (-not $Quiet) { Write-Host "   $msg" -ForegroundColor DarkGray } }

# ------------------------------------------------------------------ presence

if (-not (Test-Path -LiteralPath $libDir -PathType Container) -or
    -not (Get-ChildItem -LiteralPath $libDir -Force | Select-Object -First 1)) {
    if (-not $Quiet) { Write-Host "   libraries missing at $libDir" -ForegroundColor DarkYellow }
    exit 4
}

# ------------------------------------------------------------------ the pin

$recorded = $null
try {
    $line = & git -C $CyclesRoot ls-tree HEAD lib/windows_x64 2>$null
    if ($line -match '\b([0-9a-f]{40})\b') { $recorded = $Matches[1] }
} catch { }
if (-not $recorded) {
    if (-not $Quiet) { Write-Host "   cannot read the pinned bundle commit (is $CyclesRoot a git checkout?) - bundle not verified" -ForegroundColor DarkYellow }
    exit 3
}

# ------------------------------------------------------------ what is checked out

$actual = $null
$layout = $null
$cloneDir = $null

if (Test-Path -LiteralPath (Join-Path $libDir '.git')) {
    $layout = 'submodule'
    $cloneDir = $libDir
}
else {
    # Junction layout: any entry's target, one level up, is the sibling clone.
    $first = Get-ChildItem -LiteralPath $libDir -Force | Where-Object { $_.Attributes -band [IO.FileAttributes]::ReparsePoint } | Select-Object -First 1
    if ($first) {
        $target = @($first.Target)[0]
        if ($target) {
            $candidate = Split-Path -Parent $target
            if (Test-Path -LiteralPath (Join-Path $candidate '.git')) {
                $layout = 'junctions'
                $cloneDir = $candidate
            }
        }
    }
}

if ($cloneDir) {
    try { $actual = (& git -C $cloneDir rev-parse HEAD 2>$null).Trim() } catch { }
}
if (-not $actual -or $actual -notmatch '^[0-9a-f]{40}$') {
    if (-not $Quiet) { Write-Host "   cannot tell which bundle commit $libDir holds (layout: $(if ($layout) { $layout } else { 'unknown' })) - bundle not verified" -ForegroundColor DarkYellow }
    exit 3
}

# ------------------------------------------------------------------ LFS pointers

# A handful of binaries that every bundle has. An LFS pointer is a ~130-byte text
# file starting with the spec line; a real library is never that small.
$probes = @(
    'tbb\lib\tbb12.lib', 'embree\lib\embree4.lib', 'openimageio\lib\OpenImageIO.lib',
    'openvdb\lib\openvdb.lib', 'zstd\lib\zstd_static.lib'
) | ForEach-Object { Join-Path $libDir $_ } | Where-Object { Test-Path -LiteralPath $_ }
$pointers = @()
foreach ($p in $probes) {
    $item = Get-Item -LiteralPath $p
    if ($item.Length -lt 400) {
        $head = Get-Content -LiteralPath $p -TotalCount 1 -ErrorAction SilentlyContinue
        if ("$head" -like 'version https://git-lfs*') { $pointers += $p }
    }
}

# ------------------------------------------------------------------ verdict

$short = { param($s) $s.Substring(0, 9) }

if ($actual -ne $recorded) {
    $fix = if ($layout -eq 'submodule') {
        @("cd `"$CyclesRoot`"", "python src\cmake\make_update.py --no-cycles")
    } else {
        @("git -C `"$cloneDir`" fetch origin", "git -C `"$cloneDir`" checkout $recorded", "git -C `"$cloneDir`" lfs pull")
    }
    Write-Loud (@(
        "This tree pins Blender library bundle  $(& $short $recorded)",
        "but lib\windows_x64 holds               $(& $short $actual)   ($layout$(if ($layout -eq 'junctions') { " into $cloneDir" }))",
        "",
        "Everyone builds against the pinned bundle. Building against another one",
        "links Cycles to the wrong library versions and nothing else will tell you.",
        "",
        "To fix:"
    ) + ($fix | ForEach-Object { "    $_" }) + @(
        "",
        "If the pin itself is what should move, change the lib/windows_x64 submodule",
        "commit in this repository and republish the payload."
    ))
    exit 1
}

if ($pointers.Count) {
    $fix = if ($layout -eq 'submodule') { "git -C `"$libDir`" lfs pull" } else { "git -C `"$cloneDir`" lfs pull" }
    Write-Loud (@(
        "lib\windows_x64 is at the pinned bundle $(& $short $recorded), but these are Git LFS",
        "pointers, not libraries - the LFS objects were never downloaded:"
    ) + ($pointers | ForEach-Object { "    $_" }) + @("", "To fix:", "    $fix"))
    exit 2
}

if (-not $Quiet) {
    Write-Host ("   {0,-12} {1} ({2}{3})" -f 'bundle', (& $short $recorded), $layout, $(if ($layout -eq 'junctions') { " into $cloneDir" })) -ForegroundColor Green
}
exit 0
