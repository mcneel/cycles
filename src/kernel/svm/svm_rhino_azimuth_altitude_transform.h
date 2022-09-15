/*
 * Copyright 2011-2013 Blender Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

CCL_NAMESPACE_BEGIN

/* Checker */

ccl_device float3 azimuth_altitude_transform(float3 vector, float azimuth, float altitude, float threshold)
{
  float3 ref;
  ref.x = ref.y = 0.0f;
  ref.z = 1.0f;

  float3 location;
  location.x = location.y = location.z = 0.0f;

  float3 rotation;
  rotation.x = rotation.y = 0.0f;
  rotation.z = azimuth;

  float3 scale;
  scale.x = scale.y = scale.z= 1.0f;

  float3 azimuth_rotation = svm_mapping(NODE_MAPPING_TYPE_VECTOR, vector, location, rotation, scale);

  float3 axis;
  float dummy;
  svm_vector_math(&dummy, &axis, NODE_VECTOR_MATH_CROSS_PRODUCT, azimuth_rotation, ref, 1.0f);
  float axis_len;
  axis = normalize_len(axis, &axis_len);

  float3 rotated_vector = rotate_around_axis(azimuth_rotation, axis_len > threshold ? axis : ref, altitude);

  return rotated_vector;
}

ccl_device void svm_rhino_node_azimuth_altitude_transform(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint vector_in_offset, azimuth_offset, altitude_offset, threshold_offset;
  uint vector_out_offset = node.z;

  svm_unpack_node_uchar4(node.y, &vector_in_offset, &azimuth_offset, &altitude_offset, &threshold_offset);

  float4 data_node = read_node_float(kg, offset);

  float3 vector_in = stack_load_float3(stack, vector_in_offset);
  float azimuth = data_node.x;
  float altitude = data_node.y;
  float threshold = data_node.z;

  float3 vector_out = azimuth_altitude_transform(vector_in, azimuth, altitude, threshold);

  if (stack_valid(vector_out_offset))
    stack_store_float3(stack, vector_out_offset, vector_out);
}

CCL_NAMESPACE_END
