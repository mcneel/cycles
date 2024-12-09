#include "scene/rhino_shader_nodes.h"
#include <atomic>
#include "scene/svm.h"

CCL_NAMESPACE_BEGIN

/* MatrixMath */

NODE_DEFINE(MatrixMathNode)
{
  NodeType *type = NodeType::add("matrix_math", create, NodeType::SHADER);

  static NodeEnum type_enum;
  type_enum.insert("Point", NODE_MATRIX_MATH_POINT);
  type_enum.insert("Direction", NODE_MATRIX_MATH_DIRECTION);
  type_enum.insert("Perspective", NODE_MATRIX_MATH_PERSPECTIVE);
  type_enum.insert("Direction Transposed", NODE_MATRIX_MATH_DIR_TRANSPOSED);
  SOCKET_ENUM(type, "Type", type_enum, NODE_MATRIX_MATH_POINT);
  SOCKET_TRANSFORM(tfm, "Transform", transform_identity());

  SOCKET_IN_VECTOR(vector, "Vector", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_OUT_VECTOR(vector, "Vector");

  return type;
}

MatrixMathNode::MatrixMathNode() : ShaderNode(node_type)
{
  tfm = transform_identity();
}

/* TODO: ADD MATRIXMATH CONSTANT FOLD THINGY */

void MatrixMathNode::compile(SVMCompiler &compiler)
{
  ShaderInput *vector_in = input("Vector");
  ShaderOutput *vector_out = output("Vector");

  compiler.stack_assign(vector_in);
  compiler.stack_assign(vector_out);

  if (vector_in->stack_offset == SVM_STACK_INVALID ||
      vector_out->stack_offset == SVM_STACK_INVALID)
    return;

  compiler.add_node(
      RHINO_NODE_MATRIX_MATH, type, compiler.stack_assign(vector_in), compiler.stack_assign(vector_out));

  compiler.add_node(tfm.x);
  compiler.add_node(tfm.y);
  compiler.add_node(tfm.z);
}

void MatrixMathNode::compile(OSLCompiler &compiler)
{
}


/* Azimuth Altitude Transform */

NODE_DEFINE(AzimuthAltitudeTransformNode)
{
  NodeType *type = NodeType::add("azimuth_altitude_transform", create, NodeType::SHADER);

  SOCKET_IN_POINT(
      vector, "Vector", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(azimuth, "Azimuth", 0.0f);
  SOCKET_IN_FLOAT(altitude, "Altitude", 0.0f);
  SOCKET_IN_FLOAT(threshold, "Threshold", 0.001f);

  SOCKET_OUT_VECTOR(vector, "Vector");

  return type;
}

AzimuthAltitudeTransformNode::AzimuthAltitudeTransformNode() : ShaderNode(node_type)
{
}

void AzimuthAltitudeTransformNode::compile(SVMCompiler &compiler)
{
  ShaderInput *vector_in = input("Vector");
  ShaderOutput *vector_out = output("Vector");

  compiler.add_node(RHINO_NODE_AZIMUTH_ALTITUDE_TRANSFORM,
                    compiler.stack_assign(vector_in),
                    compiler.stack_assign_if_linked(vector_out)
                    );
  compiler.add_node(
    __float_as_int(azimuth),
    __float_as_int(altitude),
    __float_as_int(threshold));
}

void AzimuthAltitudeTransformNode::compile(OSLCompiler &compiler)
{
}

/* Checker Texture 2D */

NODE_DEFINE(RhinoCheckerTextureNode)
{
  NodeType *type = NodeType::add("rhino_checker_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  SOCKET_OUT_COLOR(color, "Color");
  SOCKET_OUT_FLOAT(alpha, "Alpha");

  return type;
}

RhinoCheckerTextureNode::RhinoCheckerTextureNode() : ShaderNode(node_type)
{
}

void RhinoCheckerTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_CHECKER_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));
}

void RhinoCheckerTextureNode::compile(OSLCompiler &compiler)
{
}

/* Noise */

