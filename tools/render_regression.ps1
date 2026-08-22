<#
.SYNOPSIS
  Render fixed models in a Rhino build and compare them against stored images.

.DESCRIPTION
  A golden-image test for the whole render stack: Rhino, RhinoCycles, csycles and
  ccycles. It exists because the faults on the 5.2 port have almost all been of a
  kind that compiles, links, asserts nothing, and renders the wrong pixels - a
  texture coordinate node writing to the wrong SVM stack slot turned every render
  black while every build stayed green. A stored image catches that on the first
  run.

  Two numbers make the threshold meaningful. Rendering the same build twice
  differs by a mean of about 0.03 per channel out of 255, so the renderer is very
  nearly deterministic at these settings; and the difference between the 5.2
  build and shipping Rhino 9 WIP on the same scene was 11.27. A tolerance of 1.0
  therefore sits thirty times above the noise and ten times below a real
  regression.

  Scenes have to set their own render resolution, since the render window
  otherwise takes its size from the viewport and no two runs can be compared. The
  two used here are 300x300 and 600x600 respectively.

  The model has to be passed on Rhino's command line rather than opened over MCP:
  opening a document tears the MCP listener down, and every call after it fails.

.EXAMPLE
  # Check a build against the stored images
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1

.EXAMPLE
  # Re-record them, having decided the new output is correct
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 -UpdateReference

.EXAMPLE
  # One scene only, or a scene of your own
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 -Only rdk_material_scene
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 `
      -Model C:\path\to\scene.3dm -Reference C:\path\to\expected.png
#>
param(
  [string]$Exe = 'C:\Users\Lars\dev\rhino\9.x\src4\bin\Debug\Rhino.exe',
  [int]$Port = 10501,
  [string]$Model = '',
  [string]$Reference = '',
  [string]$Only = '',
  [double]$Tolerance = 1.0,
  [switch]$UpdateReference,
  [int]$StartupTimeoutSeconds = 240
)

$ErrorActionPreference = 'Stop'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$refDir = Join-Path $toolsDir 'reference'
$assets = 'C:\Users\Lars\dev\rhino\9.x\src4\rhino4\assets'
$rdkTools = 'C:\Users\Lars\dev\rhino\9.x\src4\rhino4\Plug-ins\RDK\Library\Tools'

if ($Model) {
  if (-not $Reference) { Write-Error 'give -Reference with -Model'; exit 2 }
  $cases = @([pscustomobject]@{ Name = 'custom'; Model = $Model; Reference = $Reference })
}
else {
  $cases = @(
    [pscustomobject]@{
      Name      = 'rdk_material_scene'
      Model     = (Join-Path $assets 'rdk_material_scene.3dm')
      Reference = (Join-Path $refDir 'rdk_material_scene.png')
    }
    [pscustomobject]@{
      Name      = 'material_scene_final'
      Model     = (Join-Path $rdkTools 'Material_Scene_Final.3dm')
      Reference = (Join-Path $refDir 'material_scene_final.png')
    }
  )
  # @() matters: Where-Object returns a bare object for a single match, and a
  # bare object has no .Count.
  if ($Only) { $cases = @($cases | Where-Object { $_.Name -eq $Only }) }
  if ($cases.Count -eq 0) { Write-Error "no case named '$Only'"; exit 2 }
}

function Invoke-Rhino([string]$tool, $arguments) {
  $payload = @{
    jsonrpc = '2.0'; id = 1; method = 'tools/call'
    params  = @{ name = $tool; arguments = $arguments }
  } | ConvertTo-Json -Depth 20 -Compress
  try {
    $r = Invoke-WebRequest -Uri "http://localhost:$Port/" -Method POST -Body $payload `
                           -ContentType 'application/json' -UseBasicParsing -TimeoutSec 1800
  }
  catch { throw ('Rhino call "' + $tool + '" failed: ' + $_.Exception.Message) }
  $j = $r.Content | ConvertFrom-Json
  if ($j.error) { throw ('Rhino returned an error for "' + $tool + '": ' + ($j.error | ConvertTo-Json -Depth 6)) }
  ($j.result.content | Where-Object { $_.type -eq 'text' } | ForEach-Object { $_.text }) -join "`n"
}

