import argparse
import json
from pathlib import Path
import re

from codegen import code as gen
from codegen import mappings as mp
from codegen import adjust

parser = argparse.ArgumentParser(description='Generate C# shader nodes and wrapper code.')
parser.add_argument('file', type=Path, help='Path to the JSON file containing the shader nodes')
parser.add_argument('outdir', type=Path, help='Path to the output directory')


def get_socket_type(socketdef, nodedef):
    st = ""
    do = True
    hasdot = '.' in socketdef['member_name']
    if socketdef['datatype'] in ('unknown',):
        st = ""
        do = False
    elif hasdot:
        st = ""
        do = False
        return st, do
    else:
        for [tp, stp] in mp.socketdatatype_mapping:
            if socketdef['datatype'] in tp:
                st = stp['cs_socket']
                if nodedef.get('superclass', None) in ('Node', 'Geometry',):
                    st = st.replace("Socket", "NodeSocket")
                return st, do

    raise ValueError(f"Unhandled datatype {socketdef['datatype']}")


def get_cs_socketname(socketdef):
   return socketdef['ui_name'].replace(" ", "").replace("-", "")


makefloat_re = re.compile(r"\((.*)\)")


def get_cs_socketdefvalue(socketdef, nodedef, everything):
  if socketdef.get('is_input', False) is False:
    return ""

  defval, forenum = _default_value_for_socket(socketdef, nodedef.get('prepared_shaderenums', dict()), everything)
  if not defval.startswith('ccl.') and forenum:
    defval = f"{nodedef['name']}.{defval}"

  if socketdef['datatype'] in ('UINT',):
    try:
      intval = int(defval)
    except ValueError:
      defval = f'(uint){defval}'
  if defval is None:
    defval = "null"
  elif 'u_color' in defval:
    defval = f'"{defval}"'
  elif 'make_float' in defval:
    params = re.search(makefloat_re, defval).groups()[0]
    defval = f"new float3({params})"
  elif 'zero_float2' in defval:
    defval = "new float2(0.0f, 0.0f)"
  elif 'zero_float' in defval:
    defval = "new float3(0.0f, 0.0f, 0.0f)"
  elif 'one_float' in defval:
    defval = "new float3(1.0f, 1.0f, 1.0f)"
  elif defval == 'ustring()':
    defval = '""'
  elif 'transform_identity' in defval:
    defval = "ccl.Transform.Identity()"
  elif 'M_PI_F' in defval:
    defval = defval.replace('M_PI_F', '((float)Math.PI)')

  # adjust for enums
  # split on '::'.
  # if first part is same as or in default_value_type then prepend default_value_type
  defvalpts = defval.split("::")
  if len(defvalpts) > 1:
    defvaltype = socketdef['default_value_type']
    if defvalpts[0] in defvaltype:
      defval = f"ccl.{defvaltype}.{defvalpts[-1]}"
      defval = defval.replace('::', '_')

  if socketdef['datatype'] == 'NODE':
    defval = "null"

  defval = defval.strip()
  if defval == "":
    if socketdef['datatype'] in ('FLOAT',):
      defval = "0.0f"
    elif socketdef['datatype'] in ('UINT', 'UINT64', 'INT64', 'INT',):
      defval = "0"
    elif socketdef['datatype'] in ('BOOLEAN',):
      defval = "false"
    else:
      pass

  return defval


def gen_socket_code(socketdef, nodedef, everything):
  """ Generate code to set up sockets and, in case of input sockets,
      initialize them with default values.

      ````
      public ColorSocket BaseColer { get; set; }

      BaseColor = new ColorSocket(parentNode, "Base Color", "base_color");
      BaseColor.Value = new float4(0.8f, 0.8f, 0.8f, 1.0f);
      ````
  """
  socktype, do = get_socket_type(socketdef, nodedef)
  definition = ""
  initialisation = ""
  set_defvals = ""
  if do:
    is_input = str(socketdef['is_input']).lower()
    cssock_name = get_cs_socketname(socketdef)
    cssock_defval = get_cs_socketdefvalue(socketdef, nodedef, everything)
    definition = f"        public {socktype} {cssock_name} {{ get; private set; }}"
    initialisation = f"""            {cssock_name} = new {socktype}(parentNode, \"{socketdef['ui_name']}\", \"{socketdef['member_name']}\", {is_input});"""
    if socketdef['is_input'] and cssock_defval != "null" and 'Closure' not in socktype:
      set_defvals += f"""            ins.{cssock_name}.Value = {cssock_defval};"""
    initialisation += f"""\n            AddSocket({cssock_name});"""
    socketdef['cs_socketdefinition'] = definition
    socketdef['cs_socketinitialization'] = initialisation
    socketdef['cs_socket_setdefvals'] = set_defvals

  return definition, initialisation, set_defvals


def gen_socket_setter_code(socketdef, nodedef, everything):
    """ Setter code to the shadernode.
    """
    nodename = nodedef['name']
    socktype, do = get_socket_type(socketdef, nodedef)
    setter = f"// {nodename}"
    if do:
        #_ = gen.socket_set_code(socketdef, nodedef)
       gen.socket_set_code(socketdef, nodedef, everything)

    #return setter


def gen_socket_getter_code(socketdef, nodedef, everything):
  """ Getter code to the shadernode.
  """
  nodename = nodedef['name']
  socktype, do = get_socket_type(socketdef, nodedef)
  getter = f"// XXYYZZ {nodename}"
  if do:
    #getter += gen.socket_get_code(socketdef, nodedef)
    gen.socket_get_code(socketdef, nodedef, everything)

  #return getter


def do_sockets_code(nodename, is_inputs, sockdefs, sockinits, inheritance):
  sockdefcode = ""
  socktypename = ""
  if inheritance in (' : Node', ' : Geometry', 'NodeOwner', ):
    parentClass = 'Node'
  else:
    parentClass = 'ShaderNode'
  if is_inputs:
    socketFlavor = 'Inputs'
    if parentClass == 'Node':
      socketFlavor = 'NodeInputs'
  else:
    socketFlavor = 'Outputs'
    if parentClass == 'Node':
      socketFlavor= 'NodeOutputs'
  if len(sockdefs) > 0 and len(sockinits) > 0:
    socktypename = f"{nodename}{socketFlavor}"
    sockdefcode = f"""
    public class {socktypename} : {socketFlavor}
    {{
{sockdefs}
        public {socktypename}({parentClass} parentNode)
        {{
{sockinits.rstrip()}
        }}
    }}
"""
  return sockdefcode, socktypename


def type_for_member(member_name, nodedef, everything):
  member_name = member_name.replace(".", "_")
  member_type = ""
  getter = f"get_{member_name}"
  clean_elements = ("const", "&", "*",)
  for mt in nodedef["members"]:
    if mt["name"] == getter:
      member_type = mt["ret_type"]
      for cleanup in clean_elements:
        member_type = member_type.replace(cleanup, "")
      if len(member_type) > 0:
        break
  if len(member_type) == 0:
    if "superclass" in nodedef:
      nd = nodedef["superclass"]
      if nd in everything:
        ndd = everything[nd]
        member_type = type_for_member(member_name, ndd, everything)

  # if "::" in member_type:
  #   member_type = member_type.split("::")[-1]
  return member_type.strip()


