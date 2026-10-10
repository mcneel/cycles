/**
Copyright 2014-2024 Robert McNeel and Associates

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

using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	public class TwoColorInputs : Inputs
	{
		public ColorSocket Color1 { get; set; }
		public FloatSocket Alpha1 { get; set; }
		public ColorSocket Color2 { get; set; }
		public FloatSocket Alpha2 { get; set; }

		public TwoColorInputs(ShaderNode parentNode)
		{
			Color1 = new ColorSocket(parentNode, "Color1", "color1");
			Color1.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			AddSocket(Color1);
			Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1");
			Alpha1.Value = 1.0f;
			AddSocket(Alpha1);
			Color2 = new ColorSocket(parentNode, "Color2", "color2");
			Color2.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			AddSocket(Color2);
			Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2");
			Alpha2.Value = 1.0f;
			AddSocket(Alpha2);
		}
	}

	public class TwoColorOutputs : Outputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Alpha { get; set; }

		public TwoColorOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "out_color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha");
			AddSocket(Alpha);
		}
	}
}
