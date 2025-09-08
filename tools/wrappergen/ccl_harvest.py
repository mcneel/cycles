# See https://github.com/llvm/llvm-project/tree/main/clang/bindings/python/examples
# for more examples of how to use the clang python bindings
import clang.cindex
import ctypes
import argparse
import json
import itertools as it
import time
from pathlib import Path
from pprint import pprint

parser = argparse.ArgumentParser(description='Get Cycles classes and structs C++ file or a folder of C++ files')
parser.add_argument('file', type=Path, help='The file or path to harvest')
parser.add_argument('libs', type=Path, help='top-level path where all dependencies are located')
parser.add_argument('third_party', type=Path, help='top-level path where third-party dependencies are located')

def rec_children(node, marker=False, tokens=False, depth=0):
  sp = "  "
  for c in node.get_children():
    if marker: print("="*100)
    ts = " ".join([t.spelling for t in c.get_tokens()]) if tokens else '-'
    print(f'{sp*depth} | {c.kind} | {c.type.spelling} | {c.spelling} >> {ts}')
    rec_children(c, marker, tokens, depth+1)

types_to_skip = (
    'AlembicObject',
    'AlembicProcedural',
    'AttrKernelDataType',
    'Attribute',
    'AttributeDescriptor',
    'AttributeElement',
    'AttributeFlag',
    'AttributeMap',
    'AttributePrimitive',
    'AttributeRequest',
    'AttributeRequestSet',
    'AttributeSet',
    'AttributeStandard',
    'BVH',
    'BVH2',
    'BVHBuild',
    'BVHBuildTask',
    'BVHEmbree',
    'BVHHIPRT',
    'BVHMixedSplit',
    'BVHMulti',
    'BVHNode',
    'BVHObjectBinning',
    'BVHObjectSplit',
    'BVHOptiX',
    'BVHParams',
    'BVHRange',
    'BVHReference',
    'BVHReferenceCompare',
    'BVHSpatialBin',
    'BVHSpatialSplit',
    'BVHSpatialSplitBuildTask',
    'BVHSpatialStorage',
    'BVHStackEntry',
    'BVH_STAT',
    'BVHUnaligned',
    #'BoundBox',
    #'BoundBox2D',
    'CCLFirstHitContext',
    'CCLIntersectContext',
    'CCLLocalContext',
    'CCLShadowContext',
    'CCLVolumeContext',
    'ConstantFolder',
    'CPUDevice',
    'CPUKernelThreadGlobals',
    'CPUKernels',
    'CUDAContextScope',
    'CUDADevice',
    'CUDADeviceGraphicsInterop',
    'CUDADeviceKernel',
    'CUDADeviceKernels',
    'CUDADeviceQueue',
    'CachedData',
    'Child',
    'ClosureLabel',
    'CurvesSchemaData', # Alembic
    'DataType',
    'DedicatedTaskPool',
    'DenoiseImage',
    'DenoiseImageLayer',
    'DenoiseParams',
    'DenoiseTask',
    'Denoiser',
    'DenoiserGPU',
    'DenoiserPipeline',
    'DeviceKernel',
    'DeviceKernelArguments',
    'DeviceQueue',
    'DeviceScene',
    'DiagSplit',
    'DumpTraversalContext',
    'FaceSetShaderIndexPair',
    'Filter_Function_Table_Index',
    'GuardedAllocator',
    'HIPContextScope',
    'HIPDevice',
    'HIPDeviceKernel',
    'HIPDeviceKernels',
    'HIPDeviceQueue',
    'HIPRTDevice',
    'HIPRTDeviceQueue',
    'IESFile',
    'IESTextParser',
    'ImageDataType',
    'ImageDeviceFeatures',
    'ImageHandle',
    'ImageKey',
    'ImageLoader',
    'InnerNode',
    'IntegratorQueueCounter',
    'IntegratorShadowStateCPU',
    'IntegratorStateCPU',
    'IntegratorStateGPU',
    'Intersection',
    'Intersection_Function_Table_Index',
    'KernelAreaLight',
    'KernelBVH',
    'KernelBVHLayout',
    'KernelBackground',
    'KernelBake',
    'KernelCamera',
    'KernelContext',
    'KernelCurve',
    'KernelCurveSegment',
    'KernelData',
    'KernelDistantLight',
    'KernelExecutionInfo',
    'KernelFilm',
    'KernelFilmConvert',
    'KernelGlobalsCPU',
    'KernelGlobalsGPU',
    'KernelInfo',
    'KernelIntegrator',
    'KernelLight',
    'KernelLightDistribution',
    'KernelLightLinkSet',
    'KernelLightTreeEmitter',
    'KernelLightTreeNode',
    'KernelObject',
    'KernelParamsCUDA',
    'KernelParamsHIP',
    'KernelParamsHIPRT',
    'KernelParamsOptiX',
    'KernelParticle',
    'KernelSVMUsage',
)


