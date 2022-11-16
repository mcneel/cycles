#include "render/colorspace.h"
#include "render/film.h"
#include "render/image.h"
#include "render/integrator.h"
#include "render/light.h"
#include "render/mesh.h"
#include "render/nodes.h"
#include "render/rhino_nodes.h"
#include "render/scene.h"
#include "render/svm.h"
#include "kernel/svm/svm_color_util.h"
#include "kernel/svm/svm_ramp_util.h"
#include "kernel/svm/svm_math_util.h"
#include "kernel/svm/svm_mapping_util.h"
#include "render/osl.h"
#include "render/constant_fold.h"

#include "util/util_sky_model.h"
#include "util/util_foreach.h"
#include "util/util_logging.h"
#include "util/util_transform.h"

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
      NODE_MATRIX_MATH, type, compiler.stack_assign(vector_in), compiler.stack_assign(vector_out));

  compiler.add_node(tfm.x);
  compiler.add_node(tfm.y);
  compiler.add_node(tfm.z);
}

void MatrixMathNode::compile(OSLCompiler &compiler)
{
  compiler.parameter("Matrix", tfm);
  compiler.add(this, "node_matrix_math");
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

NODE_DEFINE(RhinoCheckerTexture2dNode)
{
  NodeType *type = NodeType::add("rhino_checker_texture_2d", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));

  SOCKET_OUT_COLOR(color, "Color");

  return type;
}

RhinoCheckerTexture2dNode::RhinoCheckerTexture2dNode() : ShaderNode(node_type)
{
}

void RhinoCheckerTexture2dNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_CHECKER_TEXTURE_2D,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out))
                   );
  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);
}

void RhinoCheckerTexture2dNode::compile(OSLCompiler &compiler)
{
}

/* Noise */

NODE_DEFINE(RhinoNoiseTextureNode)
{
  NodeType *type = NodeType::add("rhino_noise_texture", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));

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

  return type;
}

RhinoNoiseTextureNode::RhinoNoiseTextureNode() : ShaderNode(node_type)
{
}

void RhinoNoiseTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_NOISE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));
  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);

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
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color3, "Color3", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));
  static NodeEnum wave_type_enum;
  wave_type_enum.insert("linear", RHINO_WAVES_LINEAR);
  wave_type_enum.insert("radial", RHINO_WAVES_RADIAL);
  SOCKET_ENUM(wave_type, "WaveType", wave_type_enum, RHINO_WAVES_LINEAR);
  SOCKET_FLOAT(wave_width, "WaveWidth", 0);
  SOCKET_BOOLEAN(wave_width_texture_on, "WaveWidthTextureOn", 0);
  SOCKET_FLOAT(contrast1, "Contrast1", 0.0f);
  SOCKET_FLOAT(contrast2, "Contrast2", 0.0f);

  SOCKET_OUT_COLOR(color, "Color");

  return type;
}

RhinoWavesTextureNode::RhinoWavesTextureNode() : ShaderNode(node_type)
{
}

void RhinoWavesTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *color3_in = input("Color3");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_WAVES_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color3_in)),
                    compiler.encode_uchar4(compiler.stack_assign(color_out)));

  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);

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

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));

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

  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);

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

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));

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

  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);
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
  SOCKET_IN_COLOR(
      color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(
      color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));

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

  return type;
}

RhinoGradientTextureNode::RhinoGradientTextureNode() : ShaderNode(node_type)
{
}

void RhinoGradientTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_GRADIENT_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));

  compiler.add_node(uvw_transform.x);
  compiler.add_node(uvw_transform.y);
  compiler.add_node(uvw_transform.z);

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
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(blend_color, "BlendColor", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_BOOLEAN(use_blend_color, "UseBlendColor", false);
  SOCKET_FLOAT(blend_factor, "BlendFactor", 0.5f);

  SOCKET_OUT_COLOR(out_color, "Color");

  return type;
}

RhinoBlendTextureNode::RhinoBlendTextureNode() : ShaderNode(node_type)
{
}

void RhinoBlendTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");
  ShaderInput *blend_color_in = input("BlendColor");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_BLEND_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(blend_color_in)),
                    compiler.encode_uchar4(compiler.stack_assign(color_out)));

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

  return type;
}

RhinoExposureTextureNode::RhinoExposureTextureNode() : ShaderNode(node_type)
{
}

void RhinoExposureTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *color_in = input("Color");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(
      RHINO_NODE_EXPOSURE_TEXTURE,
      compiler.encode_uchar4(compiler.stack_assign(color_in), compiler.stack_assign(color_out)));

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
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_BOOLEAN(is_turbulent, "IsTurbulent", false);
  SOCKET_INT(max_octaves, "MaxOctaves", 3);
  SOCKET_FLOAT(gain, "Gain", 0.5f);
  SOCKET_FLOAT(roughness, "Roughness", 0.5f);

  SOCKET_OUT_COLOR(out_color, "Color");

  return type;
}

RhinoFbmTextureNode::RhinoFbmTextureNode() : ShaderNode(node_type)
{
}

void RhinoFbmTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(
      RHINO_NODE_FBM_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));

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
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_INT(cells, "Cells", 0);
  SOCKET_FLOAT(font_thickness, "FontThickness", 1.0f);

  SOCKET_OUT_COLOR(out_color, "Color");

  return type;
}

RhinoGridTextureNode::RhinoGridTextureNode() : ShaderNode(node_type)
{
}

void RhinoGridTextureNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UVW");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_GRID_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));

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

  compiler.add_node(RHINO_NODE_MASK_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(color_in),
                                           compiler.stack_assign(alpha_in),
                                           compiler.stack_assign(color_out)));

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

  SOCKET_OUT_VECTOR(out_color, "Color");

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

  compiler.add_node(RHINO_NODE_PERLIN_MARBLE_TEXTURE,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign(color_out)));

  compiler.add_node(
      levels, __float_as_int(noise_amount), __float_as_int(blur), __float_as_int(size));
  compiler.add_node(__float_as_int(color1_sat), __float_as_int(color2_sat));
}

void RhinoPerlinMarbleTextureNode::compile(OSLCompiler &compiler)
{
}

CCL_NAMESPACE_END
