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

/* ShaderGraph::connect turns one type into another with a registered convert node, and
 * anything into a closure with an emission. For a pair with no convert node, such as a value
 * into a boolean, it would build one from a null type and crash. */
static bool can_connect(const ccl::ShaderOutput *output, const ccl::ShaderInput *input)
{
	const ccl::SocketType::Type from = output->type();
	const ccl::SocketType::Type to = input->type();
	if (from == to || to == ccl::SocketType::CLOSURE) {
		return true;
	}

	/* convert_closure_to_* exist, but ShaderGraph::connect leaves such a link out. */
	if (from == ccl::SocketType::CLOSURE) {
		return false;
	}

	const std::string convert = "convert_" + ccl::SocketType::type_name(from).string() + "_to_" +
	                            ccl::SocketType::type_name(to).string();
	return ccl::NodeType::find(ustring(convert)) != nullptr;
}

/* Whether node takes anything from target, directly or through other nodes. A connect from
 * node into target would close a loop, which Cycles' graph code recurses into for ever. */
static bool takes_from(ccl::ShaderNode *node, const ccl::ShaderNode *target)
{
	std::vector<ccl::ShaderNode *> pending = {node};
	std::unordered_set<ccl::ShaderNode *> seen;
	while (!pending.empty()) {
		ccl::ShaderNode *current = pending.back();
		pending.pop_back();
		if (current == target) {
			return true;
		}
		if (!seen.insert(current).second) {
			continue;
		}
		for (ccl::ShaderInput *in : current->inputs) {
			if (in->link != nullptr) {
				pending.push_back(in->link->parent);
			}
		}
	}
	return false;
}

/* Why xml_read_node would leave an attribute's value out, or nothing if it reads it. */
static std::string unread_value(const ccl::SocketType &socket, const char *value)
{
	if (socket.type == ccl::SocketType::CLOSURE || socket.type == ccl::SocketType::UNDEFINED ||
	    (socket.flags & ccl::SocketType::INTERNAL))
	{
		return "is not read from XML";
	}

	size_t count = 0;
	switch (socket.type) {
		case ccl::SocketType::ENUM:
			return socket.enum_values->exists(ustring(value)) ? std::string() : "is not one of its choices";
		case ccl::SocketType::COLOR:
		case ccl::SocketType::VECTOR:
		case ccl::SocketType::POINT:
		case ccl::SocketType::NORMAL:
			count = 3;
			break;
		case ccl::SocketType::POINT2:
			count = 2;
			break;
		case ccl::SocketType::TRANSFORM:
			count = 12;
			break;
		default:
			return std::string();
	}

	ccl::vector<std::string> tokens;
	ccl::string_split(tokens, value);
	return tokens.size() == count ? std::string() : ccl::string_printf("needs %d numbers", (int)count);
}

/* A path that does not start at a drive, a share or the root. */
static bool is_relative_path(const char *path)
{
	if (path == nullptr || path[0] == '\0') {
		return false;
	}
	if (path[0] == '/' || path[0] == '\\') {
		return false;
	}
	const bool drive = ((path[0] >= 'A' && path[0] <= 'Z') || (path[0] >= 'a' && path[0] <= 'z')) && path[1] == ':';
	return !drive;
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

	/* Cycles' example scenes wrap the body in <shader>, sometimes inside <cycles>. */
	ccl::xml_node body = doc;
	ccl::xml_node shaders = doc.child("shader") ? doc : doc.child("cycles");
	if (ccl::xml_node shader = shaders.child("shader")) {
		body = shader;

		int count = 0;
		for (ccl::xml_node s = shader; s; s = s.next_sibling("shader")) {
			count++;
		}
		if (count > 1) {
			problem(ccl::string_printf("the XML has %d shaders, and only the first, '%s', is read", count, shader.attribute("name").value()));
		}
	}

	auto graph = ccl::make_unique<ccl::ShaderGraph>();
	ccl::XMLReader reader;
	reader.node_map[ustring("output")] = graph->output();

	int elements = 0;
	for (ccl::xml_node node = body.first_child(); node; node = node.next_sibling()) {
		if (node.type() != PUGIXML_NAMESPACE::node_element) {
			continue;
		}
		elements++;

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

			if (!can_connect(output, input)) {
				problem(ccl::string_printf("Cycles cannot connect %s %s (%s) to %s %s (%s)",
				                           from_node.c_str(), from_socket.c_str(),
				                           ccl::SocketType::type_name(output->type()).c_str(),
				                           to_node.c_str(), to_socket.c_str(),
				                           ccl::SocketType::type_name(input->type()).c_str()));
				continue;
			}

			if (takes_from(from, to)) {
				problem(ccl::string_printf("connect from %s %s to %s %s would make a loop",
				                           from_node.c_str(), from_socket.c_str(), to_node.c_str(), to_socket.c_str()));
				continue;
			}

			/* ShaderGraph::connect keeps an input's first link. */
			if (input->link != nullptr) {
				problem(ccl::string_printf("%s %s is connected already, so the connect from %s %s is left out",
				                           to_node.c_str(), to_socket.c_str(), from_node.c_str(), from_socket.c_str()));
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

		/* Cycles adds convert nodes itself; one placed by hand can leave a dangling link. */
		const ccl::NodeType *node_type = ccl::NodeType::find(node_name);
		if (node_type == nullptr || node_type->type != ccl::NodeType::SHADER || node_type->create == nullptr ||
		    ccl::string_startswith(node_name.string(), "convert_"))
		{
			problem(ccl::string_printf("'%s' is not a shader node type", node.name()));
			continue;
		}

		/* "output" is the graph's output; a node of that name would take its connects. */
		const ustring name(node.attribute("name").value());
		if (name == "output") {
			problem(ccl::string_printf("the %s named 'output' is left out, as 'output' is the graph's output", node.name()));
			continue;
		}
		if (!name.empty() && reader.node_map.find(name) != reader.node_map.end()) {
			problem(ccl::string_printf("two nodes are named '%s', so connects after the second use it", name.c_str()));
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
			if (attr_name == "name") {
				continue;
			}

			const ccl::SocketType *socket = snode->type->find_input(attr_name);
			if (socket == nullptr) {
				problem(ccl::string_printf("%s has no input '%s'", node.name(), attr.name()));
				continue;
			}

			const std::string why = unread_value(*socket, attr.value());
			if (!why.empty()) {
				problem(ccl::string_printf("%s input '%s': '%s' %s, so it keeps its default",
				                           node.name(), attr.name(), attr.value(), why.c_str()));
			}

			/* Cycles' own scene files look for these next to the XML file, which text has none of. */
			if (attr_name == "filename" && is_relative_path(attr.value())) {
				problem(ccl::string_printf("%s input 'filename': '%s' has no folder, so it is looked for in Rhino's working folder; use a full path",
				                           node.name(), attr.value()));
			}
		}

		ccl::xml_read_node(reader, snode, node);

		/* The sizes of Rhino's Dots texture data, which the kernel indexes by; only Rhino sets them. */
		if (node_name == "rhino_dots_texture") {
			ccl::RhinoDotsTextureNode *dots = static_cast<ccl::RhinoDotsTextureNode *>(snode);
			if (dots->dots_data_count != 0 || dots->dots_tree_node_count != 0) {
				problem(ccl::string_printf("%s: dots_data_count and dots_tree_node_count are Rhino's, so they stay 0", node.name()));
				dots->dots_data_count = 0;
				dots->dots_tree_node_count = 0;
			}
		}
	}

	if (elements == 0) {
		problem("the XML has no elements, so the graph is empty");
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
