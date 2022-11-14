#ifndef __RHINONODES_H__
#define __RHINONODES_H__

#include "render/graph.h"
#include "graph/node.h"
#include "render/nodes.h"
#include "kernel/svm/svm_rhino_types.h"

#include "util/util_array.h"
#include "util/util_string.h"

CCL_NAMESPACE_BEGIN

class ImageManager;
class LightManager;
class Scene;
class Shader;

class MatrixMathNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(MatrixMathNode)

  Transform tfm;
  float3 vector;

  NodeMatrixMath type;
};

class AzimuthAltitudeTransformNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(AzimuthAltitudeTransformNode)

  float3 vector;
  float azimuth, altitude, threshold;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoCheckerTexture2dNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoCheckerTexture2dNode)

  float3 uvw, color1, color2;
  Transform uvw_transform;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoNoiseTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoNoiseTextureNode)

  float3 uvw, color1, color2;
  Transform uvw_transform;
  RhinoProceduralNoiseType noise_type;
  RhinoProceduralSpecSynthType spec_synth_type;
  int octave_count;
  float frequency_multiplier;
  float amplitude_multiplier;
  float clamp_min;
  float clamp_max;
  bool scale_to_clamp;
  bool inverse;
  float gain;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoWavesTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoWavesTextureNode)

  float3 uvw, color1, color2, color3;
  Transform uvw_transform;
  RhinoProceduralWavesType wave_type;
  float wave_width;
  bool wave_width_texture_on;
  float contrast1;
  float contrast2;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoWavesWidthTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoWavesWidthTextureNode)

  float3 uvw;
  Transform uvw_transform;
  RhinoProceduralWavesType wave_type;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoPerturbingPart1TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPerturbingPart1TextureNode)

  float3 uvw;
  Transform uvw_transform;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoPerturbingPart2TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPerturbingPart2TextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  float3 color3;
  float amount;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoGradientTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoGradientTextureNode)

  float3 uvw;
  Transform uvw_transform;
  float3 color1;
  float3 color2;
  RhinoProceduralGradientType gradient_type;
  bool flip_alternate;
  bool use_custom_curve;
  int point_width;
  int point_height;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoBlendTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoBlendTextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  float3 blend_color;
  bool use_blend_color;
  float blend_factor;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoExposureTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoExposureTextureNode)

  float3 color;
  float exposure;
  float multiplier;
  float world_luminance;
  float max_luminance;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoFbmTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoFbmTextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  bool is_turbulent;
  float max_octaves;
  float gain;
  float roughness;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoGridTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoGridTextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  int cells;
  float font_thickness;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoProjectionChangerTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoProjectionChangerTextureNode)

  float3 uvw;
  RhinoProceduralProjectionType input_projection_type;
  RhinoProceduralProjectionType output_projection_type;
  float azimuth;
  float altitude;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

CCL_NAMESPACE_END

#endif /* __RHINONODES_H__ */
