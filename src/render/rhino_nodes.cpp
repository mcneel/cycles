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

/* Azimuth Altitude Transform */

NODE_DEFINE(AzimuthAltitudeTransformNode)
{
  NodeType *type = NodeType::add("azimuth_altitude_transform", create, NodeType::SHADER);

  SOCKET_IN_POINT(
      vector, "Vector", make_float3(0.0f, 0.0f, 0.0f), SocketType::LINK_TEXTURE_GENERATED);
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

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f), SocketType::LINK_TEXTURE_GENERATED);
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
                                           compiler.stack_assign_if_linked(color_out))
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

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f), SocketType::LINK_TEXTURE_GENERATED);
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));
  SOCKET_INT(noise_type, "NoiseType", 0);
  SOCKET_INT(spec_synth_type, "SpecSynthType", 0);
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
                                           compiler.stack_assign_if_linked(color_out)));
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

  SOCKET_IN_POINT(uvw, "UVW", make_float3(0.0f, 0.0f, 0.0f), SocketType::LINK_TEXTURE_GENERATED);
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color3, "Color3", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_TRANSFORM(uvw_transform,
                   "UvwTransform",
                   make_transform(1.0f, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0));
  SOCKET_INT(wave_type, "WaveType", 0);
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
                    compiler.encode_uchar4(compiler.stack_assign_if_linked(color_out)));

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

CCL_NAMESPACE_END
