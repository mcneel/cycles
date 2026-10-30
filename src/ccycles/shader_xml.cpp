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

/* Shader graphs from Cycles' own XML, the shader body app/cycles_xml.cpp reads: one
 * element per node named by its type, its inputs as attributes, and connect elements
 * naming "node socket" at each end. "output" is the graph's output node. Every node type
 * and socket in the registry works, without a csycles wrapper per node. */

#include "internal_types.h"

#ifdef WITH_PUGIXML
#  include "graph/node_xml.h"
#endif

OIIO_NAMESPACE_USING

#ifdef WITH_PUGIXML

static ccl::ShaderOutput *find_output(ccl::ShaderNode *node, const std::string &name)
{
	for (ccl::ShaderOutput *out : node->outputs) {
		if (ccl::string_iequals(out->socket_type.name.string(), name)) {
			return out;
		}
	}
	return nullptr;
}

static ccl::ShaderInput *find_input(ccl::ShaderNode *node, const std::string &name)
{
	for (ccl::ShaderInput *in : node->inputs) {
		if (ccl::string_iequals(in->socket_type.name.string(), name)) {
			return in;
		}
	}
	return nullptr;
}

/* Split "node socket" into its two parts. */
static bool split_socket_ref(const char *text, std::string &node, std::string &socket)
{
	ccl::vector<std::string> tokens;
	ccl::string_split(tokens, text);
	if (tokens.size() != 2) {
		return false;
	}

	node = tokens[0];
	socket = tokens[1];
	return true;
}

static bool shader_graph_from_xml(ccl::Shader *shader, const char *xml, std::string &problems)
{
	auto problem = [&problems](const std::string &text) {
		problems += text;
		problems += "\n";
	};

	ccl::xml_document doc;
	/* A shader body is a list of elements, not one rooted document. */
	const ccl::xml_parse_result parsed = doc.load_string(
	        xml, PUGIXML_NAMESPACE::parse_default | PUGIXML_NAMESPACE::parse_fragment);
	if (!parsed) {
		problem(ccl::string_printf("XML error at offset %d: %s", (int)parsed.offset, parsed.description()));
		return false;
	}

	auto graph = ccl::make_unique<ccl::ShaderGraph>();
	ccl::XMLReader reader;
	reader.node_map[ustring("output")] = graph->output();

	for (ccl::xml_node node = doc.first_child(); node; node = node.next_sibling()) {
		if (node.type() != PUGIXML_NAMESPACE::node_element) {
			continue;
		}

		ustring node_name(node.name());

		if (node_name == "connect") {
			std::string from_node, from_socket, to_node, to_socket;
			if (!split_socket_ref(node.attribute("from").value(), from_node, from_socket) ||
			    !split_socket_ref(node.attribute("to").value(), to_node, to_socket))
			{
				problem("connect needs from=\"node socket\" and to=\"node socket\"");
				continue;
			}

			const auto from_it = reader.node_map.find(ustring(from_node));
			const auto to_it = reader.node_map.find(ustring(to_node));
			if (from_it == reader.node_map.end() || to_it == reader.node_map.end()) {
				problem(ccl::string_printf("connect names node '%s', which is not defined before it",
				                           from_it == reader.node_map.end() ? from_node.c_str() : to_node.c_str()));
				continue;
			}

			ccl::ShaderNode *from = static_cast<ccl::ShaderNode *>(from_it->second);
			ccl::ShaderNode *to = static_cast<ccl::ShaderNode *>(to_it->second);
			ccl::ShaderOutput *output = find_output(from, from_socket);
			ccl::ShaderInput *input = find_input(to, to_socket);
			if (output == nullptr || input == nullptr) {
				problem(ccl::string_printf("%s has no %s socket '%s'",
				                           output == nullptr ? from->type->name.c_str() : to->type->name.c_str(),
				                           output == nullptr ? "output" : "input",
				                           output == nullptr ? from_socket.c_str() : to_socket.c_str()));
				continue;
			}

			graph->connect(output, input);
			continue;
		}

		/* The graph has its output node already; an element for it only sets its inputs. */
		if (node_name == "output") {
			ccl::xml_read_node(reader, graph->output(), node);
			reader.node_map[ustring("output")] = graph->output();
			continue;
		}

		/* Name collision with the scene's background, as in cycles_xml.cpp. */
		if (node_name == "background") {
			node_name = ustring("background_shader");
		}

		const ccl::NodeType *node_type = ccl::NodeType::find(node_name);
		if (node_type == nullptr || node_type->type != ccl::NodeType::SHADER || node_type->create == nullptr) {
			problem(ccl::string_printf("'%s' is not a shader node type", node.name()));
			continue;
		}

		ccl::ShaderNode *snode = graph->create_node(node_type);

		/* Rhino's UV map name, as cycles_add_shader_node sets it; XML attributes override it. */
		if (node_name == "tangent") {
			ccl::TangentNode *tangent = static_cast<ccl::TangentNode *>(snode);
			tangent->set_direction_type(ccl::NodeTangentDirectionType::NODE_TANGENT_UVMAP);
			tangent->set_attribute(ustring("uvmap1"));
		}

		for (const ccl::xml_attribute attr : node.attributes()) {
			const ustring attr_name(attr.name());
			if (attr_name != "name" && snode->type->find_input(attr_name) == nullptr) {
				problem(ccl::string_printf("%s has no input '%s'", node.name(), attr.name()));
			}
		}

		ccl::xml_read_node(reader, snode, node);
	}

	shader->set_graph(std::move(graph));
	return true;
}

#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Replace the shader's graph with one read from Cycles XML. Problems with single nodes or
 * connections skip just those and are listed in strholder, one per line; false means the
 * XML did not parse and the graph is unchanged. */
CCL_CAPI bool CDECL cycles_shader_graph_from_xml(ccl::Shader *shader, const char *xml, void *strholder)
{
	std::string problems;
	bool ok = false;

#ifdef WITH_PUGIXML
	if (shader != nullptr && xml != nullptr) {
		ok = shader_graph_from_xml(shader, xml, problems);
	}
#else
	problems = "ccycles was built without pugixml\n";
#endif

	if (strholder != nullptr) {
		((StringHolder *)strholder)->thestring = problems;
	}

	return ok;
}

#ifdef __cplusplus
}
#endif
