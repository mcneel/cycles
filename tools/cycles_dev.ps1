<#
.SYNOPSIS
  Report what the next build would do about Cycles, and override it if you must.

.DESCRIPTION
  Normal use needs none of this. Cycles is rebuilt when its sources differ from
  the fingerprint the prebuilt payload was stamped with, and left alone
  otherwise; cycles_build_if_needed.ps1 decides, and this script asks it rather
  than keeping a second copy of the rules.

  The overrides exist for the cases the comparison cannot cover - forcing a
  rebuild after changing a GPU SDK, or keeping a build quick while working on
  something else entirely.

    -Status   Report what the next build would do, per configuration. Default.
    -On       Always build from source (RHINOCYCLESDEV=1), stamp or no stamp.
    -Off      Never build from source (RHINOCYCLESDEV=0).
    -Auto     Remove the variable, restoring the automatic comparison.

  Two things this reports that are easy to get wrong:

    A running Visual Studio does not see the change. devenv.exe reads the
    environment once, at launch. -On after VS is already open changes nothing
    until VS is restarted, and the build output will keep saying the flag is
    unset. If devenv is running, this script says so.

    Native debugging needs the debug payload. RhinoCyclesCore copies the debug
    Cycles payload from big_libs only when that folder exists, and only the
    release payload is committed - the debug one is 444 MB and gitignored. So on
    a fresh checkout a Debug Rhino runs against release Cycles and native
    stepping into ccycles gets you nothing, whatever this flag says. One Debug
    solution build with -On populates it.

  Exit code is 0 unless the requested change failed.

.EXAMPLE
  # What is my current state?
  powershell -ExecutionPolicy Bypass -File tools/cycles_dev.ps1

.EXAMPLE
  # Build Cycles from source from now on, then restart VS
  powershell -ExecutionPolicy Bypass -File tools/cycles_dev.ps1 -On

.EXAMPLE
  # Back to the prebuilt payload
  powershell -ExecutionPolicy Bypass -File tools/cycles_dev.ps1 -Off
#>
param(
  [switch]$On,
  [switch]$Off,
  [switch]$Auto,
  [switch]$Status
)

$ErrorActionPreference = 'Stop'

if (@($On, $Off, $Auto | Where-Object { $_ }).Count -gt 1) {
  Write-Host 'cycles_dev: -On, -Off and -Auto are mutually exclusive.' -ForegroundColor Red
  exit 1
}

$varName = 'RHINOCYCLESDEV'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$cyclesDir = Split-Path -Parent $toolsDir
# tools -> cycles -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
$repoRoot = (Resolve-Path (Join-Path $cyclesDir '..\..\..\..\..')).Path
$payloadRoot = Join-Path $repoRoot 'big_libs\RhinoCycles\ccycles\win'

function Get-DevenvCount {
  try {
    return @(Get-Process -Name devenv -ErrorAction Stop).Count
  } catch {
    return 0
  }
}

