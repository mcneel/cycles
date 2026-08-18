/**
Copyright 2026 Robert McNeel and Associates

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

/* A symbolised stack trace for crashes inside ccycles.
 *
 * ccycles is loaded by a managed host, so a native access violation surfaces
 * as an AccessViolationException with no native frames - useless for finding
 * which Cycles call went wrong. A vectored handler runs before the CLR gets a
 * look in, so this prints the real stack and then lets the normal handling
 * proceed.
 *
 * Opt in with cycles_debug_install_crash_handler(); nothing is installed
 * otherwise, so shipping builds are unaffected.
 */

#include "internal_types.h"

#ifdef _WIN32

#  include <windows.h>

#  include <dbghelp.h>
#  include <stdio.h>

#  pragma comment(lib, "dbghelp.lib")

static LONG CALLBACK ccycles_vectored_handler(EXCEPTION_POINTERS *info)
{
	const DWORD code = info->ExceptionRecord->ExceptionCode;

	/* Only report the ones that mean a genuine memory fault. Everything else,
	 * C++ exceptions in particular, is normal traffic. */
	if (code != EXCEPTION_ACCESS_VIOLATION && code != EXCEPTION_STACK_OVERFLOW &&
	    code != EXCEPTION_ILLEGAL_INSTRUCTION && code != EXCEPTION_ARRAY_BOUNDS_EXCEEDED)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}

	static bool reported = false;
	if (reported) {
		return EXCEPTION_CONTINUE_SEARCH;
	}
	reported = true;

	HANDLE process = GetCurrentProcess();
	SymSetOptions(SYMOPT_DEFERRED_LOADS | SYMOPT_LOAD_LINES | SYMOPT_UNDNAME);
	SymInitialize(process, nullptr, TRUE);

	printf("\n=== ccycles crash: code 0x%08lx at %p ===\n",
	       (unsigned long)code,
	       info->ExceptionRecord->ExceptionAddress);

	if (code == EXCEPTION_ACCESS_VIOLATION && info->ExceptionRecord->NumberParameters >= 2) {
		printf("    %s address %p\n",
		       info->ExceptionRecord->ExceptionInformation[0] ? "write to" : "read from",
		       (void *)info->ExceptionRecord->ExceptionInformation[1]);
	}

	CONTEXT context = *info->ContextRecord;
	STACKFRAME64 frame = {};
	frame.AddrPC.Offset = context.Rip;
	frame.AddrPC.Mode = AddrModeFlat;
	frame.AddrFrame.Offset = context.Rbp;
	frame.AddrFrame.Mode = AddrModeFlat;
	frame.AddrStack.Offset = context.Rsp;
	frame.AddrStack.Mode = AddrModeFlat;

	char buffer[sizeof(SYMBOL_INFO) + MAX_SYM_NAME * sizeof(char)] = {};
	SYMBOL_INFO *symbol = (SYMBOL_INFO *)buffer;
	symbol->SizeOfStruct = sizeof(SYMBOL_INFO);
	symbol->MaxNameLen = MAX_SYM_NAME;

	for (int depth = 0; depth < 48; depth++) {
		if (!StackWalk64(IMAGE_FILE_MACHINE_AMD64,
		                 process,
		                 GetCurrentThread(),
		                 &frame,
		                 &context,
		                 nullptr,
		                 SymFunctionTableAccess64,
		                 SymGetModuleBase64,
		                 nullptr))
		{
			break;
		}

		if (frame.AddrPC.Offset == 0) {
			break;
		}

		DWORD64 displacement = 0;
		if (SymFromAddr(process, frame.AddrPC.Offset, &displacement, symbol)) {
			IMAGEHLP_LINE64 line = {};
			line.SizeOfStruct = sizeof(line);
			DWORD line_displacement = 0;

			if (SymGetLineFromAddr64(process, frame.AddrPC.Offset, &line_displacement, &line)) {
				printf("  %2d  %s  (%s:%lu)\n", depth, symbol->Name, line.FileName,
				       (unsigned long)line.LineNumber);
			}
			else {
				printf("  %2d  %s + 0x%llx\n", depth, symbol->Name,
				       (unsigned long long)displacement);
			}
		}
		else {
			printf("  %2d  0x%llx\n", depth, (unsigned long long)frame.AddrPC.Offset);
		}
	}

	printf("=== end of stack ===\n\n");
	fflush(stdout);

	return EXCEPTION_CONTINUE_SEARCH;
}

extern "C" CCL_CAPI void CDECL cycles_debug_install_crash_handler()
{
	/* First parameter 1 == run before any previously registered handler, and
	 * before the CLR's. */
	AddVectoredExceptionHandler(1, ccycles_vectored_handler);
}

#else

extern "C" CCL_CAPI void CDECL cycles_debug_install_crash_handler() {}

#endif
