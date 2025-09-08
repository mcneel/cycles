import copy
import re
import time
from pathlib import Path
import mappings

year = time.strftime("%Y")
gentime = time.strftime("%Y-%m-%d %H:%M:%S")

dllimport = """
    [DllImport(Constants.ccycles, SetLastError = false,
     CharSet=CharSet.Ansi,
     CallingConvention = CallingConvention.Cdecl)
    ]"""

header_license = f"""/**
Copyright 2014-{year} Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

----------------------------------------------------------------------
NOTE: Do NOT modify this file directly, it is automatically generated.

Code generated at: {gentime} UTC
----------------------------------------------------------------------

**/"""

cs_stringholder = """
#region string holder
    internal class CSStringHolder : IDisposable
    {
      IntPtr stringHolderPtr;
      public CSStringHolder()
      {
        stringHolderPtr = cycles_string_holder_new();
      }

      public string Value
      {
        get
        {
          if (stringHolderPtr != IntPtr.Zero)
          {
            IntPtr strPtr = cycles_string_holder_get(stringHolderPtr);
            string s = Marshal.PtrToStringAnsi(strPtr);
            return s;
          }
          return "";
        }
      }

      public IntPtr Ptr { get { return stringHolderPtr; } }

      #region IDisposable Support
      private bool disposedValue = false; // To detect redundant calls

      protected virtual void Dispose(bool disposing)
      {
        if (!disposedValue)
        {
          if (disposing)
          {
          }

          cycles_string_holder_delete(stringHolderPtr);
          stringHolderPtr = IntPtr.Zero;

          disposedValue = true;
        }
      }

      // This code added to correctly implement the disposable pattern.
      public void Dispose()
      {
        Dispose(true);
      }
      #endregion
    }
    [DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr cycles_string_holder_new();
    [DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void cycles_string_holder_delete(IntPtr strHolder);
    [DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr cycles_string_holder_get(IntPtr strHolder);
#endregion
"""


def _collate_setter_code(nodename, nodes):
    nodedef = nodes[nodename]
    inputs = nodedef.get('inputs', None)

    code = ''

    if inputs is not None:
        pass

    return code


no_casting = [
    'float',
    'bool',
    'int',
    'double',
    'const float',
    'const bool',
    'const int',
    'const double',
    'int32_t',
    'int64_t',
    'uint_t',
    'uint32_t',
    'uint64_t',

    # following are actually errors in harvesting, but put here to work around
    # until fixed
    '0.0',
    '1.0',
]


def _gettersetter(nodename : str, name : str, nodes : dict):
    """
    Check if given name starts with `get_` or `set_`.
    If it does find it from the inputs dictionary for the given node name.

    Returns: input name, input dict or None, True if setter or False
    """
    m = re.match('[g|s]et_(.*)', name)
    input_name = None
    input_dict = None
    if m is not None:
        input_name = m.group(1)
        inputs = nodes.get(nodename, {}).get('inputs', {})
        for inp in inputs:
            if inp['member_name'] == input_name:
                input_dict = inp
                break

    is_setter = name.startswith('set_')

    return input_name, input_dict, is_setter


def get_gettersetter_cast_type(socketdef):
    cast_type = socketdef.get('default_value_type', None)
    if cast_type in no_casting:
        cast_type = None
    cast_type = f'(ccl::{cast_type})' if cast_type is not None and len(cast_type) > 0 else ''
    return cast_type


