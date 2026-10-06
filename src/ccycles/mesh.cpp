/**
Copyright 2014-2017 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#include <cassert>

#include "internal_types.h"

#include "util/algorithm.h"
#include "util/math.h"

#include "mikktspace.h"

using namespace OIIO;

ccl::Geometry *cycles_scene_add_mesh(ccl::Session *session, ccl::Shader *shader_id)
{
	ccl::Scene* sce = session->scene.get();
	if(sce)
	{
		ccl::Geometry* mesh = sce->create_node<ccl::Mesh>();

		if (shader_id == nullptr)
			shader_id = sce->default_surface;

		mesh->get_used_shaders().push_back_slow(shader_id);

		logger.logit("Add mesh ", sce->geometry.size() - 1, " in scene ", session, " using default surface shader ", shader_id);

		return mesh;
	}

	return nullptr;
}

void cycles_geometry_set_shader(ccl::Session *session, ccl::Geometry *mesh_id, ccl::Shader *shader_id)
{
	ccl::Scene* sce = session->scene.get();
	if(sce) {

		ccl::array<ccl::Node *>& used_shaders = mesh_id->get_used_shaders();

		int idx = -1; 
		for (int i = 0; i < used_shaders.size(); i++) {
			ccl::Node *node = used_shaders[i];
			if (node == shader_id) {
				idx = i;
				break;
			}
		}

		if (idx == -1) {
			idx = (int)used_shaders.size();
			used_shaders.push_back_slow(shader_id);
		}

		ccl::Mesh *mesh = dynamic_cast<ccl::Mesh *>(mesh_id);
		assert(mesh);

		mesh->get_shader().resize(mesh->get_triangles().size());
		for (int i = 0; i < mesh->get_triangles().size(); i++) {
			mesh->get_shader()[i] = idx;
		}

		shader_id->tag_update(sce);
		shader_id->tag_used(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL); // Is UPDATE_ALL correct here?
	}
}

void cycles_geometry_clear(ccl::Session* session, ccl::Geometry* geometry)
{
	/* No-op: the geometry stays in the scene. */
	assert(geometry);
}

void cycles_geometry_tag_rebuild(ccl::Session* session_id, ccl::Geometry* geometry)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		geometry->tag_update(sce, true);
		sce->light_manager->tag_update(sce, ccl::LightManager::MESH_NEED_REBUILD);
	}
}

void cycles_mesh_resize(ccl::Session* session_id, ccl::Geometry* geometry, unsigned vcount, unsigned fcount)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			mesh->resize_mesh(vcount, fcount);
		}
	}
}


void cycles_mesh_set_verts(ccl::Session* session_id, ccl::Geometry* geometry, float *in_verts, unsigned int in_vcount)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			/* 5.2 keeps vertices in the ATTR_STD_POSITION attribute rather than a
			 * plain array, and that attribute has no elements until the mesh is
			 * sized. Resize before taking any pointer, otherwise the writes below
			 * run past the end of a zero-length buffer. */
			mesh->resize_mesh((int)in_vcount, (int)mesh->num_triangles());
			/* ATTR_STD_GENERATED is TypePoint, and 5.2 stores points as packed_float3
			 * - 12 bytes, not the 16 of float3. Writing float3 through this pointer
			 * strides past the end of the buffer and corrupts the heap. */

			ccl::packed_float3 *generated =
				mesh->attributes.add(ccl::ATTR_STD_GENERATED)->data_for_write<ccl::packed_float3>();
			ccl::packed_float3 *cycles_mesh_vertices = mesh->get_position_for_write();

			if (cycles_mesh_vertices == nullptr || generated == nullptr) {
				return;
			}

			for (int i = 0U, j = 0U; i < in_vcount * 3; i += 3, j++)
			{
				ccl::float3 cycles_vertex;

				cycles_vertex.x = in_verts[i];
				cycles_vertex.y = in_verts[i + 1];
				cycles_vertex.z = in_verts[i + 2];

				cycles_mesh_vertices[j] = cycles_vertex;
				generated[j] = cycles_vertex;
			}
		}
	}
}

void cycles_mesh_set_tris(ccl::Session *session_id, ccl::Geometry *geometry, int *faces, unsigned int fcount, ccl::Shader *shader_id, unsigned int smooth)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			/* This was reserve_mesh before 5.2, which only reserved capacity.
			 * resize_mesh actually reallocates, so sizing vertices to fcount * 3
			 * here would discard whatever cycles_mesh_set_verts already uploaded.
			 * Keep the current vertex count; set_verts does the same for tris. */
			mesh->resize_mesh((int)mesh->num_verts(), (int)fcount);

			auto& cycles_mesh_triangles = mesh->get_triangles();

			for (auto i = 0U, j = 0U; i < fcount * 3; i += 3, j++)
			{
				cycles_mesh_triangles[i + 0] = faces[i + 0];
				cycles_mesh_triangles[i + 1] = faces[i + 1];
				cycles_mesh_triangles[i + 2] = faces[i + 2];

				mesh->get_smooth()[j] = (1 == smooth);
			}

			/* Writing straight into the socket arrays bypasses the generated
			 * setters, so nothing marks them dirty and GeometryManager skips
			 * the mesh entirely - it never reaches the BVH and the object
			 * renders as empty space. Tag them by hand. */
			mesh->tag_triangles_modified();
			mesh->tag_shader_modified();
			mesh->tag_smooth_modified();
			mesh->tag_modified();
			mesh->tag_update(sce, true);

			cycles_geometry_set_shader(session_id, geometry, shader_id);
		}
	}
}