NODE_DEFINE(RhinoNoiseTextureNode)
{
  NodeType *type = NodeType::add("rhino_noise_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  static NodeEnum noise_type_enum;
  noise_type_enum.insert("perlin", RHINO_NOISE_PERLIN);
  noise_type_enum.insert("value noise", RHINO_NOISE_VALUE_NOISE);
  noise_type_enum.insert("perlin plus value", RHINO_NOISE_PERLIN_PLUS_VALUE);
  noise_type_enum.insert("simplex", RHINO_NOISE_SIMPLEX);
  noise_type_enum.insert("sparse convolution", RHINO_NOISE_SPARSE_CONVOLUTION);
  noise_type_enum.insert("lattice convolution", RHINO_NOISE_LATTICE_CONVOLUTION);
  noise_type_enum.insert("wards hermite", RHINO_NOISE_WARDS_HERMITE);
  noise_type_enum.insert("aaltonen", RHINO_NOISE_AALTONEN);
  SOCKET_ENUM(noise_type, "NoiseType", noise_type_enum, RHINO_NOISE_PERLIN);

  static NodeEnum spec_synth_type_enum;
  spec_synth_type_enum.insert("fractal sum", RHINO_SPEC_SYNTH_FRACTAL_SUM);
  spec_synth_type_enum.insert("turbulence", RHINO_SPEC_SYNTH_TURBULENCE);

  SOCKET_ENUM(
      spec_synth_type, "SpecSynthType", spec_synth_type_enum, RHINO_SPEC_SYNTH_FRACTAL_SUM);
  SOCKET_INT(octave_count, "OctaveCount", 0);
  SOCKET_FLOAT(frequency_multiplier, "FrequencyMultiplier", 0.0f);
  SOCKET_FLOAT(amplitude_multiplier, "AmplitudeMultiplier", 0.0f);
  SOCKET_FLOAT(clamp_min, "ClampMin", 0.0f);
  SOCKET_FLOAT(clamp_max, "ClampMax", 0.0f);
  SOCKET_BOOLEAN(scale_to_clamp, "ScaleToClamp", false);
  SOCKET_BOOLEAN(inverse, "Inverse", false);
  SOCKET_FLOAT(gain, "Gain", 0.0f);

  SOCKET_OUT_COLOR(color, "Color");
  SOCKET_OUT_FLOAT(alpha, "Alpha");

  return type;
}

RhinoNoiseTextureNode::RhinoNoiseTextureNode() : ShaderNode(node_type)
{
}

void RhinoNoiseTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_NOISE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node(
      (int)noise_type, (int)spec_synth_type, octave_count, __float_as_int(frequency_multiplier));
  compiler.add_node(__float_as_int(amplitude_multiplier),
                    __float_as_int(clamp_min),
                    __float_as_int(clamp_max),
                    (int)scale_to_clamp);
  compiler.add_node((int)inverse, __float_as_int(gain));
}

void RhinoNoiseTextureNode::compile(OSLCompiler &compiler)
{
}

/* Waves */

NODE_DEFINE(RhinoWavesTextureNode)
{
  NodeType *type = NodeType::add("rhino_waves_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);
  SOCKET_IN_COLOR(color3, "Color3", make_float3(0.0f, 0.0f, 0.0f));

  static NodeEnum wave_type_enum;
  wave_type_enum.insert("linear", RHINO_WAVES_LINEAR);
  wave_type_enum.insert("radial", RHINO_WAVES_RADIAL);
  SOCKET_ENUM(wave_type, "WaveType", wave_type_enum, RHINO_WAVES_LINEAR);
  SOCKET_FLOAT(wave_width, "WaveWidth", 0);
  SOCKET_BOOLEAN(wave_width_texture_on, "WaveWidthTextureOn", 0);
  SOCKET_FLOAT(contrast1, "Contrast1", 0.0f);
  SOCKET_FLOAT(contrast2, "Contrast2", 0.0f);

  SOCKET_OUT_COLOR(color, "Color");
  SOCKET_OUT_FLOAT(alpha, "Alpha");

  return type;
}

RhinoWavesTextureNode::RhinoWavesTextureNode() : ShaderNode(node_type)
{
}

void RhinoWavesTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");
  ShaderInput *color3_in = input("Color3");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_WAVES_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color3_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node((int)wave_type,
                    __float_as_int(wave_width),
                    (int)wave_width_texture_on,
                    __float_as_int(contrast1));
  compiler.add_node(__float_as_int(contrast2));
}

