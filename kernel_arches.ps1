<#
.SYNOPSIS
    Which Cycles kernel architectures Rhino ships, and a hash of their sources.

.DESCRIPTION
    Dot-sourced by build_cycles.ps1 (passes them to CMake with -D) and publish_payload.ps1
    (verifies the payload against them), so the two cannot drift. Ours, not upstream's:
    its HIP list has no RDNA4 and its CUDA list still names Kepler. Passing them with -D
    keeps upstream's CMakeLists untouched. nvcc and hipcc cross-compile: building for a GPU
    generation needs no hardware of it, only testing does.
#>

# AMD, HIP. gfx906, gfx1152, gfx1200 and gfx1201 are additions over upstream; without the
# RDNA4 pair an RX 9070 finds no kernel and falls back to the CPU.
$CyclesHipShippingArches = @(
    'gfx900', 'gfx902', 'gfx906', 'gfx90c'                                  # Vega
    'gfx1010', 'gfx1011', 'gfx1012'                                         # RDNA
    'gfx1030', 'gfx1031', 'gfx1032', 'gfx1034', 'gfx1035', 'gfx1036'        # RDNA2
    'gfx1100', 'gfx1101', 'gfx1102', 'gfx1103'                              # RDNA3
    'gfx1150', 'gfx1151', 'gfx1152'                                         # RDNA3.5 APUs
    'gfx1200', 'gfx1201'                                                    # RDNA4
)

# NVIDIA, CUDA, from Cycles' own floor of 5.0 (device_impl.cpp). Do not drop old cubins:
# CUDADevice only walks PTX *down* from a card's capability, so compute_75 never reaches a
# card below 7.5 - it gets no CUDA kernel and renders on the CPU. To trim them, add a low
# PTX (compute_52) instead so old cards JIT.
$CyclesCudaShippingArches = @(
    'sm_50', 'sm_52'              # Maxwell   - GTX 750 Ti, GTX 900, 940M
    'sm_60', 'sm_61'              # Pascal    - GTX 10xx, Quadro P
    'sm_70'                       # Volta     - Titan V, V100
    'sm_75'                       # Turing    - GTX 16xx, RTX 20xx
    'sm_86'                       # Ampere    - RTX 30xx
    'sm_89'                       # Ada       - RTX 40xx
    'sm_120'                      # Blackwell - RTX 50xx
    'compute_75'                  # PTX fallback for Turing and newer
)

# OptiX modules: architecture-independent, but publish checks them by name since a missing
# one fails at runtime. No kernel_optix_osl*: Cycles is built with WITH_CYCLES_OSL=OFF.
$CyclesOptixModules = @(
    'kernel_optix'
    'kernel_optix_mnee'
    'kernel_optix_shader_raytrace'
)

# Minimum driver branch per PTX ISA version: a driver refuses newer PTX (RH-87727, RH-98331).
# publish_payload.ps1 fails on a version missing here on purpose - add the row from that
# CUDA release's notes.
$CyclesPtxIsaMinimumDriver = [ordered]@{
    '7.8' = 520   # CUDA 11.8
    '8.0' = 525   # CUDA 12.0
    '8.1' = 530   # CUDA 12.1
    '8.2' = 535   # CUDA 12.2
    '8.3' = 545   # CUDA 12.3
    '8.4' = 550   # CUDA 12.4
    '8.5' = 555   # CUDA 12.5, 12.6
    '8.7' = 570   # CUDA 12.8
    '8.8' = 575   # CUDA 12.9
    '9.0' = 580   # CUDA 13.0
}

# Oldest driver per OptiX SDK; below it there are no OptiX devices, but CUDA still renders.
# Keep in step with device_optix_minimum_driver() in src/device/optix/device.cpp.
$CyclesOptixSdkMinimumDriver = [ordered]@{
    '8.0' = 535
    '8.1' = 555
    '9.0' = 570
    '9.1' = 590
}

# Oldest driver for the cubins, per CUDA major (minor version compatibility).
$CyclesCudaMajorMinimumDriver = [ordered]@{
    '12' = 525
    '13' = 580
}

# Kernel source hash, recorded in ccycles_payload.json; build_cycles.ps1 compares it to the
# tree to spot a stale payload. Covers src/util too: kernel/types.h includes util headers,
# but the kernel build only tracks src/kernel, so a util edit gives stale kernels.
function Get-CyclesKernelSourceHash {
    param([Parameter(Mandatory)][string]$CyclesRoot)

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $lines = [System.Collections.Generic.List[string]]::new()
        foreach ($sub in 'src\kernel', 'src\util') {
            $root = Join-Path $CyclesRoot $sub
            if (-not (Test-Path $root)) { continue }
            foreach ($file in Get-ChildItem $root -Recurse -File) {
                $rel = $file.FullName.Substring($CyclesRoot.Length).Replace('\', '/')
                $fileHash = [BitConverter]::ToString($sha.ComputeHash([IO.File]::ReadAllBytes($file.FullName))).Replace('-', '')
                $lines.Add("$rel $fileHash")
            }
        }

        # Ordinal: a culture-aware sort made the hash differ between pwsh 7 and 5.1.
        $lines.Sort([System.StringComparer]::Ordinal)

        # Explicit "`n": Environment.NewLine would hash the same tree differently on a Mac.
        $bytes = [Text.Encoding]::UTF8.GetBytes(($lines -join "`n") + "`n")
        return [BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-', '').ToLowerInvariant()
    }
    finally { $sha.Dispose() }
}
