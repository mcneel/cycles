$ErrorActionPreference = 'Stop'

$cwd = $PSScriptRoot

#$configs = @('Debug', 'Release')
$configs = @('Release')
$tuneup = (Convert-Path "tune_dylibs.py")

foreach($buildConfig in $configs)
{
    $lowerconfig = $buildConfig.ToLower()

    #create dynamic list to track visited libraries
    $visited_libs = New-Object System.Collections.Generic.HashSet[string]

    New-Item -Type Directory -Force "$cwd\build_$lowerconfig"
    New-Item -Type Directory -Force "$cwd\install_$lowerconfig"

    $biglibs_location = (Convert-Path "$cwd\..\..\..\..\..\..\big_libs\RhinoCycles")
    $biglibs_kernel_location = (Convert-Path "$biglibs_location\lib")
    $biglibs_source_location = (Convert-Path "$biglibs_location\ccycles\source")
    New-Item -Type Directory -Force "$biglibs_location\ccycles\osx\$lowerconfig"
    $biglibs_dll_location = (Convert-Path "$biglibs_location\ccycles\osx\$lowerconfig")
    $biglibs_deps_location = (Convert-Path "$biglibs_location\ccycles\osx\deps\$lowerconfig")
    $local_build = (Convert-Path "$cwd\build_$lowerconfig")
    $local_install = (Convert-Path "$cwd\install_$lowerconfig")
    $local_install_release = (Convert-Path "$cwd\install_release")
    #$local_install_debug = (Convert-Path "$cwd\install_debug")
    
    Push-Location $cwd

    make $lowerconfig

    Pop-Location

    Remove-Item -Confirm -Recurse -Force "$biglibs_dll_location\*"

    $ccycles = (Convert-Path "$local_install\libccycles.dylib")

    function Copy-RPathDylibsFromOtool($original_dylib, $biglibs_dll_location, $biglibs_deps_location) {

        if($visited_libs.Contains($original_dylib)) {
            return
        }

        $otool = (otool -L $original_dylib)
        $visited_libs.Add($original_dylib)
        foreach($line in $otool) {
            if ($line -match 'rpath') {
                #@rpath/libtbb.12.dylib (compatibility version 12.0.0, current version 12.13.0)
                $line = $line.Trim()
                $parts = $line.Split(' ')
                $dylib_path = $parts[0]
                $parts = $dylib_path.Split('/')
                $dylib_name = $parts[1]

                #Substitute version number in dylib name with empty string
                #$dylib_name = $dylib_name -replace '\.\d+',''

                Write-Host $dylib_name

                $dylibpath = Get-ChildItem -Path . -Recurse  -Include $dylib_name

                if ($null -ne $dylibpath) {
                    Write-Host $dylibpath
                    Copy-Item $dylibpath $biglibs_dll_location
                    Copy-RPathDylibsFromOtool $dylibpath $biglibs_dll_location $biglibs_deps_location
                }
                else {
                    $actual_dylib = Get-ChildItem -Path $biglibs_deps_location -Recurse  -Include $dylib_name
                    Copy-Item $actual_dylib $biglibs_dll_location  
                    Copy-RPathDylibsFromOtool $actual_dylib $biglibs_dll_location $biglibs_deps_location
                }
            }
        }
    }

    Copy-RPathDylibsFromOtool $ccycles $biglibs_dll_location $biglibs_deps_location

    Push-Location $biglibs_dll_location

    python3 $tuneup

    Pop-Location

    $dylibs = Get-ChildItem -Path $biglibs_dll_location -Filter *.dylib | Where-Object { $_.Name -notmatch "crh" }
    foreach($dylib in $dylibs) {
        Remove-Item $dylib
    }
}