def cs_node_code(
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
    nodes
):
    inheritance = inheritance.rstrip()

    inheritance = inheritance.replace('NodeOwner', 'Node')

    # we don't want to inherit from "Node" on the C# side of this, as that is
    # an internal detail for the C++ version of Cycles that is of no use to us
    # if inheritance.endswith(" Node"):
    #     inheritance = ''

    # input socket setter and getter code has been generated by this point
    # but that is still inside the "inputs" dictionaries of each node that has
    # those. Collate those pieces into the correct get and set methods for use
    # here.
    inputssetter = "#region Setters\n"
    inputsgetter = "#region Getters\n"
    nodedef = nodes[nodename]
    inputs = nodedef.get('inputs', None)
    setter_dict = dict()
    getter_dict = dict()
    if inputs is not None:
        for sockdef in inputs:
            dt = sockdef['datatype']
            _settercodeblock = sockdef.get('setter', '')
            _gettercodeblock = sockdef.get('getter', '')
            _fullsettercodeblock = setter_dict.get(dt, '')
            if _settercodeblock != '':
                if dt == 'ENUM':
                    pass
                setter_dict[dt] = f'{_fullsettercodeblock}\n{_settercodeblock}'
            _fullgettercodeblock = getter_dict.get(dt, '')
            if _gettercodeblock != '':
                getter_dict[dt] = f'{_fullgettercodeblock}\n{_gettercodeblock}'
    for tp, tpinfo in mappings.socketdatatype_mapping:
        if tp[0] == 'CLOSURE':
            continue
        setterblock = setter_dict.get(tp[0], None)
        getterblock = getter_dict.get(tp[0], None)
        if tpinfo['cs_set'] == 'SetClosure' or tpinfo['cs_get'] == 'GetClosure':
            continue
        if setterblock is not None:
            sig = tpinfo['cs_set']
            defret = tpinfo.get('cs_get_return', '')
            methodcode = f"""
        internal override void {sig}
        {{
            switch(name) {{
                {setterblock}
                default: throw new ArgumentException($"Unknown input socket name '{{name}}' for node type {nodename} (setter)");
            }}
        }}
            """
            inputssetter += methodcode
        if getterblock is not None:
            sig = tpinfo['cs_get']
            methodcode = f"""
        internal override {sig}
        {{
            switch(name) {{
                {getterblock}
                default: throw new ArgumentException($"Unknown input socket name '{{name}}' for node type {nodename} (getter)");
            }}
        }}
            """
            inputsgetter += methodcode
    inputssetter += "\n#endregion"
    inputsgetter += "\n#endregion"
    if len(inputssetter) < 50:
        inputssetter = ""
    if len(inputsgetter) < 50:
        inputsgetter = ""

    # if outputdefcode contains a new ClosureSocket take the name and add
    # implementation for GetClosureSocket()
    if len(outputdefcode)>0:
        for codeline in outputdefcode.split('\n'):
            closure_match = re.match(r'\s+(\S+) = new ClosureSocket', codeline)
            if closure_match:
                getclosuresocket = f"""

        public override ClosureSocket GetClosureSocket()
        {{
            return outs.{closure_match.groups(0)[0]};
        }}

"""
                constructors += getclosuresocket
                break

    namespace = "ccl.ShaderNodes"
    if not nodename.endswith("Node"):
        #constructors = ""
        namespace = "ccl"

    extra_code = ""
    extra_path = Path('manualcs/extra') / f"{nodename.replace('::', '_')}.cs.extra"
    if extra_path.exists():
        extra_code = extra_path.read_text()
    postnode_code = ""
    postnode_path = Path('manualcs/extra') / f"{nodename.replace('::', '_')}.cs.post"
    if postnode_path.exists():
        postnode_code = f"    /* code from {postnode_path.name} */\n    {postnode_path.read_text()}\n    /* end manual post class code */"
    usings_code = ""
    usings_path = Path('manualcs/extra') / f"{nodename.replace('::', '_')}.cs.usings"
    if usings_path.exists():
        usings_code = usings_path.read_text()

    node_code = f"""{header_license}

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;
{usings_code}

namespace {namespace}
{{
    using cclext;
{inputdefcode}
{outputdefcode}
    {shadernode_attr}
    public class {nodename}{inheritance}
    {{
{nodeenums.rstrip()}
{enumprops.rstrip()}
{constructors}
{extra_code.rstrip()}
{setenums.rstrip()}
{members.rstrip()}
{inputssetter.rstrip()}
{inputsgetter.rstrip()}
    }}
{postnode_code.rstrip()}
}}"""

    node_code = re.sub(r"^ {1,}$", "", node_code, flags=re.MULTILINE)
    node_code = re.sub(r"\n{3,}", "\n", node_code)
    node_code = re.sub(r"\{\n{2,}", "{\n", node_code)

    node_code = node_code.replace("const & ", "")

    return node_code