def _prep_enum_name(enumname, prefix=None):
  """
  Prepare the enum name to be a valid C# enum name or enum entry name.

  If prefix is given then we're most likely handing an enum entry name. In that
  case if the enum entry name starts with a digit prepend the prefix.
  """
  enumname = enumname.replace('ccl::', '')
  enumname = enumname.replace('_enum', '')
  enumname = enumname.replace('::', '_')
  enumname = enumname.replace('.', '_')
  enumname = enumname.replace('-', '_')
  enumname = enumname.replace(' ', '_')
  if re.match(r'^[0-9]', enumname) is not None and prefix is not None:
    enumname = f"{prefix}_{enumname}"
    enumname = "".join(enumname.split('_'))
  else:
    enumname = "".join(map(str.capitalize, enumname.split('_')))

  return enumname


def _default_value_for_socket(socketdef, nodeenums, everything):
  original_default_value = socketdef['default_value']
  if original_default_value is None:
    return 'null', False

  # clean up static_cast, at least Pass.mode uses that
  original_default_value = re.sub(r'static_cast.*?\((.*?)\)', r'\1', original_default_value).strip()

  if socketdef['datatype'] not in ('ENUM', 'UINT',):
    return original_default_value, False

  default_value = original_default_value.split('::')[-1]
  # first search in nodeenums
  for enumname, enum in nodeenums.items():
    for membername, _, origval in enum.get('_ordered_members', []):
      if membername == '_name':
        continue
      if origval == default_value or origval.endswith(default_value):
        return f'{enumname}.{membername}', True

  # then try in everything
  allenums = { k:v for k,v in everything.items() if v.get('type', None) == 'enum' }

  for enumname, enum in allenums.items():
    for member in enum.get('members', []):
      membername = member.get('name', '')
      if membername == default_value:
        return f'{enumname.replace('::', '.')}.{membername}', False

  # if that fails then try to much default value type and value together
  default_value_type = socketdef['default_value_type']
  default_value = socketdef['default_value']
  # but don't do smushing for primitive types
  if default_value_type in ('bool', 'float', 'int', 'uint', 'uchar', 'ustring',):
    return default_value, False

  return f'{default_value_type}.{default_value}', False

  # raise Exception("figure out better way to detect def val for enum")


def get_members_ordered(ems, kaikki):
  keys = list(ems.values())
  vals = list()
  for k in keys:
    k = k.replace('ccl::', '')
    origenum = '::'.join(k.split('::')[:-1])
    k = k.split('::')[-1]
    _origenum = kaikki.get(origenum, None)
    if _origenum is None:
      origenum = kaikki.get(f'ccl::{origenum}', None)
    else:
      origenum = _origenum
    if origenum is None:
      return []
    for ov in origenum.get('members', []):
      if ov.get('name', None) == k:
        vals.append(ov.get('value', None))
        break
  return [val for _, val in sorted(zip(vals, ems.items()))]


def prepare_shadernode_enums(nodedef, everything):
  # CSycles.shadernode_set_enum(Id, "distribution", (int)Distribution);
  nodeenums_str = ""
  node_enums = dict()
  nodename = nodedef['name']
  if "shaderenums" in nodedef:
    for origenumname, enummembers in nodedef['shaderenums'].items():
      node_enum = dict()
      enumname = f'{nodename}{_prep_enum_name(origenumname)}'
      node_enum['_name'] = origenumname
      items = get_members_ordered(enummembers, everything)
      node_enum['_ordered_members'] = list()
      for membername, origmemberval in items:
        needs_ccl = 'ccl::' in origmemberval
        memberval = origmemberval.replace('ccl::', '')
        _colons = memberval.count("::")
        if _colons > 1:
          memberval = re.sub('::', '_', memberval, count=_colons-1)
        memberval = memberval.replace("::", ".")
        if needs_ccl:
          memberval = 'ccl.' + memberval
        membername = _prep_enum_name(membername, prefix=enumname[:3])
        node_enum['_ordered_members'].append((membername, memberval, origmemberval,))
      node_enums[enumname] = node_enum

    nodedef['prepared_shaderenums'] = node_enums

    for enumname, enum in node_enums.items():
      nodeenums_str += f"        public enum {enumname} : uint {{\n"
      for membername, memberval, origmemberval in enum['_ordered_members']:
        if membername == "_name":
          continue
        nodeenums_str += f"            {membername} = {memberval},\n"
      nodeenums_str += "        }\n"

  enumprops = ""
  setenums = ""
  # enum_dict = dict()
  # enum sockets exist only as inputs
  socks = nodedef.get("inputs", [])
  for sockdef in socks:
    if sockdef["datatype"] == 'ENUM':
      enumtype = type_for_member(sockdef["member_name"], nodedef, everything)
      if enumtype in everything:
        enumtype = everything[enumtype]["name"].replace("::", ".")
      enumprop_defval, _ = _default_value_for_socket(sockdef, node_enums, everything)
      propertyname = f'{get_cs_socketname(sockdef)}'
      #enumprops += f"        public {nodename}{enumtype} {propertyname} {{ get; set; }} = {enumprop_defval};\n"
      #setenums += f"            CSycles.shadernode_set_enum(Id, \"{sockdef['member_name']}\", (int){propertyname});\n"

  if len(setenums) > 0:
    setenums = f"""        internal override void SetEnums()
        {{
{setenums.rstrip()}
        }}
"""
  return enumprops, setenums, nodeenums_str


def prepare_socketdefs_inits(nodedef, socktype : str, everything):
  """Get the socket definition and the initialization code."""
  sockdefs = ""
  sockinits = ""
  sockdefvals = ""
  socks = nodedef.get(socktype, [])
  for sockdef in socks:
    _member = { 'name': sockdef['member_name'], }
    if check_member_skip(_member, nodedef):
      continue
    define, init, defvalcode = gen_socket_code(sockdef, nodedef, everything)
    gen_socket_setter_code(sockdef, nodedef, everything)
    if socktype == "inputs":
      gen_socket_getter_code(sockdef, nodedef, everything)
    if len(define) > 0:
      sockdefs  += f"{define}\n"
      sockinits += f"{init}\n"
      sockdefvals += f"{defvalcode}\n"

  if socktype == "outputs":
    sockgetter = ""

  return sockdefs, sockinits, sockdefvals