datatypes = dict()


def tabs(depth):
    return '\t' * depth


class CclEnumConstant:
    def __init__(self, name, value):
        self.name = name
        self.value = value

    def key(self):
        return self.value

    def __str__(self):
        return f"{self.name} {self.value}"

    def __repr__(self):
        return self.__str__()

    def __eq__(self, value):
        if isinstance(value, CclEnumConstant):
            return self.value == value.value and self.name == value.name

        return False

    def __hash__(self):
        return hash(self.value) + hash(self.name)


class CclMemberType:
    def __init__(self, name, kind):
        self.name = name
        self.kind = kind

    def __eq__(self, value):
        if isinstance(value, CclMemberType):
            return self.name == value.name and self.kind == value.kind
        return False

    def __hash__(self):
        return hash(self.name) + hash(self.kind)

def _string_from_tokens(tokens, strip_spaces=False):
    """
    From a list of tokens generate a string using the spellings of the tokens.
    Use for creating signatures.

    Filters out comments as well as bodies starting with '{' and leave out
    anything after ':' (constructor).
    """
    tokens = it.filterfalse(lambda t: t.kind in (clang.cindex.TokenKind.COMMENT, ), tokens)
    tokens = it.takewhile(lambda t: not t.spelling in ("{", ":", ), tokens)

    tokens = [(t.spelling, t.kind) for t in tokens]

    tlen = len(tokens)
    adjusted_tokens = list()
    for i, (spelling, kind) in enumerate(tokens):
        if i + 1 == tlen:
            adjusted_tokens.append(spelling)
            break
        (next_spelling, next_kind) = tokens[i+1]
        if kind in (clang.cindex.TokenKind.KEYWORD, clang.cindex.TokenKind.IDENTIFIER,) and next_kind in (clang.cindex.TokenKind.KEYWORD, clang.cindex.TokenKind.IDENTIFIER,):
            adjusted_tokens.append(f"{spelling} ")
        elif spelling == ")" and next_spelling == "const":
            adjusted_tokens.append(f"{spelling} ")
        else:
            adjusted_tokens.append(spelling)

    strts = "".join(list(adjusted_tokens)).strip()
    if strip_spaces:
        strts = strts.replace(" ", "")
    return strts

def _type_from_cursor(cursor):
    kind = cursor.kind
    if kind == clang.cindex.CursorKind.INTEGER_LITERAL:
        return "int"
    elif kind == clang.cindex.CursorKind.FLOATING_LITERAL:
        return "float"
    elif kind == clang.cindex.CursorKind.CXX_BOOL_LITERAL_EXPR:
        return "bool"
    elif kind == clang.cindex.CursorKind.STRING_LITERAL:
        return "string"
    elif kind == clang.cindex.CursorKind.CXX_STATIC_CAST_EXPR:
        c = list(cursor.get_children())[0]
        c = list(c.get_children())[0]
        tp = _string_from_tokens(c.get_tokens(), strip_spaces=True)
        return tp
    elif kind in (clang.cindex.CursorKind.BINARY_OPERATOR, clang.cindex.CursorKind.UNARY_OPERATOR, clang.cindex.CursorKind.PAREN_EXPR, ):
        l = list(cursor.get_children())
        if l[-1].kind == clang.cindex.CursorKind.PAREN_EXPR:
            l = list(l[-1].get_children())
        if l[-1].kind == clang.cindex.CursorKind.INTEGER_LITERAL:
            return "int"
        elif l[-1].kind == clang.cindex.CursorKind.FLOATING_LITERAL:
            return "float"
        else:
            raise Exception("unknown_binary_operator")
    elif kind == clang.cindex.CursorKind.UNEXPOSED_EXPR:
        c = list(cursor.get_children())[0]
        cl = list(c.get_children())
        if len(cl) > 0:
            c = cl[0]
        tp = _string_from_tokens(c.get_tokens(), strip_spaces=True)
        return tp
    raise Exception("unknown_cursor_kind")

