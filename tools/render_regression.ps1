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
  # Fetch the RH-81636 model set from Drive and check the build against it.
  # Needs rclone with a Drive remote named 'gdrive' - see the -FromDrive branch.
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 -FromDrive

.EXAMPLE
  # A folder of models you already have, one case per .3dm
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 `
      -ModelDir C:\Users\Lars\rhino-render-test-models
  # ...and record the stored images the first time, having checked them by eye
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 `
      -ModelDir C:\Users\Lars\rhino-render-test-models -UpdateReference

.EXAMPLE
  # One scene only, or a scene of your own
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 -Only rdk_material_scene
  powershell -ExecutionPolicy Bypass -File tools/render_regression.ps1 `
      -Model C:\path\to\scene.3dm -Reference C:\path\to\expected.png
#>
param(
  [string]$Exe = 'C:\Users\Lars\dev\rhino\9.x\src4\bin\Debug\Rhino.exe',
  [string]$Model = '',
  [string]$Reference = '',
  [string]$ModelDir = '',
  [switch]$FromDrive,
  [string]$DriveFolderId = '1UqHKifO8qWE35x67XnwpNsJ189u-k0vk',
  [string]$Remote = 'gdrive',
  [string]$Cache = "$env:LOCALAPPDATA
hino-render-test-models",
  # Models that cannot produce a usable stored image today. Recording one for a
  # model that renders wrongly would enshrine the bug as the expected result, so
  # these stay out until their issues are fixed:
  #   PBRMatTest                 - 620 MB, and the only file with UseViewportSize set
  #   Test_backgroundimage       - RH-98416, wallpaper background does not render
  #   Wash Basin v7 room         - RH-98416, same wallpaper fault, window renders white
  #   Rhino Logo_texture_...     - RH-98419, every texture-mapped object renders black
  #   GoudaSSS_Distribute_Candle - RH-98420, hangs in RDK scene construction
  [string[]]$Exclude = @(
    'PBRMatTest'
    'Test_backgroundimage'
    'Wash Basin v7 room'
    'Rhino Logo_texture_mapping_types_saved_from_v8'
    'GoudaSSS_Distribute_Candle'
  ),
  [string]$Only = '',
  [double]$Tolerance = 1.0,
  [switch]$UpdateReference,
  [string]$Harness = 'C:\Users\Lars\rhino9-cycles-harness\render_one.ps1',
  [ValidateSet('dev','beta')][string]$Build = 'dev',
  # render_one.ps1's SelectedDeviceStr: 0 = HIP GPU, 1 = CPU. Baselines are not
  # comparable across devices, so a change here invalidates every stored image.
  [string]$Device = '0',
  [int]$Width = 1920,
  [int]$Height = 1080,
  [int]$Samples = 100,
  [int]$TimeoutMin = 120,
  # By default each model renders at its own stored resolution, which is
  # stable across runs and is what its author composed. -PinSize forces
  # -Width x -Height instead, re-framing anything not already 16:9.
  [switch]$PinSize
)

$ErrorActionPreference = 'Stop'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$refDir = Join-Path $toolsDir 'reference'
$assets = 'C:\Users\Lars\dev\rhino\9.x\src4\rhino4\assets'
$rdkTools = 'C:\Users\Lars\dev\rhino\9.x\src4\rhino4\Plug-ins\RDK\Library\Tools'

if ($FromDrive) {
  # The model set lives in a private Drive folder, so this needs a credential -
  # there is no anonymous path to it. rclone holds the OAuth token in its own
  # config, and the same command line works unattended on a build agent by
  # adding --drive-service-account-file, which is why it is rclone and not gdown.
  if (-not (Get-Command rclone -ErrorAction SilentlyContinue)) {
    Write-Error ("rclone is not installed. Install it (winget install Rclone.Rclone), then " +
                 "run 'rclone config' once to create a Drive remote named '$Remote'.")
    exit 2
  }
  if (-not (Test-Path $Cache)) { New-Item -ItemType Directory -Path $Cache | Out-Null }

  # --drive-root-folder-id makes the shared folder the root of the remote, so no
  # one has to know where it sits in anybody's Drive tree. copy, not sync: sync
  # would delete anything else in the cache, including a model added by hand.
  Write-Host ("fetching models from Drive folder " + $DriveFolderId + " into " + $Cache)
  & rclone copy "${Remote}:" $Cache --drive-root-folder-id $DriveFolderId `
      --include '*.3dm' --checkers 4 --transfers 4 --stats-one-line --stats 30s
  if ($LASTEXITCODE -ne 0) { Write-Error "rclone copy failed with exit code $LASTEXITCODE"; exit 2 }

  $ModelDir = $Cache
}

