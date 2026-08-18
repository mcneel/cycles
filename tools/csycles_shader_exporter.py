#Copyright 2014 Robert McNeel and Associates
#
#Licensed under the Apache License, Version 2.0 (the "License");
#you may not use this file except in compliance with the License.
#You may obtain a copy of the License at
#
#http://www.apache.org/licenses/LICENSE-2.0
#
#Unless required by applicable law or agreed to in writing, software
#distributed under the License is distributed on an "AS IS" BASIS,
#WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#See the License for the specific language governing permissions and
#limitations under the License.

import bpy
from bpy import data as D
from bpy import context as C

import functools

# contain all nodes for a material
allnodes = set()
# contain all links for a material
alllinks = set()

# Node type to CSycle class name
# TODO: add all nodes
nodemapping = {

# shader nodes
    'ADD_SHADER' : 'AddClosureNode',
    'MIX_SHADER' : 'MixClosureNode',
    
    'EMISSION' : 'EmissionNode',
    
    'HOLDOUT' : 'HoldoutNode',

    'BSDF_DIFFUSE' : 'DiffuseBsdfNode',
    'BSDF_GLOSSY' : 'GlossyBsdfNode',
    'BSDF_REFRACTION' : 'RefractionBsdfNode',
    'BSDF_TRANSPARENT' : 'TransparentBsdfNode',

# texture nodes

    'TEX_IMAGE' : 'ImageTextureNode',
    'TEX_BRICK' : 'BrickTexture',
    'TEX_CHECKER' : 'CheckerTexture',
    'TEX_ENVIRONMENT' : 'EnvironmentTextureNode',
    'TEX_VORONOI' : 'VoronoiTexture',
        
# color nodes
    'MIX_RGB' : 'MixNode',
    'GAMMA' : 'GammaNode',
    'BRIGHTCONTRAST' : 'BrightnessContrastNode',
    'HUE_SAT' : 'HueSaturationNode',
    'LIGHT_FALLOFF' : 'LightFalloffNode',

# conversion nodes
    'MAPPING' : 'MappingNode',
    'MATH' : 'MathNode',
    'RGBTOBW' : 'RgbToBwNode',
    'SEPRGB' : 'SeparateRgbNode',
    'COMBRGB' : 'CombineRgbNode',
    'SEPHSV' : 'SeparateHsvNode',
    'COMBXYZ' : 'CombineXyzNode',
    'SEPXYZ' : 'SeparateXyzNode',

# vector nodes
    'BUMP' : 'BumpNode',

# input nodes
    'VALUE' : 'ValueNode',
    'RGB' : 'ColorNode',
    'FRESNEL' : 'FresnelNode',
    'TEX_COORD' : 'TextureCoordinateNode',
    'LIGHT_PATH' : 'LightPathNode',
    'LAYER_WEIGHT' : 'LayerWeightNode',

# output nodes
    'BACKGROUND' : 'BackgroundNode',
}

# Socket type to CSycles class name mapping
socketmapping = {
    'SHADER' : 'ClosureSocket',
    'VALUE' : 'FloatSocket',
    'RGBA' : 'Float4Socket',
    'VECTOR' : 'Float4Socket',
}

class Link():
    """
    Class to denote a link between two nodes and sockets. We need this
    because we very likely need to simplify the graph by dropping unnecessary
    nodes like REROUTE and relinking nodes and sockets instead.
    """
    def __init__(self, fromnode, fromsock, tonode, tosock):
        self.from_node = fromnode
        self.from_socket = fromsock
        self.to_node = tonode
        self.to_socket = tosock
    
    def __str__(self):
        if self.from_node:
            fromnode = self.from_node.label if self.from_node.label else self.from_node.name
            tonode = self.to_node.label if self.to_node.label else self.to_node.name
            return '['+ fromnode + ']>' + self.from_socket.name + ' -> ' + \
                    self.to_socket.name + '('+self.to_socket.type+')<[' + tonode + ']'
        else:
            tonode = self.to_node.label if self.to_node.label else self.to_node.name
            return 'VALUE [' + str(self.from_socket) + '] -> ' + \
                    self.to_socket.name + '('+self.to_socket.type+')<[' + tonode + ']'
    
    def __repr__(self):
        return self.__str__()
    
    def __lt__(a, b):
        return str(a) < str(b)

def get_node_name(node):
    """
    Return a cleaned up node name, usable as variable
    """
    return cleanup_name(node.label if node.label else node.name)

def get_node_name_from_tuple(ntup):
    return get_node_name(ntup[0])

