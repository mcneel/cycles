from pathlib import Path
import subprocess as sp
import re

dll_matcher = r"^\s*(\S+).dll"

def is_excluded(name : str) -> bool:
    return name.startswith('api') \
        or 'cycles' in name \
        or 'RH' in name \
        or name.isupper()

processed_dlls = list()
all_deps = set()

def get_dependents(dll : Path):
    cmd = [
        'dumpbin',
        '/dependents',
        f'{dll}'
    ]
    print(f"running {cmd}")
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')

    # print(cp.stdout)

    dlls = list()
    matches = re.finditer(dll_matcher, cp.stdout, re.MULTILINE)
    for matchIdx, match in enumerate(matches, start=1):
        name = match.group(1).strip()
        # print(f"\t{name} - {match}")
        if is_excluded(name):
            continue
        dll_name = match.group(0).strip()
        dll_path = Path(dll_name)
        if not dll_path.exists():
            continue
        dll_fixed = name[:-2] + 'RH.dll'
        dlls.append((name, dll_name, dll_fixed, dll_path))
        all_deps.add(dlls[-1])

    return dlls


def tune_dll(dll : Path):
    print("=> Tuning", dll)
    if dll in processed_dlls:
        print("\t\talready done", dll)
        return
    processed_dlls.append(dll)

    deps = get_dependents(dll)
    if len(deps) > 0:
        dll_data = dll.read_bytes()
        for _, orig, target, dll_path in deps:
            print(f"replacing {orig} with {target}")
            dll_data = dll_data.replace(orig.encode(), target.encode())
        dll.write_bytes(dll_data)
    else:
        print(f"nothing to do for {dll}")

    for (_, _, _, dll_path) in deps:
        tune_dll(dll_path)

    print("done tuning", dll, "<=")


cc = Path('ccycles.dll')
tune_dll(cc)

cc = Path('cycles_kernel_oneapi_aot.dll')
tune_dll(cc)

print("\n\n===================\n\n")
for pd in processed_dlls:
    print(pd)

print("\n\n===================\n\n")
for name, orig_dll, fixed_dll, dll_path in all_deps:
    orig_dll = Path(orig_dll)
    fixed_dll = Path(fixed_dll)
    orig_dll.rename(fixed_dll)

