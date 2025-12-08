$bd = [System.DateTime]::UtcNow

$yy = $bd.ToString("yy")
$doy = $bd.DayOfYear.ToString("D3")
$bhr = $bd.ToString("HH")
$bmm = $bd.Minute.ToString("D2")

$dotted = "9.0.$yy$doy.$bhr$bmm" + "1"
$commas = $dotted.Replace(".", ",")

Write-Host "-> $dotted"
Write-Host "-> $commas"

$cycles_install = ".\cycles\install_release"
$cycles_install_deps = ".\cycles\install_release\ccycles"

$depdlls = @{
    "Iex"            = "ccycles\Iex.dll"
    "IlmThread"      = "ccycles\IlmThread.dll"
    "Imath"          = "ccycles\Imath.dll"
    "OpenColorIO"    = "ccycles\OpenColorIO_2_4.dll"
    "OpenImageIO"    = "ccycles\OpenImageIO.dll"
    "OpenImage Util" = "ccycles\OpenImageIO_Util.dll"
    "OpenEXR"        = "ccycles\OpenEXR.dll"
    "OpenEXRCore"    = "ccycles\OpenEXRCore.dll"
}

# Write .rc files
# -- ccycles
(Get-Content .\dll_version_replace.template).Replace("VERSIONCOMMAS", $commas).Replace("VERSIONDOTS", $dotted) | Set-Content .\cycles\install_release\dll_version_replace.rc
# -- openvdb
(Get-Content .\openvdb_manifest_replace.template).Replace("VERSIONCOMMAS", $commas).Replace("VERSIONDOTS", $dotted) | Set-Content .\cycles\install_release\openvdb_manifest_replace.rc
# Write manifest files
# -- ccycles
(Get-Content .\ccycles_manifest.txt).Replace("VERSIONCOMMAS", $commas).Replace("VERSIONDOTS", $dotted) | Set-Content .\cycles\install_release\ccycles_manifest.txt
# -- openvdb
(Get-Content .\openvdb_manifest.txt).Replace("VERSIONCOMMAS", $commas).Replace("VERSIONDOTS", $dotted) | Set-Content .\cycles\install_release\openvdb_manifest.txt

Push-Location .\cycles\install_release

<#
Remove-Item *gyd* -ErrorAction SilentlyContinue
Remove-Item *_d.dll -ErrorAction SilentlyContinue
Remove-Item *.so -ErrorAction SilentlyContinue
Remove-Item *.so.* -ErrorAction SilentlyContinue
Remove-Item lib\*.so -ErrorAction SilentlyContinue
Remove-Item lib\*.so.* -ErrorAction SilentlyContinue
Remove-Item *_d_*.dll -ErrorAction SilentlyContinue
#>

cmd.exe /c "ResourceHacker -open .\dll_version_replace.rc -save .\dll_version_replace.res -action compile"
cmd.exe /c "ResourceHacker -open .\openvdb_manifest_replace.rc -save .\openvdb_manifest_replace.res -action compile"
cmd.exe /c "ResourceHacker -open ccycles\openvdb.dll -save ccycles\openvdb.dll -resource .\openvdb_manifest_replace.res -action addoverwrite -mask MANIFEST,,"
cmd.exe /c "ResourceHacker -open cycles_kernel_oneapi_aot.dll -save cycles_kernel_oneapi_aot.dll -resource .\dll_version_replace.res -action addoverwrite -mask VERSIONINFO,,"

foreach($key in $depdlls.Keys) {
    $depdll = $depdlls[$key]
    cmd.exe /c "ResourceHacker -open $depdll -save $depdll -resource .\dll_version_replace.res -action addoverwrite -mask VERSIONINFO,,"
}

Pop-Location

cmd.exe /c "ResourceHacker -script .\ccycles_resource_update.txt"

Write-Host "ready"
Write-Host ""
