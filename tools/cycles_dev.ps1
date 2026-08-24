<#
.SYNOPSIS
  Turn building Cycles from source on or off, and report whether the switch has
  actually taken effect.

.DESCRIPTION
  Building Cycles from source is gated on one environment variable,
  RHINOCYCLESDEV. RhinoBuilder's "Cycles Core" checkbox passes it as
  /p:RHINOCYCLESDEV=1; Visual Studio has no equivalent UI and can only inherit
  it from the environment. This script is that missing UI.

    -On       Always build Cycles from source (RHINOCYCLESDEV=1).
    -Off      Never build it from source (RHINOCYCLESDEV=0). This beats the
              build tree, so it works without deleting an hour of CMake output.
    -Auto     Remove the variable and let the build tree decide. This is the
              natural state: a fresh checkout has no tree and so uses the
              prebuilt payload, and one source build turns it on for good.
    -Status   Report which trigger is active, whether a running Visual Studio is
              out of date, and what big_libs holds. This is the default.

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

  # Three states, matching ccycles.vcxproj exactly:
  #   an explicit off (0/false/no/off) wins over everything
  #   any other non-empty value means always build from source
  #   empty means the build tree decides
  #
  # And two answers, not one: a build from this shell inherits the session value,
  # a newly launched VS gets the persisted one.
  $persistedFlag = if (![string]::IsNullOrEmpty($persisted)) { $persisted }
                   elseif (![string]::IsNullOrEmpty($machine)) { $machine }
                   else { '' }
  $hereFlag = if (![string]::IsNullOrEmpty($session)) { $session } else { $persistedFlag }

  $buildTree = Join-Path $cyclesDir 'build\CMakeCache.txt'
  $haveTree = Test-Path $buildTree

  function Resolve-State($flag) {
    if ($flag -match '^\s*(0|false|no|off)\s*$') { return 'OFF - turned off explicitly, which beats the build tree' }
    if (![string]::IsNullOrEmpty($flag))           { return "ON  - $varName=$flag" }
    if ($haveTree)                                 { return 'ON  - a Cycles build tree exists' }
    return 'OFF - no flag and no build tree, so the prebuilt payload is used'
  }

  $hereState = Resolve-State $hereFlag
  $newState = Resolve-State $persistedFlag

  Write-Host ''
  Write-Host 'Cycles source builds' -ForegroundColor Cyan
  Write-Host ("  from this shell        : {0}" -f $hereState) -ForegroundColor $(if ($hereState.StartsWith('ON')) { 'Green' } else { 'Gray' })
  Write-Host ("  from a new shell or VS : {0}" -f $newState) -ForegroundColor $(if ($newState.StartsWith('ON')) { 'Green' } else { 'Gray' })
  if ($hereState -ne $newState) {
    Write-Host '  These differ, so where you build from decides what happens.' -ForegroundColor Yellow
  }
  if ($haveTree -and [string]::IsNullOrEmpty($hereFlag)) {
    Write-Host '  Edit Cycles, press Build, and only what you changed is rebuilt.'
    Write-Host '  Use -Off to stop without discarding the build tree.'
  } elseif (-not $haveTree) {
    Write-Host '  After one source build the tree exists and keeps it on by itself.'
  }

  Write-Host ''
  Write-Host 'Cycles build tree' -ForegroundColor Cyan
  if ($haveTree) {
    Write-Host ("  present : {0}" -f (Split-Path -Parent $buildTree))
  } else {
    Write-Host '  none    - a fresh checkout has no build tree, and only CMake creates one.'
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
    Write-Host '            Source builds are off, and stay off even though a build tree'
    Write-Host '            exists - an explicit off beats it. Your build tree is left'
    Write-Host '            alone, so -On picks up where you left off rather than'
    Write-Host '            reconfiguring CMake from scratch.'
  } else {
    Write-Host "cycles_dev: $varName removed for this user." -ForegroundColor Green
    if (Test-Path (Join-Path $cyclesDir 'build\CMakeCache.txt')) {
      Write-Host '            A Cycles build tree exists, so source builds are ON again.'
    } else {
      Write-Host '            No build tree either, so builds use the prebuilt payload.'
    }
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
