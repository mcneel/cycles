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
  RHINO_PERLIN,
  RHINO_VALUE_NOISE,
  RHINO_PERLIN_PLUS_VALUE,
  RHINO_SIMPLEX,
  RHINO_SPARSE_CONVOLUTION,
  RHINO_LATTICE_CONVOLUTION,
  RHINO_WARDS_HERMITE,
  RHINO_AALTONEN,
} RhinoProceduralNoiseType;

typedef enum RhinoProceduralSpecSynthType {
  RHINO_FRACTAL_SUM,
  RHINO_TURBULENCE,
} RhinoProceduralSpecSynthType;

typedef enum RhinoProceduralWavesType {
  RHINO_LINEAR,
  RHINO_RADIAL,
} RhinoProceduralWavesType;

CCL_NAMESPACE_END

#endif /*  __SVM_RHINO_TYPES_H__ */