def gen_cs_node_code(outputdir : Path, nodedef, everything):
    """ Generate C# code for a class based on either `Node` or `ShaderNode`.
      Results are written to `outputdir`.
    """
    nodename = nodedef["name"]
    if nodename in ('ShaderNode', 'Node',):
      return

    inheritance = f" : {nodedef['superclass']}" if "superclass" in nodedef else ""

    enumprops, setenums, nodeenums = prepare_shadernode_enums(nodedef, everything)

    inputsdef, inputsinit, inputdefvals = prepare_socketdefs_inits(nodedef, "inputs", everything)
    outputsdef, outputsinit, _ = prepare_socketdefs_inits(nodedef, "outputs", everything)

    inputdefcode, inputdeftype = do_sockets_code(nodename, True, inputsdef, inputsinit, inheritance)
    outputdefcode, outputdeftype = do_sockets_code(nodename, False, outputsdef, outputsinit, inheritance)

    shadernode_name = nodedef.get("shadernode_name", nodedef["name"].lower())
    shadernode_attr = ""
    members = ""
    superclass = nodedef.get('superclass', None)
    if superclass in ('ShaderNode', 'ImageSlotTextureNode', 'VolumeNode', 'TextureNode', 'BsdfNode', 'BsdfBaseNode', 'CurvesNode',):
        shadernode_attr = f"[ShaderNode(name: \"{shadernode_name}\")]" if "shadernode_name" in nodedef else ""
        if nodedef.get('name', '') in ('ImageSlotTextureNode', 'VolumeNode', 'TextureNode', 'BsdfNode', 'BsdfBaseNode', 'CurvesNode', 'ConvertNode', ):
            shadernode_attr = f"[ShaderNode(name: \"{shadernode_name}\", for_public_sdk: false)]" if "name" in nodedef else ""
    elif superclass == "Node":
        shadernode_attr = f"[Node(\"{shadernode_name}\")]" if "shadernode_name" in nodedef else ""
    for member in nodedef.get("members", []):
      if check_member_skip(member, nodedef):
        continue
      if member.get('arguments', None) is not None:
        if member.get('ret_type', None) is not None:
          _, _, _ = gen_api_method(nodename, member, everything)
          cs_wrapper_method = member.get('cs_wrapper_method', '')
          if 'Socket' not in cs_wrapper_method and 'Modified' not in cs_wrapper_method:
            if 'override' in member.get('arguments', ''):
              cs_wrapper_method = cs_wrapper_method.replace('public ', 'public override ')
            if 'virtual' in member.get('ret_type', ''):
              cs_wrapper_method = cs_wrapper_method.replace('public ', 'public virtual ')
            members+= f"{cs_wrapper_method}\n\n"
      else:
        # data member
        gen_api_datamember(nodename, member, everything)
        cs_wrapper_method = member.get('cs_wrapper_method', '')
        if len(cs_wrapper_method)>0:
          members+= f"{cs_wrapper_method}\n\n"
        pass

    members = members.replace('consf & ', '')

    constructors = gen.cs_shader_constructors(nodename, shadernode_name, inputdefcode != '', inputdeftype, inputdefvals, outputdefcode != '', outputdeftype, inheritance, everything)

    node_code = gen.cs_node_code(
        nodename,
        inputdefcode,
        inputdeftype,
        outputdefcode,
        outputdeftype,
        shadernode_attr,
        members,
        inheritance,
        nodeenums,
        enumprops,
        constructors,
        setenums,
        everything
    )

    outputfile = outputdir / f"{nodename.replace('::', '_')}.cs"
    outputfile.write_text(node_code)


def gen_capi_getset(nodename : str, member: dict, nodes : dict):
  ccl_ns = ''  # 'ccl::' if member['datatype'] in nodes else ''
  get_member_type = adjust.type_for_capi(member['datatype'], nodes, (nodename, member,), True)
  set_member_type = get_member_type
  string_handler_type = ''
  is_ustring = 'ustring' in member['datatype']  # get_member_type
  is_string = re.match(r'(?<!\w)string', member['datatype']) is not None
  get_body = f'return ptr->{member['name']};'
  set_body = f'if(ptr!=nullptr) {{ ptr->{member['name']} = value; }}'
  if nodename == 'DeviceInfo' and member['name'] in ('id', 'description',):
    get_member_type = 'void'
    set_member_type = 'const char*'
    string_handler_type = ', void* stringholder'
    set_body = f'ptr->{member['name']} = std::string(value);'
    get_body = f"""if(stringholder!=nullptr) {{
      StringHolder* holder = (StringHolder*)stringholder;
      std::string name{{ptr->{member['name']}.c_str()}};
      holder->thestring = name;
  }}"""
  elif is_ustring:
    get_member_type = 'void'
    set_member_type = 'const char*'
    string_handler_type = ', void* stringholder'
    set_body = f'ptr->{member['name']} = OpenImageIO_v3_0::ustring(value);'
    get_body = f"""if(stringholder!=nullptr) {{
      StringHolder* holder = (StringHolder*)stringholder;
      std::string name{{ptr->{member['name']}.c_str()}};
      holder->thestring = name;
  }}"""
  elif is_string:
    get_member_type = 'void'
    set_member_type = 'const char*'
    string_handler_type = ', void* stringholder'
    set_body = f'ptr->{member['name']} = std::string(value);'
    get_body = f"""if(stringholder!=nullptr) {{
      StringHolder* holder = (StringHolder*)stringholder;
      holder->thestring = ptr->{member['name']};
  }}"""
    # End of if is_ustring, don't get fooled by the indentation structure
    # of prior lines. That is due to the multiline triple-quoted f-string.
  getter = f"""/** {nodename}::{member['name']} -> {member['datatype']} */\n"""
  getter += f"""CCL_CAPI {ccl_ns}{get_member_type} call_{nodename.lower()}_get_{member['name']}(ccl::{nodename}* ptr{string_handler_type})
{{
  {get_body}
}}"""
  setter = f"""/** {nodename}::{member['name']} <- {member['datatype']} */\n"""
  setter += f"""CCL_CAPI void call_{nodename.lower()}_set_{member['name']}(ccl::{nodename}* ptr, {ccl_ns}{set_member_type} value)
{{
  {set_body}
}}"""

  member['capi_getter'] = getter
  member['capi_setter'] = setter

  code = f"{getter}\n{setter}\n"

  return code


c_to_cs_types = {
  'ccl::': '',
  'void': 'void',
  'size_t': 'long',
  'static long': 'long',
  'static bool': 'bool',
  'static void': 'void',
  'uint64_t': 'ulong',
  'uint16_t': 'ushort',
  'Spectrum': 'float3', # in C++ it is: using Spectrum = float3
  'SocketModifiedFlags': 'ulong', # in C++ it is: using SocketModifiedFlags = uint64_t
  'uchar': 'byte',
  'ustring': 'string', # oiio ustring
  'const ': '',
  '&': '',
  'string_view': 'string',
  'virtual': '',
  'Graph * graph': 'IntPtr graph',
  'const string& substatus_': 'string substatus_',
  'const string& status_': 'string status_',
  '::': '_',
}


def map_c_to_cs(inp: str, is_arg : bool = False) -> str:
  inp = ''.join(inp.split('=')[0])
  inp = inp.strip()
  inp = inp.replace("params", "parameters")
  inp_parts = inp.split('*')
  if len(inp_parts) > 1:
    outp = 'IntPtr ' + inp_parts[-1].strip()
    outp = outp.strip()
    if is_arg and ' ' not in outp:
      outp = outp + ' value'
    return outp

  for c, cs in c_to_cs_types.items():
    inp = inp.replace(c, cs)

  if is_arg and ' ' not in inp:
    inp = inp + ' value'

  return inp.strip()


# record generated methods. If found, suffix with number before adding here
generated_methods = list()


