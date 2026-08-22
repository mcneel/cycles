<#
.SYNOPSIS
  Render a fixed model in a Rhino build and compare it against a stored image.

.DESCRIPTION
  A golden-image test for the whole render stack: Rhino, RhinoCycles, csycles and
  ccycles. It exists because the faults on the 5.2 port have almost all been of a
  kind that compiles, links, asserts nothing, and renders the wrong pixels - a
  texture coordinate node writing to the wrong SVM stack slot turned every render
  black while every build stayed green. A stored image catches that on the first
  run.

  Two numbers make it usable. Rendering the same build twice differs by a mean of
  about 0.05 per channel out of 255, so the renderer is very nearly deterministic
  at these settings; and the difference between the 5.2 build and shipping Rhino 9
  WIP on the same scene was 11.27. A tolerance of 1.0 therefore sits twenty times
  above the noise and ten times below a real regression.

  The model has to be passed on Rhino's command line rather than opened over MCP:
  opening a document tears the MCP listener down, and every call after it fails.

.EXAMPLE
  # Check a build against the stored image
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1

.EXAMPLE
  # Re-record the stored image, having decided the new output is correct
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 -UpdateReference

.EXAMPLE
  # Compare a different build, e.g. an installed Rhino, on the same scene
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 `
      -Exe 'C:\Program Files\Rhino 9 WIP\System\Rhino.exe' -Port 10500 -Compare
#>
param(
  [string]$Exe = 'C:\Users\Lars\dev\rhino\9.x\src4\bin\Debug\Rhino.exe',
  [int]$Port = 10501,
  [string]$Model = 'C:\Users\Lars\dev\rhino\9.x\src4\rhino4\assets\rdk_material_scene.3dm',
  [string]$Reference = '',
  [double]$Tolerance = 1.0,
  [switch]$UpdateReference,
  [switch]$Compare,
  [int]$StartupTimeoutSeconds = 240
)

$ErrorActionPreference = 'Stop'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not $Reference) {
  $Reference = Join-Path $toolsDir 'reference\rdk_material_scene.png'
}
$actual = Join-Path ([IO.Path]::GetTempPath()) ('render_regression_' + $Port + '.png')

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

if (-not (Test-Path $Model)) { Write-Error "model not found: $Model"; exit 2 }
if (-not (Test-Path $Exe)) { Write-Error "Rhino not found: $Exe"; exit 2 }

Get-Process Rhino -ErrorAction SilentlyContinue |
  Where-Object { $_.Path -eq $Exe } |
  ForEach-Object { try { $_.Kill() } catch {} }
for ($i = 0; $i -lt 30; $i++) {
  Start-Sleep -Seconds 1
  if (-not (Get-Process Rhino -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $Exe })) { break }
}

# Post-build steps in this tree fail with 9009 when this is set, and it leaks in
# from some shells.
$env:NoDefaultCurrentDirectoryInExePath = $null

Write-Host ('launching  ' + $Exe)
Start-Process -FilePath $Exe -ArgumentList @(('"' + $Model + '"'), ('/runscript="_MCPStart ' + $Port + ' _Enter"')) | Out-Null

$deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
$up = $false
while ((Get-Date) -lt $deadline) {
  Start-Sleep -Seconds 3
  try { $c = New-Object Net.Sockets.TcpClient; $c.Connect('127.0.0.1', $Port); $c.Close(); $up = $true; break } catch {}
}
if (-not $up) {
  Write-Error ("no MCP listener on port $Port after $StartupTimeoutSeconds s. " +
               'A Debug build is slow to start; raise -StartupTimeoutSeconds if it is merely late.')
  exit 2
}

$objects = Invoke-Rhino 'list_objects' @{}
if ($objects -match '"count":0\b') {
  Write-Error ('the document is empty, so the model did not load. Rhino wants a ' +
               'backslash path here; a forward-slash one fails silently.')
  exit 2
}

Write-Host 'rendering'
Invoke-Rhino 'run_command' @{ command = '_Render' } | Out-Null
if (Test-Path $actual) { Remove-Item $actual -Force }
Invoke-Rhino 'run_command' @{ command = ('-_SaveRenderWindowAs "' + $actual + '"') } | Out-Null
if (-not (Test-Path $actual)) { Write-Error "the render was not saved to $actual"; exit 2 }

if ($UpdateReference) {
  $dir = Split-Path -Parent $Reference
  if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
  Copy-Item $actual $Reference -Force
  Write-Host ('recorded   ' + $Reference)
  exit 0
}

if ($Compare) { Write-Host ('wrote      ' + $actual); exit 0 }

if (-not (Test-Path $Reference)) {
  Write-Error ("no stored image at $Reference. Run once with -UpdateReference to record one, " +
               'after checking by eye that the render is right.')
  exit 2
}

$d = Measure-Difference $Reference $actual
Write-Host ('{0}x{1}  mean={2:N3}  worst={3}  tolerance={4:N3}' -f $d.Width, $d.Height, $d.Mean, $d.Worst, $Tolerance)
if ($d.Mean -gt $Tolerance) {
  Write-Host ''
  Write-Host ('FAIL - the render moved. Keep ' + $actual + ' and compare it against ' + $Reference + ' by eye.')
  Write-Host 'If the new output is right, re-record with -UpdateReference.'
  exit 1
}
Write-Host 'ok'
exit 0
