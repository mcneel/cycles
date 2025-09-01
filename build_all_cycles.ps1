$cwd = $PSScriptRoot

$configs = @('Debug', 'Release')

foreach($buildConfig in $configs)
{
    $lowerconfig = $buildConfig.ToLower()

    New-Item -Type Directory -Force "$cwd\build_$lowerconfig"
    New-Item -Type Directory -Force "$cwd\install_$lowerconfig"

    $install_location = (Convert-Path "$cwd\..\..\..\..\..\bin\$buildConfig\Plug-ins")
    $biglibs_location = (Convert-Path "$cwd\..\..\..\..\..\..\big_libs\RhinoCycles")
    $biglibs_kernel_location = (Convert-Path "$biglibs_location\lib")
    $biglibs_source_location = (Convert-Path "$biglibs_location\ccycles\source")
    $biglibs_dll_location = (Convert-Path "$biglibs_location\ccycles\win\$lowerconfig")
    $local_build = (Convert-Path "$cwd\build_$lowerconfig")
    $local_install = (Convert-Path "$cwd\install_$lowerconfig")
    $local_install_release = (Convert-Path "$cwd\install_release")
    $local_install_debug = (Convert-Path "$cwd\install_debug")
    
    #Remove-Item -Recurse -Force .\install

    Push-Location $cwd

    .\make_rhino.bat $lowerconfig all

    Pop-Location

    # For release builds fix up DLLs with version info
    # and manifests
    if($buildConfig -eq 'Release') {
        $up = (Convert-Path "$cwd\..")
        Push-Location $up

        & ".\versioninfo_changer.ps1"

        Pop-Location
    }

    Copy-Item $local_build\bin\ccycles.pdb $local_install

    $dependencies = Get-ChildItem -Path $local_install -Recurse -File -Include *.dll, *.pdb | `
                        Where-Object { `
                            $_.Name.ToLower() -notmatch '^(api|material|msvc|shader|vcrun|conc|ucrt|ur_lo|ur_ad|epo)' `
                            -and $_.Name.ToLower() -notmatch 'malloc' `
                            -and $_.Name.ToLower() -notmatch 'exruti' `
                            -and $_.Name.ToLower() -notmatch 'denoise' `
                            -and $_.Name.ToLower() -notmatch 'osl' `
                        }

    # Now filter list to choose between debug or non-debug version (_d.dll or not)
    $deps = foreach ($f in $dependencies) {
        $name = $f.Name

        if ($name -match '^(.*)\.pdb$')
        {
            $f
        }
        else {

            if ($buildConfig -eq 'Debug') {
                # Prefer the *_d.dll variant if it exists
                if ($name -match '^(.*)_d\.dll$') {
                    $f
                }
                elseif (-not ($dependencies.Name -contains ($name -replace '\.dll$', '_d.dll'))) {
                    # Include base dll only if no *_d.dll exists
                    $f
                }
            }
            elseif ($buildConfig -eq 'Release') {
                # Prefer the base .dll variant if it exists
                if ($name -match '^(.*)\.dll$' -and $name -notmatch '_d\.dll') {
                    $f
                }
                elseif (-not ($dependencies.Name -contains ($name -replace '_d\.dll$', '.dll'))) {
                    # Include *_d.dll only if no base dll exists
                    $f
                }
            }
        }
    }

    # further clean up
    if($buildConfig -eq 'Debug') {
        $dependencies = $deps | Where-Object {
            $_.Name.ToLower() -notmatch 'o_2' `
            -and $_.Name.ToLower() -notmatch 'loader.dll'
        }
        # add back openvdb, since that is always linked as openvdb.dll
        $openvdb = (Convert-Path "$local_install_release\ccycles\openvdb.dll")
        $dependencies += Get-Item $openvdb
        # also need tbb12.dll
        $tbb12 = (Convert-Path "$local_install_release\ccycles\tbb12.dll")
        $dependencies += Get-Item $tbb12
        # and OpenColorIO_d_2_4.dll
        $oiio = (Convert-Path "$local_install_release\ccycles\OpenColorIO_d_2_4.dll")
        $dependencies += Get-Item $oiio
    }
    else {
        $dependencies = $deps | Where-Object {
            $_.Name.ToLower() -notmatch '8d.dll' `
            -and $_.Name.ToLower() -notmatch 'loaderd.dll' `
            -and $_.Name.ToLower() -notmatch 'o_d_2' `
            -and $_.Name.ToLower() -notmatch 'debug'
        }
    }

    Remove-Item -Recurse -Force "$biglibs_kernel_location\*"
    Copy-Item -Recurse -Force "$local_install\lib\*" "$biglibs_kernel_location\."
    Remove-Item -Recurse -Force "$biglibs_source_location\*"
    Copy-Item -Recurse -Force "$local_install\source\*" "$biglibs_source_location\."

    Remove-Item -Recurse -Force "$biglibs_dll_location\*"
    foreach($dependency in $dependencies)
    {
        Write-Host "${buildConfig}: Copying $dependency"
        Copy-Item "$dependency" "$install_location\."
        Copy-Item "$dependency" "$biglibs_dll_location\."
    }
}
