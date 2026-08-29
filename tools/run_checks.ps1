<#
.SYNOPSIS
  Run every check this port has: the static audits, and optionally the
  golden-image render regression.

.DESCRIPTION
  One entry point, because the checks are only useful if they are cheap to run
  and nobody remembers four commands. Two tiers:

    Static audits    No build, no Rhino, about a second. These catch the four
                     ways the 3.5 -> 5.2 port has silently drifted: a renamed or
                     retyped socket, a renumbered enum, a stock SVM node
                     emitted in Rhino's packed layout, and a parameter
                     exposed as both a member and a socket. Every one of those
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
    foreach ($audit in 'audit_enums', 'audit_sockets', 'audit_svm_nodes', 'audit_rhino_stock_sockets', 'audit_member_socket_clash') {
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
