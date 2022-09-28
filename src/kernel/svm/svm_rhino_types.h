/**
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
**/

#ifndef __SVM_RHINO_TYPES_H__
#define __SVM_RHINO_TYPES_H__

CCL_NAMESPACE_BEGIN

typedef enum RhinoProceduralNoiseType {
  PERLIN,
  VALUE_NOISE,
  PERLIN_PLUS_VALUE,
  SIMPLEX,
  SPARSE_CONVOLUTION,
  LATTICE_CONVOLUTION,
  WARDS_HERMITE,
  AALTONEN,
} RhinoProceduralNoiseType;

typedef enum RhinoProceduralSpecSynthType {
  FRACTAL_SUM,
  TURBULENCE,
} RhinoProceduralSpecSynthType;

CCL_NAMESPACE_END

#endif /*  __SVM_RHINO_TYPES_H__ */
