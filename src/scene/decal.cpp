#include "scene/decal.h"

#include "scene/scene.h"
#include "scene/stats.h"

#include "util/time.h"

CCL_NAMESPACE_BEGIN

/* Decal */

NODE_DEFINE(Decal)
{
  NodeType *type = NodeType::add("decal", create);

  SOCKET_VECTOR(decal_origin, "Origin", zero_float3());
  SOCKET_VECTOR(decal_across, "Across", zero_float3());
  SOCKET_VECTOR(decal_up, "Up", zero_float3());

  SOCKET_TRANSFORM(pxyz, "Pxyz", transform_identity());
  SOCKET_TRANSFORM(nxyz, "Nxyz", transform_identity());
  SOCKET_TRANSFORM(uvw, "Uvw", transform_identity());

  SOCKET_FLOAT(horizontal_sweep_start, "Horizontal Sweep Start", 0.0f);
  SOCKET_FLOAT(horizontal_sweep_end, "Horizontal Sweep End", 0.0f);
  SOCKET_FLOAT(vertical_sweep_start, "Vertical Sweep Start", 0.0f);
  SOCKET_FLOAT(vertical_sweep_end, "Vertical Sweep End", 0.0f);

  SOCKET_FLOAT(height, "Height", 0.0f);
  SOCKET_FLOAT(radius, "Radius", 0.0f);

  return type;
}

Decal::Decal() : Node(get_node_type())
{
}

Decal::~Decal()
{
}

int Decal::get_device_index() const
{
  return index;
}


DecalManager::DecalManager()
{}

DecalManager::~DecalManager()
{}
void DecalManager::device_update(Device *device, DeviceScene *dscene, Scene *scene, Progress &progress)
{
  {
    scoped_callback_timer timer([scene](double time) {
      if(scene->update_stats)
      {
        scene->update_stats->geometry.times.add_entry({"decal update", time});
      }
    });
  }
}

void DecalManager::device_free(Device *device, DeviceScene *dscene, bool force_free)
{

}

void DecalManager::tag_update(Scene *scene)
{
  need_update = true;
}

CCL_NAMESPACE_END