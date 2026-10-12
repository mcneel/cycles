#Requires -Version 5.1
<#
.SYNOPSIS
    Builds OpenImageIO_Cycles: Blender's OpenImageIO build without libheif, for the payload.

.DESCRIPTION
    Blender's prebuilt OpenImageIO links libheif (LGPL-3.0) statically (RH-99462). This
    builds the same OpenImageIO release from source with Blender's options
    (build_files/build_environment/cmake/openimageio.cmake), except: no libheif, no Python
    bindings, no command-line tools, and the libraries are named OpenImageIO_Cycles and
    OpenImageIO_Util_Cycles. Dependencies come from the library bundle in lib/. libtiff and
    libdeflate are not in it (Blender links them into OpenImageIO), so this builds them
    first with Blender's settings (tiff.cmake: zlib, JPEG and libdeflate); OpenImageIO's
    own build fetches header-only robin-map.

    The result is installed into big_libs and committed there; src/cmake/external_libs.cmake
    uses it instead of the bundle's OpenImageIO. Re-run after moving the library bundle pin,
    with -Version set to what Blender's versions.cmake names for that bundle.

.PARAMETER Version
    OpenImageIO release tag without the leading v. Must match the bundle's OpenImageIO.

.PARAMETER WorkDir
    Source and build tree. Default: <cycles-core>\build_oiio (gitignored).

.PARAMETER InstallDir
    Default: <repo>\big_libs\OpenImageIO-<Version>-Cycles\windows_x64.
#>
param(
    [string]$Version = '3.1.13.1',
    [string]$WorkDir,
    [string]$InstallDir
)

$ErrorActionPreference = 'Stop'
$cyclesRoot = Split-Path -Parent $PSScriptRoot
$libDir = Join-Path $cyclesRoot 'lib\windows_x64'
if (-not $WorkDir) { $WorkDir = Join-Path $cyclesRoot 'build_oiio' }
if (-not $InstallDir) {
    # cycles-core -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
    $InstallDir = Join-Path $cyclesRoot "..\..\..\..\..\big_libs\OpenImageIO-$Version-Cycles\windows_x64"
}
$InstallDir = [IO.Path]::GetFullPath($InstallDir)
$srcDir = Join-Path $WorkDir 'src'
$buildDir = Join-Path $WorkDir 'build'

function Write-Step([string]$Text) { Write-Host "==> $Text" -ForegroundColor Cyan }

# The bundle's own OpenImageIO says which release its libraries were built for.
$versionHeader = Join-Path $libDir 'OpenImageIO\include\OpenImageIO\oiioversion.h'
if (-not (Test-Path $versionHeader)) { throw "No library bundle at '$libDir'. Fetch it first (BUILDING.md)." }
$parts = foreach ($n in 'MAJOR', 'MINOR', 'PATCH', 'TWEAK') {
    (Select-String -Path $versionHeader -Pattern "#define OIIO_VERSION_$n (\d+)").Matches[0].Groups[1].Value
}
$bundleVersion = $parts -join '.'
if ($bundleVersion -ne $Version) {
    throw "The bundle's OpenImageIO is $bundleVersion, not $Version. Pass -Version $bundleVersion."
}

Write-Step "OpenImageIO $Version source"
if (-not (Test-Path (Join-Path $srcDir '.git'))) {
    git clone --quiet --depth 1 --branch "v$Version" https://github.com/AcademySoftwareFoundation/OpenImageIO.git $srcDir
    if ($LASTEXITCODE) { throw 'git clone failed.' }
}
foreach ($patch in Get-ChildItem (Join-Path $PSScriptRoot 'oiio') -Filter *.diff | Sort-Object Name) {
    git -C $srcDir apply --check --reverse $patch.FullName 2>$null
    if ($LASTEXITCODE -eq 0) { Write-Host "   $($patch.Name): already applied"; continue }
    git -C $srcDir apply $patch.FullName
    if ($LASTEXITCODE) { throw "Patch $($patch.Name) does not apply." }
    Write-Host "   $($patch.Name): applied"
}

Write-Step 'Visual Studio and CMake'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$vsPath = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vsMajor = [int]((& $vswhere -latest -products * -property installationVersion) -split '\.')[0]
$generator = @{ 17 = 'Visual Studio 17 2022'; 18 = 'Visual Studio 18 2026' }[$vsMajor]
if (-not $generator) { throw "Unsupported Visual Studio major version $vsMajor." }
$cmake = Join-Path $vsPath 'Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe'
if (-not (Test-Path $cmake)) { throw 'CMake not found; install "C++ CMake tools for Windows".' }
Write-Host "   $generator, $(& $cmake --version | Select-Object -First 1)"

