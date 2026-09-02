Cycles Renderer
===============

Cycles is a path tracing renderer focused on interactivity and ease of use, while supporting many production features.

https://www.cycles-renderer.org

## Building

Cycles can be built as a standalone application or a Hydra render delegate. See [BUILDING.md](BUILDING.md) for instructions.

## Examples

The repository contains example xml scenes which could be used for testing.

Example usage:

    ./cycles scene_monkey.xml

You can also use optional parameters (see `./cycles --help`), like:

    ./cycles --samples 100 --output ./image.png scene_monkey.xml

For the OSL scene you need to enable the OSL shading system:

    ./cycles --shadingsys osl scene_osl_stripes.xml
	
## Instructions for updating ccycles.dll

How to Update ccycles.dll Properly

Ensure both the **Mac** and the **Windows** sections are completed.

### Mac

1. **For Apple Silicon devices:**
   - Ensure you have the universal binaries.
     Download them [here](https://drive.google.com/file/d/10UxUQBOm9kRH1y6GN1NXLdFU4dpu-dCq/view?usp=sharing).
   - Use the Terminal to delete the quarantine attribute from the zip-file: `xattr -d com.apple.quarantine darwin_universal.zip`.
   - Unzip the binaries to `RDK/cycles/lib/darwin_universal`.

2. **For non-Apple Silicon devices:**
   - Navigate to `RDK/cycles/cycles`.
   - Run the command `make update`.

3. Go to `RDK/cycles/cycles` and run `make clean && make release`.

4. Make the built `libccycles.dylib` portable (strips the machine-specific absolute rpaths `make release` bakes in and adds portable `@loader_path` rpaths so the RhinoCycles kernel compiler can find the Cycles dependency dylibs — see RH-96549):

 ```
./fix-cycles-rpaths.sh
 ```

5. Execute `cp -r install/* ../../../../../../big_libs/RhinoCycles/ccycles/osx/release/`.

   Steps 3-5 as one command, from `RDK/cycles/cycles`:

 ```
make clean && make release && ./fix-cycles-rpaths.sh && cp -r install/* ../../../../../../big_libs/RhinoCycles/ccycles/osx/release/
 ```

6. Go to `big_libs`, create a branch if needed and execute:

 ```
git add -f RhinoCycles/ccycles/osx/release/libccycles.dylib
git commit
git push <branch_name>
 ```

### Windows

1. Checkout the `big_libs` branch made above.

2. Navigate to `RDK/cycles/cycles`.

3. Execute `rm -fr build/`.

4. Run `./make_rhino.bat release all`.
- This will end with a build error.

5. Open `Cycles.sln`.
- Switch the target to Release.
- Right-click the `cycles_device` project and select `Open folder in File Explorer`.

6. Open `cycles_device.vcxproj` with a text editor.
- Search for the string `/J /bigobj`.
- Ensure you found the one for the Release target and change it to `/J /bigobj /Zc:__cplusplus`.

7. Go back to `Cycles.sln`, reload the project, and then press `Build`.

8. After the build is finished, explicitly build the `INSTALL` project.

9. Download [ResourceHacker](http://www.angusj.com/resourcehacker/) if you haven't and ensure the executable is in Windows' PATH.

10. Navigate to `RDK/cycles` with PowerShell and execute the script:
 ```
 .\versioninfo_changer.ps1
 ```

11. Copy the following two files:
 - `RDK/cycles/cycles/install/ccycles.dll`
 - `RDK/cycles/cycles/install/cycles_kernel_oneapi_jit.dll`

 To the following folder: `big_libs\RhinoCycles\ccycles\win\release`

12. Go to `big_libs` and execute:
 ```
 git add -f RhinoCycles/ccycles/win/release/ccycles.dll RhinoCycles/ccycles/win/release/cycles_kernel_oneapi_jit.dll
 git commit
 git push
 ```

## Contact

For help building or running Cycles, see the channels listed here:

https://www.cycles-renderer.org/development/