class CclMethod(CclMemberType):
    def __init__(self, node):
        super().__init__(node.spelling, node.kind)
        self.ret_type = node.result_type.spelling
        self.const = node.is_const_method()
        self.static = node.storage_class in (clang.cindex.StorageClass.STATIC,)
        self.condestructor = node.kind in (clang.cindex.CursorKind.CONSTRUCTOR, clang.cindex.CursorKind.DESTRUCTOR, )

        # prefer to get the signature from the tokens, but sometimes the tokens
        # are not usable, so we fall back to get_arguments()
        # The tokens are preferred since it will keep templated types intact.
        # Example 'array<bool>' in get_arguments() (or spelling even) will end up
        # as 'int &', which isn't all that helpful.
        sig_from_tokens = _string_from_tokens(node.get_tokens())
        if sig_from_tokens and "(" in sig_from_tokens:
            ret_type, args = sig_from_tokens.split(f"{self.name}(")
            self.ret_type = ret_type.strip()
            args = f"({args}"
            args = args.strip()

            # clean up const from the generated output
            if self.const and args.endswith("const"):
                args = args[:-len("const")]
                args = args.strip()

            # adjust formatting so it is more readable
            for c in ("&", ",", "*"):
                args = args.replace(c, f"{c} ")
        else:
            # use arguments
            args = ", ".join([f"{a.type.spelling} {a.spelling}" for a in node.get_arguments()])
            args = f"({args})"

        # double-check ret_type in case it is 'int', since it could be a pointer
        if self.ret_type == "int" and self.name == 'create':
            self.ret_type = "void*"

        self.argument_list = args

    def __eq__(self, value):
        if isinstance(value, CclMethod):
            return super().__eq__(value) \
                    and self.ret_type == value.ret_type \
                    and self.const == value.const \
                    and self.static == value.static \
                    and self.argument_list == value.argument_list
        return False

    def __hash__(self):
        return super().__hash__() \
            + hash(self.ret_type) \
            + hash(self.const) \
            + hash(self.static) \
            + hash(self.argument_list)


class CclField(CclMemberType):
    def __init__(self, name, kind, datatype):
        super().__init__(name, kind)
        self.datatype = datatype

    def __eq__(self, value):
        if isinstance(value, CclField):
            return super().__eq__(value) and self.datatype == value.datatype
        return False

    def __hash__(self):
        return super().__hash__() + hash(self.datatype)


class CclSocket():
    def __init__(self, member_name, ui_name, datatype, defval, is_input=True):
        self.member_name = member_name
        self.ui_name = ui_name
        self.datatype = datatype
        self.default_value = defval
        self.is_input = is_input

simplekinds_to_try_for_defval = (
    clang.cindex.CursorKind.FLOATING_LITERAL,
    clang.cindex.CursorKind.INTEGER_LITERAL,
    clang.cindex.CursorKind.CXX_BOOL_LITERAL_EXPR,
    clang.cindex.CursorKind.STRING_LITERAL,
)

