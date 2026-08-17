# C[CS]?ycles — a C and C# API for Cycles

This repository provides a C API around [Cycles](https://projects.blender.org/blender/cycles)
(`ccycles`) and a C# wrapper over it (`csycles`), used by the RhinoCycles
render plug-in.

See [BUILDING.md](BUILDING.md). Short version: you probably do not need to
build Cycles — just build `Rhino.sln`.

## Layout

| Path | What it is |
| --- | --- |
| `cycles/` | Submodule: the McNeel Cycles fork (`mcneel/cycles`). |
| `cycles/src/ccycles/` | The C API. Lives inside the fork and is built by the Cycles CMake build. |
| `csycles/` | The C# wrapper. Built by `Rhino.sln`. |
| `ccycles.vcxproj` | Drives the Cycles CMake build from `Rhino.sln`. |
| `tools/` | Shader exporter helper for Blender. |
| `versioninfo_changer.ps1` | Stamps resources onto prebuilt third-party DLLs. |

## Relationship to upstream Cycles

`mcneel/cycles` is a fork of upstream Cycles carrying Rhino-specific additions.
The bulk of it is additive — Rhino procedural textures in SVM, the Rhino shader
nodes, and the `ccycles` C API. A smaller set of upstream files is modified in
place, mostly around texture coordinates, image sampling, shader graph handling
and film output.

Dependencies (OpenImageIO, OpenEXR, OpenVDB, TBB, OpenColorIO and the GPU
toolkits) come from Blender's precompiled library bundle, fetched by
`make update` into `cycles/lib/`.

## Note on OSL

There is no OSL support. RhinoCycles uses SVM.

## License

Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