# Mean absolute difference per channel, and the worst single channel.
function Measure-Difference([string]$pathA, [string]$pathB) {
  Add-Type -AssemblyName System.Drawing
  $a = New-Object System.Drawing.Bitmap $pathA
  $b = New-Object System.Drawing.Bitmap $pathB
  try {
    if ($a.Width -ne $b.Width -or $a.Height -ne $b.Height) {
      throw ('size mismatch: {0}x{1} against {2}x{3}. The render resolution comes from the document, so a differing size means a different scene, not a regression.' -f $a.Width, $a.Height, $b.Width, $b.Height)
    }
    [long]$total = 0; [int]$worst = 0; [long]$n = 0
    for ($y = 0; $y -lt $a.Height; $y++) {
      for ($x = 0; $x -lt $a.Width; $x++) {
        $p = $a.GetPixel($x, $y); $q = $b.GetPixel($x, $y)
        foreach ($d in @([Math]::Abs($p.R - $q.R), [Math]::Abs($p.G - $q.G), [Math]::Abs($p.B - $q.B))) {
          $total += $d; $n++
          if ($d -gt $worst) { $worst = $d }
        }
      }
    }
    [pscustomobject]@{ Mean = $total / $n; Worst = $worst; Width = $a.Width; Height = $a.Height }
  }
  finally { $a.Dispose(); $b.Dispose() }
}

function Invoke-Render([string]$model, [string]$outPath) {
  Get-Process Rhino -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -eq $Exe } |
    ForEach-Object { try { $_.Kill() } catch {} }
  for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Seconds 1
    if (-not (Get-Process Rhino -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $Exe })) { break }
  }

  # Post-build steps in this tree fail with 9009 when this is set, and it leaks
  # in from some shells.
  $env:NoDefaultCurrentDirectoryInExePath = $null

  Start-Process -FilePath $Exe -ArgumentList @(('"' + $model + '"'), ('/runscript="_MCPStart ' + $Port + ' _Enter"')) | Out-Null

  $deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
  $up = $false
  while ((Get-Date) -lt $deadline) {
    Start-Sleep -Seconds 3
    try { $c = New-Object Net.Sockets.TcpClient; $c.Connect('127.0.0.1', $Port); $c.Close(); $up = $true; break } catch {}
  }
  if (-not $up) {
    throw ("no MCP listener on port $Port after $StartupTimeoutSeconds s. " +
           'A Debug build is slow to start; raise -StartupTimeoutSeconds if it is merely late.')
  }

  if ((Invoke-Rhino 'list_objects' @{}) -match '"count":0\b') {
    throw ('the document is empty, so the model did not load. Rhino wants a ' +
           'backslash path here; a forward-slash one fails silently.')
  }

  Invoke-Rhino 'run_command' @{ command = '_Render' } | Out-Null
  if (Test-Path $outPath) { Remove-Item $outPath -Force }
  Invoke-Rhino 'run_command' @{ command = ('-_SaveRenderWindowAs "' + $outPath + '"') } | Out-Null
  if (-not (Test-Path $outPath)) { throw "the render was not saved to $outPath" }
}

if (-not (Test-Path $Exe)) { Write-Error "Rhino not found: $Exe"; exit 2 }

$failed = 0
foreach ($case in $cases) {
  Write-Host ('--- ' + $case.Name)
  if (-not (Test-Path $case.Model)) { Write-Host ('  model not found: ' + $case.Model); $failed++; continue }
  $actual = Join-Path ([IO.Path]::GetTempPath()) ('render_regression_' + $case.Name + '.png')
  try { Invoke-Render $case.Model $actual }
  catch { Write-Host ('  ' + $_.Exception.Message); $failed++; continue }

  if ($UpdateReference) {
    if (-not (Test-Path $refDir)) { New-Item -ItemType Directory -Path $refDir | Out-Null }
    Copy-Item $actual $case.Reference -Force
    Write-Host ('  recorded ' + $case.Reference)
    continue
  }

  if (-not (Test-Path $case.Reference)) {
    Write-Host ('  no stored image at ' + $case.Reference +
                ' - run once with -UpdateReference to record one, after checking the render by eye')
    $failed++
    continue
  }

  $d = Measure-Difference $case.Reference $actual
  Write-Host ('  {0}x{1}  mean={2:N3}  worst={3}  tolerance={4:N3}' -f $d.Width, $d.Height, $d.Mean, $d.Worst, $Tolerance)
  if ($d.Mean -gt $Tolerance) {
    Write-Host ('  FAIL - the render moved. Compare ' + $actual + ' against ' + $case.Reference + ' by eye.')
    Write-Host '  If the new output is right, re-record with -UpdateReference.'
    $failed++
  }
  else { Write-Host '  ok' }
}

Write-Host ''
$total = @($cases).Count
if ($failed) { Write-Host ("$failed of $total case(s) failed"); exit 1 }
Write-Host "$total case(s) ok"
exit 0
