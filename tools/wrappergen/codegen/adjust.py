
import sys
import re

print(sys.path)

import mappings as map


def type_for_capi(_name : str, nodes : dict, _context : tuple, is_return=False):
  _name = _name.split('=')[0]  # clean up case 'float mn=0.0f': drop '=0.0f'
  _name = re.sub(r"(.*)unique_ptr<(.*)>(.*)", r"\1 \2* \3", _name)
  _name = _name.strip()
  nodename, _member = _context

  def handle_context(nodename, _memberctx, _namectx, contexts):
    handled = False
    for context in contexts:
      orig_str, repl_str, retrepl_str, repl_fn = context
      if repl_fn is not None:
        _namectx = repl_fn(nodename, _memberctx, _namectx)
      """
      if _name == orig_str:
        _name = repl_str
        handled = True
        break
      elif f' {orig_str}' in _name or f':{orig_str}' in _name:
        _name = _name.replace(orig_str, repl_str)
        handled = True
        break
      """
      if re.match(orig_str, _namectx) is not None:
        handled = True
        _namectx = re.sub(orig_str, repl_str, _namectx)
        break
    return _namectx, handled

  handled = False
  if nodename in map.capi_mappings_through_context:
    contexts = map.capi_mappings_through_context[nodename]
    _name, handled = handle_context(nodename, _member, _name, contexts)
  if not handled or nodename not in map.capi_mappings_through_context:
    _name, _ = handle_context(nodename, _member, _name, map.capi_mappings_through_context['*'])

  if _name.startswith('array<'):
    _name = f'ccl::{_name}'
  if '<float3' in _name:
    _name = _name.replace('<float3', '<ccl::float3')
  if '<float2' in _name:
    _name = _name.replace('<float2', '<ccl::float2')

  name = _name
  is_ref = '&' in name
  is_ptr = '*' in name
  # is_const = 'const' in name
  name = name.replace('*', '').replace('&', '').replace('const', '').strip()
  name = orig_name = name.split(' ')[0]
  ccl_name = 'ccl::' + name
  camera_name = 'Camera::' + name
  found_name = name in nodes
  found_ccl_name = ccl_name in nodes
  found_camera_name = camera_name in nodes
  if found_name or found_ccl_name or found_camera_name:
    if found_ccl_name:
      name = ccl_name
    elif found_camera_name:
      name = 'ccl::' + camera_name
    else:
      name = 'ccl::' + name
    name = _name.replace(orig_name, name)
  else:
    name = _name

  if is_return:
    if not is_ptr:
      name = name.replace('const', '')
    if is_ref:
      name = name.replace('&', '')
    name = name.strip()

  if is_return is False and (name.endswith('&') or name.endswith('*')):
    name += ' value'

  """
  if name.startswith('string'):
    name = 'std::' + name
  if ' string' in name:
    name = name.replace('string', 'std::string')
  if name.startswith('ustring'):
    name = 'OpenImageIO_v3_0::' + name
  """

  return name


def adjust_type_for_pinvoke_lvl1(_name : str, nodes : dict, _context : tuple, is_return=False):
  _name = _name.split('=')[0]  # clean up case 'float mn=0.0f': drop '=0.0f'
  _name = re.sub(r"(.*)unique_ptr<(.*)>(.*)", r"\1 \2* \3", _name)
  _name = _name.strip()
  nodename, _member = _context
  pass

