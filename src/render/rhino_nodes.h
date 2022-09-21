#ifndef __RHINONODES_H__
#define __RHINONODES_H__

#include "render/graph.h"
#include "graph/node.h"
#include "render/nodes.h"

#include "util/util_array.h"
#include "util/util_string.h"

CCL_NAMESPACE_BEGIN

class ImageManager;
class LightManager;
class Scene;
class Shader;

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

class Rhino_CheckerTexture2dNode : public ShaderNode {
 public:
  SHADER_NODE_CLASS(Rhino_CheckerTexture2dNode)

  float3 uvw, color1, color2;

  virtual int get_group()
  {
    return NODE_GROUP_LEVEL_0;
  }
};


CCL_NAMESPACE_END

#endif /* __RHINONODES_H__ */