void cycles_mesh_set_uvs(ccl::Session* session_id, ccl::Geometry* geometry, float *uvs, unsigned int uvcount, const char* uvmap_name)
{
	assert(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			ccl::ustring uvmap = uvmap_name ? ccl::ustring(uvmap_name) : ccl::ustring("uvmap1");

			ccl::Attribute* attr = mesh->attributes.add(ccl::ATTR_STD_UV, uvmap);
			ccl::float2* fdata = attr->data_for_write<ccl::float2>();

			ccl::float2 f2;

			for (int i = 0, j = 0; i < (int)uvcount * 2; i += 2, j++)
			{
				f2.x = uvs[i];
				f2.y = uvs[i + 1];
				fdata[j] = f2;
			}
		}
	}
}

void cycles_mesh_set_vertex_normals(ccl::Session* session_id, ccl::Geometry* geometry, float *vnormals, unsigned int vnormalcount)
{
	assert(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			ccl::Attribute* attr = mesh->attributes.add(ccl::ATTR_STD_VERTEX_NORMAL);
			/* Normals are stored octahedron-encoded since 5.x, so the buffer is
			 * packed_normal and not float3. Asking for float3 trips the
			 * data_sizeof() == sizeof(T) assert in Attribute::data_for_write. */
			ccl::packed_normal* fdata = attr->data_for_write<ccl::packed_normal>();

			ccl::float3 f3;

			for (int i = 0, j = 0; i < (int)vnormalcount * 3; i += 3, j++)
			{
				f3.x = vnormals[i];
				f3.y = vnormals[i + 1];
				f3.z = vnormals[i + 2];
				fdata[j] = f3;
			}
		}
	}
}

void cycles_mesh_set_vertex_colors(ccl::Session* session_id, ccl::Geometry* geometry, float *vcolors, unsigned int vcolorcount)
{
	assert(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			ccl::Attribute *attr = mesh->attributes.add(ustring("vertexcolor"),
												 ccl::TypeRGBA,
												 ccl::ATTR_ELEMENT_CORNER_BYTE);

			ccl::uchar4 *cdata = attr->data_for_write<ccl::uchar4>();

			ccl::float4 f4;

			for (int i = 0, j = 0; i < (int)vcolorcount * 3; i += 3, j++)
			{
				f4.x = vcolors[i];
				f4.y = vcolors[i + 1];
				f4.z = vcolors[i + 2];
				f4.w = 1.0f;
				cdata[j] = ccl::color_float4_to_uchar4(f4);
			}
		}
	}
}


struct MikkUserData {
	MikkUserData(
			 ustring layer_name,
			 const ccl::Mesh *mesh,
			 ccl::packed_float3 *tangent,
			 float *tangent_sign)
		: mesh(mesh),
		  texface(NULL),
		  tangent(tangent),
		  tangent_sign(tangent_sign)
	{
		const ccl::AttributeSet& attributes = mesh->attributes;

		ccl::Attribute *attr_vN = attributes.find(ccl::ATTR_STD_VERTEX_NORMAL);
		vertex_normal = attr_vN->data<ccl::packed_normal>();

		ccl::Attribute *attr_uv = attributes.find(layer_name);
		if(attr_uv != NULL) {
			texface = attr_uv->data_for_write<ccl::float2>();
		}
	}

	const ccl::Mesh *mesh;
	int num_faces;

	const ccl::packed_normal *vertex_normal;
	ccl::float2 *texface;

	ccl::packed_float3 *tangent;
	float *tangent_sign;
};

static int mikk_get_num_faces(const SMikkTSpaceContext *context)
{
	const MikkUserData *userdata = (const MikkUserData *)context->m_pUserData;
	return userdata->mesh->num_triangles();
}

static int mikk_get_num_verts_of_face(const SMikkTSpaceContext *context,
									  const int face_num)
{
	return 3;
}

static int mikk_vertex_index(const ccl::Mesh *mesh, const int face_num, const int vert_num)
{
	return mesh->get_triangles()[face_num * 3 + vert_num];
}

static int mikk_corner_index(const ccl::Mesh *mesh, const int face_num, const int vert_num)
{
	return face_num * 3 + vert_num;
}