void RhinoWavesTextureNode::compile(OSLCompiler &compiler)
{
}

/* Waves Width */

NODE_DEFINE(RhinoWavesWidthTextureNode)
{
  NodeType *type = NodeType::add("rhino_waves_width_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));

  static NodeEnum wave_type_enum;
  wave_type_enum.insert("linear", RHINO_WAVES_LINEAR);
  wave_type_enum.insert("radial", RHINO_WAVES_RADIAL);
  SOCKET_ENUM(wave_type, "WaveType", wave_type_enum, RHINO_WAVES_LINEAR);

  SOCKET_OUT_POINT(out_uvw, "UVW");

  return type;
}

RhinoWavesWidthTextureNode::RhinoWavesWidthTextureNode() : ShaderNode(node_type)
{
}

void RhinoWavesWidthTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");

  ShaderOutput *uvw_out = output("UVW");

  compiler.add_node(RHINO_NODE_WAVES_WIDTH_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(uvw_out)));

  compiler.add_node((int)wave_type);
}

void RhinoWavesWidthTextureNode::compile(OSLCompiler &compiler)
{
}

/* Perturbing Part 1 */

NODE_DEFINE(RhinoPerturbingPart1TextureNode)
{
  NodeType *type = NodeType::add("rhino_perturbing_part1_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_OUT_POINT(out_uvw1, "UVW1");
  SOCKET_OUT_POINT(out_uvw2, "UVW2");
  SOCKET_OUT_POINT(out_uvw3, "UVW3");

  return type;
}

RhinoPerturbingPart1TextureNode::RhinoPerturbingPart1TextureNode() : ShaderNode(node_type)
{
}

void RhinoPerturbingPart1TextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");

  ShaderOutput *uvw1_out = output("UVW1");
  ShaderOutput *uvw2_out = output("UVW2");
  ShaderOutput *uvw3_out = output("UVW3");

  compiler.add_node(RHINO_NODE_PERTURBING_PART1_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(uvw1_out),
                                           compiler.stack_assign(uvw2_out),
                                           compiler.stack_assign(uvw3_out)));
}

void RhinoPerturbingPart1TextureNode::compile(OSLCompiler &compiler)
{
}

/* Perturbing Part 2 */

