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

#include "internal_types.h"

#include "util/algorithm.h"
#include "util/math.h"

#include "mikktspace.h"

using namespace OIIO;

ccl::Geometry *cycles_scene_add_mesh(ccl::Session *session, ccl::Shader *shader_id)
{
	ccl::Scene* sce = session->scene;
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
	ccl::Scene* sce = session->scene;
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
	ASSERT(geometry);

	#if 0
	if (geometry && session->scene)
	{
		session->scene->delete_node(geometry);
	}
	#endif
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

void cycles_mesh_set_smooth(ccl::Session* session_id, ccl::Geometry* geometry, unsigned int smooth)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			bool use_smooth = smooth == 1;
			mesh->get_smooth().resize(mesh->get_triangles().size());

			for (int i = 0; i < mesh->get_triangles().size(); i++)
			{
				mesh->get_smooth()[i] = use_smooth;
			}
		}
	}
}


void cycles_mesh_reserve(ccl::Session* session_id, ccl::Geometry* geometry, unsigned vcount, unsigned fcount)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			mesh->reserve_mesh(vcount, fcount);
		}
	}
}

void cycles_mesh_resize(ccl::Session* session_id, ccl::Geometry* geometry, unsigned vcount, unsigned fcount)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

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

		ASSERT(mesh);

		if (mesh)
		{
			ccl::float3* generated = mesh->attributes.add(ccl::ATTR_STD_GENERATED)->data_float3();

			auto& cycles_mesh_vertices = mesh->get_verts();

			for (int i = 0U, j = 0U; i < in_vcount * 3; i += 3, j++)
			{
				ccl::float3 cycles_vertex;

				cycles_vertex.x = in_verts[i];
				cycles_vertex.y = in_verts[i + 1];
				cycles_vertex.z = in_verts[i + 2];


				//logger.logit("v: ", f3.x, ",", f3.y, ",", f3.z);
				cycles_mesh_vertices[j] = cycles_vertex;
				generated[j] = cycles_vertex;
			}

			//ALB: IT looks like all meshes are triangles in CyclesX
			//mesh->get_geometry_flags = ccl::Mesh::GeometryFlags::GEOMETRY_TRIANGLES;
		}
	}
}

void cycles_mesh_set_tris(ccl::Session *session_id, ccl::Geometry *geometry, int *faces, unsigned int fcount, ccl::Shader *shader_id, unsigned int smooth)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			mesh->reserve_mesh(fcount * 3, fcount);

			auto& cycles_mesh_triangles = mesh->get_triangles();

			for (auto i = 0U, j = 0U; i < fcount * 3; i += 3, j++)
			{
				//logger.logit("f: ", faces[i], ",", faces[i + 1], ",", faces[i + 2]);
				cycles_mesh_triangles[i + 0] = faces[i + 0];
				cycles_mesh_triangles[i + 1] = faces[i + 1];
				cycles_mesh_triangles[i + 2] = faces[i + 2];

				// TODO: XXXX revisit shader handling
				//mesh->shader[j] = shader_id;

				mesh->get_smooth()[j] = (1 == smooth);
			}

			//ALB: IT looks like all meshes are triangles in CyclesX
			//mesh->geometry_flags = ccl::Mesh::GeometryFlags::GEOMETRY_TRIANGLES;

			cycles_geometry_set_shader(session_id, geometry, shader_id);
		}
	}
}

void cycles_mesh_set_triangle(ccl::Session* session_id, ccl::Geometry* geometry, unsigned tri_idx, unsigned int v0, unsigned int v1, unsigned int v2, ccl::Shader *shader_id, unsigned int smooth)
{
	assert(false);

	#if OLD_NOT_USED
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			mesh->get_triangle(tri_idx).v[0] = (int)v0;
			mesh->get_triangle(tri_idx).v[1] = (int)v1;
			mesh->get_triangle(tri_idx).v[2] = (int)v2;


			// TODO: XXXX revisit shader handling
			//me->shader[tri_idx / 3] = shader_id;

			mesh->get_smooth()[tri_idx] = (1 == smooth);

			//I'm not sure about this - this is the old code. [NATHAN_LOOK]
			//mesh->get_smooth()[mesh_id] = (1 == smooth);
		}
	}
	#endif
}

