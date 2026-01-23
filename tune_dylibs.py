from pathlib import Path
import subprocess as sp

visited_dylibs = list()
all_dylibs = list(Path('.').rglob('*.dylib'))


def rpaths_from_stdout(data):
    """Get a list of rpaths as `Path`s."""
    rpaths = list()

    for l in data.split('\n'):
        l = l.strip()
        if len(l) < 1:
            continue
        l = l.split()[0]
        if '@rpath' in l:
            rpaths.append(Path(l))

    return rpaths


def get_dylibpath(name : str):
    """Get the Path for dylib with `name`."""
    dlbs = [dlp.resolve() for dlp in all_dylibs if dlp.name == name]
    if len(dlbs) > 0:
        return dlbs[0]

    raise Exception(f'dylib not found for: {name}')


def tune_name(rpath : Path):
    """Get a triplet of fixed dylib name, fixed name and actual dylib path for given rpath"""
    rp = Path(rpath)
    if '_crh' in rp.name:
        name = rp.name[:-len('.dylib')]
    else:
        name = rp.name[:-len('.dylib')] + '_crh'

    actual_dylib = get_dylibpath(rp.name)
    #print("tune_name:", actual_dylib)


    if 'ccycles' in name and name.endswith('_crh'):
        name = name[:-4]

    if name.startswith('lib'):
        name = name[3:]

    fixed_dylib = name + '.dylib'

    #print("tune_name:", rp, type(rp), rp.name, name, fixed_dylib)

    return fixed_dylib, name, actual_dylib


def fix_id(dylib_path : Path, new_id : str) -> None:
    """Change id for the given dylib to `new_id`"""
    cmd = [
        'install_name_tool',
        '-id',
        new_id,
        f'{dylib_path}'
    ]
    #print(f'fix_id: going to run: {cmd}')
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')


def do_load_commands(dylib_path):
    """Adjust load commands:
        * remove paths to absolute locations under /Users
        * add @loader_path with six up relative
    """
    if 'ccycles' not in dylib_path.name: return

    cmd = [
        'otool',
        '-l',
        f'{dylib_path}'
    ]
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')

    absolute_user_paths = [
        l.strip().split()[1] for l in cp.stdout.split('\n')
        if 'path /Users' in l
    ]

    #print("do_load_commands:", absolute_user_paths)

    for lc in absolute_user_paths:
        cmd = [
            'install_name_tool',
            '-delete_rpath',
            lc,
            dylib_path
        ]
        #print(f'do_load_commands: run command {cmd}')
        cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')
    cmd = [
        'install_name_tool',
        '-add_rpath',
        '"@loader_path/../../../../../../"',
        dylib_path
    ]
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')


def fix_rpath(dylib_path, old_name, new_name):
    cmd = [
        'install_name_tool',
        '-change',
        f'@rpath/{old_name}',
        f'@rpath/{new_name}',
        f'{dylib_path}'
    ]
    #print(f'fix_rpath: running command {cmd}')
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')


count = 0
def do_dylib(original_path, original_name, new_name):
    global count
    #print(f'do_dylib: called with {original_path}, {original_name}, {new_name}')
    count += 1
    if original_path in visited_dylibs:
        #print("\talready handled")
        return
    if count > 200:
        #print("\tcount reached 201")
        return

    visited_dylibs.append(original_path)

    cmd = [
        'otool',
        '-L',
        f'{original_path}'
    ]
    #print(f'do_dylib: running cmd {cmd}')
    cp = sp.run(cmd, check=True, capture_output=True, encoding='utf-8')

    curr_fixed_dylib, curr_id, _ = tune_name(original_path)

    fix_id(original_path, curr_id)

    rpaths = rpaths_from_stdout(cp.stdout)

    for rpath in rpaths:
        fixed, new_id, actual_dylib = tune_name(rpath)
        fix_rpath(original_path, rpath.name, fixed)

    for rpath in rpaths:
        fixed, new_id, actual_dylib = tune_name(rpath)
        do_dylib(actual_dylib, rpath.name, new_id)
    
    do_load_commands(original_path)


def copy_dylib(original_path, target_folder):
    curr_fixed_dylib, curr_id, actual_dylib = tune_name(original_path)
    target_file = target_folder / curr_fixed_dylib
    # Only copy if target is different from source
    if actual_dylib.resolve() != target_file.resolve():
        actual_dylib.copy(target_file)


def main():
    cwd = Path('.').resolve()
    ccycles = Path('libccycles.dylib').resolve()
    do_dylib(ccycles, 'libccycles', 'ccycles')

    print(f'{visited_dylibs}')
    for dylib in visited_dylibs:
        copy_dylib(dylib, cwd)


main()