NODE_DEFINE(RhinoPerturbingPart2TextureNode)
{
  NodeType *type = NodeType::add("rhino_perturbing_part2_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(
      uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(
      color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(
      color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(
      color3, "Color3", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_FLOAT(amount, "Amount", 0.5f);

  SOCKET_OUT_POINT(out_uvw, "Perturbed UVW");

  return type;
}

RhinoPerturbingPart2TextureNode::RhinoPerturbingPart2TextureNode() : ShaderNode(node_type)
{
}

void RhinoPerturbingPart2TextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *color3_in = input("Color3");

  ShaderOutput *uvw_out = output("Perturbed UVW");

  compiler.add_node(RHINO_NODE_PERTURBING_PART2_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color3_in)),
                    compiler.encode_uchar4(compiler.stack_assign(uvw_out)),
                    __float_as_int(amount));
}

void RhinoPerturbingPart2TextureNode::compile(OSLCompiler &compiler)
{
}

/* Gradient */

NODE_DEFINE(RhinoGradientTextureNode)
{
  NodeType *type = NodeType::add("rhino_gradient_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  static NodeEnum gradient_type_enum;
  gradient_type_enum.insert("linear", RHINO_GRADIENT_LINEAR);
  gradient_type_enum.insert("box", RHINO_GRADIENT_BOX);
  gradient_type_enum.insert("radial", RHINO_GRADIENT_RADIAL);
  gradient_type_enum.insert("tartan", RHINO_GRADIENT_TARTAN);
  gradient_type_enum.insert("sweep", RHINO_GRADIENT_SWEEP);
  gradient_type_enum.insert("pong", RHINO_GRADIENT_PONG);
  gradient_type_enum.insert("spiral", RHINO_GRADIENT_SPIRAL);
  SOCKET_ENUM(gradient_type, "GradientType", gradient_type_enum, RHINO_GRADIENT_LINEAR);
  SOCKET_BOOLEAN(flip_alternate, "FlipAlternate", false);
  SOCKET_BOOLEAN(use_custom_curve, "UseCustomCurve", false);
  SOCKET_INT(point_width, "PointWidth", 1.0f);
  SOCKET_INT(point_height, "PointHeight", 1.0f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoGradientTextureNode::RhinoGradientTextureNode() : ShaderNode(node_type)
{
}

void RhinoGradientTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_GRADIENT_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node((int)gradient_type, (int)flip_alternate, (int)use_custom_curve, point_width);
  compiler.add_node(point_height);
}

void RhinoGradientTextureNode::compile(OSLCompiler &compiler)
{
}

/* Blend */

NODE_DEFINE(RhinoBlendTextureNode)
{
  NodeType *type = NodeType::add("rhino_blend_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);
  SOCKET_IN_COLOR(blend_color, "BlendColor", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_BOOLEAN(use_blend_color, "UseBlendColor", false);
  SOCKET_FLOAT(blend_factor, "BlendFactor", 0.5f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoBlendTextureNode::RhinoBlendTextureNode() : ShaderNode(node_type)
{
}

void RhinoBlendTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");
  ShaderInput *blend_color_in = input("BlendColor");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_BLEND_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(blend_color_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node((int)use_blend_color, __float_as_int(blend_factor));
}

void RhinoBlendTextureNode::compile(OSLCompiler &compiler)
{
}

/* Exposure */

NODE_DEFINE(RhinoExposureTextureNode)
{
  NodeType *type = NodeType::add("rhino_exposure_texture", create, NodeType::SHADER);

  SOCKET_IN_COLOR(color, "Color", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_FLOAT(exposure, "Exposure", 1.0f);
  SOCKET_FLOAT(multiplier, "Multiplier", 0.5f);
  SOCKET_FLOAT(world_luminance, "WorldLuminance", 0.5f);
  SOCKET_FLOAT(max_luminance, "MaxLuminance", 0.5f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_COLOR(out_alpha, "Alpha");

  return type;
}

RhinoExposureTextureNode::RhinoExposureTextureNode() : ShaderNode(node_type)
{
}

void RhinoExposureTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *color_in = input("Color");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(
      RHINO_NODE_EXPOSURE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(color_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node(__float_as_int(exposure), __float_as_int(multiplier), __float_as_int(world_luminance), __float_as_int(max_luminance));
}

void RhinoExposureTextureNode::compile(OSLCompiler &compiler)
{
}

/* FBm */

NODE_DEFINE(RhinoFbmTextureNode)
{
  NodeType *type = NodeType::add("rhino_fbm_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  SOCKET_BOOLEAN(is_turbulent, "IsTurbulent", false);
  SOCKET_INT(max_octaves, "MaxOctaves", 3);
  SOCKET_FLOAT(gain, "Gain", 0.5f);
  SOCKET_FLOAT(roughness, "Roughness", 0.5f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoFbmTextureNode::RhinoFbmTextureNode() : ShaderNode(node_type)
{
}

void RhinoFbmTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(
      RHINO_NODE_FBM_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node((int)is_turbulent,
                    max_octaves, __float_as_int(gain), __float_as_int(roughness));
}

void RhinoFbmTextureNode::compile(OSLCompiler &compiler)
{
}

/* Grid */

NODE_DEFINE(RhinoGridTextureNode)
{
  NodeType *type = NodeType::add("rhino_grid_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  SOCKET_INT(cells, "Cells", 0);
  SOCKET_FLOAT(font_thickness, "FontThickness", 1.0f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoGridTextureNode::RhinoGridTextureNode() : ShaderNode(node_type)
{
}

void RhinoGridTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_GRID_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node(cells, __float_as_int(font_thickness));
}

void RhinoGridTextureNode::compile(OSLCompiler &compiler)
{
}

/* Projection Changer */

NODE_DEFINE(RhinoProjectionChangerTextureNode)
{
  NodeType *type = NodeType::add("rhino_projection_changer_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));

  static NodeEnum projection_changer_type_enum;
  projection_changer_type_enum.insert("none", RHINO_PROJECTION_NONE);
  projection_changer_type_enum.insert("planar", RHINO_PROJECTION_PLANAR);
  projection_changer_type_enum.insert("lightprobe", RHINO_PROJECTION_LIGHTPROBE);
  projection_changer_type_enum.insert("equirect", RHINO_PROJECTION_EQUIRECT);
  projection_changer_type_enum.insert("cubemap", RHINO_PROJECTION_CUBEMAP);
  projection_changer_type_enum.insert("vertical_cross_cubemap", RHINO_PROJECTION_VERTICAL_CROSS_CUBEMAP);
  projection_changer_type_enum.insert("horizontal_cross_cubemap", RHINO_PROJECTION_HORIZONTAL_CROSS_CUBEMAP);
  projection_changer_type_enum.insert("emap", RHINO_PROJECTION_EMAP);
  projection_changer_type_enum.insert("same_as_input", RHINO_PROJECTION_SAME_AS_INPUT);
  projection_changer_type_enum.insert("hemispherical", RHINO_PROJECTION_HEMISPHERICAL);

  SOCKET_ENUM(input_projection_type,
              "InputProjectionType",
              projection_changer_type_enum,
              RHINO_PROJECTION_NONE);
  SOCKET_ENUM(output_projection_type,
              "OutputProjectionType",
              projection_changer_type_enum,
              RHINO_PROJECTION_NONE);
  SOCKET_FLOAT(azimuth, "Azimuth", 1.0f);
  SOCKET_FLOAT(altitude, "Altitude", 1.0f);

  SOCKET_OUT_VECTOR(out_uvw, "Output UVW");

  return type;
}

RhinoProjectionChangerTextureNode::RhinoProjectionChangerTextureNode() : ShaderNode(node_type)
{
}

void RhinoProjectionChangerTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");

  ShaderOutput *uvw_out = output("Output UVW");

  compiler.add_node(
      RHINO_NODE_PROJECTION_CHANGER_TEXTURE,
      compiler.encode_uchar4(compiler.stack_assign(uvw_in), compiler.stack_assign(uvw_out)));

  compiler.add_node((int)input_projection_type, (int)output_projection_type, __float_as_int(azimuth), __float_as_int(altitude));
}

void RhinoProjectionChangerTextureNode::compile(OSLCompiler &compiler)
{
}

/* Mask */

NODE_DEFINE(RhinoMaskTextureNode)
{
  NodeType *type = NodeType::add("rhino_mask_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(color, "Color", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha, "Alpha", 1.0f);

  static NodeEnum mask_type_enum;
  mask_type_enum.insert("luminance", RHINO_MASK_LUMINANCE);
  mask_type_enum.insert("red", RHINO_MASK_RED);
  mask_type_enum.insert("green", RHINO_MASK_GREEN);
  mask_type_enum.insert("blue", RHINO_MASK_BLUE);
  mask_type_enum.insert("alpha", RHINO_MASK_ALPHA);

  SOCKET_ENUM(mask_type, "MaskType", mask_type_enum, RHINO_MASK_LUMINANCE);

  SOCKET_OUT_VECTOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoMaskTextureNode::RhinoMaskTextureNode() : ShaderNode(node_type)
{
}

void RhinoMaskTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *color_in = input("Color");
  ShaderInput *alpha_in = input("Alpha");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_MASK_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(color_in),
                                           compiler.stack_assign(alpha_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node((int)mask_type);
}

void RhinoMaskTextureNode::compile(OSLCompiler &compiler)
{
}

/* Perlin Marble */

NODE_DEFINE(RhinoPerlinMarbleTextureNode)
{
  NodeType *type = NodeType::add("rhino_perlin_marble_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_INT(levels, "Levels", 0);
  SOCKET_FLOAT(noise_amount, "Noise", 0.0f);
  SOCKET_FLOAT(blur, "Blur", 0.0f);
  SOCKET_FLOAT(size, "Size", 0.0f);
  SOCKET_FLOAT(color1_sat, "Color1Saturation", 0.0f);
  SOCKET_FLOAT(color2_sat, "Color2Saturation", 0.0f);

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoPerlinMarbleTextureNode::RhinoPerlinMarbleTextureNode() : ShaderNode(node_type)
{
}

void RhinoPerlinMarbleTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(RHINO_NODE_PERLIN_MARBLE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha_out)));

  compiler.add_node(
      levels, __float_as_int(noise_amount), __float_as_int(blur), __float_as_int(size));
  compiler.add_node(__float_as_int(color1_sat), __float_as_int(color2_sat));
}

void RhinoPerlinMarbleTextureNode::compile(OSLCompiler &compiler)
{
}

/* Physical Sky */

NODE_DEFINE(RhinoPhysicalSkyTextureNode)
{
  NodeType *type = NodeType::add("rhino_physical_sky_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_VECTOR(sun_dir, "SunDirection", make_float3(0.0f));
  SOCKET_FLOAT(atmospheric_density, "AtmosphericDensity", 0.0f);
  SOCKET_FLOAT(rayleigh_scattering, "RayleighScattering", 0.0f);
  SOCKET_FLOAT(mie_scattering, "MieScattering", 0.0f);
  SOCKET_BOOLEAN(show_sun, "ShowSun", false);
  SOCKET_FLOAT(sun_brightness, "SunBrightness", 0.0f);
  SOCKET_FLOAT(sun_size, "SunSize", 0.0f);
  SOCKET_VECTOR(sun_color, "SunColor", make_float3(0.0f));
  SOCKET_VECTOR(inv_wavelengths, "InverseWavelengths", make_float3(0.0f));
  SOCKET_FLOAT(exposure, "Exposure", 0.0f);

  SOCKET_OUT_VECTOR(out_color, "Color");

  return type;
}

RhinoPhysicalSkyTextureNode::RhinoPhysicalSkyTextureNode() : ShaderNode(node_type)
{
}

void RhinoPhysicalSkyTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_PHYSICAL_SKY_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color_out)));

  compiler.add_node(make_float4(sun_dir.x, sun_dir.y, sun_dir.z, atmospheric_density));
  compiler.add_node(__float_as_int(rayleigh_scattering),
                    __float_as_int(mie_scattering),
                    (int)show_sun,
                    __float_as_int(sun_brightness));
  compiler.add_node(make_float4(sun_size, sun_color.x, sun_color.y, sun_color.z));
  compiler.add_node(
      make_float4(inv_wavelengths.x, inv_wavelengths.y, inv_wavelengths.z, exposure));
}

void RhinoPhysicalSkyTextureNode::compile(OSLCompiler &compiler)
{
}

/* Texture Adjustment */

NODE_DEFINE(RhinoTextureAdjustmentTextureNode)
{
  NodeType *type = NodeType::add("rhino_texture_adjustment_texture", create, NodeType::SHADER);

  SOCKET_IN_COLOR(color, "Color", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_BOOLEAN(grayscale, "Grayscale", false);
  SOCKET_BOOLEAN(invert, "Invert", false);
  SOCKET_BOOLEAN(clamp, "Clamp", false);
  SOCKET_BOOLEAN(scale_to_clamp, "ScaleToClamp", false);

  SOCKET_FLOAT(multiplier, "Multiplier", 0.0f);
  SOCKET_FLOAT(clamp_min, "ClampMin", 0.0f);
  SOCKET_FLOAT(clamp_max, "ClampMax", 0.0f);
  SOCKET_FLOAT(gain, "Gain", 0.0f);
  SOCKET_FLOAT(gamma, "Gamma", 0.0f);
  SOCKET_FLOAT(saturation, "Saturation", 0.0f);
  SOCKET_FLOAT(hue_shift, "HueShift", 0.0f);

  SOCKET_BOOLEAN(is_hdr, "IsHdr", false);

  SOCKET_OUT_VECTOR(out_color, "Color");

  return type;
}

RhinoTextureAdjustmentTextureNode::RhinoTextureAdjustmentTextureNode() : ShaderNode(node_type)
{
}

void RhinoTextureAdjustmentTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *color_in = input("Color");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(
      RHINO_NODE_TEXTURE_ADJUSTMENT_TEXTURE,
      compiler.encode_uchar4(compiler.stack_assign(color_in), compiler.stack_assign(color_out)));

  compiler.add_node((int)grayscale, (int)invert, (int)clamp, (int)scale_to_clamp);
  compiler.add_node(__float_as_int(multiplier),
                    __float_as_int(clamp_min),
                    __float_as_int(clamp_max),
                    __float_as_int(gain));
  compiler.add_node(__float_as_int(gamma),
                    __float_as_int(saturation),
                    __float_as_int(hue_shift),
                    (int)is_hdr);
}

void RhinoTextureAdjustmentTextureNode::compile(OSLCompiler &compiler)
{
}

/* Tile */

NODE_DEFINE(RhinoTileTextureNode)
{
  NodeType *type = NodeType::add("rhino_tile_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha1, "Alpha1", 1.0f);
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_FLOAT(alpha2, "Alpha2", 1.0f);

  static NodeEnum type_enum;
  type_enum.insert("3dRectangular", RHINO_TILE_3D_RECTANGULAR);
  type_enum.insert("2dRectangular", RHINO_TILE_2D_RECTANGULAR);
  type_enum.insert("2dHexagonal", RHINO_TILE_2D_HEXAGONAL);
  type_enum.insert("2dTriangular", RHINO_TILE_2D_TRIANGULAR);
  SOCKET_ENUM(tile_type, "Type", type_enum, RHINO_TILE_3D_RECTANGULAR);
  SOCKET_VECTOR(phase, "Phase", make_float3(0.0f));
  SOCKET_VECTOR(join_width, "JoinWidth", make_float3(0.0f));

  SOCKET_OUT_COLOR(out_color, "Color");
  SOCKET_OUT_FLOAT(out_alpha, "Alpha");

  return type;
}

RhinoTileTextureNode::RhinoTileTextureNode() : ShaderNode(node_type)
{
}

void RhinoTileTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *alpha1_in = input("Alpha1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *alpha2_in = input("Alpha2");

  ShaderOutput *color_out = output("Color");
  ShaderOutput *alpha_out = output("Alpha");

  compiler.add_node(
      RHINO_NODE_TILE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(alpha1_in),
                                           compiler.stack_assign(color2_in)),
                    compiler.encode_uchar4(compiler.stack_assign(alpha2_in),
                                           compiler.stack_assign(color_out),
                                           compiler.stack_assign(alpha_out)));

  compiler.add_node(
      (int)tile_type, __float_as_int(phase.x), __float_as_int(phase.y), __float_as_int(phase.z));
  compiler.add_node(
      __float_as_int(join_width.x), __float_as_int(join_width.y), __float_as_int(join_width.z));
}

void RhinoTileTextureNode::compile(OSLCompiler &compiler)
{
}

/* Dots */

NODE_DEFINE(RhinoDotsTextureNode)
{
  NodeType *type = NodeType::add("rhino_dots_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_INT(dots_data_count, "DataCount", 0);
  SOCKET_INT(dots_tree_node_count, "TreeNodeCount", 0);
  SOCKET_FLOAT(sample_area_size, "SampleAreaSize", 0.0f);
  SOCKET_BOOLEAN(rings, "Rings", false);
  SOCKET_FLOAT(ring_radius, "RingRadius", 0.0f);

  static NodeEnum falloff_enum;
  falloff_enum.insert("Flat", RHINO_DOTS_FALLOFF_FLAT);
  falloff_enum.insert("Linear", RHINO_DOTS_FALLOFF_LINEAR);
  falloff_enum.insert("Cubic", RHINO_DOTS_FALLOFF_CUBIC);
  falloff_enum.insert("Elliptic", RHINO_DOTS_FALLOFF_ELLIPTIC);
  SOCKET_ENUM(falloff_type, "FalloffType", falloff_enum, RHINO_DOTS_FALLOFF_FLAT);
  static NodeEnum composition_enum;
  composition_enum.insert("Maximum", RHINO_DOTS_COMPOSITION_MAXIMUM);
  composition_enum.insert("Addition", RHINO_DOTS_COMPOSITION_ADDITION);
  composition_enum.insert("Subtraction", RHINO_DOTS_COMPOSITION_SUBTRACTION);
  composition_enum.insert("Multiplication", RHINO_DOTS_COMPOSITION_MULTIPLICATION);
  composition_enum.insert("Average", RHINO_DOTS_COMPOSITION_AVERAGE);
  composition_enum.insert("Standard", RHINO_DOTS_COMPOSITION_STANDARD);
  SOCKET_ENUM(
      composition_type, "CompositionType", composition_enum, RHINO_DOTS_COMPOSITION_MAXIMUM);

  SOCKET_OUT_COLOR(out_color, "Color");

  return type;
}

RhinoDotsTextureNode::RhinoDotsTextureNode() : ShaderNode(node_type)
{
}

void RhinoDotsTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_DOTS_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));

  compiler.add_node(dots_data_count,
                    dots_tree_node_count,
                    __float_as_int(sample_area_size), (int)rings);
  compiler.add_node(__float_as_int(ring_radius), (int)falloff_type, (int)composition_type);
}

void RhinoDotsTextureNode::compile(OSLCompiler &compiler)
{
}

/* Normal Part 1 */

NODE_DEFINE(RhinoNormalPart1TextureNode)
{
  NodeType *type = NodeType::add("rhino_normal_part1_texture", create, NodeType::SHADER);

  SOCKET_IN_VECTOR(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_OUT_VECTOR(uvw1_out, "UVW1");
  SOCKET_OUT_VECTOR(uvw2_out, "UVW2");
  SOCKET_OUT_VECTOR(uvw3_out, "UVW3");
  SOCKET_OUT_VECTOR(uvw4_out, "UVW4");
  SOCKET_OUT_VECTOR(uvw5_out, "UVW5");
  SOCKET_OUT_VECTOR(uvw6_out, "UVW6");
  SOCKET_OUT_VECTOR(uvw7_out, "UVW7");
  SOCKET_OUT_VECTOR(uvw8_out, "UVW8");

  return type;
}

RhinoNormalPart1TextureNode::RhinoNormalPart1TextureNode() : ShaderNode(node_type)
{
}

void RhinoNormalPart1TextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");

  ShaderOutput *uvw1_out = output("UVW1");
  ShaderOutput *uvw2_out = output("UVW2");
  ShaderOutput *uvw3_out = output("UVW3");
  ShaderOutput *uvw4_out = output("UVW4");
  ShaderOutput *uvw5_out = output("UVW5");
  ShaderOutput *uvw6_out = output("UVW6");
  ShaderOutput *uvw7_out = output("UVW7");
  ShaderOutput *uvw8_out = output("UVW8");

  compiler.add_node(RHINO_NODE_NORMAL_PART1_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(uvw1_out),
                                           compiler.stack_assign(uvw2_out),
                                           compiler.stack_assign(uvw3_out)),
                    compiler.encode_uchar4(compiler.stack_assign(uvw4_out),
                                           compiler.stack_assign(uvw5_out),
                                           compiler.stack_assign(uvw6_out),
                                           compiler.stack_assign(uvw7_out)),
                    compiler.encode_uchar4(compiler.stack_assign(uvw8_out)));
}

void RhinoNormalPart1TextureNode::compile(OSLCompiler &compiler)
{
}

/* Normal Part 2 */

NODE_DEFINE(RhinoNormalPart2TextureNode)
{
  NodeType *type = NodeType::add("rhino_normal_part2_texture", create, NodeType::SHADER);

  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color3, "Color3", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color4, "Color4", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color5, "Color5", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color6, "Color6", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color7, "Color7", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color8, "Color8", make_float3(0.0f, 0.0f, 0.0f));


  SOCKET_OUT_COLOR(color_out, "Color");

  return type;
}

RhinoNormalPart2TextureNode::RhinoNormalPart2TextureNode() : ShaderNode(node_type)
{
}

void RhinoNormalPart2TextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *color3_in = input("Color3");
  ShaderInput *color4_in = input("Color4");
  ShaderInput *color5_in = input("Color5");
  ShaderInput *color6_in = input("Color6");
  ShaderInput *color7_in = input("Color7");
  ShaderInput *color8_in = input("Color8");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_NORMAL_PART2_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color3_in),
                                           compiler.stack_assign(color4_in)),
                    compiler.encode_uchar4(compiler.stack_assign(color5_in),
                                           compiler.stack_assign(color6_in),
                                           compiler.stack_assign(color7_in),
                                           compiler.stack_assign(color8_in)),
                    compiler.encode_uchar4(compiler.stack_assign(color_out)));
}

void RhinoNormalPart2TextureNode::compile(OSLCompiler &compiler)
{
}

CCL_NAMESPACE_END
