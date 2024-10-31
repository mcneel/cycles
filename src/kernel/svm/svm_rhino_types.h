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

typedef enum NodeMatrixMath {
  NODE_MATRIX_MATH_POINT,
  NODE_MATRIX_MATH_DIRECTION,
  NODE_MATRIX_MATH_PERSPECTIVE,
  NODE_MATRIX_MATH_DIR_TRANSPOSED,
} NodeMatrixMath;

typedef enum RhinoProceduralNoiseType {
  RHINO_NOISE_PERLIN,
  RHINO_NOISE_VALUE_NOISE,
  RHINO_NOISE_PERLIN_PLUS_VALUE,
  RHINO_NOISE_SIMPLEX,
  RHINO_NOISE_SPARSE_CONVOLUTION,
  RHINO_NOISE_LATTICE_CONVOLUTION,
  RHINO_NOISE_WARDS_HERMITE,
  RHINO_NOISE_AALTONEN,
} RhinoProceduralNoiseType;

typedef enum RhinoProceduralSpecSynthType {
  RHINO_SPEC_SYNTH_FRACTAL_SUM,
  RHINO_SPEC_SYNTH_TURBULENCE,
} RhinoProceduralSpecSynthType;

typedef enum RhinoProceduralWavesType {
  RHINO_WAVES_LINEAR,
  RHINO_WAVES_RADIAL,
} RhinoProceduralWavesType;

typedef enum RhinoProceduralGradientType {
  RHINO_GRADIENT_LINEAR,
  RHINO_GRADIENT_BOX,
  RHINO_GRADIENT_RADIAL,
  RHINO_GRADIENT_TARTAN,
  RHINO_GRADIENT_SWEEP,
  RHINO_GRADIENT_PONG,
  RHINO_GRADIENT_SPIRAL
} RhinoProceduralGradientType;

typedef enum RhinoProceduralProjectionType {
  RHINO_PROJECTION_NONE,
  RHINO_PROJECTION_PLANAR,
  RHINO_PROJECTION_LIGHTPROBE,
  RHINO_PROJECTION_EQUIRECT,
  RHINO_PROJECTION_CUBEMAP,
  RHINO_PROJECTION_VERTICAL_CROSS_CUBEMAP,
  RHINO_PROJECTION_HORIZONTAL_CROSS_CUBEMAP,
  RHINO_PROJECTION_EMAP,
  RHINO_PROJECTION_SAME_AS_INPUT,
  RHINO_PROJECTION_HEMISPHERICAL,
} RhinoProceduralProjectionType;

typedef enum RhinoProceduralMaskType {
  RHINO_MASK_LUMINANCE,
  RHINO_MASK_RED,
  RHINO_MASK_GREEN,
  RHINO_MASK_BLUE,
  RHINO_MASK_ALPHA,
} RhinoProceduralMaskType;

typedef enum RhinoProceduralTileType {
  RHINO_TILE_3D_RECTANGULAR,
  RHINO_TILE_2D_RECTANGULAR,
  RHINO_TILE_2D_HEXAGONAL,
  RHINO_TILE_2D_TRIANGULAR,
  RHINO_TILE_2D_OCTAGONAL,
} RhinoProceduralTileType;

typedef enum RhinoProceduralDotsFalloffType {
  RHINO_DOTS_FALLOFF_FLAT,
  RHINO_DOTS_FALLOFF_LINEAR,
  RHINO_DOTS_FALLOFF_CUBIC,
  RHINO_DOTS_FALLOFF_ELLIPTIC,
} RhinoProceduralDotsFalloffType;

typedef enum RhinoProceduralDotsCompositionType {
  RHINO_DOTS_COMPOSITION_MAXIMUM,
  RHINO_DOTS_COMPOSITION_ADDITION,
  RHINO_DOTS_COMPOSITION_SUBTRACTION,
  RHINO_DOTS_COMPOSITION_MULTIPLICATION,
  RHINO_DOTS_COMPOSITION_AVERAGE,
  RHINO_DOTS_COMPOSITION_STANDARD,
} RhinoProceduralDotsCompositionType;

CCL_NAMESPACE_END

#endif /*  __SVM_RHINO_TYPES_H__ */
