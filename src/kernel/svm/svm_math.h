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

/* Nodes */

ccl_device void svm_node_math(KernelGlobals *kg, ShaderData *sd, float *stack, uint itype, uint f1_offset, uint f2_offset, int *offset)
{
	NodeMath type = (NodeMath)itype;
	float f1 = stack_load_float(stack, f1_offset);
	float f2 = stack_load_float(stack, f2_offset);
	float f = svm_math(type, f1, f2);

	uint4 node1 = read_node(kg, offset);

	stack_store_float(stack, node1.y, f);
}

ccl_device void svm_node_vector_math(KernelGlobals *kg, ShaderData *sd, float *stack, uint itype, uint v1_offset, uint v2_offset, int *offset)
{
	NodeVectorMath type = (NodeVectorMath)itype;
	float3 v1 = stack_load_float3(stack, v1_offset);
	float3 v2 = stack_load_float3(stack, v2_offset);
	float f;
	float3 v;

	svm_vector_math(&f, &v, type, v1, v2);

	uint4 node1 = read_node(kg, offset);

	if(stack_valid(node1.y)) stack_store_float(stack, node1.y, f);
	if(stack_valid(node1.z)) stack_store_float3(stack, node1.z, v);
}

ccl_device void svm_node_matrix_math(KernelGlobals *kg, ShaderData *sd, float *stack, uint itype, uint vec_offset, uint out_offset, int *offset)
{
	NodeMatrixMath type = (NodeMatrixMath)itype;
	float3 v = stack_load_float3(stack, vec_offset);

	Transform tfm;
	tfm.x = read_node_float(kg, offset);
	tfm.y = read_node_float(kg, offset);
	tfm.z = read_node_float(kg, offset);

	float3 r;
	switch (type) {
		case NODE_MATRIX_MATH_DIRECTION:
		{
			r = transform_direction(&tfm, v);
			break;
		}
		case NODE_MATRIX_MATH_PERSPECTIVE:
		{
#ifndef __KERNEL_GPU__
			ProjectionTransform pt(tfm);
			r = transform_perspective(&pt, v);
#endif
			break;
		}
		case NODE_MATRIX_MATH_DIR_TRANSPOSED:
		{
			r = transform_direction_transposed(&tfm, v);
			break;
		}
		default:
			r = transform_point(&tfm, v);
	}
	stack_store_float3(stack, out_offset, r);
}

CCL_NAMESPACE_END