def cs_shader_constructors(nodename, shadernodename, has_inputs, inputdeftype, inputdefvals, has_outputs, outputdeftype, inheritance, everything):
    nodedef = everything[nodename]
    baseclass = inheritance.split(" : ")[-1]
    inputs = f"inputs = new {inputdeftype}(this);" if has_inputs else ""
    outputs = f"outputs = new {outputdeftype}(this);" if has_outputs else ""
    superclass = nodedef.get('superclass', None)

    if superclass is None:
        constructors = f"""\n        public IntPtr Ptr {{ get; private set; }} = IntPtr.Zero;\n\n        public {nodename}() {{}}\n\n        public {nodename}(IntPtr intPtr) {{ Ptr = intPtr; }}\n\n
        """
    else:
        if baseclass in ('ShaderNode', 'BsdfNode', 'TextureNode', 'ImageSlotTextureNode', 'BsdfBaseNode', 'VolumeNode', 'CurvesNode' ):
            constructors = ""
            if len(inputdeftype) > 0:
                constructors += f"""\n        public {inputdeftype} ins => ({inputdeftype})inputs;"""
            if len(outputdeftype) > 0:
                constructors += f"""\n        public {outputdeftype} outs => ({outputdeftype})outputs;"""
            constructors += f"""\n        public {nodename}(Shader shader) : this(shader, "a {shadernodename} node") {{ }}

        public {nodename}(Shader shader, string name) :
            base(shader, name)
        {{
            FinalizeConstructor();
        }}

        internal {nodename}(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {{
            FinalizeConstructor();
        }}

        private void FinalizeConstructor()
        {{
            {inputs}
            {outputs}
        }}
"""
        else:
            if len(inputs)>0:
                inputs = f"{inputdeftype} = new {inputdeftype}(this);"
            if len(outputs)>0:
                outputs = f"{outputdeftype} = new {outputdeftype}(this);"
            inputsprop = f"\n        public {inputdeftype} {inputdeftype} {{ get; set; }}\n        public {inputdeftype} ins => {inputdeftype};\n" if has_inputs else ""
            outputsprop = f"\n        public {outputdeftype} {outputdeftype} {{ get; set; }}\n        public {outputdeftype} outs => {outputdeftype};\n" if has_outputs else ""
            constructors = f"""{inputsprop}{outputsprop}
        public {nodename}() : this("a {shadernodename} node") {{ }}

        public {nodename}(string name) :
            base(name)
        {{
            FinalizeConstructor();
        }}

        internal {nodename}(IntPtr intPtr) : base(intPtr)
        {{
            FinalizeConstructor();
        }}

        private void FinalizeConstructor()
        {{
            {inputs}
            {outputs}
        }}
"""


    return constructors


def methodbody_for_string(nodename, member : dict, wrapper_name, call_args, is_static : bool) -> str:
    to_cstr = '.c_str()' if 'ustring' in member['ret_type'] else ''
    final_return_type = 'void'
    cs_call_args = f'{call_args}, stringHolder.Ptr'
    if not cs_call_args.startswith(', '):
        cs_call_args = ', ' + cs_call_args
    string_handler_type = ', void* stringholder'
    ret_keyword = 'return '
    if is_static:
        method_body = f"""if(stringholder!=nullptr) {{
  StringHolder* holder = (StringHolder*)stringholder;
  std::string value{{ccl::{nodename}::{member['name']}({call_args}){to_cstr}}};
  holder->thestring = value;
}}"""
    else:
      method_body = f"""if(stringholder!=nullptr) {{
  StringHolder* holder = (StringHolder*)stringholder;
  std::string value{{ptr->{member['name']}({call_args}){to_cstr}}};
  holder->thestring = value;
}}"""

    final_cs_call_args = f'ptr{cs_call_args}'
    if is_static:
        final_cs_call_args = final_cs_call_args.replace('ptr, ', '')
    cs_body = f"""
        CSStringHolder stringHolder = new();
        {wrapper_name}({final_cs_call_args});
        return stringHolder.Value;"""

    return method_body, final_return_type, string_handler_type, cs_body


