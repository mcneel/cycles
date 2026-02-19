#!/usr/bin/env python3
#
# "make update" for all platforms, updating svn libraries and Cycles
# git repository.
#
# For release branches, this will check out the appropriate branches of
# libraries.

import argparse
import os
import platform
import shutil
import subprocess
import sys
import time

import make_utils
from make_utils import call, check_output


def print_stage(text):
    print("")
    print(text)
    print("")

# Parse arguments
def parse_arguments():
    parser = argparse.ArgumentParser()
    parser.add_argument("--no-libraries", action="store_true")
    parser.add_argument("--no-cycles", action="store_true")
    parser.add_argument("--svn-command", default="svn")
    parser.add_argument("--git-command", default="git")
    return parser.parse_args()


def get_cycles_git_root():
    return check_output([args.git_command, "rev-parse", "--show-toplevel"])


def _svn_action_and_wc_dir(cmd):
    actions = {"checkout", "switch", "update", "cleanup"}
    action_index = -1
    action = ""
    for index, token in enumerate(cmd):
        if token in actions:
            action_index = index
            action = token
            break
    wc_dir = cmd[-1] if cmd else ""
    return action_index, action, wc_dir


def _svn_call(cmd, retries=None, allow_failure=False, timeout_seconds=None):
    if retries is None:
        retries = max(1, int(os.environ.get("CYCLES_SVN_RETRIES", "6")))
    if timeout_seconds is None:
        timeout_seconds = max(60, int(os.environ.get("CYCLES_SVN_TIMEOUT_SECONDS", "900")))

    for attempt in range(1, retries + 1):
        # Keep output behavior similar to make_utils.call().
        print(" ".join(cmd))
        sys.stdout.flush()
        sys.stderr.flush()

        timed_out = False
        try:
            retcode = subprocess.call(cmd, timeout=timeout_seconds)
        except subprocess.TimeoutExpired:
            retcode = -1
            timed_out = True

        if retcode == 0:
            return 0

        if attempt < retries:
            action_index, action, wc_dir = _svn_action_and_wc_dir(cmd)
            if action in {"checkout", "switch", "update"} and os.path.isdir(wc_dir):
                if action_index >= 0:
                    cleanup_cmd = cmd[:action_index] + ["cleanup", wc_dir]
                else:
                    cleanup_cmd = [cmd[0], "--non-interactive", "cleanup", wc_dir]
                print(" ".join(cleanup_cmd))
                sys.stdout.flush()
                sys.stderr.flush()
                try:
                    subprocess.call(cleanup_cmd, timeout=timeout_seconds)
                except subprocess.TimeoutExpired:
                    pass

            wait_seconds = min(120, 5 * attempt)
            if timed_out:
                print_stage(
                    "SVN command timed out after {}s (attempt {}/{}), retrying in {}s".format(
                        timeout_seconds, attempt, retries, wait_seconds))
            else:
                print_stage(
                    "SVN command failed (exit {}) (attempt {}/{}), retrying in {}s".format(
                        retcode, attempt, retries, wait_seconds))
            time.sleep(wait_seconds)
            continue

        if allow_failure:
            return retcode

        sys.exit(retcode)


def _is_windows_special_link_file(filepath):
    # Subversion checkouts from Windows can materialize symlinks as text files:
    # "link <target>".
    if not os.path.isfile(filepath) or os.path.islink(filepath):
        return False

    try:
        with open(filepath, "rb") as file:
            return file.readline(256).startswith(b"link ")
    except OSError:
        return False


def _linux_checkout_issue(lib_platform_dirpath):
    if sys.platform != "linux":
        return ""

    checks = (
        "dpcpp/bin/clang++",
        "dpcpp/lib/libsycl.so",
    )
    for relpath in checks:
        path = os.path.join(lib_platform_dirpath, relpath)
        if _is_windows_special_link_file(path):
            return relpath

    return ""