def get_wrapper_name(nodename : str, name : str) -> str:
    _wrapper_name = f"call_{nodename.lower()}_{name}"
    wrapper_name = _wrapper_name
    count = 1
    while wrapper_name in generated_methods:
        wrapper_name = f"{_wrapper_name}_{count}"
        count += 1
    generated_methods.append(wrapper_name)
    return wrapper_name


def _get_stride(datatype : str):
    if 'ARRAY' not in datatype:
        raise Exception(f"Not an array datatype {datatype}")
    # remove '_ARRAY' from the string, as we want to compare the datatype from
    # the mapping fully
    datatype = datatype.replace('_ARRAY', '')
    mappings = {
        9: ['TRANSFORM'],
        3: ['POINT', 'VECTOR', 'NORMAL', 'COLOR'],
        2: ['POINT2'],
        1: ['FLOAT', 'INT', 'BOOLEAN'],
    }
    for count, tp in mappings.items():
        for t in tp:
            if t == datatype:
                return count


done_getter_setter = dict()

def _gen_api_method_gettersetter(nodename : str, member : dict, nodes : dict):
    name = member['name'].lower()
    socket_name, input_dict, is_setter = gen._gettersetter(nodename, name, nodes)
    datatype = input_dict['datatype']
    is_array_type = 'ARRAY' in datatype
    stride = _get_stride(datatype) if is_array_type else 1

    key = f"{nodename} {name} {datatype}"
    if key in done_getter_setter:
        return '', '', ''

    getset_code = f"/* NOT YET DONE FOR {nodename} - {name} - {datatype} */\n\n"
    pinvoke_code = f"/* NOT YET DONE FOR {nodename} - {name} - {datatype} */\n\n"
    cs_code = f"/* NOT YET DONE FOR {nodename} - {name} - {datatype} */\n\n"

    if is_array_type:
        if is_setter:
          getset_code, pinvoke_code, cs_code = gen.array_set_code(nodename, socket_name, name, member, input_dict, stride, nodes)
        else:
          getset_code, pinvoke_code, cs_code = gen.array_get_code(nodename, socket_name, name, member, input_dict, nodes)
    else:
        if is_setter:
          getset_code, pinvoke_code, cs_code = gen.set_code(nodename, socket_name, name, member, input_dict, stride, nodes)
        else:
          getset_code, pinvoke_code, cs_code = gen.get_code(nodename, socket_name, name, member, input_dict, stride, nodes)
    done_getter_setter[key] = True

    member['method_code'] = getset_code
    member['pinv_method'] = pinvoke_code
    member['cs_method'] = cs_code

    return getset_code, pinvoke_code, cs_code