def get_alternative_ret_type(ret_type : str, node : dict = {}, member : dict = {}, everything : dict = {}) -> str:
    alternative_ret = ""
    if '' == ret_type:
        alternative_ret = ""
    elif '*' in ret_type:
        alternative_ret = "\n  return nullptr;"
    # elif 'ustring' in ret_type:
    #     alternative_ret = """\n    return OpenImageIO_v3_0::ustring("");"""
    elif 'PrimitiveType' in ret_type:
        alternative_ret = "\n  return ccl::PrimitiveType::PRIMITIVE_NONE;"
    elif 'size_t' in ret_type:
        alternative_ret = "\n  return -1;"
    elif 'Triangle' in ret_type:
        alternative_ret = "\n  ccl::Mesh::Triangle tri = {{-1, -1, -1}};\n  return tri;"
    elif 'Transform' in ret_type:
        alternative_ret = "\n  return ccl::transform_identity();"
    elif 'MotionType' in ret_type:
        alternative_ret = "\n  return MotionType::MOTION_NONE;"

    elif 'double' in ret_type:
        alternative_ret = "\n  return -1.0;"

    elif 'uchar' in ret_type:
        alternative_ret = "\n  return (ccl::uchar)-1;"

    elif 'array<bool' in ret_type:
        alternative_ret = "\n  ccl::array<bool> barr;\n  return barr;"
    elif 'bool' in ret_type:
        alternative_ret = "\n  return false;"

    elif 'array<int' in ret_type:
        alternative_ret = "\n  ccl::array<int> iarr;\n  return iarr;"
    elif 'packed_int3' in ret_type:
        alternative_ret = "\n  return ccl::packed_int3(-INT_MAX, -INT_MAX, -INT_MAX);"
    elif 'packed_uint3' in ret_type:
        alternative_ret = "\n  return ccl::packed_uint3(INT_MAX, INT_MAX, INT_MAX);"
    elif 'int' in ret_type:
        alternative_ret = "\n  return -INT_MAX;"

    elif 'float4' in ret_type:
        alternative_ret = "\n  return ccl::make_float4(-FLT_MAX, -FLT_MAX, -FLT_MAX, -FLT_MAX);"
    elif 'array<float3' in ret_type:
        alternative_ret = "\n  ccl::array<ccl::float3> fl3arr;\n  return fl3arr;"
    elif 'float3' in ret_type:
        alternative_ret = "\n  return ccl::make_float3(-FLT_MAX, -FLT_MAX, -FLT_MAX);"
    elif 'array<float2' in ret_type:
        alternative_ret = "\n  ccl::array<ccl::float2> fl2arr;\n  return fl2arr;"
    elif 'float2' in ret_type:
        alternative_ret = "\n  return ccl::make_float2(-FLT_MAX, -FLT_MAX);"
    elif 'array<float>' in ret_type:
        alternative_ret = "\n  ccl::array<float> flarr;\n  return flarr;"
    elif 'float' in ret_type:
        alternative_ret = "\n  return -FLT_MAX;"

    elif 'BoundBox2D' in ret_type:
        alternative_ret = """\n  ccl::BoundBox2D bb;
   bb.left=bb.right=bb.top=bb.bottom=FLT_MAX;
   return BoundBox2D_TO_C(bb);"""
    elif 'BoundBox' in ret_type:
        alternative_ret = "\n  return BoundBox_to_C(ccl::BoundBox::empty);"

    elif 'ClosureType' in ret_type:
        alternative_ret = "\n  return ccl::CLOSURE_NONE_ID;"
    elif 'MotionPosition' in ret_type:
        alternative_ret = "\n  return ccl::MOTION_POSITION_START;"
    elif 'RollingShutterType' in ret_type:
        alternative_ret = "\n  return ccl::Camera::ROLLING_SHUTTER_NONE;"
    elif 'StereoEye' in ret_type:
        alternative_ret = "\n  return ccl::Camera::STEREO_NONE;"
    elif 'SocketType::Type' == ret_type:
        alternative_ret = "\n  return ccl::SocketType::Type::UNDEFINED;"


    # now check still to see whether this is a method related to an input and
    # whether that input is an enum. Get the default enum value in that case.
    nodename = node.get('name', None)
    if nodename is not None and member.get('name', None) is not None and nodename in everything:
        _, inpdict, _ = _gettersetter(nodename, member['name'], everything)
        if inpdict is not None and inpdict.get('datatype', '')=='ENUM':
            defval = inpdict.get('default_value', None)
            defvaltype = inpdict.get('default_value_type', None)
            alternative_ret = f'\n  return ccl::{defvaltype}::{defval};'


    return alternative_ret


def enum_code(outputdir : Path, nodedef, nodes):
  enumname = nodedef["name"].replace("ccl::", "").replace("::", "_")
  membercode = ""
  for member in nodedef["members"]:
    membercode += f"        {member['name']} = {member['value']},\n"
  enum_code = f"""{header_license}

namespace ccl
{{
    public enum {enumname} : uint
    {{
{membercode.rstrip()}
    }}
}}
"""
  enum_code = re.sub("^ {1,}$", "", enum_code)
  enum_code = re.sub("\n{2,}", "\n", enum_code)
  enum_code = enum_code.replace("DEVICE_MASK_", "")
  outputfile = outputdir / f"{enumname}.cs"
  outputfile.write_text(enum_code)