class CclDataType:
    def __init__(self, node, parent=None):
        self.kind = node.kind
        self.name = node.spelling
        self.lowlevel_name = node.type.spelling
        self.enum_type = None
        self.superclass = None
        self.members = set()
        self.inherited_members = set()
        self.inputs = set()
        self.outputs = set()
        self.file = node.extent.start.file.name
        self.parent = parent
        self.shaderenums = dict()
        self.shadernode_name = None
        if self.parent:
            self.name = f"{self.parent}::{self.name}"

    def merge(self, other):
        """Add stuff from other that is not in self."""
        self.members = self.members.union(other.members)
        self.inputs = self.inputs.union(other.inputs)
        self.outputs = self.outputs.union(other.outputs)
        if self.superclass is None and other.superclass is not None:
            self.superclass = other.superclass
        for k,v in other.shaderenums.items():
            self.shaderenums[k] = v
        if self.shadernode_name is None and other.shadernode_name is not None:
            self.shadernode_name = other.shadernode_name

    def add_member(self, member):
        self.members.add(member)

    def realize_inheritance(self):
        if self.superclass in datatypes:
            self.inherited_members = datatypes[self.superclass].members.copy().union(datatypes[self.superclass].inherited_members.copy())

    def set_superclass(self, superclass):
        self.superclass = superclass

    def is_class_or_struct(self):
        return self.kind in (
            clang.cindex.CursorKind.CLASS_DECL,
            clang.cindex.CursorKind.STRUCT_DECL,
        )

    def is_enum(self):
        return self.kind in (
            clang.cindex.CursorKind.ENUM_DECL,
        )

    def harvest_shadernode_info(self, node):
        def rec_get_specific(node, cursor_type, spelling=None):
            for c in node.get_children():
                if spelling:
                    if c.kind == cursor_type and c.spelling == spelling:
                        return c
                elif c.kind == cursor_type:
                    return c
                r = rec_get_specific(c, cursor_type, spelling)
                if r:
                    return r
            return None
        def rec_collect_specific(node, cursor_type, spelling=None, recurse=True):
            nodes = list()
            for c in node.get_children():
                if spelling:
                    if c.kind == cursor_type and c.spelling == spelling:
                        nodes.append(c)
                elif c.kind == cursor_type:
                    nodes.append(c)
                if recurse:
                    r = rec_collect_specific(c, cursor_type, spelling)
                    if r:
                        nodes.extend(r)
            return nodes
        def find_type(node):
            type_name = list()

            decl_ref_exprs = rec_collect_specific(node, clang.cindex.CursorKind.DECL_REF_EXPR)
            for  decl_ref_expr in decl_ref_exprs:
                socktype = rec_get_specific(decl_ref_expr, clang.cindex.CursorKind.TYPE_REF, 'struct ccl::SocketType')
                if decl_ref_expr is not None and socktype is not None:
                    type_name.append(decl_ref_expr.spelling)
            try:
                type_name.remove('T')
            except:
                pass

            type_name = type_name[0]

            return type_name if type_name is not None else "@@@"
        def find_register_type(node, defval):
            register_asts = list()
            register_types = list()
            # get all compounds
            choices = ('register_input', 'register_output', )
            is_input = False
            for which in choices:
                nodes = rec_collect_specific(node, clang.cindex.CursorKind.CALL_EXPR, spelling=which)
                if len(nodes)>0:
                    is_input = which == 'register_input'
                    register_asts.append((node, nodes))
                else:
                    pass

            for reg_type in register_asts:
                compound = reg_type[0]
                node = reg_type[1]
                tp = find_type(compound)
                for n in node:
                    names = rec_collect_specific(n, clang.cindex.CursorKind.STRING_LITERAL)
                    names = [n.spelling.replace("\"", "") for n in names]
                    register_types.append(CclSocket(names[0], names[1], tp, defval, is_input))
            #nodes = rec_collect_specific(node, clang.cindex.CursorKind.CALL_EXPR, register_type)
            return is_input, register_types[0]
        def find_add_name_simple(node):
            addstr = _string_from_tokens(node.get_tokens())
            return addstr.split('"')[1]
        def find_add_name(node):
            _addstr = _string_from_tokens(node.get_tokens())
            node = rec_get_specific(node, clang.cindex.CursorKind.CALL_EXPR, "add")
            if node is None:
                return _addstr.split('"')[1]
            literal = rec_get_specific(node, clang.cindex.CursorKind.STRING_LITERAL)
            return literal.spelling.replace("\"", "")
        def get_defval(node):
            cs = list(node.get_children()) # COMPOUND_STMT children
            cs = list(cs[0].get_children()) # DECL_STMT children
            cs = list(cs[0].get_children()) # VAR_DECL children
            if len(cs) > 1:
                tp = _string_from_tokens(cs[0].get_tokens(), strip_spaces=True)
                val = _string_from_tokens(cs[1].get_tokens(), strip_spaces=True)
            elif len(cs) == 1:
                if cs[0].referenced is not None:
                    tp = cs[0].referenced.type.spelling
                else:
                    tp = _type_from_cursor(cs[0])
                tp = tp.replace('ccl::', '')
                val = _string_from_tokens(cs[0].get_tokens(), strip_spaces=True)
            else:
                tp = None
                val = None
            return tp, val
        def get_enumname(node):
            c = [c for c in node.get_children()][0]
            is_enum = c.type.spelling == 'NodeEnum'
            return (is_enum, c.spelling)
        def get_enummember(node):
            cs = [c for c in node.get_children()]
            if len(cs) >= 3:
                namenode = cs[1]
                nn_lit  = [c for c in namenode.get_children()][0]
                valnode = cs[2]
                if valnode.referenced is not None:
                    valfullname = f"{valnode.referenced.type.spelling}::{valnode.spelling}"
                else:
                    if valnode.kind in (clang.cindex.CursorKind.INTEGER_LITERAL,):
                        intval = int(_string_from_tokens(valnode.get_tokens(), strip_spaces=True))
                    valfullname = f"{intval}"
                return (True, nn_lit.spelling.replace("\"", ""), valfullname)
            return (False, None, None)

        # first get the top COMPOUND_STMT, this will contain everything we
        # want to look at
        node = rec_get_specific(node, clang.cindex.CursorKind.COMPOUND_STMT)

        # first decl statement will contain the NodeType::add call. We can get
        # the name string from that
        topdecl = [c for c in node.get_children() if c.kind == clang.cindex.CursorKind.DECL_STMT][0]

        cns = [
            c for c in node.get_children() if c.kind not in (
                                            clang.cindex.CursorKind.RETURN_STMT,
                                            clang.cindex.CursorKind.NULL_STMT,
                                            )
            ]

        cmpds = [c for c in cns if c.kind in (
                                        clang.cindex.CursorKind.COMPOUND_STMT, )
                        ]
        noncmpds = [c for c in cns if c.kind not in (
                                        clang.cindex.CursorKind.COMPOUND_STMT, )
                        ]
        handling_an_enum = False
        enum_dict = dict()
        curname = None
        cur_enum = dict()
        for noncmpd in noncmpds:
            if noncmpd.kind == clang.cindex.CursorKind.DECL_STMT:
                if len(cur_enum.keys())>0 and curname is not None:
                    enum_dict[curname] = cur_enum
                cur_enum = dict()
                is_enum, enumname = get_enumname(noncmpd)
                if is_enum:
                    curname = enumname
                    handling_an_enum = True
                else:
                    curname = None
                    handling_an_enum = False
            if noncmpd.kind == clang.cindex.CursorKind.CALL_EXPR and handling_an_enum:
                is_enummember, membername, memberval = get_enummember(noncmpd)
                if is_enummember:
                    cur_enum[membername] = memberval
        # check if we were setting up an enum
        # after leaving previous loop. We need
        # to add it so it doesn't go missing
        if handling_an_enum and curname is not None and len(cur_enum.keys())>0:
            enum_dict[curname] = cur_enum
        for k,v in enum_dict.items():
            self.shaderenums[k] = v

        for cmpd in cmpds:
            defval = get_defval(cmpd)
            is_input, regtyp = find_register_type(cmpd, defval)
            if is_input:
                self.inputs.add(regtyp)
            else:
                self.outputs.add(regtyp)

        self.shadernode_name = find_add_name_simple(topdecl)

    def __str__(self):
        return f"{self.name}: {self.members}"

    def __repr__(self):
        return f"{self.name}: {self.members}"


