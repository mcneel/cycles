#requires -Version 5.1
<#
.SYNOPSIS
    Stamps version resources and side-by-side manifests onto the prebuilt
    third-party DLLs in the Cycles install tree.

.DESCRIPTION
    ccycles.dll no longer needs this: its VERSIONINFO and its SxS private
    assembly manifest are compiled in at link time by src/ccycles/CMakeLists.txt.

    What remains are binaries we ship but do not build - openvdb.dll and the
    oneAPI JIT DLL come out of the precompiled Blender library bundle, so the
    only way to attach resources to them is after the fact. That is what this
    script still does.

    Requires ResourceHacker on PATH (http://angusj.com/resourcehacker/).
    Run it against a populated Cycles install tree.
#>
[CmdletBinding()]
param(
    [string]$RhinoBranchName,

    # Cycles install tree to operate on.
    [string]$InstallDir
)

$ErrorActionPreference = 'Stop'

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
. (Join-Path $scriptRoot "rhino_branch_info.ps1")

if (-not $InstallDir) { $InstallDir = Join-Path $scriptRoot "install" }

if (-not (Test-Path $InstallDir)) {
    throw "Cycles install tree not found at '$InstallDir'. Build Cycles first, or pass -InstallDir."
}

if (-not (Get-Command ResourceHacker -ErrorAction SilentlyContinue)) {
    throw "ResourceHacker is not on PATH. Install it from http://angusj.com/resourcehacker/ and re-run."
}

$branchInfo = Resolve-RhinoBranchInfo -StartPath $scriptRoot -RhinoBranchName $RhinoBranchName

$bd = [System.DateTime]::UtcNow
$dotted = "{0}.0.{1}{2}.{3}{4}1" -f `
    $branchInfo.MajorVersion,
    $bd.ToString("yy"),
    $bd.DayOfYear.ToString("D3"),
    $bd.ToString("HH"),
    $bd.Minute.ToString("D2")
$commas = $dotted.Replace(".", ",")

Write-Host "-> branch: $($branchInfo.BranchName) (major $($branchInfo.MajorVersion), source $($branchInfo.Source))"
Write-Host "-> version: $dotted"
Write-Host "-> install: $InstallDir"

function Expand-Template([string]$Template, [string]$Destination) {
    (Get-Content $Template -Raw).Replace("VERSIONCOMMAS", $commas).Replace("VERSIONDOTS", $dotted) |
        Set-Content $Destination -NoNewline
}

Expand-Template (Join-Path $scriptRoot "dll_version_replace.template")      (Join-Path $InstallDir "dll_version_replace.rc")
Expand-Template (Join-Path $scriptRoot "openvdb_manifest_replace.template") (Join-Path $InstallDir "openvdb_manifest_replace.rc")
Expand-Template (Join-Path $scriptRoot "openvdb_manifest.txt")              (Join-Path $InstallDir "openvdb_manifest.txt")

Push-Location $InstallDir
try {
    # Install-tree hygiene: strip debug and non-Windows artifacts that the
    # library bundle drags along.
    foreach ($pattern in '*gyd*', '*_d.dll', '*_d_*.dll', '*.so', '*.so.*', 'lib\*.so', 'lib\*.so.*') {
        Remove-Item $pattern -ErrorAction SilentlyContinue
    }

    & ResourceHacker -open .\dll_version_replace.rc      -save .\dll_version_replace.res      -action compile
    & ResourceHacker -open .\openvdb_manifest_replace.rc -save .\openvdb_manifest_replace.res -action compile

    # openvdb.dll: SxS assembly manifest so it resolves tbb.dll within the
    # Cycles assembly rather than picking up Rhino's copy.
    & ResourceHacker -open openvdb.dll -save openvdb.dll `
        -resource .\openvdb_manifest_replace.res -action addoverwrite -mask "MANIFEST,,"

    # oneAPI JIT DLL: version stamp only.
    if (Test-Path .\cycles_kernel_oneapi_jit.dll) {
        & ResourceHacker -open cycles_kernel_oneapi_jit.dll -save cycles_kernel_oneapi_jit.dll `
            -resource .\dll_version_replace.res -action addoverwrite -mask "VERSIONINFO,,"
    }
}
finally {
    Pop-Location
}

Write-Host "ready"