def _gen_api_method_normal(nodename : str, member : dict, nodes : dict):
  name = member['name'].lower()
  # determine if we need to add the ccl:: namespace
  _ccl_rettype = adjust.type_for_capi(member.get('ret_type', "couldn'tfinditno"), nodes, (nodename, member,), is_return=True)
  ccl_rettype = 'ccl::' if _ccl_rettype in nodes and 'ccl::' not in _ccl_rettype else ''

  for replacement in mp.method_name_replacements:
    name = name.replace(replacement[0], replacement[1])

  nodename = nodename.replace("::", "_")
  wrapper_name = get_wrapper_name(nodename, name)

  ret_type = member['ret_type']
  is_unique_ptr = 'unique_ptr' in ret_type
  is_const_ret = 'const' in ret_type
  is_ptr_ret = '*' in ret_type
  for cleanup in mp.clean_rettype:
    ret_type = ret_type.replace(cleanup, "")

  ret_keyword = ""
  if 'void' not in ret_type or is_ptr_ret:
    ret_keyword = "return "

  if ret_type == 'Triangle':
    ret_type = 'ccl::Mesh::Triangle'

  is_static = member.get('static', False)

  args = ""
  call_args = ""
  cs_call_args = ""
  cs_args = ""
  if member.get('arguments', None) is not None and member['arguments']!='()':
    _args = ''.join(member['arguments'].split(')')[:-1])
    _args = _args.replace("(", "")
    _args = _args.strip()
    if len(_args) > 0:
      _args = [_arg.strip() for _arg in _args.split(",")]
      c_args = [adjust.type_for_capi(_arg, nodes, (nodename, member,)) for _arg in _args]
      args = ",\n\t\t\t\t\t\t" + ",\n\t\t\t\t\t\t".join(c_args)
      _cs_args = [map_c_to_cs(_arg, is_arg=True) for _arg in _args]
      cs_args = ", " + ", ".join(_cs_args)

    for _arg in _args:
      _arg = _arg.strip()
      _arg = _arg.split('=')[0]  # clean up case 'float mn=0.0f': drop '=0.0f'
      _arg_is_ustring = 'ustring' in _arg and 'array' not in _arg
      _arg_is_string = re.match(r'(?<!\w)string', _arg) is not None and 'array' not in _arg
      _arg_is_string = _arg_is_string or re.match(r'(?<!\w)const string', _arg) is not None
      if _arg.endswith('*') or _arg.endswith('&'):
        _arg += " value"
      _arg = _arg.split(" ")[-1]
      _arg = adjust.type_for_capi(_arg.strip(), nodes, (nodename, member,))
      if _arg_is_ustring:
        _arg = f"OpenImageIO_v3_0::ustring({_arg})"
      elif _arg_is_string:
        _arg = f"std::string({_arg})"
      call_args += f"{_arg}, "

  cs_call_args = ", " + call_args
  cs_call_args = re.sub(r"std::string\((.*?)\)", r"\1", cs_call_args)

  if len(call_args) > 0:
    call_args = call_args[:-2]

  if len(cs_call_args) > 0:
    cs_call_args = cs_call_args[:-2]

  if len(cs_args) < 3:
    cs_args = ""

  # if len(cs_call_args) < 3:
  #   cs_call_args = ""

  cs_call = ""

  pinv_method = ""
  cs_method = ""

  get_ptr = ""
  if ret_type == 'void*' or is_unique_ptr:
    get_ptr = ".get()"

  alternative_ret = gen.get_alternative_ret_type(ret_type, nodes[nodename], member, nodes)

  convert_open = ""
  convert_end = ""
  const_cast_open = ""
  const_cast_end = ""
  if is_const_ret and is_ptr_ret:
    const_cast_open = f" const_cast<ccl::{ret_type.strip()}>("
    const_cast_end = ")"
  if is_static:
    method_body = f"""{ret_keyword}{const_cast_open}{convert_open}ccl::{nodename}::{member['name']}({call_args}){get_ptr}{convert_end}{const_cast_end};"""
  else:
    method_body = f"""{ret_keyword}{const_cast_open}{convert_open}ptr->{member['name']}({call_args}){get_ptr}{convert_end}{const_cast_end};"""
    if call_args == 'std::string(value)' and '&' in member.get('arguments', ''):
      method_body = f"""std::string value_str(value);\n    {ret_keyword}{const_cast_open}ptr->{member['name']}(value_str){get_ptr}{const_cast_end};"""

  final_return_type = f"""{ccl_rettype}{adjust.type_for_capi(ret_type.strip(), nodes, (nodename, member,), is_return=True)}"""
  string_handler_type = ''

  # adjust if string or ustring as return type
  if 'string' in member['ret_type']:
    method_body, final_return_type, string_handler_type, cs_call = gen.methodbody_for_string(nodename, member, wrapper_name, call_args, is_static)
  else:
    cs_call = f"{ret_keyword}{wrapper_name}(ptr{cs_call_args});"

  cs_args = cs_args.replace(">&", "")
  cs_args = cs_args.replace("ustring","string")

  cs_call = re.sub(r"params\b", "parameters", cs_call)
  cs_call = re.sub(r"OpenImageIO_v3_0::ustring\((.*)\)", r"\1", cs_call)

  cs_call_args_full = f'(ptr{cs_call_args})'
  cs_call_args_full = re.sub(r"params\b", "parameters", cs_call_args_full)
  cs_call_args_full = re.sub(r"OpenImageIO_v3_0::ustring\((.*)\)", r"\1", cs_call_args_full)


  if is_static:
    args = args[2:]
    method = f"""/* static {member['ret_type']} {nodename}::{member['name']}({args}) */""".replace("\t", "").replace("\n", "")
    method += f"""\nCCL_CAPI {final_return_type} {wrapper_name}(\n{args}{string_handler_type}\n\t\t\t\t)\n{{
  {method_body};
}}
"""
    cs_call = cs_call.replace('ptr, ', '')
    cs_call = cs_call.replace('ptr', '')
  else:
    method = f"""\n/* {member['ret_type']} {nodename}::{member['name']}{member['arguments']} */""".replace("\t", "").replace("\n", "")
    method += f"""\nCCL_CAPI {final_return_type} {wrapper_name}(\n\t\t\t\t\t\tvoid* _ptr{args}{string_handler_type}\n\t\t\t\t)\n{{
  if(_ptr!=nullptr) {{
    ccl::{nodename}* ptr = (ccl::{nodename}*)_ptr;
    {method_body}
  }}{alternative_ret}
}}
"""
  mapped_c_to_cs = map_c_to_cs(member['ret_type'])
  pinv_ret = mapped_c_to_cs
  instance_ptr = 'IntPtr ptr'
  if len(string_handler_type) > 0:
    cs_args += " , IntPtr stringholder"
    mapped_c_to_cs = 'string'
    pinv_ret = 'void'
  if is_static:
    instance_ptr = ''
    cs_args = cs_args[2:]
  cs_wrapper_name = wrapper_name.replace('call_', '')
  cs_method_name = "".join(map(str.capitalize, cs_wrapper_name.split('_')[1:]))
  cargs = f"""// cargs: {nodename} *, {re.sub(" {2,}", " ", args.replace("\t", "").replace("\n", " "))}""".strip()
  pinv_method = f"""\n    [DllImport(Constants.ccycles, SetLastError = false,
     CharSet=CharSet.Ansi,
     CallingConvention = CallingConvention.Cdecl)
    ]
    // ret: {member['ret_type']}
    {cargs}
    private static extern {pinv_ret} {wrapper_name}({instance_ptr}{cs_args.rstrip()});
    """
  cs_method = f"public static {mapped_c_to_cs} {cs_wrapper_name}({instance_ptr}{cs_args.rstrip()})\n    {{\n        {cs_call.strip()}\n    }}\n"
  # XXX
  cs_member_ret = ''
  if mapped_c_to_cs != 'void':
    cs_member_ret = 'return '
  cs_call_args_full = cs_call_args_full.replace('ptr', 'Ptr')
  cs_args = cs_args.replace(', IntPtr stringholder', '').strip()
  if cs_args.startswith(','):
    cs_args = cs_args[2:]
  if is_static:
    cs_call_args_full = cs_call_args_full.replace('Ptr, ', '')
    cs_call_args_full = cs_call_args_full.replace('Ptr', '')
    mapped_c_to_cs = 'static ' + mapped_c_to_cs
  member['cs_wrapper_method'] = f'        public {mapped_c_to_cs} {cs_method_name}({cs_args.strip()}) {{\n            {cs_member_ret}CSycles.{cs_wrapper_name}{cs_call_args_full};\n        }}'

  pinv_method = pinv_method.replace("::", "_")
  pinv_method = re.sub(r'\s*static\s*(\S*)\s*static\s*', r' static \1 ', pinv_method)
  cs_method= re.sub(r'\s*static\s*(\S*)\s*static\s*', r' static \1 ', cs_method)
  cs_method = cs_method.replace("::", "_")
  cs_method = cs_method.replace(", IntPtr stringholder", "")

  pinv_method = pinv_method.replace("const &", "")
  cs_method = cs_method.replace("const &", "")
  method = re.sub(r"\(\s*\)", "()", method)

  member['method_code'] = method
  member['pinv_method'] = pinv_method
  member['cs_method'] = cs_method

  return method, pinv_method, cs_method


def gen_api_datamember(nodename : str, member : dict, nodes : dict):
  name = member['name'].lower()
  _, input_dict, _ = gen._gettersetter(nodename, name, nodes)

  # only handle if there isn't an input dict, otherwise already handled through
  # gen_api_method
  if input_dict is None:
    datatype = member.get('datatype', None)
    # see if a Type is part of nodename::Type
    if f'{nodename}::{datatype}' in nodes:
      datatype = f'{nodename}::{datatype}'
    wrapper_name = get_wrapper_name(nodename, name)
    wrapper_name = wrapper_name.lower()
    # create the CAPI getter and setter methods
    gen_capi_getset(nodename, member, nodes)

    c_type = adjust.type_for_capi(datatype, nodes, (nodename, member,), is_return=True)
    pinv_type = map_c_to_cs(c_type)
    mapped_c_to_cs = map_c_to_cs(datatype)
    mapped_c_to_cs_arg = map_c_to_cs(c_type, is_arg=True)

    stringholder_type = ''
    if c_type == 'const char*':
      pinv_type = 'void'
      mapped_c_to_cs = 'string'
      mapped_c_to_cs_arg = mapped_c_to_cs_arg.replace('IntPtr', 'string')
    elif '*' in c_type:
      pinv_type = 'IntPtr'

    # do setter
    set_member_name = wrapper_name.replace(f'call_{nodename.lower()}', f'call_{nodename.lower()}_set')
    set_cs_member_name = set_member_name.replace('call_', '')
    set_capi_code = member['capi_setter']
    set_pinv_code = f"""\n    [DllImport(Constants.ccycles, SetLastError = false,
     CharSet=CharSet.Ansi,
      CallingConvention = CallingConvention.Cdecl)
    ]
    private static extern void {set_member_name}(IntPtr ptr, {mapped_c_to_cs} value);
"""
    set_cs_code = f"""    public static void {set_cs_member_name}(IntPtr ptr, {mapped_c_to_cs_arg}) {{
        {set_member_name}(ptr, value);
    }}
"""

    # do getter
    get_member_name = wrapper_name.replace(f'call_{nodename.lower()}', f'call_{nodename.lower()}_get')
    get_cs_member_name = get_member_name.replace('call_', '')
    get_capi_code = member['capi_getter']
    get_pinv_code = f"""\n    [DllImport(Constants.ccycles, SetLastError = false,
     CharSet=CharSet.Ansi,
      CallingConvention = CallingConvention.Cdecl)
    ]
    private static extern {pinv_type} {get_member_name}(IntPtr ptr);
"""
    if mapped_c_to_cs == 'string':
      get_pinv_code = f"""\n    [DllImport(Constants.ccycles, SetLastError = false,
     CharSet=CharSet.Ansi,
      CallingConvention = CallingConvention.Cdecl)
    ]
    private static extern void {get_member_name}(IntPtr ptr, IntPtr stringholder);
"""
      get_cs_code = f"""    public static {mapped_c_to_cs} {get_cs_member_name}(IntPtr ptr) {{
          using(CSStringHolder holder = new ()) {{
              {get_member_name}(ptr, holder.Ptr);
              return holder.Value;
          }}
      }}
"""
    else:
      get_cs_code = f"""    public static {mapped_c_to_cs} {get_cs_member_name}(IntPtr ptr) {{
        return {get_member_name}(ptr);
    }}
"""

  cs_prop_name = "".join(map(str.capitalize, name.split('_')))
  member['cs_wrapper_method'] = f'        public {mapped_c_to_cs} {cs_prop_name} {{\n            get {{ return CSycles.{get_cs_member_name}(Ptr); }}\n            set {{ CSycles.{set_cs_member_name}(Ptr, value); }}\n        }}'
  member['method_code'] = f"{set_capi_code}\n{get_capi_code}"
  member['pinv_method'] = f"{set_pinv_code}\n{get_pinv_code}"
  member['cs_method'] = f"{set_cs_code}\n{get_cs_code}"
  return f"{set_capi_code}\n{get_capi_code}", f"{set_pinv_code}\n{get_pinv_code}", f"{set_cs_code}\n{get_cs_code}"