def cleanup_name(name):
    """
    Adapt name so it can be used as a variable name
    """
    name = name.replace(" ", "_")
    name = name.replace(".", "_")
    name = name.replace(",", "_")
    name = name.replace("+", "_")
    name = name.replace("^", "_")
    name = name.replace(":", "_")
    name = name.replace("-", "_")
    name = name.replace("(", "_")
    name = name.replace(")", "_")
    name = name.replace("[", "_")
    name = name.replace("]", "_")
    name = name.replace("%", "_")
    name = name.replace("=", "_")
    return name.lower()

def get_socket_name(socket, socketlist, is_input, node):
    """
    Determine name for a socket. The names given through BPy differ from
    what is at the lowest level. This level is also what C[CS]?ycles uses,
    so some mapping is necessary.
    """
    sockname = socket.name
    if is_input:
        if node.type == 'MATH':
            i = 0
            for sck in socketlist.items():
                i+=1
                if sck[1]==socket:
                    name = socket.name
                    sockname = name + str(i)
                    break
        if socket.name == "Shader":
            i = 0
            for sck in socketlist.items():
                if sck[1].name == "Shader": i+=1
                if sck[1]==socket:
                    sockname = "Closure" + str(i)
                    break
    else:
        if socket.name == "Shader":
            if 'BSDF' in node.type:
                sockname = "BSDF"
            else:
                sockname = "Closure"
    sockname = sockname.replace(" ", "")
    return sockname

def get_val_string(input):
    """
    String for a value, formatted for CSycles
    """
    val = ""
    if input.type == 'VALUE':
        val = "{0: .3f}f".format(input.default_value)
    elif input.type in ('RGBA', 'VECTOR'):
        val = "new float4({0: .3f}f, {1: .3f}f, {2: .3f}f)".format(
            input.default_value[0],
            input.default_value[1],
            input.default_value[2]
        )
    return val

def find_output_node_of_group(group):
    """
    Get the output node for a group.
    """
    for n in group.nodes:
        if n.type == 'GROUP_OUTPUT':
            return n

def find_actual_from_link(link, nt_and_parent, depth=0):
    """
    Find the node a link is coming from. Simplify by getting rid of
    REROUTE and group boundaries.
    """
    s = depth*"\t"
    
    nt = nt_and_parent[0]
    parentn = nt_and_parent[1]
    ntname = nt_and_parent[2]
    
    #print(s,"find_actual_from_link:", ntname, link.from_socket.name if link and link.from_socket else "no fromsocket")

    l = link
    
    if not link: raise Exception("no link!")
    
    if link.from_node:

        # REROUTE node, just check it's input
        if link.from_node.type =='REROUTE':
            l = find_actual_from_link(link.from_node.inputs[0].links[0], nt_and_parent, depth+1)
        
        # GROUP node, find its GROUP_OUTPUT and find corresponding sockets
        # (names shall be equal)
        elif link.from_node.type == 'GROUP':
            ngroup = link.from_node
            outn = find_output_node_of_group(ngroup.node_tree)

            #now find matching sockets (group output socket vs outpnode input socket
            for inps in outn.inputs:

                # CUSTOM is a point where new sockets can be created in blender interface
                if inps.type=='CUSTOM':
                    continue
                if inps.links[0].to_socket.name==link.from_socket.name:
                    l = find_actual_from_link(inps.links[0], (nt, ngroup.node_tree, ntname), depth+1)
        elif link.from_node.type == 'GROUP_INPUT':
            for inp in parentn.inputs:
                if inp.name == link.from_socket.name:
                    #print(s,"find_actual_from_link: @", parentn, inp.name)
                    #print(s,"find_actual_from_link >")
                    #[print(s,l) for l in inp.links]
                    #print(s,"find_actual_from_link <")
                    
                    if inp.is_linked:
                        l = find_actual_from_link(inp.links[0], nt_and_parent, depth+1) # got a match, lets use that!
                    else:
                        print(s, "find_actual_from_link: no attached node, should use socket value")
                        print(s, "find_actual_from_link: ", inp.type, inp.default_value)
                        l = Link(None, inp.default_value, None, None)

    return l

def find_output_node(nodetree):
    """
    Get first output node that has incoming links.
    """
    for n in nodetree.nodes:
        if n.type.startswith('OUTPUT'):
            for inp in n.inputs:
                if inp.is_linked:
                    return n
    return None

def find_node_tuple(needle):
    global allnodes
    for n in allnodes:
        if n[0] == needle:
            return n
    
    raise LookupError(needle + ": Node not found")