if ($Model) {
  if (-not $Reference) { Write-Error 'give -Reference with -Model'; exit 2 }
  $cases = @([pscustomobject]@{ Name = 'custom'; Model = $Model; Reference = $Reference })
}
elseif ($ModelDir) {
  if (-not (Test-Path $ModelDir)) { Write-Error "no such folder: $ModelDir"; exit 2 }
  # One case per .3dm in the folder. That is how the model set in RH-81636 was run
  # by hand before any of this existed: the file name says what the model tests,
  # and each model carries its own render resolution and sample count. Stored
  # images sit beside the others, keyed by the model's base name.
  $cases = @(Get-ChildItem -Path $ModelDir -Filter *.3dm -File | Sort-Object Name | ForEach-Object {
    [pscustomobject]@{
      Name      = $_.BaseName
      Model     = $_.FullName
      Reference = (Join-Path $refDir ($_.BaseName + '.png'))
    }
  })
  if ($cases.Count -eq 0) { Write-Error "no .3dm files in $ModelDir"; exit 2 }

  # PBRMatTest is the one model with UseViewportSize set. driver.py pins the
  # render size, so that no longer makes it uncomparable - but it is a 620 MB
  # file, so it stays out of the default set on running time alone. Pass
  # -Exclude @() to include it.
  if ($Exclude.Count) {
    $skipped = @($cases | Where-Object { $Exclude -contains $_.Name })
    foreach ($k in $skipped) { Write-Host ('skipping ' + $k.Name + ' (see -Exclude)') }
    $cases = @($cases | Where-Object { $Exclude -notcontains $_.Name })
    if ($cases.Count -eq 0) { Write-Error 'every model was excluded'; exit 2 }
  }
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
}

# @() matters: Where-Object returns a bare object for a single match, and a bare
# object has no .Count.
if ($Only) {
  $cases = @($cases | Where-Object { $_.Name -eq $Only })
  if ($cases.Count -eq 0) { Write-Error "no case named '$Only'"; exit 2 }
}

# The per-channel summation, in C#. A PowerShell loop over the pixels was fine
# for the 300x300 and 600x600 preview scenes this started on, but GetPixel costs
# a marshalled call per pixel: at 1920x1080 - which is what the RH-81636 models
# render at - it is about 6 million iterations and several minutes per pair, so
# the comparison would take longer than the render it is checking. Note this
# takes two byte arrays and references no assembly, so it compiles the same under
# Windows PowerShell and PowerShell 7; the decode still uses System.Drawing.
if (-not ('RenderDiff' -as [type])) {
  Add-Type -TypeDefinition @'
public static class RenderDiff
{
  // Returns { total, worst, count }. Alpha is skipped, matching the R/G/B-only
  // comparison this replaces - a PNG from the render window is opaque anyway.
  public static long[] Compare(byte[] a, byte[] b, int width, int height, int stride)
  {
    long total = 0; int worst = 0; long count = 0;
    for (int y = 0; y < height; y++)
    {
      int row = y * stride;
      for (int x = 0; x < width; x++)
      {
        int i = row + x * 4;
        for (int c = 0; c < 3; c++)
        {
          int d = a[i + c] - b[i + c];
          if (d < 0) d = -d;
          total += d; count++;
          if (d > worst) worst = d;
        }
      }
    }
    return new long[] { total, worst, count };
  }
}
'@
}