def gen_api_method(nodename : str, member : dict, nodes : dict):
  name = member['name'].lower()

  # Does the member name start with get_ or set_
  # - check under nodes[nodename]['inputs'] if the stripped name exists
  # - get the type of the input, this will tell what the type for get/set is

  # getter/setter
  _, input_dict, _ = gen._gettersetter(nodename, name, nodes)

  if input_dict is not None:
    getset_code, pinvoke_code, cs_code = _gen_api_method_gettersetter(nodename, member, nodes)
    return getset_code, pinvoke_code, cs_code
  else:
    method_code, pinvoke_code, cs_code = _gen_api_method_normal(nodename, member, nodes)
    return method_code, pinvoke_code, cs_code



def check_member_skip(member, nodedef):
  nodename = nodedef.get('name', 'x_x')
  for rettyp in mp.capi_member_skip_on_rettype:
    if rettyp in member.get('ret_type', '-_-') or rettyp in member.get('ret_type', '-_-'):
      return True
    if rettyp in member.get('datatype', '-_-') or rettyp in member.get('datatype', '-_-'):
      return True
  for name, (exact, datatype, args, exceptin, intype ) in mp.capi_member_to_skip.items():
    if type(args) is str:
      args = [args, ]
    if type(exceptin) is str:
      exceptin = (exceptin, )
    if type(intype) is str:
      intype = (intype, )
    if exceptin is None:
      exceptin = ()
    if intype is None:
      intype = ()
    nodename_in_exceptin = nodename in exceptin
    check_datatype = datatype is not None
    check_args = args is not None
    name_match = False
    nodename_in_type = True
    if len(intype) > 0:
        nodename_in_type = nodename in intype
    if exact:
      if name == member['name']:
        name_match = True and nodename_in_type and not nodename_in_exceptin
    else:
      if name in member['name']:
        name_match = True and nodename_in_type and not nodename_in_exceptin

    if name_match:
      if check_datatype and check_args:
        if member.get('datatype', '-_-') == datatype and member.get('arguments', "-_-") in args:
          return True
        else:
          return False
      elif check_datatype:
        if member.get('datatype', '-_-') == datatype:
          return True
        else:
          return False
      elif check_args:
        if member.get('arguments', '-_-') in args:
          return True
        else:
          return False
      return True

  return False


