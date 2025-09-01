/**
Copyright 2014-2017 Robert McNeel and Associates

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

#ifndef __CYCLES__H__
#define __CYCLES__H__

#ifdef WIN32
#include <stdint.h>
#ifdef CCL_CAPI_DLL
#define CCL_CAPI __declspec (dllexport)
#else
#define CCL_CAPI __declspec (dllimport)
#endif
#ifndef CDECL
#define CDECL __cdecl
#endif
#ifndef UTFCHAR
#define UTFCHAR wchar_t
#endif
#else
#define CCL_CAPI
#ifndef CDECL
#define CDECL
#endif
#ifndef UTFCHAR
#define UTFCHAR char
#endif
#endif

/*

// conversion matrix for rhino -> cycles view.
ccl::Transform camConvertMat = ccl::make_transform(
1.0f, 0.0f, 0.0f, 0.0f,
0.0f, -1.0f, 0.0f, 0.0f,
0.0f, 0.0f, -1.0f, 1.0f

*/


#endif