def type_string(ccltype):
    if ccltype.kind == clang.cindex.CursorKind.CLASS_DECL:
        return 'class'
    elif ccltype.kind == clang.cindex.CursorKind.STRUCT_DECL:
        return 'struct'
    elif is_enum(ccltype):
        return 'enum'


class CclDataTypeEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, CclDataType):
            members_list = list(obj.members)
            if is_enum(obj):
                members_list.sort(key=lambda x: x.key())
            d = {
                'name': obj.name,
                'type': type_string(obj),
                'members': members_list,
                'file': obj.file,
            }
            if obj.shadernode_name:
                d['shadernode_name'] = obj.shadernode_name
            if obj.superclass:
                d['superclass'] = obj.superclass
            if len(obj.inputs) > 0:
                d['inputs'] = list(obj.inputs)
            if len(obj.outputs) > 0:
                d['outputs'] = list(obj.outputs)
            if len(obj.shaderenums.keys()) > 0:
                d['shaderenums'] = obj.shaderenums
            return d
        if isinstance(obj, CclSocket):
            return {
                'member_name': obj.member_name,
                'ui_name': obj.ui_name,
                'datatype': obj.datatype,
                'is_input': obj.is_input,
                'default_value': obj.default_value[1],
                'default_value_type': obj.default_value[0],
            }
        if isinstance(obj, CclEnumConstant):
            return {
                'name': obj.name,
                'value': obj.value,
            }
        if isinstance(obj, CclMethod):
            d = {
                'name': obj.name,
                'arguments': obj.argument_list,
            }
            if not obj.condestructor:
                d['static'] = obj.static
                d['ret_type'] = obj.ret_type
                d['const'] = obj.const
            return d
        if isinstance(obj, CclField):
            return {
                'name': obj.name,
                'datatype': obj.datatype,
            }
        return super().default(obj)