capi_to_skip = [
  ("::", False, ),
  ("AttributeTableBuilder", True, ),
  ("BicubicPatch", True, ),
  ("BoundBox", True, ),
  ("BoundBox::empty_t", True, ),
  ("BoundBox2D", True, ),
  ("Boundbox", True, ),
  ("BsdfEval", True, ),
  ("BufferPass", True, ),
  ("ColorSpaceManager", True, ),
  ("CPUCapabilities", True, ),
  ("ChannelMapping", True, ),
  ("ConstIterator", True, ),
  ("CyclesDriverCrashException", True, ),
  ("DebugFlags", False, ),  # TODO: see if this needs to be reinstated
  ("DeviceDrawParams", False, ),
  ("DeviceGraphicsInterop", False, ),
  ("DeviceKernelMask", False, ),
  ("DeviceScene", False, ),  # Internal
  ("DisplayDriver", False, ),
  ("DummyDevice", False, ),
  ("EdgeDice", False, ),
  ("GPUDevice", False, ),
  ("GuidingParams", False, ),
  ("Hair", False, ),  # TODO: bring back Hair
  ("ImageMerger", False, ),  # TODO: bring back for OpenER multilayer rendering (merge into final)
  ("ImageMetaData", False, ),  # TODO: check if it is needed
  ("ImageParams", False, ),
  ("ImageStats", False, ),
  ("Iterator", False, ),
  ("KernelBounding", False, ),
  ("KernelShader", False, ),
  ("KernelSpot", False, ),
  ("KernelTables", False, ),
  ("KernelWork", False, ),
  ("LeafNode", False, ),
  ("LightTree", False, ),  # TODO: determine if this is needed
  ("LightTreeBucket", False, ),  # TODO: determine if this is needed
  ("LinearQuadPatch", False, ),
  ("LocalIntersection", False, ),
  ("LogMessageVoidify", False, ),
  ("LookupTables", False, ),
  ("MD5Hash", False, ),
  ("MemoryType", False, ),
  ("MergeImage", False, ),
  ("MergeImageLayer", False, ),
  ("MergeImagePass", False, ),
  ("MeshStats", False, ),
  ("MikkMeshWrapper", False, ),  # internal to mesh.cpp
  ("MultiDevice", False, ),
  ("NamedNestedSampleStats", False, ),
  ("NamedSampleCountStats", False, ),
  ("NamedSizeEntry", False, ),
  ("NamedSizeStats", False, ),
  ("NamedTimeEntry", False, ),
  ("NamedTimeStats", False, ),
  ("NodeEnum", False, ),
  ("NodeOwner", False, ),
  ("OIDNDenoiseContext", False, ),
  ("OIDNDenoiser", False, ),
  ("OIDNDenoiserCPU", False, ),
  ("OIDNDenoiserGPU", False, ),
  ("OIDNPass", False, ),
  ("OIIOImageLoader", False, ),
  ("OSLClosure", False, ),
  ("OSLClosureAdd", False, ),
  ("OSLClosureComponent", False, ),
  ("OSLClosureManager", False, ),
  ("OSLClosureMul", False, ),
  ("OSLCompiler", False, ),
  ("OSLGlobals", False, ),
  ("OSLKernel", False, ),
  ("OSLManager", False, ),
  ("OSLNode", False, ),
  ("OSLNoiseOptions", False, ),
  ("OSLRenderServices", False, ),
  ("OSLShaderInfo", False, ),
  ("OSLShaderManager", False, ),
  ("OSLTextureHandle", False, ),
  ("OSLTextureOptions", False, ),
  ("OSLThreadData", False, ),
  ("OSLTraceData", False, ),
  ("ObjectManager", False, ),  # Internal, shouldn't need access
  ("OneapiDevice", False, ),  # Internal, shouldn't need access
  ("OptiXDenoiser", False, ),  # Internal, shouldn't need access
  ("OptiXDevice", False, ),  # Internal, shouldn't need access
  ("OptiXDeviceQueue", False, ),  # Internal, shouldn't need access
  ("OsdData", False, ),
  ("OsdMesh", False, ),
  ("OsdPatch", False, ),
  ("OutputDriver", False, ),  # Internal, shouldn't need access apart from manual code to
  ("PackedBVH", False, ),
  ("ParticleCurveData", False, ),  # psys_closetip etc, figure out array access
  ("ParticleSystem", False, ),  # psys_closetip etc, figure out array access
  ("ParticleSystemManager", False, ),  # Internal, shouldn't need access
  ("PassAccessor", False, ),
  ("Patch", False, ),
  ("PathTrace", False, ),  # Internal
  ("PathTraceDisplay", False, ),  # Internal
  ("PathTraceTile", False, ),  # Internal
  ("PathTraceWork", False, ),  # Internal
  ("PathTraceWorkCPU", False, ),  # Internal
  ("PathTraceWorkGPU", False, ),  # Internal
  ("Procedural", True, ),
  ("ProceduralManager", False, ),
  ("RNGState", False, ),  # Internal to integrator
  ("RTCGeometryType", False, ), # embree internal
  ("RenderBuffers", False, ),  # Internal
  ("RenderScheduler", False, ),  # Internal
  ("RenderStats", False, ),  # TODO: maybe manual, otherwise just internal
  ("SVMCompiler", False, ),  # Internal
  ("SVMShaderManager", False, ),  # Internal
  ("SampleCount", False, ),  # Internal to session.cpp
  ("SbtRecord", False, ),  # Internal to Optix device implementation
  ("SceneUpdateStats", False, ),  # TODO: manual access
  ("ShaderData", False, ),  # Internal
  ("ShaderEval", False, ),  # Internal
  ("ShaderGlobals", False, ),  # Internal
  ("ShaderInput", False, ),  # Internal. TODO see if it could be useful wrapped
  ("ShaderManager", False, ),  # Internal
  ("ShaderNodeIDComparator", False, ),  # Internal
  ("ShaderOutput", False, ),  # Internal. TODO see if it could be useful wrapped
  ("ShaderVolumeClosure", False, ),  # Internal
  ("ShaderVolumePhases", False, ),  # Internal
  ("SkyLoader", False, ),  # Internal
  ("SourceReplaceState", False, ),
  ("StubStream", False, ),
  ("SubdAttribute", False, ),  # Only when we start supporting SubD in Cycles directly
  ("SubdByte", False, ),
  ("SunSky", False, ),  # Internal
  ("SyclQueue", False, ),  # Internal
  ("TaskPool", False, ),  # Internal
  ("TaskScheduler", False, ),  # Internal
  ("ThreadKernelGlobalsCPU", False, ),  # Internal
  ("ThreadKernelGlobalsGPU", False, ),  # Internal
  ("TileManager", False, ),  # Internal
  ("TileSize", False, ),  # Internal
  ("UpdateObjectTransformState", False, ),  # Internal
  ("UpdateTimeStats", False, ),  # Internal
  ("VDBImageLoader", False, ),  # Internal
  ("VolumeStack", False, ),  # Internal
  ("WorkBalanceInfo", False, ),  # Internal
  ("WorkTileScheduler", False, ),  # Internal
  ("device_memory", False, ),  # Internal
  ("scoped_callback_timer", False, ),
  ("scoped_timer", False, ),
  ("static_init", False, ),
]


