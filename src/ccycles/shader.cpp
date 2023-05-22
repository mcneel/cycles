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

#include "util/param.h"

OIIO_NAMESPACE_USING

ccl::ShaderNode* _shader_node_find(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id)
{
#if LEGACY_SHADERS
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCShader* sh = csce->shaders[shader_id];
		auto psh = sh->graph->nodes.begin();
		auto end = sh->graph->nodes.end();
		while (psh != end)
		{
			ccl::ShaderNode* shn = (*psh);
			if (shn->id == shnode_id) {
				return shn;
			}
			++psh;
		}
	}
#endif
	return nullptr;
}

void _set_colorspace(ustring& colorspace, int value)
{
	if (value == 0) {
		colorspace = ccl::u_colorspace_raw;
	}
	else {
		colorspace = ccl::u_colorspace_auto;
	}
}

ccl::Shader* cycles_create_shader(ccl::Session* session)
{
	ccl::Shader* shader = session->scene->create_node<ccl::Shader>();
	shader->set_displacement_method(ccl::DisplacementMethod::DISPLACE_TRUE);
	shader->has_displacement = true;
	return shader;
}

int cycles_shader_node_count(ccl::Shader* shader)
{
	return shader->graph->nodes.size();
}

ccl::ShaderNode *cycles_shader_node_get(ccl::Shader *shader, int idx)
{
	int count = 0;
	auto it = shader->graph->nodes.cbegin();
	while (it != shader->graph->nodes.cend() && count < shader->graph->nodes.size() - 1) {
		if (count == idx)
			return *it;
		it++;
	}

	return nullptr;
}

bool cycles_shadernode_get_name(ccl::ShaderNode* shn, void* strholder)
{
	if (shn && strholder) {
		StringHolder *holder = (StringHolder *)strholder;
		std::string name{shn->type->name};

		holder->thestring = name;

		return true;
	}

	return false;
}

bool cycles_shader_get_name(ccl::Shader* sh, void* strholder)
{
	if (sh && strholder) {
		StringHolder *holder = (StringHolder *)strholder;
		std::string name{sh->name};

		holder->thestring = name;

		return true;
	}

	return false;
}

/* Add shader to specified scene. */
unsigned int cycles_scene_add_shader(ccl::Session* session_id, unsigned int shader_id)
{
#if REDO_SHADERS
	// TODO: XXXX redo shader creation
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCShader* sh = csce->shaders[shader_id];
		sce->shaders.push_back(sh->shader);
		sh->shader->tag_update(sce);
		sh->shader->tag_used(sce);
		unsigned int shid = (unsigned int)(sce->shaders.size() - 1);
		sh->scene_mapping.insert({ session_id, shid });
		return shid;
	}
#endif

	return (unsigned int)(-1);
}

void cycles_scene_tag_shader(ccl::Session* session_id, unsigned int shader_id, bool use)
{
#if LEGACY_SHADERS
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCShader* sh = csce->shaders[shader_id];
		sh->shader->tag_update(sce);
		if (use) {
			sh->shader->tag_used(sce);
		}
	}
#endif
}

/* Get Cycles shader ID in specific scene. */
unsigned int cycles_scene_shader_id(ccl::Session* session_id, unsigned int shader_id)
{
#if LEGACY_SHADERS
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCShader* sh = csce->shaders[shader_id];
		if (sh->scene_mapping.find(session_id) != sh->scene_mapping.end()) {
			return sh->scene_mapping[session_id];
		}
	}
#endif
	return (unsigned int)(-1);
}

void cycles_shader_new_graph(ccl::Shader* shader)
{
	shader->set_graph(new ccl::ShaderGraph());
}


void cycles_shader_set_name(ccl::Shader* shader, const char* _name)
{
	shader->name = _name;
}

void cycles_shader_set_use_mis(ccl::Session* session_id, unsigned int shader_id, unsigned int use_mis)
{
	// TODO: XXXX see if something similar is still necessary or if
	// can just fully remove
	//SHADER_SET(session_id, shader_id, bool, use_mis, use_mis == 1)
}

void cycles_shader_set_use_transparent_shadow(ccl::Session* session_id, unsigned int shader_id, unsigned int use_transparent_shadow)
{
	// TODO: XXXX investigate if transparent shadows are managed differently now
	//SHADER_SET(session_id, shader_id, bool, use_transparent_shadow, use_transparent_shadow == 1)
}

void cycles_shader_set_heterogeneous_volume(ccl::Session* session_id, unsigned int shader_id, unsigned int heterogeneous_volume)
{
	// TODO: XXXX figure out if we want volume rendering at all
	// SHADER_SET(session_id, shader_id, bool, heterogeneous_volume, heterogeneous_volume == 1)
}

