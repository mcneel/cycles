#ifndef __DECAL_H__
#define __DECAL_H__

#include "graph/node.h"

#include "scene/film.h"
#include "scene/image.h"
#include "scene/shader.h"

CCL_NAMESPACE_BEGIN

class Decal : public Node
{
public:
  NODE_DECLARE

  enum Type {
    BOX,
    SPHERE,
    CYLINDER,
    UV
  };

  Type decal_type;
  /* decal origin */
  NODE_SOCKET_API(float3, decal_origin);
  /* decal across vector */
  NODE_SOCKET_API(float3, decal_across);
  /* decal up vector */
  NODE_SOCKET_API(float3, decal_up);

  /* Pxyz transform to map ShaderData->P to normalized mapping primitive */
  NODE_SOCKET_API(Transform, pxyz);
  /* Nxyz transform to map ShaderData->N to normalized mapping primitive */
  NODE_SOCKET_API(Transform, nxyz);
  /* UVW transform to map point to UV(W) space*/
  NODE_SOCKET_API(Transform, uvw);
  /* Sweep used in spherical and tubular projections t. */
  NODE_SOCKET_API(float, horizontal_sweep_start);
  NODE_SOCKET_API(float, horizontal_sweep_end);
  NODE_SOCKET_API(float, vertical_sweep_start);
  NODE_SOCKET_API(float, vertical_sweep_end);

  /* Height used in tubular, radius in spherical and tubular projections. */
  NODE_SOCKET_API(float, height);
  NODE_SOCKET_API(float, radius);
  /* Projection used by decal: both, forward or backward */
  NodeImageDecalProjection decal_projection;

  int get_device_index() const;

  Decal();
  ~Decal();

  protected:
   int index;

   friend class DecalManager;
   friend class SceneManager;

};

class DecalManager
{
  public:
    DecalManager();
    ~DecalManager();

    void device_update(Device *device, DeviceScene *dscene, Scene *scene, Progress &progress);
    void device_free(Device *device, DeviceScene *dscene, bool force_free);

    void tag_update(Scene *scene);
  private:
   bool need_update = false;
};

CCL_NAMESPACE_END

#endif // __DECAL_H__