# Return True if the node is a class or struct
def is_class_or_struct(node):
    return node.kind in (
        clang.cindex.CursorKind.CLASS_DECL,
        clang.cindex.CursorKind.STRUCT_DECL,
    )


def is_namespace(node):
    return node.kind in (
        clang.cindex.CursorKind.NAMESPACE,
        clang.cindex.CursorKind.NAMESPACE_ALIAS,
    )


def is_function(node):
    return node.kind in (
        clang.cindex.CursorKind.FUNCTION_DECL,
        clang.cindex.CursorKind.CXX_METHOD,
    )


def is_enum(node):
    return node.kind in (
        clang.cindex.CursorKind.ENUM_DECL,
    )


# Get member field out of the node. parent is the
# name of the class or struct to which this field
# belongs
def member(node, parent):
    # don't do unnamed structs
    if "unnamed struct" in node.spelling:
        return
    if parent in datatypes:
        #print(f"\t\t-> Found member: ({node.result_type.spelling}) {node.spelling}")
        ccl_type = datatypes[parent]

        if is_enum(ccl_type):
            return

        if is_function(node):
            datatypes[parent].add_member(f"{node.result_type.spelling} {node.result_type.spelling} (fn)")
        else:
            datatypes[parent].add_member(f"{node.type.spelling} {node.spelling}")

def do_method(node):
    meth = CclMethod(node)
    return meth

def do_field(node):
    if skip_node(node):
        return None
    #print(f"\tField: {node.type.spelling} {node.spelling}")
    fld = CclField(node.spelling, node.kind, node.type.spelling)
    return fld

def skip_node(node):
    for to_skip in (
        'thread', 'third_party', 'linux_x64',
        #'svm',
        'profiling.h', 'memory.h'):
        if to_skip in node.extent.start.file.name:
            return True
    for node_to_skip in types_to_skip:
        if node_to_skip == node.spelling:
            return True
    for unnamed in ("unnamed struct", "unnamed enum", ):
        if unnamed in node.spelling:
            return True
        if unnamed in node.type.spelling:
            return True
    return False


def do_class(node, parent=None):
    if skip_node(node):
        return

    #print(f"Found class or struct: {node.spelling}")
    cls = CclDataType(node, parent)
    for child_node in node.get_children():
        member = None
        # skip anything non-public
        if child_node.access_specifier in (
                                    clang.cindex.AccessSpecifier.PRIVATE,
                                    clang.cindex.AccessSpecifier.PROTECTED,
                                        ):
            continue
        if child_node.kind in (
                                clang.cindex.CursorKind.CXX_METHOD,
                                clang.cindex.CursorKind.CONSTRUCTOR,
                                clang.cindex.CursorKind.DESTRUCTOR,
                            ):
            member = do_method(child_node)
        elif child_node.kind in (clang.cindex.CursorKind.FIELD_DECL,):
            member = do_field(child_node)
        elif child_node.kind == clang.cindex.CursorKind.CXX_BASE_SPECIFIER:
            cls.set_superclass(child_node.spelling)
        elif child_node.kind == clang.cindex.CursorKind.ENUM_DECL:
            do_enum(child_node)
        elif is_class_or_struct(child_node):
            do_class(child_node, node.spelling)
        else:
            pass
        if member:
            cls.add_member(member)

    name = cls.name
    if name in datatypes:
        clsold = datatypes[name]
        cls.merge(clsold)

    datatypes[name] = cls