function Show-Status {
  $persisted = [Environment]::GetEnvironmentVariable($varName, 'User')
  $machine = [Environment]::GetEnvironmentVariable($varName, 'Machine')
  $session = $env:RHINOCYCLESDEV

  # Do not re-derive the decision here. cycles_build_if_needed.ps1 owns it, and a
  # second copy of the rules is a second thing to get out of step.
  Write-Host ''
  Write-Host 'What the next build would do' -ForegroundColor Cyan
  foreach ($cfg in @('Release', 'Debug')) {
    $dir = Join-Path $payloadRoot $cfg.ToLower()
    $out = & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $toolsDir 'cycles_build_if_needed.ps1') `
             -Configuration $cfg -InstallDir $dir -DecideOnly 2>&1
    Write-Host ("  {0,-8}:" -f $cfg)
    foreach ($line in @($out)) { Write-Host ("      {0}" -f ($line -replace '^ccycles: ', '')) }
  }

  Write-Host ''
  Write-Host 'Where it is set' -ForegroundColor Cyan
  Write-Host ("  user environment     : {0}" -f $(if ([string]::IsNullOrEmpty($persisted)) { '<unset>' } else { $persisted }))
  if (![string]::IsNullOrEmpty($machine)) {
    Write-Host ("  machine environment  : {0}" -f $machine)
  }
  Write-Host ("  this shell           : {0}" -f $(if ([string]::IsNullOrEmpty($session)) { '<unset>' } else { $session }))

  $devenvCount = Get-DevenvCount
  if ($devenvCount -gt 0) {
    Write-Host ''
    Write-Host "  $devenvCount Visual Studio instance(s) running. Each one is still using the" -ForegroundColor Yellow
    Write-Host '  environment it was launched with. Restart VS for a change here to reach it.' -ForegroundColor Yellow
  }

  Write-Host ''
  Write-Host 'Payload in big_libs' -ForegroundColor Cyan
  foreach ($cfg in @('release', 'debug')) {
    $dll = Join-Path $payloadRoot "$cfg\ccycles.dll"
    if (Test-Path $dll) {
      $stamp = (Get-Item $dll).LastWriteTime.ToString('yyyy-MM-dd HH:mm')
      Write-Host ("  {0,-8}: present, ccycles.dll {1}" -f $cfg, $stamp)
    } else {
      Write-Host ("  {0,-8}: absent" -f $cfg)
    }
  }
  if (-not (Test-Path (Join-Path $payloadRoot 'debug\ccycles.dll'))) {
    Write-Host '  No debug payload, so a Debug Rhino runs release Cycles and native' -ForegroundColor Yellow
    Write-Host '  stepping into ccycles will not work. Build the solution once as Debug' -ForegroundColor Yellow
    Write-Host '  with this switch ON to populate it.' -ForegroundColor Yellow
  }
  Write-Host ''
}

if ($On -or $Off -or $Auto) {
  # 1 = always build from source. 0 = never, beating the build tree as well.
  # Removed = let the build tree decide, which is the natural state.
  $value = if ($On) { '1' } elseif ($Off) { '0' } else { $null }
  [Environment]::SetEnvironmentVariable($varName, $value, 'User')

  if ($On) {
    Write-Host "cycles_dev: $varName=1 persisted for this user." -ForegroundColor Green
    Write-Host '            Cycles will be built from source by the next build. The first'
    Write-Host '            pass configures CMake and is slow; later ones are incremental.'
  } elseif ($Off) {
    Write-Host "cycles_dev: $varName=0 persisted for this user." -ForegroundColor Green
    Write-Host '            Cycles will not be rebuilt even if you change its sources.'
    Write-Host '            Your CMake build tree is left alone, so -Auto or -On picks up'
    Write-Host '            where you left off rather than reconfiguring from scratch.'
  } else {
    Write-Host "cycles_dev: $varName removed for this user." -ForegroundColor Green
    Write-Host '            Back to the automatic comparison, which is what you want.'
  }

  $machine = [Environment]::GetEnvironmentVariable($varName, 'Machine')
  if ($Off -and ![string]::IsNullOrEmpty($machine)) {
    Write-Host ''
    Write-Host "cycles_dev: $varName is also set machine-wide (=$machine), which this" -ForegroundColor Yellow
    Write-Host '            script does not touch because that needs elevation. Builds will' -ForegroundColor Yellow
    Write-Host '            still build Cycles from source until that is removed too.' -ForegroundColor Yellow
  }

  $devenvCount = Get-DevenvCount
  if ($devenvCount -gt 0) {
    Write-Host ''
    Write-Host "cycles_dev: restart Visual Studio ($devenvCount instance(s) running)." -ForegroundColor Yellow
    Write-Host '            It read the environment at launch and will not see this until then.' -ForegroundColor Yellow
  }

  Show-Status
  exit 0
}

Show-Status
exit 0