void cycles_mesh_add_triangle(ccl::Session* session_id, ccl::Geometry* geometry, unsigned int v0, unsigned int v1, unsigned int v2, ccl::Shader *shader_id, unsigned int smooth)
{
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			mesh->add_triangle((int)v0, (int)v1, (int)v2, shader_id->id, smooth == 1);
		}
	}
}

void cycles_mesh_set_uvs(ccl::Session* session_id, ccl::Geometry* geometry, float *uvs, unsigned int uvcount, const char* uvmap_name)
{
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			ccl::ustring uvmap = uvmap_name ? ccl::ustring(uvmap_name) : ccl::ustring("uvmap1");

			ccl::Attribute* attr = mesh->attributes.add(ccl::ATTR_STD_UV, uvmap);
			ccl::float2* fdata = attr->data_float2();

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
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			ccl::Attribute* attr = mesh->attributes.add(ccl::ATTR_STD_VERTEX_NORMAL);
			ccl::float3* fdata = attr->data_float3();

			ccl::float3 f3;

			for (int i = 0, j = 0; i < (int)vnormalcount * 3; i += 3, j++)
			{
				f3.x = vnormals[i];
				f3.y = vnormals[i + 1];
				f3.z = vnormals[i + 2];
				fdata[j] = f3;
			}

			//ALB: IT looks like all meshes are triangles in CyclesX
			//me->geometry_flags = ccl::Mesh::GeometryFlags::GEOMETRY_TRIANGLES;
		}
	}
}

void cycles_mesh_set_vertex_colors(ccl::Session* session_id, ccl::Geometry* geometry, float *vcolors, unsigned int vcolorcount)
{
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			ccl::Attribute *attr = mesh->attributes.add(ustring("vertexcolor"),
												 ccl::TypeRGBA,
												 ccl::ATTR_ELEMENT_CORNER_BYTE);

			ccl::uchar4 *cdata = attr->data_uchar4();

			ccl::float4 f4;

			for (int i = 0, j = 0; i < (int)vcolorcount * 3; i += 3, j++)
			{
				f4.x = vcolors[i];
				f4.y = vcolors[i + 1];
				f4.z = vcolors[i + 2];
				f4.w = 1.0f;
				cdata[j] = ccl::color_float4_to_uchar4(f4); //ccl::color_float_to_byte(f3);
			}

			//ALB: IT looks like all meshes are triangles in CyclesX
			//me->geometry_flags = ccl::Mesh::GeometryFlags::GEOMETRY_TRIANGLES;
		}
	}
}


struct MikkUserData {
	MikkUserData(
			 ustring layer_name,
			 const ccl::Mesh *mesh,
			 ccl::float3 *tangent,
			 float *tangent_sign)
		: mesh(mesh),
		  texface(NULL),
		  tangent(tangent),
		  tangent_sign(tangent_sign)
	{
		const ccl::AttributeSet& attributes = mesh->attributes;

		ccl::Attribute *attr_vN = attributes.find(ccl::ATTR_STD_VERTEX_NORMAL);
		vertex_normal = attr_vN->data_float3();

		ccl::Attribute *attr_uv = attributes.find(layer_name);
		if(attr_uv != NULL) {
			texface = attr_uv->data_float2();
		}
	}

	const ccl::Mesh *mesh;
	int num_faces;

	ccl::float3 *vertex_normal;
	ccl::float2 *texface;