def do_enum(node):
    if skip_node(node):
        return

    enumname = node.spelling
    if node.semantic_parent and len(node.semantic_parent.spelling)>0:
        enumname = f"{node.semantic_parent.spelling}::{node.spelling}"

    if node.spelling in datatypes:
        ccl_enum = datatypes[enumname]
    else:
        parent = None
        if node.semantic_parent and len(node.semantic_parent.spelling)>0:
            parent = node.semantic_parent.spelling
        ccl_enum = CclDataType(node, parent)

    for c in node.get_children():
        if c.kind == clang.cindex.CursorKind.ENUM_CONSTANT_DECL:
            ccl_enum_constant = CclEnumConstant(c.spelling, c.referenced.enum_value)
            ccl_enum.add_member(ccl_enum_constant)

    datatypes[enumname] = ccl_enum

    return ccl_enum

def investigate_children(node, depth=0, parent=None):
    # The classes are typically found at first depth (1)
    # and the members are found at second depth (2)
    # we are not interested in tokens deeper than that
    # so we can skip early. When we want to understand actual
    # implementations of methods for instance we will want to
    # go deeper
    #if depth == 1 and is_namespace(node) and not node.spelling in ('ccl', ):

    if is_namespace(node) and not node.spelling in ('ccl', ):
        return

    #if node.kind == clang.cindex.CursorKind.ENUM_DECL:
    #    print(f"{node.spelling} @ {depth} ({parent} {node.extent.start.file.name} : {node.extent.start.line})")

    if 'register_type'==node.spelling and node.kind!=clang.cindex.CursorKind.OVERLOADED_DECL_REF:
        shndt_name = node.semantic_parent.spelling
        if shndt_name in datatypes:
            shndt : CclDataType = datatypes[shndt_name]
            shndt.harvest_shadernode_info(node)
        else:
            print(f"Could not find {shndt_name} in datatypes XXXXX")

    if is_class_or_struct(node):
        do_class(node)
    elif is_enum(node):
        do_enum(node)
    else:
        for child in node.get_children():
            # skip anything from system headers
            # to avoid unnecessary clutter
            if child.location.is_in_system_header:
                continue

            # recurse
            investigate_children(child, depth + 1, child.spelling)


def skip_path(path):
    if path.suffix in ('.h', ):
        return True
    # Skip files from select directories.
    for prt in (
            'ccycles', 'cmake', 'kernel', 'hydra',
            'test', 'app', 'thread',
                ):
        if prt in path.parts:
            return True
    return False


def get_source_files(path):
    source_files = set()
    include_dirs = set()
    p = Path(path)
    if p.is_file():
        source_files.add(f"{p}")
        include_dirs.add(f"{p.parent}")
        include_dirs.add(f"{p.parent.parent}")
    else:
        include_dirs.add(f"{p}")
        for f in p.iterdir():
            if skip_path(f):
                continue
            if f.is_dir():
                sfs, ids = get_source_files(f)
                source_files = source_files.union(set(sfs))
                include_dirs = include_dirs.union(set(ids))
            elif f.is_file() and f.suffix in ('.cpp', '.h',):
                source_files.add(f"{f}")
    return list(source_files), list(include_dirs)


def get_diag_info(diag):
    fn = diag.location.file.name if diag.location.file else '-'
    return {
        "severity": diag.severity,
        "location": f'{fn} +{diag.location.line}',
        "message": diag.spelling,
        #"ranges": [f'{ddiag.ranges,
        #"fixits": diag.fixits,
    }