def _types_from_datatype(method : dict):
    datatype = method['datatype']
    base_type = "ZYX"
    ccl_type = base_type
    cs_type = base_type
    marshal = ''
    marshal_reint = ''
    pinv_type = ''
    for [sdt, tpinfo] in mappings.socketdatatype_mapping:
        if datatype in sdt:
            base_type = tpinfo['base_type']
            ccl_type = tpinfo['ccl_type']
            cs_type = tpinfo['cs_socket_type']
            marshal = tpinfo.get('marshal', '')
            marshal_reint = tpinfo.get('marshal_reinterpret', '')
            pinv_type = tpinfo.get('pinv_type', '')

            if datatype == 'ENUM':
                base_type = method.get('default_value_type', '')
                if base_type not in ('int',):
                    ccl_type = 'ccl::' + base_type
                    cs_type = 'ccl.' + base_type.replace('::', '_')
            break

    return base_type, ccl_type, cs_type, marshal, marshal_reint, pinv_type


def find_name_in_superclass(nodedef, member_name, getset, nodes):
    superclass_name = nodedef.get('superclass', None)
    if superclass_name is not None:
        superclass = nodes.get(superclass_name, None)
        if superclass is not None:
            inputs = superclass.get('inputs', None)
            if inputs is not None:
                for inputdef in inputs:
                    if inputdef['member_name'] == member_name:
                        return True, f'{superclass_name.lower()}_{getset}_{member_name.lower()}'
            members = superclass.get('members', None)
            names = (f'{getset}_{member_name}', )
            if members is not None:
                for member in members:
                    if member['name'] in names:
                        return True, f'{superclass_name.lower()}_{member['name'].lower()}'
            return find_name_in_superclass(superclass, member_name, getset, nodes)
    return False, ''

def socket_setter_name(socketdef, nodedef, everything):
    if nodedef['name'] not in ('ColorNode', 'ValueNode', 'OutputAOVNode', 'ClampNode', 'FloatCurveNode', 'HSVNode', 'MapRangeNode', ):
        insuper, setter = find_name_in_superclass(nodedef, socketdef['member_name'], 'set', everything)
        if insuper:
            return setter

    nodename = nodedef['name']
    setter = f'{nodename.lower()}_set_{socketdef['member_name'].lower()}'

    return setter


def socket_getter_name(socketdef, nodedef, everything):
    if nodedef['name'] not in ('ColorNode', 'ValueNode', 'OutputAOVNode', 'ClampNode', 'FloatCurveNode', 'HSVNode', 'MapRangeNode', ):
        insuper, getter = find_name_in_superclass(nodedef, socketdef['member_name'], 'get', everything)
        if insuper:
            return getter

    nodename = nodedef['name']
    getter = f'{nodename.lower()}_get_{socketdef['member_name'].lower()}'

    return getter


def socket_array_setter(socketdef, nodedef, everything):
    """Generate C# code for setting an array of values to the socket in
    question"""

    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data);"""

    return method


def socket_array_getter(socketdef, nodedef, everything):
    """Generate C# code for getting an array of values from the socket in
    question"""

    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


def socket_vec3array_setter(socketdef, nodedef, everything):
    """Generate C# code for setting an array of vec3 to the socket in
    question"""

    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data);"""

    return method


def socket_vec3array_getter(socketdef, nodedef, everything):
    """Generate C# code for getting an array of vec3 from the socket in
    question"""

    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


def socket_vec2array_setter(socketdef, nodedef, everything):
    """Generate C# code for setting an array of POINT2 values to the socket in
    question"""

    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data);"""

    return method


def socket_vec2array_getter(socketdef, nodedef, everything):
    """Generate C# code for getting an array of POINT2 values from the socket in
    question"""

    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


def socket_xfrmarray_setter(socketdef, nodedef, everything):
    """Generate C# code for setting an array of transforms to the socket in
    question"""
    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data);"""

    return method


def socket_xfrmarray_getter(socketdef, nodedef, everything):
    """Generate C# code for getting an array of transforms from the socket in
    question"""
    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


def socket_datum_setter(socketdef, nodedef, everything):
    setter = socket_setter_name(socketdef, nodedef, everything)
    cast = ''
    if socketdef['datatype'] == 'ENUM':
        if socketdef.get('default_value_type', '') not in ('int',):
            cast = f'(ccl.{socketdef["default_value_type"]})'.replace('::', '_')
    method = f"""
                    CSycles.{setter}(this.Ptr, {cast}data);"""

    return method


