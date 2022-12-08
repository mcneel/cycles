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

class RhinoCheckerTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoCheckerTextureNode)

  float3 uvw;
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoNoiseTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoNoiseTextureNode)

  float3 uvw;
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;

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

  float3 uvw;
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;
  float3 color3;

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
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;

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
  float alpha1;
  float3 color2;
  float alpha2;
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
  float alpha1;
  float3 color2;
  float alpha2;

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
  float alpha1;
  float3 color2;
  float alpha2;

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

class RhinoMaskTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoMaskTextureNode)

  float3 color;
  float alpha;
  RhinoProceduralMaskType mask_type;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoPerlinMarbleTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPerlinMarbleTextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  
  int levels;
  float noise_amount;
  float blur;
  float size;
  float color1_sat;
  float color2_sat;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoPhysicalSkyTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPhysicalSkyTextureNode)

  float3 uvw;

  float3 sun_dir;
  float atmospheric_density;
  float rayleigh_scattering;
  float mie_scattering;
  bool show_sun;
  float sun_brightness;
  float sun_size;
  float3 sun_color;
  float3 inv_wavelengths;
  float exposure;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoTextureAdjustmentTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoTextureAdjustmentTextureNode)

  float3 color;

  bool grayscale;
  bool invert;
  bool clamp;
  bool scale_to_clamp;
  float multiplier;
  float clamp_min;
  float clamp_max;
  float gain;
  float gamma;
  float saturation;
  float hue_shift;
  bool is_hdr;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoTileTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoTileTextureNode)

  float3 uvw;
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;

  RhinoProceduralTileType tile_type;
  float3 phase;
  float3 join_width;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoDotsTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoDotsTextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;

  int dots_data_count;
  int dots_tree_node_count;
  float sample_area_size;
  bool rings;
  float ring_radius;
  RhinoProceduralDotsFalloffType falloff_type;
  RhinoProceduralDotsCompositionType composition_type;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoNormalPart1TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoNormalPart1TextureNode)

  float3 uvw;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

class RhinoNormalPart2TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoNormalPart2TextureNode)

  float3 color1;
  float3 color2;
  float3 color3;
  float3 color4;
  float3 color5;
  float3 color6;
  float3 color7;
  float3 color8;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};

CCL_NAMESPACE_END

#endif /* __RHINONODES_H__ */
