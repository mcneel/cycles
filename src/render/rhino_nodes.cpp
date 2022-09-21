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

NODE_DEFINE(Rhino_CheckerTexture2dNode)
{
  NodeType *type = NodeType::add("rhino_checker_texture_2d", create, NodeType::SHADER);

  SOCKET_IN_POINT(uvw, "UV", make_float3(0.0f, 0.0f, 0.0f), SocketType::LINK_TEXTURE_GENERATED);
  SOCKET_IN_COLOR(color1, "Color1", make_float3(0.0f, 0.0f, 0.0f));
  SOCKET_IN_COLOR(color2, "Color2", make_float3(0.0f, 0.0f, 0.0f));

  SOCKET_OUT_COLOR(color, "Color");

  return type;
}

Rhino_CheckerTexture2dNode::Rhino_CheckerTexture2dNode() : ShaderNode(node_type)
{
}

void Rhino_CheckerTexture2dNode::compile(SVMCompiler &compiler)
{
  ShaderInput *uvw_in = input("UV");
  ShaderInput *color1_in = input("Color1");
  ShaderInput *color2_in = input("Color2");

  ShaderOutput *color_out = output("Color");

  compiler.add_node(RHINO_NODE_CHECKER_TEXTURE_2D,
                    compiler.encode_uchar4(compiler.stack_assign(uvw_in),
                                           compiler.stack_assign(color1_in),
                                           compiler.stack_assign(color2_in),
                                           compiler.stack_assign_if_linked(color_out))
                   );
}

void Rhino_CheckerTexture2dNode::compile(OSLCompiler &compiler)
{
}

CCL_NAMESPACE_END