	ccl::float3 *tangent;
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
	const ccl::float3 vP = mesh->get_verts()[vertex_index];
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
		vN = userdata->vertex_normal[vertex_index];
	}
	else {
		const ccl::Mesh::Triangle tri = mesh->get_triangle(face_num);
		vN = tri.compute_normal(&mesh->get_verts()[0]);
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
	//auto uvattr = attributes.find(ccl::ATTR_STD_UV);
	attr = attributes.add(ccl::ATTR_STD_UV_TANGENT, name);

	ccl::float3 *tangent = attr->data_float3();
	/* Create bitangent sign attribute. */
	float *tangent_sign = NULL;
	ccl::Attribute *attr_sign;
	ustring name_sign = ustring(std::string(uvmap_name.c_str()) + std::string(".tangent_sign"));

	attr_sign = attributes.add(ccl::ATTR_STD_UV_TANGENT_SIGN, name_sign);
	tangent_sign = attr_sign->data_float();
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
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce))
	{
		auto mesh = dynamic_cast<ccl::Mesh*>(geometry);

		ASSERT(mesh);

		if (mesh)
		{
			mikk_compute_tangents(mesh, ccl::ustring(uvmap_name));
		}
	}
}

#if 0 // POINTINESS
/* Compare vertices by sum of their coordinates. */
class VertexAverageComparator {
public:
	VertexAverageComparator(const ccl::array<ccl::float3>& verts)
			: verts_(verts) {
	}

	bool operator()(const int& vert_idx_a, const int& vert_idx_b)
	{
		const ccl::float3 &vert_a = verts_[vert_idx_a];
		const ccl::float3 &vert_b = verts_[vert_idx_b];
		if(vert_a == vert_b) {
			/* Special case for doubles, so we ensure ordering. */
			return vert_idx_a > vert_idx_b;
		}
		const float x1 = vert_a.x + vert_a.y + vert_a.z;
		const float x2 = vert_b.x + vert_b.y + vert_b.z;
		return x1 < x2;
	}

protected:
	const ccl::array<ccl::float3>& verts_;
};

void attr_create_pointiness(ccl::Mesh *mesh)
{
	const int num_verts = mesh->verts.size();
	if(num_verts == 0) {
		return;
	}
	/* STEP 1: Find out duplicated vertices and point duplicates to a single
	 *         original vertex.
	 */
	ccl::vector<int> sorted_vert_indeices(num_verts);
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		sorted_vert_indeices[vert_index] = vert_index;
	}
	VertexAverageComparator compare(mesh->verts);
	sort(sorted_vert_indeices.begin(), sorted_vert_indeices.end(), compare);
	/* This array stores index of the original vertex for the given vertex
	 * index.
	 */
	ccl::vector<int> vert_orig_index(num_verts);
	for(int sorted_vert_index = 0;
		sorted_vert_index < num_verts;
		++sorted_vert_index)
	{
		const int vert_index = sorted_vert_indeices[sorted_vert_index];
		const ccl::float3 &vert_co = mesh->verts[vert_index];
		bool found = false;
		for(int other_sorted_vert_index = sorted_vert_index + 1;
			other_sorted_vert_index < num_verts;
			++other_sorted_vert_index)
		{
			const int other_vert_index =
					sorted_vert_indeices[other_sorted_vert_index];
			const ccl::float3 &other_vert_co = mesh->verts[other_vert_index];
			/* We are too far away now, we wouldn't have duplicate. */
			if((other_vert_co.x + other_vert_co.y + other_vert_co.z) -
			   (vert_co.x + vert_co.y + vert_co.z) > 3 * FLT_EPSILON)
			{
				break;
			}
			/* Found duplicate. */
			if(len_squared(other_vert_co - vert_co) < FLT_EPSILON) {
				found = true;
				vert_orig_index[vert_index] = other_vert_index;
				break;
			}
		}
		if(!found) {
			vert_orig_index[vert_index] = vert_index;
		}
	}
	/* Make sure we always points to the very first orig vertex. */
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		int orig_index = vert_orig_index[vert_index];
		while(orig_index != vert_orig_index[orig_index]) {
			orig_index = vert_orig_index[orig_index];
		}
		vert_orig_index[vert_index] = orig_index;
	}
	sorted_vert_indeices.free_memory();
	/* STEP 2: Calculate vertex normals taking into account their possible
	 *         duplicates which gets "welded" together.
	 */
	ccl::vector<ccl::float3> vert_normal(num_verts, ccl::make_float3(0.0f, 0.0f, 0.0f));
	/* First we accumulate all vertex normals in the original index. */
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		const ccl::float3 normal = ccl::make_float3(0.0f);// [TODO] ccl::get_float3(b_mesh.vertices[vert_index].normal());
		const int orig_index = vert_orig_index[vert_index];
		vert_normal[orig_index] += normal;
	}
	/* Then we normalize the accumulated result and flush it to all duplicates
	 * as well.
	 */
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		const int orig_index = vert_orig_index[vert_index];
		vert_normal[vert_index] = normalize(vert_normal[orig_index]);
	}
	/* STEP 3: Calculate pointiness using single ring neighborhood. */
	ccl::vector<int> counter(num_verts, 0);
	ccl::vector<float> raw_data(num_verts, 0.0f);
	ccl::vector<ccl::float3> edge_accum(num_verts, ccl::make_float3(0.0f, 0.0f, 0.0f));