# Setup for precompiled libraries and tests from svn.
def svn_update(args):
    svn_non_interactive = [args.svn_command, '--non-interactive']
    svn_max_connections = os.environ.get("CYCLES_SVN_MAX_CONNECTIONS", "1").strip()
    if svn_max_connections:
        svn_non_interactive.extend([
            "--config-option",
            "servers:global:http-max-connections={}".format(svn_max_connections),
        ])

    lib_dirpath = os.path.join(get_cycles_git_root(), '..', 'lib')
    svn_url = make_utils.svn_libraries_base_url()

    # Checkout precompiled libraries
    if sys.platform == 'darwin':
        if platform.machine() == 'x86_64':
            libs_platform = ["darwin"]
        elif platform.machine() == 'arm64':
            libs_platform = ["darwin_arm64"]
        else:
            libs_platform = []
    elif sys.platform == 'win32' and platform.machine() == 'AMD64':
        libs_platform = ["win64_vc15"]
    elif sys.platform == 'linux' and platform.machine() == 'x86_64':
        libs_platform = ["linux_x86_64_glibc_228", "linux_centos7_x86_64"]
    else:
        libs_platform = []

    requested_platforms = os.environ.get("CYCLES_LIB_PLATFORMS", "").strip()
    if requested_platforms:
        requested_set = {entry.strip() for entry in requested_platforms.split(",") if entry.strip()}
        libs_platform = [entry for entry in libs_platform if entry in requested_set]

    for lib_platform in libs_platform:
        lib_platform_dirpath = os.path.join(lib_dirpath, lib_platform)

        issue_relpath = _linux_checkout_issue(lib_platform_dirpath)
        if issue_relpath:
            print_stage("Recreating Precompiled Libraries for {} ({} is a Windows placeholder link file)".format(
                lib_platform, issue_relpath))
            shutil.rmtree(lib_platform_dirpath)

        if not os.path.exists(lib_platform_dirpath):
            print_stage("Checking out Precompiled Libraries")

            if make_utils.command_missing(args.svn_command):
                sys.stderr.write("svn not found, can't checkout libraries\n")
                sys.exit(1)

            svn_url_platform = svn_url + lib_platform
            _svn_call(svn_non_interactive + ["checkout", svn_url_platform, lib_platform_dirpath])

    # Update precompiled libraries and tests
    print_stage("Updating Precompiled Libraries")

    allowed_update_dirs = set(libs_platform)

    if os.path.isdir(lib_dirpath):
        for dirname in os.listdir(lib_dirpath):
            dirpath = os.path.join(lib_dirpath, dirname)

            if dirname == ".svn":
                # Cleanup must be run from svn root directory if it exists.
                if not make_utils.command_missing(args.svn_command):
                    _svn_call(svn_non_interactive + ["cleanup", lib_dirpath])
                continue

            if dirname not in allowed_update_dirs:
                continue

            svn_dirpath = os.path.join(dirpath, ".svn")
            svn_root_dirpath = os.path.join(lib_dirpath, ".svn")

            if (
                    os.path.isdir(dirpath) and
                    (os.path.exists(svn_dirpath) or os.path.exists(svn_root_dirpath))
            ):
                if make_utils.command_missing(args.svn_command):
                    sys.stderr.write("svn not found, can't update libraries\n")
                    sys.exit(1)

                # Cleanup to continue with interrupted downloads.
                if os.path.exists(svn_dirpath):
                    _svn_call(svn_non_interactive + ["cleanup", dirpath])
                # Switch to appropriate branch and update.
                _svn_call(svn_non_interactive + ["switch", svn_url + dirname, dirpath], allow_failure=True)
                _svn_call(svn_non_interactive + ["update", dirpath])

# Test if git repo can be updated.
def git_update_skip(args, check_remote_exists=True):
    if make_utils.command_missing(args.git_command):
        sys.stderr.write("git not found, can't update code\n")
        sys.exit(1)

    # Abort if a rebase is still progress.
    rebase_merge = check_output([args.git_command, 'rev-parse', '--git-path', 'rebase-merge'], exit_on_error=False)
    rebase_apply = check_output([args.git_command, 'rev-parse', '--git-path', 'rebase-apply'], exit_on_error=False)
    merge_head = check_output([args.git_command, 'rev-parse', '--git-path', 'MERGE_HEAD'], exit_on_error=False)
    if (
            os.path.exists(rebase_merge) or
            os.path.exists(rebase_apply) or
            os.path.exists(merge_head)
    ):
        return "rebase or merge in progress, complete it first"

    # Abort if uncommitted changes.
    changes = check_output([args.git_command, 'status', '--porcelain', '--untracked-files=no'])
    if len(changes) != 0:
        return "you have unstaged changes"

    # Test if there is an upstream branch configured
    if check_remote_exists:
        branch = check_output([args.git_command, "rev-parse", "--abbrev-ref", "HEAD"])
        remote = check_output([args.git_command, "config", "branch." + branch + ".remote"], exit_on_error=False)
        if len(remote) == 0:
            return "no remote branch to pull from"

    return ""


# Update cycles repository.
def cycles_update(args):
    print_stage("Updating Cycles Git Repository")
    call([args.git_command, "pull", "--rebase"])


if __name__ == "__main__":
    args = parse_arguments()
    cycles_skip_msg = ""

    if not args.no_libraries:
        svn_update(args)
    if not args.no_cycles:
        cycles_skip_msg = git_update_skip(args)
        if cycles_skip_msg:
            cycles_skip_msg = "Cycles repository skipped: " + cycles_skip_msg + "\n"
        else:
            cycles_update(args)

    # Report any skipped repositories at the end, so it's not as easy to miss.
    if cycles_skip_msg:
        print_stage(cycles_skip_msg.strip())
