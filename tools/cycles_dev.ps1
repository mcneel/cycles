<#
.SYNOPSIS
  Turn building Cycles from source on or off, and report whether the switch has
  actually taken effect.

.DESCRIPTION
  Building Cycles from source is gated on one environment variable,
  RHINOCYCLESDEV. RhinoBuilder's "Cycles Core" checkbox passes it as
  /p:RHINOCYCLESDEV=1; Visual Studio has no equivalent UI and can only inherit
  it from the environment. This script is that missing UI.

    -On       Persist RHINOCYCLESDEV=1 for the current user.
    -Off      Remove it.
    -Status   Report what is set, what a new build would see, and whether a
              running Visual Studio is out of date. This is the default.

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
  [switch]$Status
)

$ErrorActionPreference = 'Stop'

if ($On -and $Off) {
  Write-Host 'cycles_dev: -On and -Off are mutually exclusive.' -ForegroundColor Red
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

  # A vcxproj condition tests for non-empty, so anything at all counts as on.
  #
  # There are two answers, not one. A build started from this shell inherits the
  # session value; a newly launched VS or shell gets the persisted one. Reporting
  # only the persisted value would have called this OFF in a shell that was about
  # to build Cycles.
  $persistedEffective = if (![string]::IsNullOrEmpty($persisted)) { $persisted }
                        elseif (![string]::IsNullOrEmpty($machine)) { $machine }
                        else { '' }
  $hereEffective = if (![string]::IsNullOrEmpty($session)) { $session } else { $persistedEffective }

  function Format-State($value) {
    if ([string]::IsNullOrEmpty($value)) { return 'OFF - prebuilt payload from big_libs' }
    return "ON  - $varName=$value, so build_cycles.ps1 runs"
  }

  Write-Host ''
  Write-Host 'Cycles source builds' -ForegroundColor Cyan
  $hereColour = if ([string]::IsNullOrEmpty($hereEffective)) { 'Gray' } else { 'Green' }
  $newColour = if ([string]::IsNullOrEmpty($persistedEffective)) { 'Gray' } else { 'Green' }
  Write-Host ("  from this shell        : {0}" -f (Format-State $hereEffective)) -ForegroundColor $hereColour
  Write-Host ("  from a new shell or VS : {0}" -f (Format-State $persistedEffective)) -ForegroundColor $newColour
  if ($hereEffective -ne $persistedEffective) {
    Write-Host '  These differ, so where you build from decides what happens.' -ForegroundColor Yellow
  }

  # The project tests for non-empty, not for truth, so these all mean ON - the
  # opposite of what anyone setting them intends.
  foreach ($v in @($hereEffective, $persistedEffective) | Select-Object -Unique) {
    if ($v -match '^\s*(0|false|no|off)\s*$') {
      Write-Host ''
      Write-Host "  Careful: '$v' still counts as ON. The build tests whether the" -ForegroundColor Yellow
      Write-Host '  variable is non-empty, not whether it looks true, so this builds' -ForegroundColor Yellow
      Write-Host '  Cycles and its GPU kernels. Off means removed - use -Off.' -ForegroundColor Yellow
    }
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

if ($On -or $Off) {
  $value = if ($On) { '1' } else { $null }
  [Environment]::SetEnvironmentVariable($varName, $value, 'User')

  if ($On) {
    Write-Host "cycles_dev: $varName=1 persisted for this user." -ForegroundColor Green
    Write-Host '            Cycles will be built from source by the next build. The first'
    Write-Host '            pass configures CMake and is slow; later ones are incremental.'
  } else {
    Write-Host "cycles_dev: $varName removed for this user." -ForegroundColor Green
    Write-Host '            Builds go back to the prebuilt payload in big_libs.'
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
