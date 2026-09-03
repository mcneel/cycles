<#
.SYNOPSIS
  Run every check this port has: the static audits, and optionally the
  golden-image render regression.

.DESCRIPTION
  One entry point, because the checks are only useful if they are cheap to run
  and nobody remembers four commands. Two tiers:

    Static audits    No build, no Rhino, about a second. These catch the five
                     ways the 3.5 -> 5.2 port has silently drifted: a renamed or
                     retyped socket, a renumbered enum, a stock SVM node
                     emitted in Rhino's packed layout, a parameter exposed as
                     both a member and a socket, and an interpreter case that
                     falls through into the next one. Every one of those
                     compiles, links, asserts nothing and renders the wrong
                     pixels, which is why they are worth a second.

    Render           Needs a built Rhino and takes minutes. A golden-image
                     comparison of real renders, which is the only check that
                     would have caught the texture coordinate node writing to
                     the wrong SVM stack slot and turning every render black.

  Static audits run by default. Add -Render for the second tier, or -RenderOnly
  to skip the audits.

  Exit code is 0 only if everything that ran passed, so this can gate a build.

.EXAMPLE
  # The cheap tier
  powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1

.EXAMPLE
  # Everything, including renders
  powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1 -Render
#>
param(
  [switch]$Render,
  [switch]$RenderOnly,
  [string]$Exe = '',
  [double]$Tolerance = 1.0
)

$ErrorActionPreference = 'Continue'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repo = Split-Path -Parent $toolsDir
$results = @()

function Add-Result([string]$name, [int]$code, [string]$note) {
  $script:results += [pscustomobject]@{ Name = $name; Code = $code; Note = $note }
}

if (-not $RenderOnly) {
  $python = (Get-Command python -ErrorAction SilentlyContinue).Source
  if (-not $python) { $python = (Get-Command py -ErrorAction SilentlyContinue).Source }
  if (-not $python) {
    Write-Host 'python not found - skipping the static audits'
    Add-Result 'static audits' 2 'python not on PATH'
  }
  else {
    foreach ($audit in 'audit_enums', 'audit_sockets', 'audit_svm_nodes', 'audit_rhino_stock_sockets', 'audit_member_socket_clash', 'audit_svm_dispatch') {
      $script = Join-Path $toolsDir "$audit.py"
      if (-not (Test-Path $script)) { Add-Result $audit 2 'missing'; continue }
      Write-Host "--- $audit"
      $out = & $python $script 2>&1
      $code = $LASTEXITCODE
      # The audits end with a one-line summary; show it, and everything if it failed.
      if ($code -eq 0) { $out | Select-Object -Last 2 | ForEach-Object { "  $_" } }
      else { $out | ForEach-Object { "  $_" } }
      Add-Result $audit $code ''
    }
  }
}

