param (
    [Parameter(Mandatory=$true)]
    [ValidateSet('Debug','Release')]
    [string]$buildConfig
)

$cwd = $PSScriptRoot

Import-Module 'C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\Tools\Microsoft.VisualStudio.DevShell.dll'

Enter-VsDevShell -VsInstallPath "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools" -StartInPath $cwd

$cycles_lib_dir = (Convert-Path "$cwd\..\lib")
$install_location = (Convert-Path "$cwd\..\..\..\..\..\bin\$buildConfig\Plug-ins")

$dependencies = (Get-ChildItem -Recurse -Path $cycles_lib_dir -Include "OpenColorIO*dll", "openvdb.dll", "Imath*.dll", "boost_thread*dll", "OpenImageIO*dll", "tbb*dll", "OpenEXR*dll", "Iex*dll", "Ilm*dll")

Push-Location $cwd

.\make_rhino.bat $buildConfig.ToLower()

Pop-Location

foreach($dependency in $dependencies)
{
    Write-Host "Copying $dependency"
    Copy-Item "$dependency" "$install_location\."
}

$ccycles_files = Get-ChildItem "$cwd\build\bin\$buildConfig\ccycl*"
Write-Host "Copying $ccycles_files"
Copy-Item "$cwd\build\bin\$buildConfig\ccycl*" "$install_location\."