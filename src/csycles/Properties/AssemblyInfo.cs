/**
Copyright 2014-2024 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("csycles")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Robert McNeel and Associates")]
[assembly: AssemblyProduct("csycles")]
[assembly: AssemblyCopyright("Copyright © 2014-2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Setting DefaultDllImportSearchPaths to AssemblyDirectory | UseDllDirectoryForDependencies | System32
// Ensures Csycles will load its dependencies only from the Rhino\Plug-ins folder or
// from Windwos\System32 folder but never will try from the Appliction folder, where the .exe is located.
// This is especially significant when Rhino is loaded as DLL
//
// ** This attribute has no effect on non-Windows platforms or the Mono runtime.** 
// 
// See also:
// https://docs.microsoft.com/windows/desktop/dlls/dynamic-link-library-search-order
// https://msdn.microsoft.com/library/system.runtime.interopservices.dllimportsearchpath
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.UseDllDirectoryForDependencies | DllImportSearchPath.System32)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0d4bcd1a-282c-492e-a615-37efb0a56927")]
