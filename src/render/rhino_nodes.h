#ifndef __RHINONODES_H__
#define __RHINONODES_H__

#include "render/graph.h"
#include "graph/node.h"
#include "render/nodes.h"
#include "svm_rhino_types.h"

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

CCL_NAMESPACE_END

#endif /* __RHINONODES_H__ */