unsigned int cycles_add_shader_node(ccl::Session* session_id, unsigned int shader_id, shadernode_type shn_type)
{
#if LEGACY_SHADERS
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		ccl::ShaderNode* node = nullptr;
		switch (shn_type) {
		case shadernode_type::OUTPUT:
			node = new ccl::OutputNode();
			break;
		case shadernode_type::BACKGROUND:
			node = new ccl::BackgroundNode();
			break;
		case shadernode_type::DIFFUSE:
			node = new ccl::DiffuseBsdfNode();
			break;
		case shadernode_type::ANISOTROPIC:
			node = new ccl::AnisotropicBsdfNode();
			break;
		case shadernode_type::TRANSLUCENT:
			node = new ccl::TranslucentBsdfNode();
			break;
		case shadernode_type::TRANSPARENT:
			node = new ccl::TransparentBsdfNode();
			break;
		case shadernode_type::VELVET:
			node = new ccl::VelvetBsdfNode();
			break;
		case shadernode_type::TOON:
			node = new ccl::ToonBsdfNode();
			break;
		case shadernode_type::GLOSSY:
			node = new ccl::GlossyBsdfNode();
			break;
		case shadernode_type::GLASS:
			node = new ccl::GlassBsdfNode();
			break;
		case shadernode_type::REFRACTION:
			node = new ccl::RefractionBsdfNode();
			break;
		case shadernode_type::HAIR:
			node = new ccl::HairBsdfNode();
			break;
		case shadernode_type::EMISSION:
			node = new ccl::EmissionNode();
			break;
		case shadernode_type::AMBIENT_OCCLUSION:
			node = new ccl::AmbientOcclusionNode();
			break;
		case shadernode_type::ABSORPTION_VOLUME:
			node = new ccl::AbsorptionVolumeNode();
			break;
		case shadernode_type::SCATTER_VOLUME:
			node = new ccl::ScatterVolumeNode();
			break;
		case shadernode_type::SUBSURFACE_SCATTERING:
			node = new ccl::SubsurfaceScatteringNode();
			break;
		case shadernode_type::VALUE:
			node = new ccl::ValueNode();
			break;
		case shadernode_type::COLOR:
			node = new ccl::ColorNode();
			break;
		case shadernode_type::MIX_CLOSURE:
			node = new ccl::MixClosureNode();
			break;
		case shadernode_type::ADD_CLOSURE:
			node = new ccl::AddClosureNode();
			break;
		case shadernode_type::INVERT:
			node = new ccl::InvertNode();
			break;
		case shadernode_type::MIX:
			node = new ccl::MixNode();
			break;
		case shadernode_type::GAMMA:
			node = new ccl::GammaNode();
			break;
		case shadernode_type::WAVELENGTH:
			node = new ccl::WavelengthNode();
			break;
		case shadernode_type::BLACKBODY:
			node = new ccl::BlackbodyNode();
			break;
		case shadernode_type::CAMERA:
			node = new ccl::CameraNode();
			break;
		case shadernode_type::FRESNEL:
			node = new ccl::FresnelNode();
			break;
		case shadernode_type::MATH:
			node = new ccl::MathNode();
			break;
		case shadernode_type::IMAGE_TEXTURE:
			node = new ccl::ImageTextureNode();
			break;
		case shadernode_type::ENVIRONMENT_TEXTURE:
			node = new ccl::EnvironmentTextureNode();
			break;
		case shadernode_type::BRICK_TEXTURE:
			node = new ccl::BrickTextureNode();
			break;
		case shadernode_type::SKY_TEXTURE:
			node = new ccl::SkyTextureNode();
			break;
		case shadernode_type::CHECKER_TEXTURE:
			node = new ccl::CheckerTextureNode();
			break;
		case shadernode_type::NOISE_TEXTURE:
			node = new ccl::NoiseTextureNode();
			break;
		case shadernode_type::WAVE_TEXTURE:
			node = new ccl::WaveTextureNode();
			break;
		case shadernode_type::MAGIC_TEXTURE:
			node = new ccl::MagicTextureNode();
			break;
		case shadernode_type::MUSGRAVE_TEXTURE:
			node = new ccl::MusgraveTextureNode();
			break;
		case shadernode_type::TEXTURE_COORDINATE:
			node = new ccl::TextureCoordinateNode();
			break;
		case shadernode_type::BUMP:
			node = new ccl::BumpNode();
			break;
		case shadernode_type::RGBTOBW:
			node = new ccl::RGBToBWNode();
			break;
		// TODO: XXXX Port over RgbToLuminance Node from old Cycles integration
		/*
		case shadernode_type::RGBTOLUMINANCE:
			node = new ccl::RGBToLuminanceNode();
			break;
		*/
		case shadernode_type::LIGHTPATH:
			node = new ccl::LightPathNode();
			break;
		case shadernode_type::LIGHTFALLOFF:
			node = new ccl::LightFalloffNode();
			break;
		case shadernode_type::VORONOI_TEXTURE:
			node = new ccl::VoronoiTextureNode();
			break;
		case shadernode_type::LAYERWEIGHT:
			node = new ccl::LayerWeightNode();
			break;
		case shadernode_type::GEOMETRYINFO:
			node = new ccl::GeometryNode();
			break;
		case shadernode_type::COMBINE_XYZ:
			node = new ccl::CombineXYZNode();
			break;
		case shadernode_type::SEPARATE_XYZ:
			node = new ccl::SeparateXYZNode();
			break;
		case shadernode_type::HSV_SEPARATE:
			node = new ccl::SeparateHSVNode();
			break;
		case shadernode_type::HSV_COMBINE:
			node = new ccl::CombineHSVNode();
			break;
		case shadernode_type::RGB_SEPARATE:
			node = new ccl::SeparateRGBNode();
			break;
		case shadernode_type::RGB_COMBINE:
			node = new ccl::CombineRGBNode();
			break;
		case shadernode_type::MAPPING:
			node = new ccl::MappingNode();
			break;
		case shadernode_type::HOLDOUT:
			node = new ccl::HoldoutNode();
			break;
		case shadernode_type::HUE_SAT:
			node = new ccl::HSVNode();
			break;
		case shadernode_type::GRADIENT_TEXTURE:
			node = new ccl::GradientTextureNode();
			break;
		case shadernode_type::COLOR_RAMP:
			node = new ccl::RGBRampNode();
			break;
		case shadernode_type::VECT_MATH:
			node = new ccl::VectorMathNode();
			break;
		case shadernode_type::PRINCIPLED_BSDF:
			node = new ccl::PrincipledBsdfNode();
			break;
		case shadernode_type::ATTRIBUTE:
			node = new ccl::AttributeNode();
			break;
		case shadernode_type::NORMALMAP:
			node = new ccl::NormalMapNode();
			dynamic_cast<ccl::NormalMapNode*>(node)->set_attribute(ustring("uvmap1"));
			break;
		case shadernode_type::WIREFRAME:
			node = new ccl::WireframeNode();
			break;
		case shadernode_type::BRIGHT_CONTRAST:
			node = new ccl::BrightContrastNode();
			break;
		case shadernode_type::OBJECTINFO:
			node = new ccl::ObjectInfoNode();
			break;
		case shadernode_type::TANGENT:
			node = new ccl::TangentNode();
			dynamic_cast<ccl::TangentNode*>(node)->set_attribute(ustring("uvmap1"));
			dynamic_cast<ccl::TangentNode*>(node)->set_direction_type(ccl::NodeTangentDirectionType::NODE_TANGENT_UVMAP);
			break;
		case shadernode_type::DISPLACEMENT:
			node = new ccl::DisplacementNode();
			dynamic_cast<ccl::DisplacementNode*>(node)->set_space(ccl::NodeNormalMapSpace::NODE_NORMAL_MAP_OBJECT);
			break;
		case shadernode_type::RHINO_TEXTURE_COORDINATE:
			node = new ccl::RhinoTextureCoordinateNode();
			break;
		case shadernode_type::RHINO_MATRIX_MATH:
			node = new ccl::MatrixMathNode();
			break;
		case shadernode_type::RHINO_AZIMUTH_ALTITUDE_TRANSFORM:
			node = new ccl::AzimuthAltitudeTransformNode();
			break;
		case shadernode_type::RHINO_CHECKER_TEXTURE:
			node = new ccl::RhinoCheckerTextureNode();
			break;
		case shadernode_type::RHINO_NOISE_TEXTURE:
			node = new ccl::RhinoNoiseTextureNode();
			break;
		case shadernode_type::RHINO_WAVES_TEXTURE:
			node = new ccl::RhinoWavesTextureNode();
			break;
		case shadernode_type::RHINO_WAVES_WIDTH_TEXTURE:
			node = new ccl::RhinoWavesWidthTextureNode();
			break;
		case shadernode_type::RHINO_PERTURBING_PART1_TEXTURE:
			node = new ccl::RhinoPerturbingPart1TextureNode();
			break;
		case shadernode_type::RHINO_PERTURBING_PART2_TEXTURE:
			node = new ccl::RhinoPerturbingPart2TextureNode();
			break;
		case shadernode_type::RHINO_GRADIENT_TEXTURE:
			node = new ccl::RhinoGradientTextureNode();
			break;
		case shadernode_type::RHINO_BLEND_TEXTURE:
			node = new ccl::RhinoBlendTextureNode();
			break;
		case shadernode_type::RHINO_EXPOSURE_TEXTURE:
			node = new ccl::RhinoExposureTextureNode();
			break;
		case shadernode_type::RHINO_FBM_TEXTURE:
			node = new ccl::RhinoFbmTextureNode();
			break;
		case shadernode_type::RHINO_GRID_TEXTURE:
			node = new ccl::RhinoGridTextureNode();
			break;
		case shadernode_type::RHINO_PROJECTION_CHANGER_TEXTURE:
			node = new ccl::RhinoProjectionChangerTextureNode();
			break;
		case shadernode_type::RHINO_MASK_TEXTURE:
			node = new ccl::RhinoMaskTextureNode();
			break;
		case shadernode_type::RHINO_PERLIN_MARBLE_TEXTURE:
			node = new ccl::RhinoPerlinMarbleTextureNode();
			break;
		case shadernode_type::RHINO_PHYSICAL_SKY_TEXTURE:
			node = new ccl::RhinoPhysicalSkyTextureNode();
			break;
		case shadernode_type::RHINO_TEXTURE_ADJUSTMENT_TEXTURE:
			node = new ccl::RhinoTextureAdjustmentTextureNode();
			break;
		case shadernode_type::RHINO_TILE_TEXTURE:
			node = new ccl::RhinoTileTextureNode();
			break;
		case shadernode_type::RHINO_NORMAL_PART1_TEXTURE:
			node = new ccl::RhinoNormalPart1TextureNode();
			break;
		case shadernode_type::RHINO_NORMAL_PART2_TEXTURE:
			node = new ccl::RhinoNormalPart2TextureNode();
			break;
		}

		assert(node);

		if (node) {
			// TODO: XXXX csce->shaders[shader_id]->graph->add(node);
			return (unsigned int)(node->id);
		}
	}
