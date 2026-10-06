<#
.SYNOPSIS
  Run the static checks: no build, no Rhino, a few seconds.

.DESCRIPTION
  - The audits catch the ways a Cycles merge silently breaks Rhino's layer: a renamed
    or retyped socket, a renumbered enum, a stock SVM node emitted in Rhino's packed
    layout, a parameter exposed as both a member and a socket, a write to a socket
    csycles has retired, an SVM interpreter case that falls through. Each of those
    compiles and renders wrong pixels.
  - Payload freshness: the committed payload was built from these kernel sources.
  - Library bundle: lib\windows_x64 is the pinned bundle, with its LFS objects.
  - Installer kernel list: Cycles.wxs lists the kernels the payload ships.

  Exit code is 0 only if everything passed, so this can gate a build.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1
#>

$ErrorActionPreference = 'Continue'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repo = Split-Path -Parent $toolsDir
$results = @()

function Add-Result([string]$name, [int]$code, [string]$note) {
  $script:results += [pscustomobject]@{ Name = $name; Code = $code; Note = $note }
}

$python = (Get-Command python -ErrorAction SilentlyContinue).Source
if (-not $python) { $python = (Get-Command py -ErrorAction SilentlyContinue).Source }
if (-not $python) {
  Write-Host 'python not found - skipping the static audits'
  Add-Result 'static audits' 2 'python not on PATH'
}
else {
  foreach ($audit in 'audit_enums', 'audit_sockets', 'audit_svm_nodes', 'audit_rhino_stock_sockets', 'audit_member_socket_clash', 'audit_retired_socket_writes', 'audit_svm_dispatch') {
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

# Changed kernel code merged without a republish gives everyone on a plain build a new
# ccycles.dll with the old kernels. build_cycles.ps1 only warns about this; here it fails.
Write-Host '--- payload freshness'
$arches = Join-Path $repo 'kernel_arches.ps1'
# cycles-core -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
$manifest = Join-Path $repo '..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\ccycles_payload.json'
if (-not (Test-Path $arches)) {
  Write-Host '  kernel_arches.ps1 not found'
  Add-Result 'payload freshness' 2 'kernel_arches.ps1 missing'
}
elseif (-not (Test-Path $manifest)) {
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
    Write-Host '  payload was built from different kernel sources'
    Write-Host "    payload: $($recorded.Substring(0,16))"
    Write-Host "    tree:    $($current.Substring(0,16))"
    Write-Host '  Run publish_payload.ps1 and commit the payload.'
    Add-Result 'payload freshness' 1 'republish needed'
  }
}

# Not verifiable and not checked out are reported, not failed: a standalone tree may
# have no bundle.
Write-Host '--- library bundle'
& powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $toolsDir 'check_lib_bundle.ps1') -CyclesRoot $repo 2>&1 | ForEach-Object { "  $_" }
$code = $LASTEXITCODE
$note = switch ($code) { 1 { 'not the pinned commit' } 2 { 'LFS pointers not pulled' } 3 { 'not verifiable' } 4 { 'not checked out' } default { '' } }
Add-Result 'library bundle' $(if ($code -eq 1 -or $code -eq 2) { 1 } else { 0 }) $note

# Skipped when this repository is used without the Rhino tree around it.
Write-Host '--- installer kernel list'
$installerCheck = Join-Path $repo '..\..\..\..\..\installer\msi\Features\Plug-ins\update_cycles_kernels.ps1'
if (-not (Test-Path $installerCheck)) {
  Write-Host '  no Rhino installer tree here - skipped'
  Add-Result 'installer kernel list' 0 'no installer tree'
}
else {
  $out = & powershell -NoProfile -ExecutionPolicy Bypass -File $installerCheck -Check 2>&1
  $code = $LASTEXITCODE
  $out | ForEach-Object { "  $_" }
  Add-Result 'installer kernel list' $code $(if ($code -ne 0) { 'regenerate needed' } else { '' })
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
if ($failed) { Write-Host ''; Write-Host "$failed of $($results.Count) check(s) failed"; exit 1 }
Write-Host ''
Write-Host "$($results.Count) check(s) ok"
exit 0
