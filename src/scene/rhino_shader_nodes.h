#ifndef __RHINONODES_H__
#define __RHINONODES_H__

#include "scene/shader_nodes.h"
#include "kernel/svm/svm_rhino_types.h"

CCL_NAMESPACE_BEGIN

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
};

class RhinoCheckerTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoCheckerTextureNode)

  float3 uvw;
  float3 color1;
  float alpha1;
  float3 color2;
  float alpha2;
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
};

class RhinoWavesWidthTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoWavesWidthTextureNode)

  float3 uvw;
  RhinoProceduralWavesType wave_type;
};

class RhinoPerturbingPart1TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPerturbingPart1TextureNode)

  float3 uvw;
};

class RhinoPerturbingPart2TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoPerturbingPart2TextureNode)

  float3 uvw;
  float3 color1;
  float3 color2;
  float3 color3;
  float amount;
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
};

class RhinoExposureTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoExposureTextureNode)

  float3 color;
  float exposure;
  float multiplier;
  float world_luminance;
  float max_luminance;
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
  int max_octaves;
  float gain;
  float roughness;
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
};

class RhinoProjectionChangerTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoProjectionChangerTextureNode)

  float3 uvw;
  RhinoProceduralProjectionType input_projection_type;
  RhinoProceduralProjectionType output_projection_type;
  float azimuth;
  float altitude;
};

class RhinoMaskTextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoMaskTextureNode)

  float3 color;
  float alpha;
  RhinoProceduralMaskType mask_type;
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
};

class RhinoNormalPart1TextureNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(RhinoNormalPart1TextureNode)

  float3 uvw;
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
};

CCL_NAMESPACE_END

#endif /* __RHINONODES_H__ */