def harvest(file, libs, third_party):
    diags = dict()
    include_dirs = set()
    for libdir in libs.iterdir():
        if 'aarch' in f'{libdir}':
            continue
        if libdir.is_dir():
            include_dirs.add(f"{libdir}")
            include_dirs.add(f"{libdir / 'include'}")
    for libdir in third_party.iterdir():
        if libdir.is_dir():
            include_dirs.add(f"{libdir}")
            include_dirs.add(f"{libdir / 'include'}")

    source_files, _include_dirs = get_source_files(file)
    source_files.sort()
    print(f"Found {len(source_files)} source files to harvest.")
    include_dirs = list(include_dirs.union(set(_include_dirs)))
    include_dirs = ['-I' + d for d in include_dirs]
    total = len(source_files)
    args = [
        "-std=c++17",
        "-stdlib=libc++",
        "-isystem=/usr/include/c++/v1",
        "-isystem=/usr/lib/llvm-19/include/c++/v1",
        "-DCCL_NAMESPACE_BEGIN=namespace ccl {",
        "-DCCL_NAMESPACE_END=}",
        "-DWITH_ONEAPI",
        "-DWITH_CUDA",
        "-DWITH_OPTIX",
        "-DWITH_HIP",
        "-DWITH_HIPRT",
        "-DWITH_METAL",
        "-DWITH_KERNEL_NATIVE",
        "-DWITH_NANOVDB",
        "-DWITH_OPENVDB",
        "-DWITH_EMBREE",
        "-DWITH_EMBREE_GPU",
        "-DWITH_PATH_GUIDING",
        "-DWITH_OSL",
        "-DWITH_OCIO",
        #"-DWITH_USD",
        "-DWITH_OPENIMAGEDENOISE",
        #"-DWITH_ALEMBIC",
        "-DWITH_PTEX",
        "-DWITH_OPENSUBDIV",
        "-Dfloorf=floor",
        "-Dfminf=min",
        '-Wno-invalid-offsetof',
    ] + include_dirs
    diags['args'] = args
    start_all = time.time()
    print("->")
    clr = " " * 100
    def rec_children(node, depth=0):
      for c in node.get_children():
        if hasattr(c, 'get_tokens'):
            s = _string_from_tokens(c.get_tokens())
        else:
            s = ''
        print("    "*depth, f"{c.kind} | {c.spelling} | {c.type.spelling} ->{s}<-")
        rec_children(c, depth+1)
    for i, sf in enumerate(source_files):
        psf = Path(sf)
        print(f"\r{clr}", end='')
        print(f"\rHarvesting {psf.name}.. ({i+1}/{total}), total elapsed time: {time.time() - start_all:.2f}s", end='')
        index = clang.cindex.Index.create()
        tu = index.parse(sf, args=args)
        diags[f'{sf}'] = [get_diag_info(d) for d in tu.diagnostics]

        #print(f"\t.. parsed {psf.name}, extracting now datatypes and their members...")
        investigate_children(tu.cursor)
        #rec_children(tu.cursor)
        #print(f"\tcompleted {psf.name}.")

    end_all = time.time()

    clss        = 0
    enums       = 0
    unknonws    = 0
    shadernodes = 0
    print("ShaderNodes:")
    for k in datatypes.keys():
        dt : CclDataType = datatypes[k]
        if dt.superclass in ('ShaderNode', 'BsdfNode', 'TextureNode', 'ImageSlotTextureNode', 'BsdfBaseNode', 'VolumeNode', ):
            if dt.name in ('BsdfNode', 'TextureNode', 'ImageSlotTextureNode', 'BsdfBaseNode', 'VolumeNode',):
                continue
            print(f"\t* {dt.name} is a ShaderNode")
            shadernodes += 1
        if is_class_or_struct(dt):
            clss += 1
        elif is_enum(dt):
            enums += 1
        else:
            unknonws += 1

    print("\n\n")
    print(f"done ({end_all - start_all:.2f}s).")
    print(f"Found {len(datatypes)} datatypes: {clss} classes/structs, {enums} enums, unknowns {unknonws}.")
    print(f"ShaderNodes: {shadernodes}")

    return diags



# Realize inheritance by adding inherited members to the datatypes
# This will then show nicely in the generated JSON of the datatypes
# list
def realize_inheritance():
    for k in datatypes:
        datatypes[k].realize_inheritance()


args = parser.parse_args()
diags = harvest(args.file, args.libs, args.third_party)

#realize_inheritance()

res = json.dumps(datatypes, indent=4, cls=CclDataTypeEncoder, sort_keys=True)
res = res.replace('cc::', '')
with open('datatypes.json', 'w') as f:
    f.write(res)

diagres = json.dumps(diags, indent=4)
with open('diagnostics.json', 'w') as f:
    f.write(diagres)
