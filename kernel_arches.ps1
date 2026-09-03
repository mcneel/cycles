<#
.SYNOPSIS
    The GPU architectures Rhino ships Cycles kernels for.

.DESCRIPTION
    Dot-sourced by build_cycles.ps1, which passes these to CMake with -D, and by
    publish_payload.ps1, which uses them to check that a payload actually contains
    what it claims. One file so the two cannot drift: a publish that verified against
    its own copy of the list would pass while shipping something else.

    These are ours rather than upstream's on purpose. Upstream's defaults are wrong for
    us in both directions - its CYCLES_HIP_BINARIES_ARCH was last touched in June 2024
    and has no RDNA4, and its CYCLES_CUDA_BINARIES_ARCH still names Kepler, which CUDA
    12 dropped but which would still build here through the optional CUDA 11 toolkit.

    Passing them with -D keeps upstream's CMakeLists untouched, so it takes merges
    cleanly.

    Adding a GPU generation is one line. Note that no hardware of that generation is
    needed to build for it: nvcc and hipcc both cross-compile, verified by building
    gfx1201 on a gfx1150-only machine. It does mean nobody can *test* it here, which
    is what the render verifier is for.
#>

# AMD, HIP. 22 targets.
#
# gfx906, gfx1152, gfx1200 and gfx1201 are the additions over upstream. The RDNA4 pair
# matters most: without them an RX 9070 finds no kernel and falls back to the CPU. They
# were already known in this tree - sitting in make_hip.sh, which the Windows build
# never called.
$CyclesHipShippingArches = @(
    'gfx900', 'gfx902', 'gfx906', 'gfx90c'                                  # Vega
    'gfx1010', 'gfx1011', 'gfx1012'                                         # RDNA
    'gfx1030', 'gfx1031', 'gfx1032', 'gfx1034', 'gfx1035', 'gfx1036'        # RDNA2
    'gfx1100', 'gfx1101', 'gfx1102', 'gfx1103'                              # RDNA3
    'gfx1150', 'gfx1151', 'gfx1152'                                         # RDNA3.5 APUs
    'gfx1200', 'gfx1201'                                                    # RDNA4
)

# NVIDIA, CUDA. 8 cubins plus one PTX fallback.
#
# compute_75 is a virtual architecture, so it produces PTX that the driver JITs for
# anything unlisted - newer cards included. It is a fallback, not a substitute: the
# payload once contained only kernel_compute_52.ptx.zst and nothing else, which meant
# every NVIDIA card ran code generated for a Maxwell virtual architecture, with none of
# upstream's per-architecture tuning applied.
$CyclesCudaShippingArches = @(
    'sm_52'                       # Maxwell   - GTX 900
    'sm_60', 'sm_61'              # Pascal    - GTX 10xx, Quadro P
    'sm_70'                       # Volta     - Titan V, V100
    'sm_75'                       # Turing    - GTX 16xx, RTX 20xx
    'sm_86'                       # Ampere    - RTX 30xx
    'sm_89'                       # Ada       - RTX 40xx
    'sm_120'                      # Blackwell - RTX 50xx
    'compute_75'                  # PTX fallback for everything else
)

# The OptiX modules. Architecture-independent PTX compiled by the OptiX runtime at
# load, so there is no list to choose - but a payload missing one of these is a payload
# that will fail at runtime on an NVIDIA card, so publish checks for them by name.
$CyclesOptixModules = @(
    'kernel_optix'
    'kernel_optix_mnee'
    'kernel_optix_shader_raytrace'
    'kernel_optix_osl'
    'kernel_optix_osl_camera'
    'kernel_optix_osl_mnee'
    'kernel_optix_osl_services'
    'kernel_optix_osl_shader_raytrace'
    'kernel_optix_osl_volume'
)