def gen_ccsapi_code(outputdir : Path, nodes):
  with open("manual_ccycles_cpp.cpp", "r") as manualcpp_f:
    manualcpp_code = manualcpp_f.read()
  cheader = f"""{gen.header_license}

#include "internal_types.h"

{manualcpp_code}

#ifdef __cplusplus
extern "C" {{
#endif
  """
  code = cheader
  with open('manualcs/CSycles.cs.template', 'r') as cs_template_f:
    cs_template = cs_template_f.read()
  cs_template = cs_template.replace("[HEADER]", gen.header_license)
  cscode = ''

  # add manual C code
  with open("manual_ccycles.cpp", "r") as manual_f:
    manual_code = manual_f.read()
    code += manual_code

  for nodename, nodedef in nodes.items():
    do_skip = False
    for (skip, exactmatch) in capi_to_skip:
      if exactmatch and skip == nodename:
        do_skip = True
        break
      else:
        if skip in nodename:
          do_skip = True
          break
    if do_skip:
      continue
    if nodedef['type'] == "enum":
      continue

    spaced_name = len(nodename) + 2
    starry_line_left = ((78 - spaced_name) // 2) * "*"
    starry_line_right = ((78 - spaced_name) // 2 + spaced_name % 2) * "*"
    divider = f"""
/******************************************************************************/
/{starry_line_left} {nodename} {starry_line_right}/
/******************************************************************************/
"""
    code += divider

    cscode += f"#region {nodename}\n" + divider

    if nodedef.get('members', None) is None:
      continue

    code += "\n"

    for member in nodedef["members"]:
      if check_member_skip(member, nodedef):
        continue
      method_code = member.get('method_code', '')
      pinv_method = member.get('pinv_method', '')
      cs_method = member.get('cs_method', '')
      if len(method_code) > 0 and len(pinv_method) > 0 and len(cs_method) > 0:
        code += f"\n{method_code}"
        cscode += f"""{pinv_method}{cs_method}"""
      else:
        if member.get('datatype', None) is not None:
          code += gen_capi_getset(nodename, member, nodes)

    cscode += f"#endregion // {nodename}\n"

  code += """
#ifdef __cplusplus
}
#endif
  """

  cs_template = cs_template.replace("[CODE]", cscode)

  code = re.sub("\n{3,}", "\n\n", code)
  code = code.replace("\t", "  ")
  code = code.replace("*_view", "*")
  # is_string = re.match(r'(?<!\w)string', member['datatype']) is not None
  # code = re.sub("\barray<", "ccl::array<", code)
  code = re.sub("AttributeStandard", "ccl::AttributeStandard", code)
  code = re.sub("MotionType", "ccl::Scene::MotionType", code)
  #code = re.sub("ccl::NodeWaveBandsDirection\)", "NodeWaveBandsDirection)", code)
  #code = re.sub("ccl::NodeWaveRingsDirection\)", "NodeWaveRingsDirection)", code)
  code = re.sub("::array<float3", "::array<ccl::float3", code)
  code = re.sub("::array<ustring", "::array<OpenImageIO_v3_0::ustring", code)
  code = re.sub("::array<Transform", "::array<ccl::Transform", code)
  code = re.sub("::array<Node", "::array<ccl::Node", code)
  code = re.sub("unique_ptr_vector<float4", "ccl::unique_ptr_vector<ccl::float4", code)
  code = re.sub("unique_ptr_vector<Background", "ccl::unique_ptr_vector<ccl::Background", code)
  code = re.sub("unique_ptr_vector<Camera", "ccl::unique_ptr_vector<ccl::Camera", code)
  code = re.sub("unique_ptr_vector<Film", "ccl::unique_ptr_vector<ccl::Film", code)
  code = re.sub("unique_ptr_vector<Geometry", "ccl::unique_ptr_vector<ccl::Geometry", code)
  code = re.sub("unique_ptr_vector<Integrator", "ccl::unique_ptr_vector<ccl::Integrator", code)
  code = re.sub("unique_ptr_vector<Object", "ccl::unique_ptr_vector<ccl::Object", code)
  code = re.sub("unique_ptr_vector<Pass", "ccl::unique_ptr_vector<ccl::Pass", code)
  code = re.sub("unique_ptr_vector<ParticleSystem", "ccl::unique_ptr_vector<ccl::ParticleSystem", code)
  code = re.sub("unique_ptr_vector<Procedural", "ccl::unique_ptr_vector<ccl::Procedural", code)
  code = re.sub("unique_ptr_vector<Shader", "ccl::unique_ptr_vector<ccl::Shader", code)
  # code = re.sub(" vector<float", " ccl::vector<float", code)

  code = re.sub(" uchar", " ccl::uchar", code)
  code = re.sub(r" (uint)(?!\d{0,2}_t)", r" ccl::\1", code)

  code = re.sub(" Point ", " ccl::PointCloud::Point ", code)
  code = re.sub(" Spectrum ", " ccl::Spectrum ", code)

  code = re.sub("CCL_CAPI void set_shaderinput_socket_type.*?}", "", code, flags=re.MULTILINE | re.DOTALL)
  code = re.sub("CCL_CAPI void set_shaderoutput_socket_type.*?}", "", code, flags=re.MULTILINE | re.DOTALL)
  code = re.sub("(CCL_CAPI\\s+)Type(\\s+.+?sockettype.+?})", "\\1ccl::SocketType::Type\\2", code, flags=re.DOTALL)
  _code = code
  code = re.sub("(CCL_CAPI\\s+.+?sockettype.+?\\s+)Type(\\s+.+?})", "\\1ccl::SocketType::Type\\2", code, flags=re.MULTILINE | re.DOTALL)
  code = code.replace("CCL_CAPI void set_sockettype_type(ccl::SocketType* ptr, Type value)", "CCL_CAPI void set_sockettype_type(ccl::SocketType* ptr, ccl::SocketType::Type value)")
  code = code.replace("SocketModifiedFlags", "ccl::SocketModifiedFlags")
  #code = code.replace("=PassMode_NOISY", "")
  code = re.sub(" Mapping ", " ccl::TextureMapping::Mapping ", code)
  code = re.sub(" Projection ", " ccl::TextureMapping::Projection ", code)
  code = re.sub("(CCL_CAPI\\s+)Type(\\s+.+?texturemapping.+?})", "\\1ccl::TextureMapping::Type\\2", code, flags=re.DOTALL)
  code = re.sub("(CCL_CAPI\\s+)Projection(\\s+.+?texturemapping.+?})", "\\1ccl::TextureMapping::Projection\\2", code, flags=re.DOTALL)
  code = re.sub("(CCL_CAPI\\s+.+?texturemapping.+?\\s+)Type(\\s+.+?})", "\\1ccl::TextureMapping::Type\\2", code, flags=re.MULTILINE | re.DOTALL)
  code = re.sub("(.+?TextureMapping\\*.+?\\s+)Type(\\s+.+?)", "\\1ccl::TextureMapping::Type\\2", code)
  if _code != code:
     print("DING")
  # code = re.sub("CCL_CAPI void set_shaderinput_socket_type\(ccl::ShaderInput\* ptr, ccl::SocketType value\).*?}", "CCL_CAPI void set_shaderinput_socket_type(ccl::ShaderInput* ptr, const ccl::SocketType& value)", code)

  cs_template = cs_template.replace("=PassMode_NOISY", "")

  outputfile = outputdir / "ccycles.cpp"
  outputfile.write_text(code)

  cs_template = re.sub(r"(\s*\n){3,}", "\n\n", cs_template)
  csoutputfile = outputdir / "CSycles.cs"
  csoutputfile.write_text(cs_template)


def iterate_definitions(data, nodes, output_dir):
  for nodename, nodedef in data.items():
    is_enum = nodedef['type'] == "enum"
    do_skip = False
    for (skip, exactmatch) in capi_to_skip:
      if is_enum:
        if exactmatch:
          if skip != '::' and skip == nodename:
            do_skip = True
            print(f"Skipping enum {nodename} due to {skip}")
            break
        else:
          if skip != '::' and skip in nodename:
            do_skip = True
            print(f"Skipping enum {nodename} due to {skip}")
            break
      else:
        if exactmatch:
          if skip == nodename or nodename == 'Node':
            do_skip = True
            print(f"Skipping {nodename} due to {skip}")
            break
        else:
          if skip in nodename or nodename == 'Node':
            do_skip = True
            print(f"Skipping {nodename} due to {skip}")
            break
    if do_skip:
      continue
    if is_enum:
      print(f"generate enumcode for {nodename}")
      gen.enum_code(output_dir, nodedef, nodes)
    else:
      print(f"generate code for {nodename}")
      gen_cs_node_code(output_dir, nodedef, nodes)


def main():
  args = parser.parse_args()

  nodes_in = Path(args.file).resolve()
  nodes_out = nodes_in.with_suffix('.processed.json')
  output_dir = Path(args.outdir).resolve()

  if not nodes_in.exists():
      print(f"File {nodes_in} does not exist")
      exit(1)

  if not output_dir.exists():
      output_dir.mkdir(parents=True)

  for x in output_dir.glob("*.cs"):
    x.unlink()

  nodes = json.loads(nodes_in.read_text())

  all_enums = { nodename: nodedef for nodename, nodedef in nodes.items() if nodedef['type'] == "enum" }
  all_structs = { nodename: nodedef for nodename, nodedef in nodes.items() if nodedef['type'] == "struct" }
  all_classes = { nodename: nodedef for nodename, nodedef in nodes.items() if nodedef['type'] == "class" }

  iterate_definitions(all_enums, nodes, output_dir)
  iterate_definitions(all_structs, nodes, output_dir)
  iterate_definitions(all_classes, nodes, output_dir)

  gen_ccsapi_code(output_dir, nodes)

  nodes_out.write_text(json.dumps(nodes, indent=2))

  print("done")


main()
