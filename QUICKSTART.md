# Cycles quick start

For developers working on Cycles. Everyone else needs none of this - a normal
Rhino build uses the prebuilt Cycles and knows nothing about it.

## Set up, once

    bootstrap.exe /cycles

Installs the CUDA toolkit and the OptiX headers. ROCm it cannot install, so it
offers you the download page - you only need ROCm if you have an AMD card, or if
you publish a payload.

## Build

Open `src4/BuildSolutions/Rhino.sln`, pick **Debug+Cycles**, build the solution.

`Debug` and `Release` do not build Cycles at all. They use the prebuilt one.

## What that builds

Kernels for the GPUs in your machine, and nothing else. Minutes, not an hour.

Kernels for other vendors' cards are copied in from the published payload, so
everything still works - but they are as old as the last publish and do not
contain your changes.

If your machine has a GPU whose SDK you have not installed, the build says so
loudly: your own renders will not show your kernel changes.

## Payloads

A payload is `ccycles.dll` plus its kernels. They live in
`big_libs/RhinoCycles/ccycles/win`.

| | |
| --- | --- |
| `release` | committed, what everyone gets |
| `local` | what you just built, ignored by git |
| `debug` | the same, for Debug builds |

Your Rhino uses `local` while it is newer than `release`. Pull a newer `release`
and it takes over again by itself.

**Your build never overwrites `release`.** That is on purpose: it holds kernels
for one machine's GPU, and everyone else needs all of them.

## Build all the kernels

    powershell -File publish_payload.ps1

Do this when you change kernel code, before it merges. Otherwise everyone gets
your new `ccycles.dll` with the old kernels, which is not slow but wrong.

Needs CUDA, OptiX and ROCm installed - oneAPI needs nothing. No GPU of any kind
is needed to build kernels for it. Takes about an hour.

It builds everything, checks the result, and stages the payload in `big_libs`.
Then commit it, and commit the `big_libs` pointer in the Rhino repo.

## Check before you commit

    powershell -File tools/run_checks.ps1

A second. Catches the usual silent breakage, and tells you if the payload no
longer matches the kernel sources.

---

Longer version, and why any of it is like this: [RHINO-CYCLES-5.md](RHINO-CYCLES-5.md).