def find_link_to_input_socket_on_group(socket, group, depth=0):
    """
    Find link on group input nodes corresponding with socket that is
    linked.
    
    None otherwise.
    """
    s = depth*"\t"
    for inp in group.inputs:
        #print(s, "find_link_to_input_socket_on_group:", inp, socket, inp.name==socket.name)
        if inp.name == socket.name and inp.is_linked:
            #print(s,"find_link_to_input_socket_on_group: -->", inp.links[0], inp.name)
            return inp.links[0]
    
    return None

def find_value_input_socket_on_group(socket, group, depth=0):
    """
    Find the value for the input socket on the group
    """
    s = depth*"\t"
    for inp in group.inputs:
        if inp.name == socket.name:
            return inp.default_value
    
    return None


def find_connections(node_and_tree, depth = 0):
    """
    Iterate over nodes and find the links needed. This process
    will also minimize the graph by removing redundant nodes like REROUTE
    and simplify through removing of group boundaries.
    """
    node = node_and_tree[0]
    nt = node_and_tree[1]
    parentn = node_and_tree[2]
    parentnname = node_and_tree[3]
    s = depth*"\t"
    for input in node.inputs:
        print(s, "find_connections: checking", input)
        if input.is_linked:
            #shorthand to link
            lnk = input.links[0]
            
            # to and from
            tonode = lnk.to_node
            tosocket = lnk.to_socket
            fromnode = lnk.from_node
            fromsocket = lnk.from_socket
            
            l = None
            # we need to minimize if any of REROUTE, GROUP_OUTPUT, GROUP_INPUT
            # or GROUP. Find the node and socket we actually need to link from
            if fromnode.type in ('GROUP_INPUT', 'GROUP_OUTPUT'):
                ntup = find_node_tuple(node)
                linktogroup = find_link_to_input_socket_on_group(fromsocket, ntup[2], depth)
                if linktogroup:
                    gtup = find_node_tuple(ntup[2])
                    print(s,"find_connections: \"", ntup[2], ntup[2].label, "=", gtup, linktogroup)
                    fromlnk = find_actual_from_link(linktogroup, (gtup[1], gtup[2], gtup[3]), depth+1)
                    if fromlnk:
                        print(s, "find_connections: !! ",fromlnk.from_node, fromlnk.from_socket, tonode, tosocket)
                        l = Link(fromlnk.from_node, fromlnk.from_socket, tonode, tosocket)
                        #print(s,"find_connections: X> ", l)
                else:
                    # we have input group but it's not linked to, so get
                    # value of socket and create special link
                    print(s, "find_connections: => ", input.node)
                    inpval = find_value_input_socket_on_group(fromsocket, ntup[2], depth)
                    l = Link(None, inpval, tonode, tosocket)
            elif fromnode.type in ('REROUTE', 'GROUP'):
                fromlnk = find_actual_from_link(input.links[0], (nt, parentn, parentnname), depth)
                l = Link(fromlnk.from_node, fromlnk.from_socket, tonode, tosocket)
            else: # nothing special, we have the node and socket we need                
                l = Link(fromnode, fromsocket, tonode, tosocket)

            if l:
                print(s, "find_connections: Adding new link", l)
                alllinks.add(l)

def is_connected(node):
    return ([True for inp in node.inputs if inp.is_linked] +
            [True for outp in node.outputs if outp.is_linked]).count(True)>0
            
def add_nodes(nodeset, nt, parentn=None):
    """
    Add all useful nodes to a set.
    """
    for n in nt.nodes:
        #print("---",type(nt), nt.name)
        if not n.type in ('FRAME','REROUTE'): #, 'GROUP_INPUT', 'GROUP_OUTPUT'):
            if n.type == 'GROUP':
                #print("Found a group", nt, parentn, n, get_node_name(n))
                nodeset.add((n, nt, parentn, parentn.name if parentn else "-"))
                add_nodes(nodeset, n.node_tree, n)
            else:
                print("add_nodes: Adding", get_node_name(n), n.type, nt, get_node_name(nt) if type(nt)!=bpy.types.ShaderNodeTree else "")
                if is_connected(n):
                    nodeset.add((n, nt, parentn, parentn.name if parentn else "-"))

def add_inputs(varname, inputs):
    """
    Iterate over given inputs and create input socket value setters
    """
    inputinits = ""
    #valnamecount = 1
    for inp in inputs:
        if inp.type == 'SHADER': continue
        if inp.is_linked: continue
    
        inpname = get_socket_name(inp, inputs, True, inp.node)
        inputinits += "\t{0}.ins.{1}.Value = {2};\n".format(
            varname, inpname, get_val_string(inp)
        )
    
    return inputinits

