# Diagnostic switches

Environment variables read by Cycles, ccycles and RhinoCycles. Set them before
starting Rhino; several are read only once per process.

| Variable | What it does | Output |
| --- | --- | --- |
| `CCYCLES_DIAG_LOG=<file>` | Turns on ccycles' diagnostics: unknown node types and sockets, rejected enum values, refused connections, pass readback failures, and the compiled flags of every shader once per process. Also routes Cycles' own log to the same place. | Appended to `<file>`; also stderr and `OutputDebugString` |
| `CCYCLES_LOG_LEVEL=<level>` | Level of Cycles' own log: `fatal`, `error`, `warning`, `info` (default), `debug`, `trace`. Needs `CCYCLES_DIAG_LOG`. | The `CCYCLES_DIAG_LOG` file |
| `CCYCLES_DUMP_FINAL=<prefix>` | Dumps every shader graph after constant folding, which is the graph the SVM compiler sees. | `<prefix>_final_mat_<n>.dot`, `<prefix>_final_bg_<n>.dot` (graphviz) |
| `CCYCLES_DUMP_SVM=<file>` | One record per compiled SVM node: inputs with stack offset and link, or the constant if unlinked; outputs with offset and user count; `matrix_math` transforms; image texture parameters. | Appended to `<file>` |
| `CCYCLES_DUMP_IMAGES=<file>` | After each image upload, every image slot: size, channels, type, average colour, colour space, loader, and whether it was loaded. | Appended to `<file>` |
| `CYCLES_CPU_NO_AVX2` | Upstream. CPU kernel uses SSE4.2 instead of AVX2. | Log line `Disabling avx2 instruction set.` |
| `CYCLES_KERNEL_PATH=<dir>` | Upstream. Kernel sources (`source/`) from `<dir>` instead of the plug-in folder. Matters wherever kernels compile at runtime (Metal, adaptive compile). | none |
| `CYCLES_CUDA_ADAPTIVE_COMPILE`, `CYCLES_HIP_ADAPTIVE_COMPILE`, `CYCLES_METAL_ADAPTIVE_COMPILE` | Upstream. Ignore the precompiled kernels and compile at runtime, specialised to the scene's features. CUDA and HIP need the toolkit on the machine. | Log |
| `CYCLES_CUDA_EXTRA_CFLAGS`, `CYCLES_HIP_EXTRA_CFLAGS` | Upstream. Extra compiler flags for a runtime kernel compile. | none |
| `CYCLES_DEBUG_PER_KERNEL_PERFORMANCE` | Upstream. With `CCYCLES_LOG_LEVEL=trace`, synchronises after every GPU kernel so the per-kernel times in the log are accurate. | Log |
| `CUDA_CACHE_PATH` | NVIDIA's kernel JIT cache. If unset, RhinoCycles sets it to `RhinoCycles\KernelCache` in Rhino's local profile folder (Windows). | none |

Other upstream switches (`CYCLES_CONCURRENT_STATES_FACTOR`, Metal and oneAPI tuning,
OSL, OIDN, the volume octree dump) are unchanged; `grep -rn getenv src/` lists them.
The `RhinoCyclesReport` command lists every set variable starting with `CYCLES`,
`CUDA`, `HIP`, `OPTIX`, `OCIO`, `NVIDIA`, `AMD_` or `RHINO`.

## Reading the output

- **Shader graphs.** RhinoCycles' `DumpMaterialShaderGraph` and
  `DumpEnvironmentShaderGraph` settings (in its settings XML, not environment
  variables) write `rhinofullnxt_<id>.dot` and `rhinobg_<id>.dot` to the user's home
  folder, as RhinoCycles built them. `CCYCLES_DUMP_FINAL` shows the same graphs after
  folding. A closure that is in the first and missing from the second was folded away.
- **SVM.** An input's `off=` must equal the `off=` of the output it links to. Compare
  two builds by matching records by shader and node name, not by file size or line
  number: compile order differs between runs.
- **Images.** `need_load=1` after upload means the image never loaded; an average
  colour of zero means it loaded empty.
- **RhinoCycles log.** `RhinoCycles<timestamp>-<pid>-<salt>.log` in the RhinoCycles
  user data folder (`RhinoCycles_ShowPaths` prints it). Set `VerboseLogging` for the
  full trace. The file is flushed continuously, so a stalled render's log is current.
- **Which Cycles is loaded.** `RhinoCycles_ListDevices` prints the path, date and
  payload manifest of the `ccycles.dll` in use.
