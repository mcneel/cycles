Get-ChildItem -Path install -Filter "*.dll" | Where-Object { $_.Name -match "cycles" } | Copy-Item -Verbose -Destination ..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\.
Copy-Item -Verbose .\build\bin\*.pdb C:\Users\Testing\dev\rhino9\big_libs\RhinoCycles\ccycles\win\release\.
Copy-Item -Verbose .\install\lib\* C:\Users\Testing\dev\rhino9\big_libs\RhinoCycles\lib\.
Copy-Item .\install\*.dll ..\..\..\..\..\bin\Release\Plug-ins\.
Copy-Item .\build\bin\*.pdb ..\..\..\..\..\bin\Release\Plug-ins\.
Copy-Item .\install\lib\* ..\..\..\..\..\bin\Release\Plug-ins\RhinoCycles\lib\.
Copy-Item .\install\*.dll ..\..\..\..\..\bin\Debug\Plug-ins\.
Copy-Item .\build\bin\*.pdb ..\..\..\..\..\bin\Debug\Plug-ins\.
Copy-Item .\install\lib\* ..\..\..\..\..\bin\Debug\Plug-ins\RhinoCycles\lib\.