def add_inputs_special_links(varname, links):
    """
    Iterate over links that have from_node None. from_socket will have
    the value to set to the input socket of to_node
    """
    inputinits = ""
    for link in links:
        valstring = ""
        value = link.from_socket
        socketlist = link.to_node.inputs
        inpname = get_socket_name(link.to_socket, socketlist, True, link.to_node)
        #print("add_inputs_special_links: ", inpname, link)
        #print("add_inputs_special_links: ", type(value), value)
        if 'bpy_prop_array' in str(type(value)):
            valstring = "new float4({0: .3f}f, {1: .3f}f, {2: .3f}f)".format(
                value[0],
                value[1],
                value[2]
            )
        elif type(value) is float:
            valstring = "{0: .3f}f".format(value)
            
        inputinits += "\t{0}.ins.{1}.Value = {2};\n".format(
            varname, inpname, valstring
        )
    return inputinits

def find_value_links_for_to_node(node):
    """
    Find links from alllinks that are Value links. These links
    have from_node None.
    """
    return [l for l in alllinks if l.to_node == node and l.from_node == None]

def code_init_node(node):
    """
    Set node variables and sockets to their proper values
    """
    varname = get_node_name(node)
    initcode = ""
    initcode = add_inputs(varname, node.inputs)
        
    # find link with from_node None and from_socket not None and to_node is node
    # to get values to set directly to inputs
    vals = find_value_links_for_to_node(node)
    if vals:
        initcode += add_inputs_special_links(varname, vals)

    # now that inputs have been set, make sure we handle the
    # extra cases, like 'direct member' values as Color and Distribution, Value
    if node.type in ('BSDF_GLOSSY', 'BSDF_REFRACTION'):
        initcode += "\t{0}.Distribution = \"{1}\";\n".format(
            varname,
            node.distribution.lower().capitalize()
        )
    if node.type == 'RGB':
        initcode += "\t{0}.Value = new float4({1:.3f}f, {2:.3f}f, {3:.3f}f);\n".format(
            varname,
            node.color[0],
            node.color[1],
            node.color[2]
        )
    if node.type == 'VALUE':
        initcode += "\t{0}.Value = {1:.3f}f;\n".format(
            varname,
            node.outputs[0].default_value
        )
    if node.type == 'MATH':
        opval = functools.reduce(
            lambda x, y: x+'_'+y,
            map(str.capitalize, node.operation.lower().split('_'))
        )
        initcode += "\t{0}.Operation = MathNode.Operations.{1};\n".format(
            varname,
            opval
        )
        if node.use_clamp:
            initcode += "\t{0}.UseClamp = true;\n".format(varname)

    return initcode

def skip_node(node):
    if node.type in ('GROUP_INPUT', 'GROUP_OUTPUT', 'GROUP'): return True

    return False
    

def code_instantiate_nodes(nodes):
    """
    Construct CSycles nodes for each Blender cycles node,
    initialise and return as one code string.
    """
    code = ""
    for ntup in nodes:
        n = ntup[0]
        if skip_node(n): continue
        if 'OUTPUT_' in n.type: continue
           
        nodeconstruct = "\tvar {0} = new {1}();\n".format(
            get_node_name(n), nodemapping[n.type])
        nodeinitialise = code_init_node(n)
        
        code = code + nodeconstruct + nodeinitialise + "\n"

    return code

def code_new_shader(shadername, shadertype):
    """Start code for a new shader."""
    return "\tvar {0} = new Shader(Client, Shader.ShaderType.{1});\n\n\t{0}.Name = \"{0}\";\n\t{0}.UseMis = false;\n\t{0}.UseTransparentShadow = true;\n\t{0}.HeterogeneousVolume = false;\n\n".format(shadername, shadertype)

def code_nodes_to_shader(shadername, nodes):
    """Create the code that adds node instances to the shader."""
    addcode = ""
    for ntup in nodes:
        n = ntup[0]
        
        if skip_node(n): continue
        if 'OUTPUT_' in n.type: continue
    
        addcode = addcode + "\t{0}.AddNode({1});\n".format(shadername, get_node_name(n))
    
    return addcode

def code_finalise(shadername, links):
    """Find the links that go into the final output node."""
    finalisecode = ""
    for link in links:
        fromnode = link.from_node
        fromsock = link.from_socket
        tonode = link.to_node
        tosock = link.to_socket
        
        if not 'OUTPUT_' in tonode.type: continue
    
        fromsockname = get_socket_name(fromsock, fromnode.inputs, False, fromnode)
        tosockname = get_socket_name(tosock, tonode.inputs, True, tonode)
        
        finalisecode = finalisecode + "\t{2}.outs.{3}.Connect({0}.Output.ins.{1});\n".format(
            shadername,
            tosockname,
            get_node_name(fromnode),
            fromsockname
        )
    finalisecode = finalisecode + "\n\t{0}.FinalizeGraph();\n".format(shadername)
    finalisecode = finalisecode + "\n\treturn {0};".format(shadername)
    return finalisecode