# Mean absolute difference per channel, and the worst single channel.
function Measure-Difference([string]$pathA, [string]$pathB) {
  Add-Type -AssemblyName System.Drawing
  $a = New-Object System.Drawing.Bitmap $pathA
  $b = New-Object System.Drawing.Bitmap $pathB
  try {
    if ($a.Width -ne $b.Width -or $a.Height -ne $b.Height) {
      throw ('size mismatch: {0}x{1} against {2}x{3}. driver.py pins the render size, so this means the stored image was recorded at a different -Width/-Height, not that the render regressed.' -f $a.Width, $a.Height, $b.Width, $b.Height)
    }
    $rect = New-Object System.Drawing.Rectangle 0, 0, $a.Width, $a.Height
    # An explicit 32bpp format so the stride and the channel order are known
    # whatever the file happened to be saved as.
    $fmt = [System.Drawing.Imaging.PixelFormat]::Format32bppArgb
    $mode = [System.Drawing.Imaging.ImageLockMode]::ReadOnly
    $la = $a.LockBits($rect, $mode, $fmt)
    try {
      $lb = $b.LockBits($rect, $mode, $fmt)
      try {
        $len = $la.Stride * $a.Height
        $ba = New-Object byte[] $len
        $bb = New-Object byte[] $len
        [Runtime.InteropServices.Marshal]::Copy($la.Scan0, $ba, 0, $len)
        [Runtime.InteropServices.Marshal]::Copy($lb.Scan0, $bb, 0, $len)
      }
      finally { $b.UnlockBits($lb) }
    }
    finally { $a.UnlockBits($la) }

    $r = [RenderDiff]::Compare($ba, $bb, $a.Width, $a.Height, $la.Stride)
    [pscustomobject]@{ Mean = $r[0] / $r[2]; Worst = [int]$r[1]; Width = $a.Width; Height = $a.Height }
  }
  finally { $a.Dispose(); $b.Dispose() }
}

function Invoke-Render([string]$model, [string]$outPath) {
  # Drive Rhino through rhino9-cycles-harness\render_one.ps1 rather than over MCP.
  # That is the route which has actually rendered these models: it sets
  # ProcessStartInfo.Arguments verbatim (Start-Process -ArgumentList silently
  # breaks /runscript), turns off the Debug build's modal error box before doing
  # anything, runs driver.py from inside a ReadCommandFile script - the only way
  # RunPythonScript works in a Debug build - dismisses blocking Warning/Assert/
  # Error dialogs while it waits, and waits on driver.py's DONE marker instead of
  # inferring completion from a command return. It also pins the render device,
  # without which no comparison between two runs means anything.
  if (-not (Test-Path $Harness)) { throw "harness not found: $Harness" }

  # driver.py keeps the document's own ImageSize unless told otherwise; it still
  # pins the size for a document with UseViewportSize set, which has none to keep.
  $env:RHDIFF_KEEPSIZE = if ($PinSize) { '0' } else { '1' }

  & $Harness -Build $Build -Model $model -Device $Device `
             -Width $Width -Height $Height -Samples $Samples -TimeoutMin $TimeoutMin

  # driver.py saves <stem>.png into the harness's own renders\<build>\ directory,
  # alongside the log it writes as it goes.
  $stem       = [IO.Path]::GetFileNameWithoutExtension($model)
  $harnessDir = Split-Path -Parent $Harness
  $src        = Join-Path $harnessDir ('renders\' + $Build + '\' + $stem + '.png')
  $driverLog  = Join-Path $harnessDir ('renders\' + $Build + '\' + $stem + '.driver.log')
  if (-not (Test-Path $src)) {
    throw "no render at $src - read $driverLog for what the driver actually did"
  }
  if (Test-Path $outPath) { Remove-Item $outPath -Force }
  Copy-Item $src $outPath -Force
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