if (-not $RenderOnly) {
  # Is the committed payload built from these kernel sources?
  #
  # The audits above catch a kernel that is wrong. This catches a kernel that is
  # right and not shipped: change kernel code, merge without republishing, and
  # everyone on a plain build gets the new ccycles.dll with the old kernels. That
  # is not slow but wrong - an SVM renumbering means every render is garbage - and
  # nothing else notices, because the build succeeds and the audits pass.
  #
  # A failure here rather than a warning, unlike the same comparison in
  # build_cycles.ps1. That one fires on every build while iterating, so it can only
  # inform; this runs when someone deliberately asks whether the tree is ready, and
  # its exit code is meant to gate. Same fact, two severities, chosen by when it is
  # being asked.
  Write-Host '--- payload freshness'
  $arches = Join-Path $repo 'kernel_arches.ps1'
  # cycles -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
  $manifest = Join-Path $repo '..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\ccycles_payload.json'

  if (-not (Test-Path $arches)) {
    Write-Host '  kernel_arches.ps1 not found'
    Add-Result 'payload freshness' 2 'kernel_arches.ps1 missing'
  }
  elseif (-not (Test-Path $manifest)) {
    # A payload published before manifests existed, or a checkout without big_libs.
    # Not a failure: there is nothing to compare against, and saying so is the
    # useful part.
    Write-Host '  no ccycles_payload.json in the committed payload - nothing to compare'
    Add-Result 'payload freshness' 0 'no manifest'
  }
  else {
    . $arches
    $recorded = (Get-Content -LiteralPath $manifest -Raw | ConvertFrom-Json).kernelSourceHash
    $current = Get-CyclesKernelSourceHash -CyclesRoot $repo

    if (-not $recorded) {
      Write-Host '  the payload manifest records no kernel source hash'
      Add-Result 'payload freshness' 0 'manifest has no hash'
    }
    elseif ($recorded -eq $current) {
      Write-Host "  payload matches the kernel sources ($($current.Substring(0,16)))"
      Add-Result 'payload freshness' 0 ''
    }
    else {
      Write-Host "  payload was built from different kernel sources"
      Write-Host "    payload: $($recorded.Substring(0,16))"
      Write-Host "    tree:    $($current.Substring(0,16))"
      Write-Host "  Run publish_payload.ps1 and commit the payload, or this change ships"
      Write-Host "  a new ccycles.dll with the previously published kernels."
      Add-Result 'payload freshness' 1 'republish needed'
    }
  }
}

if (-not $RenderOnly) {
  # Does the installer ship the kernels we build?
  #
  # Worth a second because it was wrong in the worst way: Cycles.wxs listed 32 kernel
  # files, every one an uncompressed name from before Cycles compressed them, while all
  # 41 the payload ships were absent. The runtime only opens the .zst form, so a shipped
  # Rhino had no precompiled kernels at all - and nothing noticed, because working trees
  # still hold leftover uncompressed files and the MSI kept building.
  #
  # Skipped rather than failed when the installer is not there: this repository is also
  # used standalone, without the Rhino tree around it.
  $installerCheck = Join-Path $repo '..\..\..\..\..\installer\msi\Features\Plug-ins\update_cycles_kernels.ps1'
  if (-not (Test-Path $installerCheck)) {
    Write-Host '--- installer kernel list'
    Write-Host '  no Rhino installer tree here - skipped'
    Add-Result 'installer kernel list' 0 'no installer tree'
  }
  else {
    Write-Host '--- installer kernel list'
    $out = & powershell -NoProfile -ExecutionPolicy Bypass -File $installerCheck -Check 2>&1
    $code = $LASTEXITCODE
    $out | ForEach-Object { "  $_" }
    Add-Result 'installer kernel list' $code $(if ($code -ne 0) { 'regenerate needed' } else { '' })
  }
}

if ($Render -or $RenderOnly) {
  $script = Join-Path $toolsDir 'render_regression.ps1'
  if (-not (Test-Path $script)) { Add-Result 'render regression' 2 'missing' }
  else {
    Write-Host '--- render regression'
    $rargs = @('-ExecutionPolicy', 'Bypass', '-File', $script, '-Tolerance', $Tolerance)
    if ($Exe) { $rargs += @('-Exe', $Exe) }
    & powershell @rargs
    Add-Result 'render regression' $LASTEXITCODE ''
  }
}

Write-Host ''
Write-Host 'summary'
$failed = 0
foreach ($r in $results) {
  $status = if ($r.Code -eq 0) { 'ok  ' } else { 'FAIL' }
  if ($r.Code -ne 0) { $failed++ }
  $note = if ($r.Note) { "  ($($r.Note))" } else { '' }
  Write-Host ("  $status $($r.Name)$note")
}
if ($results.Count -eq 0) { Write-Host '  nothing ran'; exit 2 }
if ($failed) { Write-Host ''; Write-Host "$failed of $($results.Count) check(s) failed"; exit 1 }
Write-Host ''
Write-Host "$($results.Count) check(s) ok"
exit 0