#endif
	return (unsigned int)-1;
}

enum class attr_type {
	INT,
	FLOAT,
	FLOAT4,
};

struct attrunion {
	attr_type type;
	union {
		int i;
		float f;
		ccl::float4 f4;
	};
};

void _set_texture_mapping_transformation(ccl::TextureMapping& mapping, int transform_type, float x, float y, float z)
{
	switch (transform_type) {
	case 0:
		mapping.translation.x = x;
		mapping.translation.y = y;
		mapping.translation.z = z;
		break;
	case 1:
		mapping.rotation.x = x;
		mapping.rotation.y = y;
		mapping.rotation.z = z;
		break;
	case 2:
		mapping.scale.x = x;
		mapping.scale.y = y;
		mapping.scale.z = z;
		break;
	}
}
void _set_mapping_node(ccl::MappingNode* node, int transform_type, float x, float y, float z)
{
	switch (transform_type) {
	case 0:
		node->set_location(ccl::make_float3(x, y, z));
		break;
	case 1:
		node->set_rotation(ccl::make_float3(x, y, z));
		break;
	case 2:
		node->set_scale(ccl::make_float3(x, y, z));
		break;
	}
}

void cycles_shadernode_texmapping_set_transformation(ccl::ShaderNode* shnode, int transform_type, float x, float y, float z)
{
#if LEGACY_SHADERS
	ccl::ShaderNode* shnode = _shader_node_find(session_id, shader_id, shnode_id);
	if (shnode) {
		std::string tp{ "UNKNOWN" };
		switch (transform_type) {
		case 0:
			tp = "TRANSLATION";
			break;
		case 1:
			tp = "ROTATION";
			break;
		case 2:
			tp = "SCALE";
			break;
		}
		switch (shn_type) {
		case shadernode_type::MAPPING:
		{
			ccl::MappingNode* node = dynamic_cast<ccl::MappingNode*>(shnode);
			_set_mapping_node(node, transform_type, x, y, z);
		}
		break;
		case shadernode_type::ENVIRONMENT_TEXTURE:
		{
			ccl::EnvironmentTextureNode* node = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::IMAGE_TEXTURE:
		{
			ccl::ImageTextureNode* node = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::GRADIENT_TEXTURE:
		{
			ccl::GradientTextureNode* node = dynamic_cast<ccl::GradientTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::WAVE_TEXTURE:
		{
			ccl::WaveTextureNode* node = dynamic_cast<ccl::WaveTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::VORONOI_TEXTURE:
		{
			ccl::VoronoiTextureNode* node = dynamic_cast<ccl::VoronoiTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::MUSGRAVE_TEXTURE:
		{
			ccl::MusgraveTextureNode* node = dynamic_cast<ccl::MusgraveTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::BRICK_TEXTURE:
		{
			ccl::BrickTextureNode* node = dynamic_cast<ccl::BrickTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::MAGIC_TEXTURE:
		{
			ccl::MagicTextureNode* node = dynamic_cast<ccl::MagicTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		case shadernode_type::NOISE_TEXTURE:
		{
			ccl::NoiseTextureNode* node = dynamic_cast<ccl::NoiseTextureNode*>(shnode);
			_set_texture_mapping_transformation(node->tex_mapping, transform_type, x, y, z);
		}
		break;
		default:
			break;
		}
	}
#endif
}

void _set_texmapping_mapping(ccl::TextureMapping& tex_mapping, ccl::TextureMapping::Mapping x, ccl::TextureMapping::Mapping y, ccl::TextureMapping::Mapping z)
{
	tex_mapping.x_mapping = x;
	tex_mapping.y_mapping = y;
	tex_mapping.z_mapping = z;
}

void cycles_shadernode_texmapping_set_mapping(ccl::ShaderNode* shnode, ccl::TextureMapping::Mapping x, ccl::TextureMapping::Mapping y, ccl::TextureMapping::Mapping z)
{
	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "mapping")
		{
			/*
			ccl::MappingNode* node = dynamic_cast<ccl::MappingNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
			*/
		}
		else if (shn_type == "environment_texture")
		{
			ccl::EnvironmentTextureNode* node = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "image_texture")
		{
			ccl::ImageTextureNode* node = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "gradient_texture")
		{
			ccl::GradientTextureNode* node = dynamic_cast<ccl::GradientTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "wave_texture")
		{
			ccl::WaveTextureNode* node = dynamic_cast<ccl::WaveTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "voronoi_texture")
		{
			ccl::VoronoiTextureNode* node = dynamic_cast<ccl::VoronoiTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "musgrave_texture")
		{
			ccl::MusgraveTextureNode* node = dynamic_cast<ccl::MusgraveTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "brick_texture")
		{
			ccl::BrickTextureNode* node = dynamic_cast<ccl::BrickTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "magic_texture")
		{
			ccl::MagicTextureNode* node = dynamic_cast<ccl::MagicTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
		else if (shn_type == "noise_texture")
		{
			ccl::NoiseTextureNode* node = dynamic_cast<ccl::NoiseTextureNode*>(shnode);
			_set_texmapping_mapping(node->tex_mapping, x, y, z);
		}
	}
}

void cycles_shadernode_texmapping_set_projection(ccl::ShaderNode* shnode, ccl::TextureMapping::Projection tm_projection)
{
	/*
	ccl::ShaderNode* shnode = _shader_node_find(session_id, shader_id, shnode_id);
	if (shnode) {
		switch (shn_type) {
		case shadernode_type::MAPPING:
			{
				ccl::MappingNode* node = dynamic_cast<ccl::MappingNode*>(shnode);
				node->tex_mapping.projection = tm_projection;
			}
			break;
		default:
			break;
		}
	}
	*/
}

void cycles_shadernode_texmapping_set_type(ccl::ShaderNode* shnode, ccl::NodeMappingType tm_type)
{
	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "mapping")
		{
			ccl::MappingNode* node = dynamic_cast<ccl::MappingNode*>(shnode);
			node->set_mapping_type(tm_type);
		}
	}
}

/* TODO: add all enum possibilities.
 */
void cycles_shadernode_set_enum(ccl::ShaderNode* shnode, const char* enum_name, int value)
{
	auto ename = std::string{ enum_name };

	auto shntype = shnode->type->name.string();

	if (shnode) {
		if (shntype == "math") {
			ccl::MathNode* node = dynamic_cast<ccl::MathNode*>(shnode);
			node->set_math_type((ccl::NodeMathType)value);
		}
		else if (shntype == "vector_math")
		{
			ccl::VectorMathNode* node = dynamic_cast<ccl::VectorMathNode*>(shnode);
			node->set_math_type((ccl::NodeVectorMathType)value);
		}
		else if (shntype == "matrix_math")
		{
			ccl::MatrixMathNode* node = dynamic_cast<ccl::MatrixMathNode*>(shnode);
			node->type = (ccl::NodeMatrixMath)value;
		}
		else if (shntype == "mix")
		{
			ccl::MixNode* node = dynamic_cast<ccl::MixNode*>(shnode);
			node->set_mix_type((ccl::NodeMix)value);
		}
		else if (shntype == "refraction_bsdf")
		{
			ccl::RefractionBsdfNode* node = dynamic_cast<ccl::RefractionBsdfNode*>(shnode);
			node->set_distribution((ccl::ClosureType)value);
		}
		else if (shntype == "toon_bsdf")
		{
			ccl::ToonBsdfNode* node = dynamic_cast<ccl::ToonBsdfNode*>(shnode);
			node->set_component((ccl::ClosureType)value);
		}
		else if (shntype == "glossy_bsdf")
		{
			ccl::GlossyBsdfNode* node = dynamic_cast<ccl::GlossyBsdfNode*>(shnode);
			node->set_distribution((ccl::ClosureType)value);
		}
		else if (shntype == "glass_bsdf")
		{
			ccl::GlassBsdfNode* node = dynamic_cast<ccl::GlassBsdfNode*>(shnode);
			node->set_distribution((ccl::ClosureType)value);
		}
		else if (shntype == "anisotropic_bsdf")
		{
			ccl::AnisotropicBsdfNode* node = dynamic_cast<ccl::AnisotropicBsdfNode*>(shnode);
			node->set_distribution((ccl::ClosureType)value);
		}
		else if (shntype == "wave_texture")
		{
			if (ename == "wave") {
				ccl::WaveTextureNode* node = dynamic_cast<ccl::WaveTextureNode*>(shnode);
				node->set_wave_type((ccl::NodeWaveType)value);
			}
			if (ename == "profile") {
				ccl::WaveTextureNode* node = dynamic_cast<ccl::WaveTextureNode*>(shnode);
				node->set_profile((ccl::NodeWaveProfile)value);
			}
		}
		else if (shntype == "voronoi_texture")
		{
			ccl::VoronoiTextureNode* node = dynamic_cast<ccl::VoronoiTextureNode*>(shnode);
			if (ename == "metric") {
				node->set_metric((ccl::NodeVoronoiDistanceMetric)value);
			}
			else if (ename == "feature") {
				node->set_feature((ccl::NodeVoronoiFeature)value);
			}
			else if (ename == "dimension") {
				node->set_dimensions(value);
			}
		}
		else if (shntype == "musgrave_texture")
		{
			ccl::MusgraveTextureNode* node = dynamic_cast<ccl::MusgraveTextureNode*>(shnode);
			if(ename=="musgrave") {
				node->set_musgrave_type((ccl::NodeMusgraveType)value);
			} else if(ename=="dimension") {
				node->set_dimensions(value);
			}
		}
		else if (shntype == "sky_texture")
		{
			ccl::SkyTextureNode* node = dynamic_cast<ccl::SkyTextureNode*>(shnode);
			node->set_sky_type((ccl::NodeSkyType)value);
		}
		else if (shntype == "environment_texture")
		{
			ccl::EnvironmentTextureNode* node = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
			if (ename == "color_space") {
				ustring colspace;
				_set_colorspace(colspace, value);
				node->set_colorspace(colspace);
			}
			else if (ename == "projection") {
				node->set_projection((ccl::NodeEnvironmentProjection)value);
			}
			if (ename == "interpolation") {
				node->set_interpolation((ccl::InterpolationType)value);
			}
		}
		else if (shntype == "image_texture")
		{
			ccl::ImageTextureNode* node = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			if (ename == "color_space") {
				ustring colspace;
				_set_colorspace(colspace, value);
				node->set_colorspace(colspace);
			}
			else if (ename == "projection") {
				node->set_projection((ccl::NodeImageProjection)value);
			}
			else if (ename == "interpolation") {
				node->set_interpolation((ccl::InterpolationType)value);
			}
		}
		else if (shntype == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *node =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			if (ename == "decal_projection") {
				node->set_decal_projection((ccl::NodeImageDecalProjection)value);
			}
		}
		else if (shntype == "gradient_texture")
		{
			ccl::GradientTextureNode* node = dynamic_cast<ccl::GradientTextureNode*>(shnode);
			node->set_gradient_type((ccl::NodeGradientType)value);
		}
		else if (shntype == "subsurface_scattering")
		{
			ccl::SubsurfaceScatteringNode* node = dynamic_cast<ccl::SubsurfaceScatteringNode*>(shnode);
			node->set_method((ccl::ClosureType)value);
		}
		else if (shntype == "principled_bsdf")
		{
			if (ename == "distribution") {
				ccl::PrincipledBsdfNode* node = dynamic_cast<ccl::PrincipledBsdfNode*>(shnode);
				node->set_distribution((ccl::ClosureType)value);
			}
			else if (ename == "sss") {
				ccl::PrincipledBsdfNode* node = dynamic_cast<ccl::PrincipledBsdfNode*>(shnode);
				node->set_subsurface_method((ccl::ClosureType)value);
			}

		}
		else if (shntype == "normal_map")
		{
			ccl::NormalMapNode* node = dynamic_cast<ccl::NormalMapNode*>(shnode);
			node->set_space((ccl::NodeNormalMapSpace)value);
		}
	}
}

CCImage* find_existing_ccimage(std::string imgname, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels, bool is_float, CCScene* csce)
{
	CCImage *existing_image = nullptr;
	for (CCImage *im : csce->images)
	{
		if (im
				&& im->filename == imgname
				&& im->width == (int)width
				&& im->height == (int)height
				&& im->depth == (int)depth
				&& im->channels == (int)channels
				&& im->is_float == is_float)
		{
			existing_image = im;
			break;
		}
	}
	return existing_image;
}

template <class T>
CCImage* get_ccimage(std::string imgname, T* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels, bool is_float, ccl::Session* session_id)
{
#if LEGACY_IMAGES
	// TODO: XXXX
	CCImage* nimg = nullptr;
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCImage* existing_image = find_existing_ccimage(imgname, width, height, depth, channels, is_float, csce);
		nimg = existing_image ? existing_image : new CCImage();
		if (!existing_image) {
			nimg->builtin_data = img;
			nimg->filename = imgname;
			nimg->width = (int)width;
			nimg->height = (int)height;
			nimg->depth = (int)depth;
			nimg->channels = (int)channels;
			nimg->is_float = is_float;
			bool found_empty_slot = false;
			int imgid{0};
			for(CCImage* slotimg : csce->images) {
				if(slotimg==nullptr) {
					csce->images[imgid] = nimg;
					found_empty_slot = true;
					break;
				}
				imgid++;
			}
			if(!found_empty_slot) {
				csce->images.push_back(nimg);
			}
		}
		else {
			existing_image->builtin_data = img;
		}

	}
	return nimg;
#endif
	return nullptr;
}

void cycles_shadernode_set_member_float_img(ccl::ShaderNode* shnode, const char* member_name, const char* img_name, float* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels)
{
	// TODO: XXXX reimplement image/env texture setting
	/*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &csce, &sce)) {

		auto mname = std::string{ member_name };
		auto imname = std::string{ img_name };

		ccl::ShaderNode* shnode = _shader_node_find(session_id, shader_id, shnode_id);
		if (shnode) {
			switch (shn_type) {
			case shadernode_type::IMAGE_TEXTURE:
			{
				CCImage* nimg = get_ccimage<float>(imname, img, width, height, depth, channels, true, session_id);
				ccl::ImageTextureNode* imtex = dynamic_cast<ccl::ImageTextureNode*>(shnode);
				imtex->builtin_data = nimg;
				imtex->filename = nimg->filename;
				sce->image_manager->tag_reload_image(imname);
			}
			break;
			case shadernode_type::ENVIRONMENT_TEXTURE:
			{
				CCImage* nimg = get_ccimage<float>(imname, img, width, height, depth, channels, true, session_id);
				ccl::EnvironmentTextureNode* envtex = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
				envtex->builtin_data = nimg;
				envtex->filename = nimg->filename;
				sce->image_manager->tag_reload_image(imname);
			}
			break;
			default:
				break;
			}
		}
	}
	*/
}

void cycles_shadernode_set_member_byte_img(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, unsigned char* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels)
{
	// TODO: XXXX reimplement image/env texture setting
	/*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &csce, &sce)) {

		auto mname = std::string{ member_name };
		auto imname = std::string{ img_name };
		ccl::ShaderNode* shnode = _shader_node_find(session_id, shader_id, shnode_id);
		if (shnode) {
			switch (shn_type) {
			case shadernode_type::IMAGE_TEXTURE:
			{
				CCImage* nimg = get_ccimage<unsigned char>(imname, img, width, height, depth, channels, false, session_id);
				ccl::ImageTextureNode* imtex = dynamic_cast<ccl::ImageTextureNode*>(shnode);
				imtex->builtin_data = nimg;
				imtex->filename = nimg->filename;
				sce->image_manager->tag_reload_image(imname);
			}
			break;
			case shadernode_type::ENVIRONMENT_TEXTURE:
			{
				CCImage* nimg = get_ccimage<unsigned char>(imname, img, width, height, depth, channels, false, session_id);
				ccl::EnvironmentTextureNode* envtex = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
				envtex->builtin_data = nimg;
				envtex->filename = nimg->filename;
				sce->image_manager->tag_reload_image(imname);
			}
			break;
			default:
				break;
			}
		}
	}
	*/
}

void cycles_shadernode_set_member_bool(ccl::ShaderNode* shnode, const char* member_name, bool value)
{
	auto mname = std::string{ member_name };
	if (shnode) {
		std::string shntype = shnode->type->name.string();
		if (shntype == "math")
		{
			ccl::MathNode* mnode = dynamic_cast<ccl::MathNode*>(shnode);
			mnode->set_use_clamp(value);
		}
		if (shntype == "rgb_ramp")
		{
			ccl::RGBRampNode* colorramp = dynamic_cast<ccl::RGBRampNode*>(shnode);
			if (mname == "interpolate")
			{
				colorramp->set_interpolate(value);
			}
		}
		if (shntype == "bump")
		{
			ccl::BumpNode* bump = dynamic_cast<ccl::BumpNode*>(shnode);
			if (mname == "invert") {
				bump->set_invert(value);
			}
		}
		if (shntype == "image_texture")
		{
			ccl::ImageTextureNode* imgtex = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			if (mname == "use_alpha") {
				if (value) {
					imgtex->set_alpha_type(ccl::ImageAlphaType::IMAGE_ALPHA_AUTO);
				}
				else {
					imgtex->set_alpha_type(ccl::ImageAlphaType::IMAGE_ALPHA_IGNORE);
				}
			}
			/*else if (mname == "is_linear") {
				imgtex->is_linear = value;
			}*/
			// TODO: XXXX port over alternate_tiles support from old Cycles integration
			/*
			else if (mname == "alternate_tiles") {
				imgtex->alternate_tiles = value;
			}
			*/
		}
		if (shntype == "environment_texture")
		{
			/*ccl::EnvironmentTextureNode* envtex = dynamic_cast<ccl::EnvironmentTextureNode*>(shnode);
			if (mname == "is_linear") {
				envtex->is_linear = value;
			}*/
		}
		if (shntype == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *texco =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			if (mname == "use_transform") {
				texco->set_use_transform(value);
			}
		}
		if (shntype == "mix")
		{
			ccl::MixNode* mix = dynamic_cast<ccl::MixNode*>(shnode);
			if (mname == "use_clamp") {
				mix->set_use_clamp(value);
			}
		}
		if (shntype == "rhino_noise_texture")
		{
			ccl::RhinoNoiseTextureNode* node = dynamic_cast<ccl::RhinoNoiseTextureNode*>(shnode);
			if (mname == "ScaleToClamp")
				node->scale_to_clamp = value;
			if (mname == "Inverse")
				node->inverse = value;
			if (mname == "OctaveCount")
				node->octave_count = value;
		}
		if (shntype == "rhino_waves_texture")
		{
			ccl::RhinoWavesTextureNode* node = dynamic_cast<ccl::RhinoWavesTextureNode*>(shnode);
			if (mname == "WaveWidthTextureOn")
				node->wave_width_texture_on = value;
		}
		if (shntype == "rhino_gradient_texture")
		{
			ccl::RhinoGradientTextureNode* node = dynamic_cast<ccl::RhinoGradientTextureNode*>(shnode);
			if (mname == "FlipAlternate")
				node->flip_alternate = value;
			else if (mname == "UseCustomCurve")
				node->use_custom_curve = value;
		}
		if (shntype == "rhino_blend_texture")
		{
			ccl::RhinoBlendTextureNode* node = dynamic_cast<ccl::RhinoBlendTextureNode*>(shnode);
			if (mname == "UseBlendColor")
				node->use_blend_color = value;
		}
		if (shntype == "rhino_fbm_texture")
		{
			ccl::RhinoFbmTextureNode* node = dynamic_cast<ccl::RhinoFbmTextureNode*>(shnode);
			if (mname == "IsTurbulent")
				node->is_turbulent = value;
		}
		if (shntype == "rhino_physical_sky_texture")
		{
			ccl::RhinoPhysicalSkyTextureNode* node = dynamic_cast<ccl::RhinoPhysicalSkyTextureNode*>(shnode);
			if (mname == "ShowSun")
				node->show_sun = value;
		}
		if (shntype == "rhino_texture_adjustment_texture")
		{
			ccl::RhinoTextureAdjustmentTextureNode* node = dynamic_cast<ccl::RhinoTextureAdjustmentTextureNode*>(shnode);
			if (mname == "Grayscale")
				node->grayscale = value;
			else if (mname == "Invert")
				node->invert = value;
			else if (mname == "Clamp")
				node->clamp = value;
			else if (mname == "ScaleToClamp")
				node->scale_to_clamp = value;
			else if (mname == "IsHdr")
				node->is_hdr = value;
		}
	}
}

void cycles_shadernode_set_member_int(ccl::ShaderNode* shnode, const char* member_name, int value)
{
	auto mname = std::string{ member_name };
	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "brick_texture")
		{
			ccl::BrickTextureNode* bricknode = dynamic_cast<ccl::BrickTextureNode*>(shnode);
			if (mname == "offset_frequency")
				bricknode->set_offset_frequency(value);
			else if (mname == "squash_frequency")
				bricknode->set_squash_frequency(value);
		}
		else if (shn_type == "image_texture")
		{
			ccl::ImageTextureNode* imgnode = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			if (mname == "interpolation") {
				imgnode->set_interpolation((ccl::InterpolationType)value);
			}
			if (mname == "extension") {
				imgnode->set_extension((ccl::ExtensionType)value);
			}
		}
		else if (shn_type == "magic_texture")
		{
			ccl::MagicTextureNode* envnode = dynamic_cast<ccl::MagicTextureNode*>(shnode);
			if (mname == "depth") {
				envnode->set_depth(value);
			}
		}
		else if (shn_type == "rhino_noise_texture")
		{
			ccl::RhinoNoiseTextureNode* node = dynamic_cast<ccl::RhinoNoiseTextureNode*>(shnode);
			if (mname == "NoiseType")
				node->noise_type = (ccl::RhinoProceduralNoiseType)value;
			if (mname == "SpecSynthType")
				node->spec_synth_type = (ccl::RhinoProceduralSpecSynthType)value;
			if (mname == "OctaveCount")
				node->octave_count = value;
		}
		else if (shn_type == "rhino_waves_texture")
		{
			ccl::RhinoWavesTextureNode* node = dynamic_cast<ccl::RhinoWavesTextureNode*>(shnode);
			if (mname == "WaveType")
				node->wave_type = (ccl::RhinoProceduralWavesType)value;
		}
		else if (shn_type == "rhino_waves_width_texture")
		{
			ccl::RhinoWavesWidthTextureNode* node = dynamic_cast<ccl::RhinoWavesWidthTextureNode*>(shnode);
			if (mname == "WaveType")
				node->wave_type = (ccl::RhinoProceduralWavesType)value;
		}
		else if (shn_type == "rhino_gradient_texture")
		{
			ccl::RhinoGradientTextureNode* node = dynamic_cast<ccl::RhinoGradientTextureNode*>(shnode);
			if (mname == "GradientType")
				node->gradient_type = (ccl::RhinoProceduralGradientType)value;
			else
				assert(false);
		}
		else if (shn_type == "rhino_fbm_texture")
		{
			ccl::RhinoFbmTextureNode* node = dynamic_cast<ccl::RhinoFbmTextureNode*>(shnode);
			if (mname == "MaxOctaves")
				node->max_octaves = value;
		}
		else if (shn_type == "rhino_grid_texture")
		{
			ccl::RhinoGridTextureNode* node = dynamic_cast<ccl::RhinoGridTextureNode*>(shnode);
			if (mname == "Cells")
				node->cells = value;
		}
		else if (shn_type == "rhino_projection_changer_texture")
		{
			ccl::RhinoProjectionChangerTextureNode* node = dynamic_cast<ccl::RhinoProjectionChangerTextureNode*>(shnode);
			if (mname == "InputProjectionType")
				node->input_projection_type = (ccl::RhinoProceduralProjectionType)value;
			else if (mname == "OutputProjectionType")
				node->output_projection_type = (ccl::RhinoProceduralProjectionType)value;
		}
		else if (shn_type == "rhino_mask_texture")
		{
			ccl::RhinoMaskTextureNode* node = dynamic_cast<ccl::RhinoMaskTextureNode*>(shnode);
			if (mname == "MaskType")
				node->mask_type = (ccl::RhinoProceduralMaskType)value;
		}
		else if (shn_type == "rhino_perlin_marble_texture")
		{
			ccl::RhinoPerlinMarbleTextureNode* node = dynamic_cast<ccl::RhinoPerlinMarbleTextureNode*>(shnode);
			if (mname == "Levels")
				node->levels = value;
		}
		else if (shn_type == "rhino_tile_texture")
		{
			ccl::RhinoTileTextureNode* node = dynamic_cast<ccl::RhinoTileTextureNode*>(shnode);
			if (mname == "Type")
				node->tile_type = (ccl::RhinoProceduralTileType)value;
		}
	}
}


void cycles_shadernode_set_member_float(ccl::ShaderNode* shnode, const char* member_name, float value)
{
	auto mname = std::string{ member_name };

	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "value")
		{
			ccl::ValueNode* valuenode = dynamic_cast<ccl::ValueNode*>(shnode);
			valuenode->set_value(value);
		}
		else if (shn_type == "image_texture")
		{
			ccl::ImageTextureNode* imtexnode = dynamic_cast<ccl::ImageTextureNode*>(shnode);
			if (mname == "projection_blend") {
				imtexnode->set_projection_blend(value);
			}
		}
		else if (shn_type == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *texconode =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			if (mname == "decal_height") {
				texconode->set_height(value);
			}
			else if (mname == "decal_radius") {
				texconode->set_radius(value);
			}
			else if (mname == "decal_hor_start") {
				texconode->set_horizontal_sweep_start(value);
			}
			else if (mname == "decal_hor_end") {
				texconode->set_horizontal_sweep_end(value);
			}
			else if (mname == "decal_ver_start") {
				texconode->set_vertical_sweep_start(value);
			}
			else if (mname == "decal_ver_end") {
				texconode->set_vertical_sweep_end(value);
			}
		}
		else if (shn_type == "brick_texture")
		{
			ccl::BrickTextureNode* bricknode = dynamic_cast<ccl::BrickTextureNode*>(shnode);
			if (mname == "offset")
				bricknode->set_offset(value);
			else if (mname == "squash")
				bricknode->set_squash(value);
		}
		else if (shn_type == "sky_texture")
		{
			ccl::SkyTextureNode* skynode = dynamic_cast<ccl::SkyTextureNode*>(shnode);
			if (mname == "turbidity")
				skynode->set_turbidity(value);
			else if (mname == "ground_albedo")
				skynode->set_ground_albedo(value);
		}
		else if (shn_type == "azimuth_altitude_transform")
		{
			ccl::AzimuthAltitudeTransformNode* azimuth_altitude_node = dynamic_cast<ccl::AzimuthAltitudeTransformNode*>(shnode);
			if (mname == "azimuth")
				azimuth_altitude_node->azimuth = value;
			else if (mname == "altitude")
				azimuth_altitude_node->altitude = value;
			else if (mname == "threshold")
				azimuth_altitude_node->threshold = value;
		}
		else if (shn_type == "rhino_noise_texture")
		{
			ccl::RhinoNoiseTextureNode* node = dynamic_cast<ccl::RhinoNoiseTextureNode*>(shnode);
			if (mname == "FrequencyMultiplier")
				node->frequency_multiplier = value;
			else if (mname == "AmplitudeMultiplier")
				node->amplitude_multiplier = value;
			else if (mname == "ClampMin")
				node->clamp_min = value;
			else if (mname == "ClampMax")
				node->clamp_max = value;
			else if (mname == "Gain")
				node->gain = value;
		}
		else if (shn_type == "rhino_waves_texture")
		{
			ccl::RhinoWavesTextureNode* node = dynamic_cast<ccl::RhinoWavesTextureNode*>(shnode);
			if (mname == "WaveWidth")
				node->wave_width = value;
			else if (mname == "Contrast1")
				node->contrast1 = value;
			else if (mname == "Contrast2")
				node->contrast2 = value;
		}
		else if (shn_type == "rhino_perturbing_part2_texture")
		{
			ccl::RhinoPerturbingPart2TextureNode* node = dynamic_cast<ccl::RhinoPerturbingPart2TextureNode*>(shnode);
			if (mname == "Amount")
				node->amount = value;
		}
		else if (shn_type == "rhino_blend_texture")
		{
			ccl::RhinoBlendTextureNode* node = dynamic_cast<ccl::RhinoBlendTextureNode*>(shnode);
			if (mname == "BlendFactor")
				node->blend_factor = value;
		}
		else if (shn_type == "rhino_exposure_texture")
		{
			ccl::RhinoExposureTextureNode* node = dynamic_cast<ccl::RhinoExposureTextureNode*>(shnode);
			if (mname == "Exposure")
				node->exposure = value;
			else if (mname == "Multiplier")
				node->multiplier = value;
			else if (mname == "WorldLuminance")
				node->world_luminance = value;
			else if (mname == "MaxLuminance")
				node->max_luminance = value;
		}
		else if (shn_type == "rhino_fbm_texture")
		{
			ccl::RhinoFbmTextureNode* node = dynamic_cast<ccl::RhinoFbmTextureNode*>(shnode);
			if (mname == "Gain")
				node->gain = value;
			else if (mname == "Roughness")
				node->roughness = value;
		}
		else if (shn_type == "rhino_grid_texture")
		{
			ccl::RhinoGridTextureNode* node = dynamic_cast<ccl::RhinoGridTextureNode*>(shnode);
			if (mname == "FontThickness")
				node->font_thickness = value;
		}
		else if (shn_type == "rhino_projection_changer_texture")
		{
			ccl::RhinoProjectionChangerTextureNode* node = dynamic_cast<ccl::RhinoProjectionChangerTextureNode*>(shnode);
			if (mname == "Azimuth")
				node->azimuth = value;
			else if (mname == "Altitude")
				node->altitude = value;
		}
		else if (shn_type == "rhino_perlin_marble_texture")
		{
			ccl::RhinoPerlinMarbleTextureNode* node = dynamic_cast<ccl::RhinoPerlinMarbleTextureNode*>(shnode);
			if (mname == "Noise")
				node->noise_amount = value;
			else if (mname == "Blur")
				node->blur = value;
			else if (mname == "Size")
				node->size = value;
			else if (mname == "Color1Saturation")
				node->color1_sat = value;
			else if (mname == "Color2Saturation")
				node->color2_sat = value;
		}
		else if (shn_type == "rhino_physical_sky_texture")
		{
			ccl::RhinoPhysicalSkyTextureNode* node = dynamic_cast<ccl::RhinoPhysicalSkyTextureNode*>(shnode);
			if (mname == "AtmosphericDensity")
				node->atmospheric_density = value;
			else if (mname == "RayleighScattering")
				node->rayleigh_scattering = value;
			else if (mname == "MieScattering")
				node->mie_scattering = value;
			else if (mname == "SunBrightness")
				node->sun_brightness = value;
			else if (mname == "SunSize")
				node->sun_size = value;
			else if (mname == "Exposure")
				node->exposure = value;
		}
		else if (shn_type == "rhino_texture_adjustment_texture")
		{
			ccl::RhinoTextureAdjustmentTextureNode* node = dynamic_cast<ccl::RhinoTextureAdjustmentTextureNode*>(shnode);
			if (mname == "Multiplier")
				node->multiplier = value;
			else if (mname == "ClampMin")
				node->clamp_min = value;
			else if (mname == "ClampMax")
				node->clamp_max = value;
			else if (mname == "Gain")
				node->gain = value;
			else if (mname == "Gamma")
				node->gamma = value;
			else if (mname == "Saturation")
				node->saturation = value;
			else if (mname == "HueShift")
				node->hue_shift = value;
		}
	}
}