def code_link_nodes(links):
    """Add code to link nodes to each other."""
    linkcode = ""
    for link in links:
        fromnode = link.from_node
        fromsock = link.from_socket
        tonode = link.to_node
        tosock = link.to_socket
        
        if not fromnode: continue
        if skip_node(tonode): continue
        if skip_node(fromnode): continue
        if 'OUTPUT_' in tonode.type: continue
        
        fromsockname = get_socket_name(fromsock, fromnode.outputs, False, fromnode)
        tosockname = get_socket_name(tosock, tonode.inputs, True, tonode)
        
        linkcode = linkcode + "\t{0}.outs.{1}.Connect({2}.ins.{3});\n".format(
            get_node_name(fromnode),
            fromsockname,
            get_node_name(tonode),
            tosockname
        )
    
    return linkcode

def rooted_node(node):
    """
    Return True if node can be traced to output node.
    """
    global alllinks

    if skip_node(node): return True
    if 'OUTPUT_' in node.type: return True

    for l in alllinks:
        if node == l.from_node:
            return rooted_node(l.to_node)
    return False

def prune_nodes():
    """
    Prune dead branches
    """
    global allnodes
    
    print("prune_nodes: ", len(allnodes))

    deadset = set([n for n in allnodes if not rooted_node(n[0])])
    
    print("prune_nodes: ", len(deadset))
    allnodes -= deadset
    print("prone_nodes: ", len(allnodes))
    
    return deadset

def prune_links(deadnodes):
    """
    Clean up links that point to nodes that have been pruned.
    """
    global allnodes, alllinks
    
    deadset = set()
    
    print("prune_links:", len(alllinks))
    for link in alllinks:
        for dntuple in deadnodes:
            dn = dntuple[0]
            if dn in (link.from_node, link.to_node):
                deadset.add(link)
    
    print("prune_links:", len(deadset))
    alllinks -= deadset

def create_shader(shadername, nt, is_world=False):
    global allnodes, alllinks
    allnodes.clear()
    alllinks.clear()
            
    ## seed our nodes set
    add_nodes(allnodes, nt)
    
    ## seed our links set
    for n in allnodes:
        if skip_node(n[0]): continue
        #if n[0].type not in ('GROUP', 'GROUP_INPUT', 'GROUP_OUTPUT'):
        #print("create_shader: Finding connections for", n[0])
        find_connections(n)

    # clean up our lists. Not all nodes are connected. We don't
    # want to export such nodes.
    deadnodes = prune_nodes()
    prune_links(deadnodes)
    
    # make nice, sorted lists
    nodelist = list(allnodes)
    nodelist.sort(key=get_node_name_from_tuple)
    linklist = list(alllinks)
    linklist.sort()
    
    print(linklist)
    
    # creat new shader
    shadertype = "World" if is_world else "Material"
    shadercode = code_new_shader(shadername, shadertype)
    
    # create node setup code        
    nodesetup = code_instantiate_nodes(nodelist)
    # add our nodes to shader
    nodeadd = code_nodes_to_shader(shadername, nodelist)
    # link our nodes in CSycles
    linksetup = code_link_nodes(linklist)
    # finalise everything
    finalisecode = code_finalise(shadername, linklist)
    
    csycles_shader_creation = "public Shader create_{0}_shader()\n{{\n{1}\n{2}\n{3}\n{4}\n{5}\n}}".format(
        shadername,
        shadercode,
        nodesetup,
        nodeadd,
        linksetup,
        finalisecode
    )
    
    exportfile = None
    exportfilename = "{0}.cs".format(shadername)
    for t in D.texts:
        if t.name == exportfilename:
            exportfile = t
    
    if not exportfile:
        exportfile = D.texts.new(exportfilename)
    
    exportfile.clear()
    exportfile.write(csycles_shader_creation)
    
def main():
    print("\n"*10)
    # do the material shaders
    for ob in C.selected_objects:
        nt = ob.material_slots[0].material.node_tree
        shadername = ob.material_slots[0].material.name
        shadername = cleanup_name(shadername)        
        create_shader(shadername, nt)
    # do the world shaders
    for world in D.worlds:
        nt = world.node_tree
        if nt:
            shadername = world.name
            shadername = cleanup_name(shadername)
            create_shader(shadername, nt, True)
main()