def socket_datum_getter(socketdef, nodedef, everything):
    getter = socket_getter_name(socketdef, nodedef, everything)
    cast = ''
    if socketdef['datatype'] == 'ENUM':
        cast = '(uint)'
    method = f"""
                    return {cast}CSycles.{getter}(this.Ptr);"""

    return method


def socket_vec3_setter(socketdef, nodedef, everything):
    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data.x, data.y, data.z);"""

    return method


def socket_vec3_getter(socketdef, nodedef, everything):
    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


def socket_vec2_setter(socketdef, nodedef, everything):
    setter = socket_setter_name(socketdef, nodedef, everything)
    method = f"""
                    CSycles.{setter}(this.Ptr, data.x, data.y);"""

    return method


def socket_vec2_getter(socketdef, nodedef, everything):
    getter = socket_getter_name(socketdef, nodedef, everything)
    method = f"""
                    return CSycles.{getter}(this.Ptr);"""

    return method


socket_setter_map = {
    'BOOLEAN'           : socket_datum_setter,
    'FLOAT'             : socket_datum_setter,
    'INT'               : socket_datum_setter,
    'UINT'              : socket_datum_setter,
    'UINT64'            : socket_datum_setter,
    'STRING'            : socket_datum_setter,
    'ENUM'              : socket_datum_setter,
    'NODE'              : socket_datum_setter,
    'COLOR'             : socket_datum_setter,
    'VECTOR'            : socket_datum_setter,
    'NORMAL'            : socket_datum_setter,
    'POINT'             : socket_datum_setter,
    'POINT2'            : socket_datum_setter,
    'TRANSFORM'         : socket_datum_setter,
    'INT_ARRAY'         : socket_array_setter,
    'FLOAT_ARRAY'       : socket_array_setter,
    'BOOLEAN_ARRAY'     : socket_array_setter,
    'COLOR_ARRAY'       : socket_vec3array_setter,
    'POINT_ARRAY'       : socket_vec3array_setter,
    'POINT2_ARRAY'      : socket_vec2array_setter,
    'NORMAL_ARRAY'      : socket_vec3array_setter,
    'TRANSFORM_ARRAY'   : socket_xfrmarray_setter,
}

socket_getter_map = {
    'BOOLEAN'           : socket_datum_getter,
    'FLOAT'             : socket_datum_getter,
    'INT'               : socket_datum_getter,
    'UINT'              : socket_datum_getter,
    'UINT64'            : socket_datum_getter,
    'STRING'            : socket_datum_getter,
    'ENUM'              : socket_datum_getter,
    'NODE'              : socket_datum_getter,
    'COLOR'             : socket_datum_getter,
    'VECTOR'            : socket_datum_getter,
    'NORMAL'            : socket_datum_getter,
    'POINT'             : socket_datum_getter,
    'POINT2'            : socket_datum_getter,
    'TRANSFORM'         : socket_datum_getter,
    'INT_ARRAY'         : socket_array_getter,
    'FLOAT_ARRAY'       : socket_array_getter,
    'BOOLEAN_ARRAY'     : socket_array_getter,
    'COLOR_ARRAY'       : socket_vec3array_getter,
    'POINT_ARRAY'       : socket_vec3array_getter,
    'POINT2_ARRAY'      : socket_vec2array_getter,
    'NORMAL_ARRAY'      : socket_vec3array_getter,
    'TRANSFORM_ARRAY'   : socket_xfrmarray_getter,
}


def _no_socket_code(socketdef, nodedef, everything):
    return ''


def clean_dict(d):
    cd = copy.deepcopy(d)
    if 'getter' in cd:
        del cd['getter']
    if 'setter' in cd:
        del cd['setter']
    if 'cs_socketdefinition' in cd:
        del cd['cs_socketdefinition']
    if 'cs_socketinitialization' in cd:
        del cd['cs_socketinitialization']
    if 'cs_socket_setdefvals' in cd:
        del cd['cs_socket_setdefvals']
    if 'cs_socket_defvals' in cd:
        del cd['cs_socket_defvals']
    return cd


def socket_set_code(socketdef, nodedef, everything):
    bodygenerator = socket_setter_map.get(socketdef['datatype'], _no_socket_code)
    body = bodygenerator(socketdef, nodedef, everything)

    sockdef = clean_dict(socketdef)

    setcode = f'''
            case "{socketdef['member_name']}":
                    /* {nodedef['name'].lower()} . {sockdef} */
                    {{
                        {body}
                    }}
                    break;
'''
    socketdef['setter'] = setcode

    #return setcode


def socket_get_code(socketdef, nodedef, everything):
    bodygenerator = socket_getter_map.get(socketdef['datatype'], _no_socket_code)
    body = bodygenerator(socketdef, nodedef, everything)

    sockdef = clean_dict(socketdef)

    getcode = f'''
            case "{socketdef['member_name']}":
                /* {nodedef['name'].lower()} . {sockdef} */
                {{
                    {body}
                }}
'''
    socketdef['getter'] = getcode

    #return getcode


def array_set_code(nodename : str, member : str, setter : str, memberdict : dict, setter_dict : dict, stride : int, everything : dict):
    """
    For arrays generate C-API setter code, P/Invoke code and C# method calling the P/Invoke

    Returns: CAPI, PInvoke, C#
    """

    orig_method = f"void {nodename}::{setter}(int & value)"
    method = f'{nodename.lower()}_{setter}'
    base_type, ccl_type, cs_type, marshal, marshal_reint, pinv_type = _types_from_datatype(setter_dict)
    make_ccl_type = ccl_type.replace('ccl::', '')
    ptr_type = f"ccl::{nodename}*"
    base_type = base_type.replace('*', '')
    item = "data[i]"
    # if stride == 2:
    #     item = f"ccl::make_{make_ccl_type}(data[i*stride], data[i*stride+1])"
    # elif stride == 3:
    #     item = f"ccl::make_{make_ccl_type}(data[i*stride], data[i*stride+1], data[i*stride+2])"

    capi_code = f"""