static void mikk_get_position(const SMikkTSpaceContext *context,
							  float P[3],
							  const int face_num, const int vert_num)
{
	const MikkUserData *userdata = (const MikkUserData *)context->m_pUserData;
	const ccl::Mesh *mesh = userdata->mesh;
	const int vertex_index = mikk_vertex_index(mesh, face_num, vert_num);
	const ccl::float3 vP = mesh->get_position()[vertex_index];
	P[0] = vP.x;
	P[1] = vP.y;
	P[2] = vP.z;
}

static void mikk_get_texture_coordinate(const SMikkTSpaceContext *context,
										float uv[2],
										const int face_num, const int vert_num)
{
	const MikkUserData *userdata = (const MikkUserData *)context->m_pUserData;
	const ccl::Mesh *mesh = userdata->mesh;
	if(userdata->texface != NULL) {
		const int corner_index = mikk_corner_index(mesh, face_num, vert_num);
		ccl::float2 tfuv = userdata->texface[corner_index];
		uv[0] = tfuv.x;
		uv[1] = tfuv.y;
	}
	else {
		uv[0] = 0.0f;
		uv[1] = 0.0f;
	}
}

static void mikk_get_normal(const SMikkTSpaceContext *context, float N[3],
							const int face_num, const int vert_num)
{
	const MikkUserData *userdata = (const MikkUserData *)context->m_pUserData;
	const ccl::Mesh *mesh = userdata->mesh;
	ccl::float3 vN;

	if(mesh->get_smooth()[face_num]) {
		const int vertex_index = mikk_vertex_index(mesh, face_num, vert_num);
		vN = userdata->vertex_normal[vertex_index].decode();
	}
	else {
		const ccl::Mesh::Triangle tri = mesh->get_triangle(face_num);
		vN = tri.compute_normal(mesh->get_position());
	}

	N[0] = vN.x;
	N[1] = vN.y;
	N[2] = vN.z;
}

static void mikk_set_tangent_space(const SMikkTSpaceContext *context,
								   const float T[],
								   const float sign,
								   const int face_num, const int vert_num)
{
	MikkUserData *userdata = (MikkUserData *)context->m_pUserData;
	const ccl::Mesh *mesh = userdata->mesh;
	const int corner_index = mikk_corner_index(mesh, face_num, vert_num);
	userdata->tangent[corner_index] = ccl::make_float3(T[0], T[1], T[2]);
	if(userdata->tangent_sign != NULL) {
		userdata->tangent_sign[corner_index] = sign;
	}
}

static void mikk_compute_tangents(ccl::Mesh *mesh, ustring uvmap_name)
{
	/* Create tangent attributes. */
	ccl::AttributeSet& attributes = mesh->attributes;
	ccl::Attribute *attr;
	ustring name = ustring(std::string(uvmap_name.c_str()) + std::string(".tangent"));
	auto uvattr = attributes.find(ccl::ATTR_STD_UV);
	attr = attributes.add(ccl::ATTR_STD_UV_TANGENT, name);

	ccl::packed_float3 *tangent = attr->data_for_write<ccl::packed_float3>();
	/* Create bitangent sign attribute. */
	float *tangent_sign = NULL;
	ccl::Attribute *attr_sign;
	ustring name_sign = ustring(std::string(uvmap_name.c_str()) + std::string(".tangent_sign"));

	attr_sign = attributes.add(ccl::ATTR_STD_UV_TANGENT_SIGN, name_sign);
	tangent_sign = attr_sign->data_for_write<float>();
	/* Setup userdata. */
	MikkUserData userdata(uvmap_name, mesh, tangent, tangent_sign);
	/* Setup interface. */
	SMikkTSpaceInterface sm_interface;
	memset(&sm_interface, 0, sizeof(sm_interface));
	sm_interface.m_getNumFaces = mikk_get_num_faces;
	sm_interface.m_getNumVerticesOfFace = mikk_get_num_verts_of_face;
	sm_interface.m_getPosition = mikk_get_position;
	sm_interface.m_getTexCoord = mikk_get_texture_coordinate;
	sm_interface.m_getNormal = mikk_get_normal;
	sm_interface.m_setTSpaceBasic = mikk_set_tangent_space;
	/* Setup context. */
	SMikkTSpaceContext context;
	memset(&context, 0, sizeof(context));
	context.m_pUserData = &userdata;
	context.m_pInterface = &sm_interface;
	/* Compute tangents. */
	genTangSpaceDefault(&context);
}

void cycles_mesh_attr_tangentspace(ccl::Session* session_id, ccl::Geometry* geometry, const char* uvmap_name)
{
	assert(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		assert(mesh);

		if (mesh)
		{
			mikk_compute_tangents(mesh, ccl::ustring(uvmap_name));
		}
	}
}
