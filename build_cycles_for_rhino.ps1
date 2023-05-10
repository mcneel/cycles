$cwd = $PSScriptRoot
$cycles_lib_dir = (Convert-Path "$cwd\..\lib")
$install_location_debug = (Convert-Path "$cwd\..\..\..\..\..\bin\Debug\Plug-ins")

$dependencies = (Get-ChildItem -Recurse -Path $cycles_lib_dir -Include "OpenColorIO*dll", "openvdb.dll", "Imath*.dll", "boost_thread*dll", "OpenImageIO*dll", "tbb*dll", "OpenEXR*dll", "Iex*dll", "Ilm*dll")

pushd $cwd

./make_rhino.bat debug

popd

foreach($dependency in $dependencies)
{
	Copy-Item "$dependency" "$install_location_debug\."
}

Copy-Item "$cwd\build\bin\Debug\ccycl*" "$install_location_debug\."
