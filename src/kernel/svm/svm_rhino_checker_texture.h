/**
Copyright 2014 Robert McNeel and Associates

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

CCL_NAMESPACE_BEGIN

ccl_device bool is_odd(int x)
{
  return (x & 1) == 1;
}

ccl_device float3 checker_texture_2d(float3 uvw, float3 color1, float3 color2)
{
  float3 color_out = make_float3(0.0f);

  uvw.x *= 2.0;
  uvw.y *= 2.0;

  int u_cell = int(floorf(uvw.x));
  int v_cell = int(floorf(uvw.y));

  if (is_odd(u_cell + v_cell)) {
    color_out = color1;
  }
  else {
    color_out = color2;
  }

  return color_out;
}

ccl_device void svm_rhino_node_checker_texture_2d(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, out_color_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &out_color_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);

  float3 out_color = checker_texture_2d(uvw, color1, color2);

  if (stack_valid(out_color_offset))
    stack_store_float3(stack, out_color_offset, out_color);
}

CCL_NAMESPACE_END