/* Array setter:
   {orig_method}
 */
CCL_CAPI void call_{method}(
            void* _ptr,
            {base_type}* data,
            size_t count
        )
{{
  if(_ptr!=nullptr && data!=nullptr && count>0) {{
    ccl::array<{ccl_type.replace('*', '')}> data_array;
    data_array.resize(count);
    for (size_t i = 0; i < count; i++)  {{
      data_array[i] = {item};
    }}
    {ptr_type} ptr = ({ptr_type})_ptr;
    ptr->{setter}(data_array);
  }}
}}"""

    pinv_code = f"""{dllimport}
    /* Array set C# P/Invoke */
    private unsafe static extern void call_{method}(
        IntPtr ptr,
        [In] {pinv_type}[] data,
        int count);
"""

    cs_code = f"""
    /* Array set C# wrapper */
    public static void {method}(
        IntPtr ptr,
        {cs_type} data
    )
    {{
        call_{method}(ptr, data.ToArray(), data.Count);
    }}

"""

    return capi_code, pinv_code, cs_code


def array_get_code(nodename : str, member : str, getter : str, memberdict : dict, getter_dict : dict, everything : dict):
    """
    For arrays generate C-API getter code, P/Invoke code and C# method calling the P/Invoke

    Returns: CAPI, PInvoke, C#
    """

    method = f'{nodename.lower()}_{getter}'
    base_type, ccl_type, cs_type, marshal, marshal_reint, pinv_type = _types_from_datatype(getter_dict)
    ptr_type = f"ccl::{nodename}*"
    #base_type = base_type.replace('*', '')
    orig_method = f"{base_type} {nodename}::{getter}()"

    blittable = base_type in ('float*', 'int*', 'bool*',)

    array_handle_code_cs = f"""
        IntPtr data = call_{method}(ptr, out int count);
        {marshal}
        {marshal_reint}
        """


    capi_code = f"""
/* {orig_method} */
CCL_CAPI {base_type} call_{method}(
            void* _ptr,
            size_t* count
        )
{{
  if(_ptr!=nullptr) {{
    {ptr_type} ptr = ({ptr_type})_ptr;
    *count = ptr->{getter}().size();
    return ptr->{getter}().data();
  }}
  *count = 0;
  return nullptr;
}}"""

    pinv_code = f"""{dllimport}
    /* Array get C# P/Invoke: {method} */
    private static extern /*{cs_type}*/ IntPtr call_{method}(
        IntPtr ptr,
        out int count);
"""

    cs_code = f"""
    /* Array get C# wrapper: {method} */
    public static {cs_type} {method}(
        IntPtr ptr
    )
    {{
        {array_handle_code_cs}
    }}

