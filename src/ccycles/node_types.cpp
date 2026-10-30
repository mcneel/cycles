/**
Copyright 2026 Robert McNeel and Associates

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

/* Read-only queries on Cycles' shader node registry, so a node UI can list every
 * node type with its sockets without a session. */

#include "internal_types.h"

OIIO_NAMESPACE_USING

/* Shader node types Cycles can create, sorted by name so indices are stable. */
static const std::vector<ustring> &shader_node_type_names()
{
	static const std::vector<ustring> names = [] {
		std::vector<ustring> result;
		for (const ustring &name : ccl::NodeType::type_names()) {
			const ccl::NodeType *type = ccl::NodeType::find(name);
			if (type != nullptr && type->type == ccl::NodeType::SHADER && type->create != nullptr) {
				result.push_back(name);
			}
		}
		std::sort(result.begin(), result.end(), [](const ustring &a, const ustring &b) {
			return strcmp(a.c_str(), b.c_str()) < 0;
		});
		return result;
	}();

	return names;
}

static const ccl::NodeType *shader_node_type_find(const char *node_type)
{
	if (node_type == nullptr) {
		return nullptr;
	}

	const ccl::NodeType *type = ccl::NodeType::find(ustring(node_type));
	if (type == nullptr || type->type != ccl::NodeType::SHADER || type->create == nullptr) {
		return nullptr;
	}

	return type;
}

static const ccl::SocketType *shader_node_type_socket(const char *node_type, bool output, int idx)
{
	const ccl::NodeType *type = shader_node_type_find(node_type);
	if (type == nullptr) {
		return nullptr;
	}

	const auto &sockets = output ? type->outputs : type->inputs;
	if (idx < 0 || (size_t)idx >= sockets.size()) {
		return nullptr;
	}

	return &sockets[idx];
}

/* NodeEnum keeps its values in hash order; sort by value for stable indices. */
static std::vector<std::pair<int, ustring>> sorted_enum_values(const ccl::NodeEnum &enm)
{
	std::vector<std::pair<int, ustring>> values;
	for (const auto &entry : enm) {
		values.emplace_back(entry.second, entry.first);
	}
	std::sort(values.begin(), values.end(), [](const auto &a, const auto &b) {
		return a.first != b.first ? a.first < b.first : strcmp(a.second.c_str(), b.second.c_str()) < 0;
	});

	return values;
}

/* Default value as xml_write_node writes it; "" for closures, arrays and transforms. */
static std::string socket_default_string(const ccl::SocketType &socket)
{
	/* SOCKET_DEFINE stores the default as the socket's own datatype. */
	const void *value = socket.default_value;
	if (value == nullptr) {
		return "";
	}

	switch (socket.type) {
		case ccl::SocketType::BOOLEAN:
			return *(const bool *)value ? "true" : "false";
		case ccl::SocketType::FLOAT:
			return ccl::string_printf("%g", (double)*(const float *)value);
		case ccl::SocketType::INT:
			return ccl::string_printf("%d", *(const int *)value);
		case ccl::SocketType::UINT:
			return ccl::string_printf("%u", *(const unsigned int *)value);
		case ccl::SocketType::UINT64:
			return ccl::string_printf("%llu", (unsigned long long)*(const uint64_t *)value);
		case ccl::SocketType::COLOR:
		case ccl::SocketType::COLOR2:
		case ccl::SocketType::VECTOR:
		case ccl::SocketType::POINT:
		case ccl::SocketType::NORMAL: {
			const ccl::float3 v = *(const ccl::float3 *)value;
			return ccl::string_printf("%g %g %g", (double)v.x, (double)v.y, (double)v.z);
		}
		case ccl::SocketType::POINT2: {
			const ccl::float2 v = *(const ccl::float2 *)value;
			return ccl::string_printf("%g %g", (double)v.x, (double)v.y);
		}
		case ccl::SocketType::STRING:
			return ((const ustring *)value)->string();
		case ccl::SocketType::ENUM: {
			const int v = *(const int *)value;
			if (socket.enum_values != nullptr && socket.enum_values->exists(v)) {
				return (*socket.enum_values)[v].string();
			}
			return "";
		}
		default:
			return "";
	}
}

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI int CDECL cycles_shader_node_type_count()
{
	return (int)shader_node_type_names().size();
}

CCL_CAPI const char *CDECL cycles_shader_node_type_name(int idx)
{
	const auto &names = shader_node_type_names();
	if (idx < 0 || (size_t)idx >= names.size()) {
		return "";
	}

	return names[idx].c_str();
}

CCL_CAPI int CDECL cycles_shader_node_type_socket_count(const char *node_type, bool output)
{
	const ccl::NodeType *type = shader_node_type_find(node_type);
	if (type == nullptr) {
		return -1;
	}

	return (int)(output ? type->outputs.size() : type->inputs.size());
}

/* The strings are ustrings, interned for the life of the process. */
CCL_CAPI bool CDECL cycles_shader_node_type_socket(const char *node_type,
												  bool output,
												  int idx,
												  const char **name,
												  const char **ui_name,
												  const char **socket_type,
												  int *flags)
{
	const ccl::SocketType *socket = shader_node_type_socket(node_type, output, idx);
	if (socket == nullptr) {
		return false;
	}

	if (name != nullptr) {
		*name = socket->name.c_str();
	}
	if (ui_name != nullptr) {
		*ui_name = socket->ui_name.c_str();
	}
	if (socket_type != nullptr) {
		*socket_type = ccl::SocketType::type_name(socket->type).c_str();
	}
	if (flags != nullptr) {
		*flags = socket->flags;
	}

	return true;
}

CCL_CAPI bool CDECL cycles_shader_node_type_socket_default(const char *node_type, int idx, void *strholder)
{
	const ccl::SocketType *socket = shader_node_type_socket(node_type, false, idx);
	if (socket == nullptr || strholder == nullptr) {
		return false;
	}

	((StringHolder *)strholder)->thestring = socket_default_string(*socket);

	return true;
}

CCL_CAPI int CDECL cycles_shader_node_type_socket_enum_count(const char *node_type, int idx)
{
	const ccl::SocketType *socket = shader_node_type_socket(node_type, false, idx);
	if (socket == nullptr || socket->enum_values == nullptr) {
		return 0;
	}

	return (int)sorted_enum_values(*socket->enum_values).size();
}

CCL_CAPI bool CDECL cycles_shader_node_type_socket_enum(const char *node_type,
													   int idx,
													   int enum_idx,
													   const char **name,
													   int *value)
{
	const ccl::SocketType *socket = shader_node_type_socket(node_type, false, idx);
	if (socket == nullptr || socket->enum_values == nullptr) {
		return false;
	}

	const auto values = sorted_enum_values(*socket->enum_values);
	if (enum_idx < 0 || (size_t)enum_idx >= values.size()) {
		return false;
	}

	if (name != nullptr) {
		*name = values[enum_idx].second.c_str();
	}
	if (value != nullptr) {
		*value = values[enum_idx].first;
	}

	return true;
}

#ifdef __cplusplus
}
#endif