static void _set_transform(ccl::Transform& tfm, float x, float y, float z, float w, int index)
{
	if (index == 0) {
		tfm.x.x = x;
		tfm.x.y = y;
		tfm.x.z = z;
		tfm.x.w = w;
	}
	else if (index == 1) {
		tfm.y.x = x;
		tfm.y.y = y;
		tfm.y.z = z;
		tfm.y.w = w;
	}
	else if (index == 2) {
		tfm.z.x = x;
		tfm.z.y = y;
		tfm.z.z = z;
		tfm.z.w = w;
	}
}

void cycles_shadernode_set_member_vec4_at_index(ccl::ShaderNode* shnode, const char* member_name, float x, float y, float z, float w, int index)
{
	auto mname = std::string{ member_name };

	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "rbg_ramp")
		{
			ccl::RGBRampNode* colorramp = dynamic_cast<ccl::RGBRampNode*>(shnode);
			auto ramp = colorramp->get_ramp();
			auto ramp_alpha = colorramp->get_ramp_alpha();
			if (ramp.capacity() < index + 1) {
				ramp.resize(index + 1);
				ramp_alpha.resize(index + 1);
			}
			ramp[index] = ccl::make_float3(x, y, z);
			ramp_alpha[index] = w;
		}
		else if (shn_type == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *texco =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			if(mname == "object_transform") {
				ccl::Transform &tf = const_cast<ccl::Transform &>(texco->get_ob_tfm());
				_set_transform(tf, x, y, z, w, index);
			}
			else if(mname == "pxyz") {
				ccl::Transform &tf = const_cast<ccl::Transform &>(texco->get_pxyz());
				_set_transform(tf, x, y, z, w, index);
			}
			else if(mname == "nxyz") {
				ccl::Transform &tf = const_cast<ccl::Transform &>(texco->get_nxyz());
				_set_transform(tf, x, y, z, w, index);
			}
			else if(mname == "uvw") {
				ccl::Transform &tf = const_cast<ccl::Transform &>(texco->get_uvw());
				_set_transform(tf, x, y, z, w, index);
			}
		}
		else if (shn_type == "matrix_math")
		{
			ccl::MatrixMathNode* matmath = dynamic_cast<ccl::MatrixMathNode*>(shnode);
			_set_transform(matmath->tfm, x, y, z, w, index);
		}
	}
}