"""

    return capi_code, pinv_code, cs_code


seen_setter = dict()


def set_code(nodename : str, member : str, setter : str, memberdict : dict, setter_dict : dict, stride : int, everything : dict):
    """
    Generate C-API setter code, P/Invoke code and C# method calling the P/Invoke

    Returns: CAPI, PInvoke, C#
    """

    setter_key = f"{nodename}::{setter}"

    if setter_key in seen_setter:
        return '', '', ''

    _setter_dict = clean_dict(setter_dict)

    orig_method = f"void {nodename}::{setter} .. {_setter_dict} "
    method = f'{nodename.lower()}_{setter}'
    base_type, ccl_type, cs_type, marshal, marshal_reint, pinv_type = _types_from_datatype(setter_dict)
    ptr_type = f"ccl::{nodename}*"
    base_type = base_type.replace('*', '')
    cast_type = get_gettersetter_cast_type(setter_dict)

    # special casing three methods of Object to handle types better:
    if nodename == 'Object':
        if setter == 'set_geometry':
            ccl_type = 'ccl::Geometry*'
            cs_type = 'IntPtr'
            cast_type = ''
        elif setter == 'set_shader':
            ccl_type = 'ccl::Shader*'
            cs_type = 'IntPtr'
            cast_type = ''
        elif setter == 'set_particle_system':
            ccl_type = 'ccl::ParticleSystem*'
            cs_type = 'IntPtr'
            cast_type = ''

    capi_code = f"""
/* {orig_method} */
CCL_CAPI void call_{method}(
            void* _ptr,
            {ccl_type} data
        )
{{
  if(_ptr!=nullptr) {{
    {ptr_type} ptr = ({ptr_type})_ptr;
    ptr->{setter}({cast_type}data);
  }}
}}

"""

    pinv_code = f"""{dllimport}
    private static extern void call_{method}(
        IntPtr ptr,
        {cs_type} data);
"""

    cs_code = f"""
    public static void {method}(
        IntPtr cclptr,
        {cs_type} data
    )
    {{
        call_{method}(cclptr, data);
    }}

"""
    seen_setter[setter_key] = (capi_code, pinv_code, cs_code,)

    return capi_code, pinv_code, cs_code


def get_code(nodename : str, member : str, getter : str, memberdict : dict, getter_dict : dict, stride : int, everything: dict):
    """
    Generate C-API getter code, P/Invoke code and C# method calling the P/Invoke

    Returns: CAPI, PInvoke, C#
    """

    _getter_dict = clean_dict(getter_dict)

    orig_method = f"void {nodename}::{getter} .. {_getter_dict} "
    method = f'{nodename.lower()}_{getter}'
    base_type, ccl_type, cs_type, marshal, marshal_reint, pinv_type = _types_from_datatype(getter_dict)
    ptr_type = f"ccl::{nodename}*"
    base_type = base_type.replace('*', '')
    alt_ret = get_alternative_ret_type(ccl_type, everything[nodename], memberdict, everything)
    pinv_ret = cs_type
    cast = ''
    #if getter_dict.get('datatype', '') == 'ENUM':
    #    cast = '(ccl::uint)'
    if 'string' in cs_type:
        pinv_ret = 'void'
    string_handler_type = ''
    string_handler_type_pinv = ''
    string_handler_type_cs = ''
    get_c_body = f"""
  if(_ptr!=nullptr) {{
    {ptr_type} ptr = ({ptr_type})_ptr;
    return {cast}ptr->{getter}();
  }}
  """
    get_cs_body = f"""
        return call_{method}(cclptr);
    """

    if "string" in ccl_type:
        ccl_type = 'void'
        string_handler_type = ', void* stringholder'
        string_handler_type_pinv = ', IntPtr stringholder'
        get_c_body = f"""
  if(stringholder!=nullptr && _ptr!=nullptr) {{
    ccl::{nodename}* ptr = (ccl::{nodename}*)(_ptr);
    StringHolder* holder = (StringHolder*)stringholder;
    std::string name{{ptr->{getter}().c_str()}};
    holder->thestring = name;
  }}"""
        get_cs_body = f"""
        CSStringHolder stringHolder = new ();
        call_{method}(cclptr, stringHolder.Ptr);
        return stringHolder.Value;
        """

    capi_code = f"""
/* {orig_method} */
CCL_CAPI {ccl_type} call_{method}(
            void* _ptr{string_handler_type}
        )
{{
  {get_c_body.strip()}
  {alt_ret.strip()}
}}
"""

    pinv_code = f"""{dllimport}
    private static extern {pinv_ret} call_{method}(IntPtr ptr{string_handler_type_pinv});
"""

    cs_code = f"""
    public static {cs_type} {method}(
        IntPtr cclptr
    )
    {{
        {get_cs_body.strip()}
    }}

"""

    return capi_code, pinv_code, cs_code