#if 0  // TODO FIXUP
	BL::Mesh::edges_iterator e;
	EdgeMap visited_edges;
	int edge_index = 0;
	memset(&counter[0], 0, sizeof(int) * counter.size());
	for(b_mesh.edges.begin(e); e != b_mesh.edges.end(); ++e, ++edge_index) {
		const int v0 = vert_orig_index[b_mesh.edges[edge_index].vertices()[0]],
				  v1 = vert_orig_index[b_mesh.edges[edge_index].vertices()[1]];
		if(visited_edges.exists(v0, v1)) {
			continue;
		}
		visited_edges.insert(v0, v1);
		ccl::float3 co0 = get_ccl::float3(b_mesh.vertices[v0].co()),
			   co1 = get_ccl::float3(b_mesh.vertices[v1].co());
		ccl::float3 edge = normalize(co1 - co0);
		edge_accum[v0] += edge;
		edge_accum[v1] += -edge;
		++counter[v0];
		++counter[v1];
	}
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		const int orig_index = vert_orig_index[vert_index];
		if(orig_index != vert_index) {
			/* Skip duplicates, they'll be overwritten later on. */
			continue;
		}
		if(counter[vert_index] > 0) {
			const ccl::float3 normal = vert_normal[vert_index];
			const float angle =
					safe_acosf(dot(normal,
								   edge_accum[vert_index] / counter[vert_index]));
			raw_data[vert_index] = angle * M_1_PI_F;
		}
		else {
			raw_data[vert_index] = 0.0f;
		}
	}
#endif
	/* STEP 3: Blur vertices to approximate 2 ring neighborhood. */
	ccl::AttributeSet& attributes = mesh->attributes;
	ccl::Attribute *attr = attributes.add(ccl::ATTR_STD_POINTINESS);
	float *data = attr->data_float();
	memcpy(data, &raw_data[0], sizeof(float) * raw_data.size());
	memset(&counter[0], 0, sizeof(int) * counter.size());
#if 0 // TODO FIXUP
	edge_index = 0;
	visited_edges.clear();
	for(b_mesh.edges.begin(e); e != b_mesh.edges.end(); ++e, ++edge_index) {
		const int v0 = vert_orig_index[b_mesh.edges[edge_index].vertices()[0]],
				  v1 = vert_orig_index[b_mesh.edges[edge_index].vertices()[1]];
		if(visited_edges.exists(v0, v1)) {
			continue;
		}
		visited_edges.insert(v0, v1);
		data[v0] += raw_data[v1];
		data[v1] += raw_data[v0];
		++counter[v0];
		++counter[v1];
	}
#endif
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		data[vert_index] /= counter[vert_index] + 1;
	}
	/* STEP 4: Copy attribute to the duplicated vertices. */
	for(int vert_index = 0; vert_index < num_verts; ++vert_index) {
		const int orig_index = vert_orig_index[vert_index];
		data[vert_index] = data[orig_index];
	}
}

#endif