void cycles_shadernode_set_member_vec(ccl::ShaderNode* shnode, const char* member_name, float x, float y, float z)
{
	auto mname = std::string{ member_name };

	if (shnode) {
		std::string shn_type = shnode->type->name.string();
		if (shn_type == "color")
		{
			ccl::ColorNode* colnode = dynamic_cast<ccl::ColorNode*>(shnode);
			colnode->set_value(ccl::make_float3(x, y, z));
		}
		else if (shn_type == "sky_texture")
		{
			ccl::SkyTextureNode* sunnode = dynamic_cast<ccl::SkyTextureNode*>(shnode);
			sunnode->set_sun_direction(ccl::make_float3(x, y, z));
		}
		else if (shn_type == "mapping")
		{
			/*ccl::MappingNode* mapping = dynamic_cast<ccl::MappingNode*>(shnode);
			if (mname == "min") {
				mapping->tex_mapping.min.x = x;
				mapping->tex_mapping.min.y = y;
				mapping->tex_mapping.min.z = z;
			}
			else if (mname == "max") {
				mapping->tex_mapping.max.x = x;
				mapping->tex_mapping.max.y = y;
				mapping->tex_mapping.max.z = z;
			}*/
		}
		else if (shn_type == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *texco =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			if (mname == "origin") {
				texco->set_decal_origin(ccl::make_float3(x, y, z));
			}
			else if (mname == "across") {
				texco->set_decal_across(ccl::make_float3(x, y, z));
			}
			else if (mname == "up") {
				texco->set_decal_up(ccl::make_float3(x, y, z));
			}
		}
		else if (shn_type == "rhino_physical_sky_texture")
		{
			ccl::RhinoPhysicalSkyTextureNode* node = dynamic_cast<ccl::RhinoPhysicalSkyTextureNode*>(shnode);
			if (mname == "SunDirection")
			{
				node->sun_dir.x = x;
				node->sun_dir.y = y;
				node->sun_dir.z = z;
			}
			else if (mname == "SunColor")
			{
				node->sun_color.x = x;
				node->sun_color.y = y;
				node->sun_color.z = z;
			}
			else if (mname == "InverseWavelengths")
			{
				node->inv_wavelengths.x = x;
				node->inv_wavelengths.y = y;
				node->inv_wavelengths.z = z;
			}
		}
		else if (shn_type == "rhino_tile_texture")
		{
			ccl::RhinoTileTextureNode* node = dynamic_cast<ccl::RhinoTileTextureNode*>(shnode);
			if (mname == "Phase")
			{
				node->phase.x = x;
				node->phase.y = y;
				node->phase.z = z;
			}
			else if (mname == "JoinWidth")
			{
				node->join_width.x = x;
				node->join_width.y = y;
				node->join_width.z = z;
			}
		}
	}
}