# Blender's Release flags (options.cmake), plus /MP for the Visual Studio generator and
# OPJ_STATIC from Blender's openimageio.diff.
$common = '/MD /MP /Wv:18 /bigobj /experimental:deterministic /utf-8 /arch:SSE4.2 /O2 /Ob2 /DNDEBUG /DPSAPI_VERSION=2 /DTINYFORMAT_ALLOW_WCHAR_STRINGS /DOPJ_STATIC'
$lib = $libDir.Replace('\', '/')
$deps = (Join-Path $WorkDir 'deps').Replace('\', '/')

# Static libraries OpenImageIO links in, built the way the rest of this script builds.
function Build-Dependency([string]$Name, [string]$Repository, [string]$Tag, [string[]]$ExtraArgs) {
    Write-Step "$Name $Tag"
    $depSrc = Join-Path $WorkDir "deps-src\$Name"
    if (-not (Test-Path (Join-Path $depSrc '.git'))) {
        git clone --quiet --depth 1 --branch $Tag $Repository $depSrc
        if ($LASTEXITCODE) { throw "git clone of $Name failed." }
    }
    $depBuild = Join-Path $WorkDir "deps-build\$Name"
    & $cmake -S $depSrc -B $depBuild -G $generator -A x64 "-DCMAKE_INSTALL_PREFIX=$deps/$Name" `
        "-DCMAKE_C_FLAGS_RELEASE=$common" "-DCMAKE_CXX_FLAGS_RELEASE=$common" -DBUILD_SHARED_LIBS=OFF @ExtraArgs
    if ($LASTEXITCODE) { throw "Configuring $Name failed." }
    & $cmake --build $depBuild --config Release --parallel
    if ($LASTEXITCODE) { throw "Building $Name failed." }
    & $cmake --install $depBuild --config Release
    if ($LASTEXITCODE) { throw "Installing $Name failed." }
}

Build-Dependency 'libdeflate' 'https://github.com/ebiggers/libdeflate.git' 'v1.23' @(
    '-DLIBDEFLATE_BUILD_SHARED_LIB=OFF', '-DLIBDEFLATE_BUILD_GZIP=OFF')
$deflateLib = (Get-ChildItem "$deps/libdeflate/lib" -Filter *.lib | Select-Object -First 1).FullName.Replace('\', '/')

Build-Dependency 'libtiff' 'https://gitlab.com/libtiff/libtiff.git' 'v4.7.1' @(
    "-DZLIB_LIBRARY=$lib/zlib/lib/libz_st.lib", "-DZLIB_INCLUDE_DIR=$lib/zlib/include",
    "-DJPEG_LIBRARY=$lib/jpeg/lib/libjpeg.lib", "-DJPEG_INCLUDE_DIR=$lib/jpeg/include",
    "-DDeflate_LIBRARY=$deflateLib", "-DDeflate_INCLUDE_DIR=$deps/libdeflate/include",
    '-Dzlib=ON', '-Djpeg=ON', '-Dlibdeflate=ON', '-Dlzma=OFF', '-Djbig=OFF', '-Dzstd=OFF',
    '-Dwebp=OFF', '-Dlerc=OFF', '-Dcxx=OFF', '-Dtiff-tools=OFF', '-Dtiff-tests=OFF',
    '-Dtiff-contrib=OFF', '-Dtiff-docs=OFF')

$cmakeArgs = @(
    '-S', $srcDir, '-B', $buildDir, '-G', $generator, '-A', 'x64',
    "-DCMAKE_INSTALL_PREFIX=$($InstallDir.Replace('\', '/'))",
    '-DCMAKE_CXX_STANDARD=20',
    "-DCMAKE_C_FLAGS_RELEASE=$common",
    "-DCMAKE_CXX_FLAGS_RELEASE=$common /D_SILENCE_ALL_CXX17_DEPRECATION_WARNINGS /DPLATFORM_WINDOWS",
    '-DOIIO_LIBNAME_SUFFIX=_Cycles',
    '-DBUILD_SHARED_LIBS=ON', '-DLINKSTATIC=ON',
    # Blender lists libjpeg-turbo, but the bundle has no CMake package for it; found as plain
    # JPEG it is the same library, and OpenImageIO's code never tests USE_JPEG_TURBO.
    '-DOpenImageIO_REQUIRED_DEPS=WebP;JPEG;TIFF;OpenEXR;PNG;OpenJPEG;fmt;Robinmap;ZLIB;pugixml;openjph;TBB',
    '-DOpenImageIO_BUILD_MISSING_DEPS=Robinmap',
    "-DTIFF_LIBRARY=$deps/libtiff/lib/tiff.lib", "-DTIFF_INCLUDE_DIR=$deps/libtiff/include",
    # libtiff is static and needs libdeflate; OpenImageIO only links ZLIB and JPEG beside it.
    "-DCMAKE_SHARED_LINKER_FLAGS=$deflateLib", '-DCMAKE_SHARED_LINKER_FLAGS_RELEASE=/INCREMENTAL:NO',
    '-DUSE_LIBHEIF=OFF', '-DUSE_PYTHON=OFF', '-DOIIO_BUILD_TOOLS=OFF',
    '-DUSE_NUKE=OFF', '-DUSE_OPENVDB=OFF', '-DUSE_FREETYPE=OFF', '-DUSE_DCMTK=OFF', '-DUSE_TBB=ON',
    '-DUSE_QT=OFF', '-DUSE_GIF=OFF', '-DUSE_OPENCV=OFF', '-DUSE_OPENJPEG=ON', '-DUSE_FFMPEG=OFF',
    '-DUSE_PTEX=OFF', '-DUSE_LIBRAW=OFF', '-DUSE_JXL=OFF', '-DUSE_OPENCOLORIO=ON', '-DUSE_WEBP=ON',
    '-DUSE_SIMD=sse4.2', '-DSTOP_ON_WARNING=OFF',
    '-DOIIO_BUILD_TESTS=OFF', '-DBUILD_TESTING=OFF', '-DBUILD_DOCS=OFF', '-DINSTALL_DOCS=OFF', '-DINSTALL_FONTS=OFF',
    # The payload ships its own MSVC runtime.
    '-DCMAKE_INSTALL_SYSTEM_RUNTIME_LIBS_SKIP=ON',
    "-DZLIB_LIBRARY=$lib/zlib/lib/libz_st.lib", "-DZLIB_INCLUDE_DIR=$lib/zlib/include",
    "-DPNG_LIBRARY=$lib/png/lib/libpng.lib", "-DPNG_PNG_INCLUDE_DIR=$lib/png/include",
    "-DJPEG_LIBRARY=$lib/jpeg/lib/libjpeg.lib", "-DJPEG_INCLUDE_DIR=$lib/jpeg/include",
    # The bundle's OpenJPEG CMake package names tools the bundle leaves out, which is fatal;
    # ignore it so OpenImageIO's FindOpenJPEG module finds the library instead.
    "-DOpenJPEG_ROOT=$lib/openjpeg", "-DCMAKE_IGNORE_PATH=$lib/openjpeg/lib/cmake/openjpeg-2.5",
    '-DOpenJPEG_DIR=OpenJPEG_DIR-NOTFOUND',
    '-DUSE_EXTERNAL_PUGIXML=ON', "-Dpugixml_DIR=$lib/pugixml/lib/cmake/pugixml",
    "-DPUGIXML_LIBRARY=$lib/pugixml/lib/pugixml.lib", "-DPUGIXML_INCLUDE_DIR=$lib/pugixml/include",
    "-DOpenColorIO_DIR=$lib/opencolorio/lib/cmake/OpenColorIO",
    "-DFMT_INCLUDE_DIR=$lib/fmt/include", "-Dfmt_ROOT=$lib/fmt",
    "-DWebP_ROOT=$lib/webp", "-DOpenEXR_ROOT=$lib/openexr", "-DImath_ROOT=$lib/imath",
    "-DTBB_ROOT=$lib/tbb", "-Dopenjph_DIR=$lib/openjph/lib/cmake/openjph"
)

Write-Step 'Configure'
& $cmake @cmakeArgs
if ($LASTEXITCODE) { throw 'CMake configure failed.' }

Write-Step 'Build'
& $cmake --build $buildDir --config Release --parallel
if ($LASTEXITCODE) { throw 'Build failed.' }

Write-Step "Install to $InstallDir"
if (Test-Path $InstallDir) { Remove-Item -Recurse -Force $InstallDir }
& $cmake --install $buildDir --config Release
if ($LASTEXITCODE) { throw 'Install failed.' }

# What this build exists to guarantee.
$dll = Join-Path $InstallDir 'bin\OpenImageIO_Cycles.dll'
if (-not (Test-Path $dll)) { throw "Expected $dll after install." }
$text = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($dll))
if ($text -match 'heif_context|aom\.dll') { throw 'OpenImageIO_Cycles.dll still contains libheif or imports aom.' }
Write-Host "   OK: $((Get-ChildItem (Join-Path $InstallDir 'bin') -Filter *.dll | ForEach-Object Name) -join ', '); no libheif, no aom"