void cycles_shadernode_set_member_string(ccl::ShaderNode* shnode, const char* member_name, const char* value)
{
	auto mname = std::string{ member_name };
	auto mval = std::string{ value };
	ustring umval = ustring(mval);

	if (shnode)
	{
		std::string shntype = shnode->type->name.string();
		if (shntype == "attribute")
		{
			ccl::AttributeNode* attrn = dynamic_cast<ccl::AttributeNode*>(shnode);
			attrn->set_attribute(umval);
		}
		else if (shntype == "rhino_texture_coordinate")
		{
			ccl::RhinoTextureCoordinateNode *texco =
				dynamic_cast<ccl::RhinoTextureCoordinateNode *>(shnode);
			texco->set_uvmap(umval);
		}
		else if (shntype == "normal_map")
		{
			ccl::NormalMapNode* normalmap = dynamic_cast<ccl::NormalMapNode*>(shnode);
			normalmap->set_attribute(umval);
		}
	}
}

/*
Set an integer attribute with given name to value. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_int(ccl::ShaderNode* shnode_id, const char* attribute_name, int value)
{
	ustring sockname{attribute_name};
	ccl::ShaderInput* inp = shnode_id->input(attribute_name);
	for (const ccl::SocketType &socket : shnode_id->type->inputs) {
		if (socket.type == ccl::SocketType::CLOSURE || socket.type == ccl::SocketType::UNDEFINED) {
			continue;
		}
		if (socket.flags & ccl::SocketType::INTERNAL) {
			continue;
		}
		if (socket.name == sockname) {
			shnode_id->set(socket, value);
			break;
		}
	}
}

/*
* Set a float attribute with given name to value. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_float(ccl::ShaderNode* shnode_id, const char* attribute_name, float value)
{
	ustring sockname{attribute_name};
	ccl::ShaderInput* inp = shnode_id->input(attribute_name);
	for (const ccl::SocketType &socket : shnode_id->type->inputs) {
		if (socket.type == ccl::SocketType::CLOSURE || socket.type == ccl::SocketType::UNDEFINED) {
			continue;
		}
		if (socket.flags & ccl::SocketType::INTERNAL) {
			continue;
		}
		if (socket.name == sockname) {
			shnode_id->set(socket, value);
			break;
		}
	}
}

/*
Set a vector of floats attribute with given name to x, y and z. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_vec(ccl::ShaderNode* shnode_id, const char* attribute_name, float x, float y, float z)
{
	ccl::float3 f3 = ccl::make_float3(x, y, z);
	ustring sockname{attribute_name};
	ccl::ShaderInput* inp = shnode_id->input(attribute_name);
	for (const ccl::SocketType &socket : shnode_id->type->inputs) {
		if (socket.type == ccl::SocketType::CLOSURE || socket.type == ccl::SocketType::UNDEFINED) {
			continue;
		}
		if (socket.flags & ccl::SocketType::INTERNAL) {
			continue;
		}
		if (socket.name == sockname) {
			shnode_id->set(socket, f3);
			break;
		}
	}
}

void cycles_shader_connect_nodes(ccl::Session* session_id, unsigned int shader_id, unsigned int from_id, const char* from, unsigned int to_id, const char* to)
{
#if LEGACY_SHADERS
	ccl::Scene* sce = nullptr;
	if (scene_find(session_id, &sce)) {
		CCShader* sh = csce->shaders[shader_id];
		auto shfrom = sh->graph->nodes.begin();
		auto shfrom_end = sh->graph->nodes.end();
		auto shto = sh->graph->nodes.begin();
		auto shto_end = sh->graph->nodes.end();

		while (shfrom != shfrom_end) {
			if ((*shfrom)->id == from_id) {
				break;
			}
			++shfrom;
		}

		while (shto != shto_end) {
			if ((*shto)->id == to_id) {
				break;
			}
			++shto;
		}

		if (shfrom == shfrom_end || shto == shto_end) {
			return; // TODO: figure out what to do on errors like this
		}

		sh->graph->connect((*shfrom)->output(from), (*shto)->input(to));
	}
#endif
}



class GammaLUT
{
public:
	GammaLUT(float gamma)
	{
		for (unsigned int i = 0; i <= 255; i++)
		{
			lut[i] = (unsigned char)(255.f * powf(i / 255.f, gamma));
		}
	}

	unsigned char Lookup(unsigned char in)
	{
		ASSERT(in <= 255);
		return lut[in];
	}
private:
	unsigned char lut[256];
};

void cycles_apply_gamma_to_byte_buffer(unsigned char* rgba_buffer, size_t size_in_bytes, float gamma)
{
	if (gamma > 0.999f && gamma < 1.001f)
		return;

	ccl::uchar4* colbuf = (ccl::uchar4*)rgba_buffer;
	if (nullptr == colbuf)
		return;

	const int pixel_count = size_in_bytes / sizeof(ccl::uchar4);

	GammaLUT lut(gamma);

#pragma omp parallel for
	for (int i = 0; i < pixel_count; i++)
	{
		colbuf[i].x = lut.Lookup(colbuf[i].x);
		colbuf[i].y = lut.Lookup(colbuf[i].y);
		colbuf[i].z = lut.Lookup(colbuf[i].z);
	}
}


void cycles_apply_gamma_to_float_buffer(float* rgba_buffer, size_t size_in_bytes, float gamma)
{
	ccl::float4* colbuf = (ccl::float4*)rgba_buffer;

	const int pixel_count = size_in_bytes / sizeof(ccl::float4);

#pragma omp parallel for
	for (int i = 0; i < pixel_count; i++)
	{
		const auto red   = powf(colbuf[i].x, gamma);
		const auto green = powf(colbuf[i].y, gamma);
		const auto blue  = powf(colbuf[i].z, gamma);

		colbuf[i].x = red;
		colbuf[i].y = green;
		colbuf[i].z = blue;
	}
}

