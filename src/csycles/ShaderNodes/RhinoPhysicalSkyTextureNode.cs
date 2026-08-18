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

using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using System;

namespace ccl.ShaderNodes
{
	public class PhysicalSkyTextureInputs : Inputs
	{
		public VectorSocket UVW { get; set; }

		public PhysicalSkyTextureInputs(ShaderNode parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class PhysicalSkyTextureOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		public PhysicalSkyTextureOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "out_color");
			AddSocket(Color);
		}
	}

	[ShaderNode("rhino_physical_sky_texture")]
	public class PhysicalSkyTextureProceduralNode : ShaderNode
	{
		public PhysicalSkyTextureInputs ins => (PhysicalSkyTextureInputs)inputs;
		public PhysicalSkyTextureOutputs outs => (PhysicalSkyTextureOutputs)outputs;

		public float SunDirectionX { get; set; }
		public float SunDirectionY { get; set; }
		public float SunDirectionZ { get; set; }
		public float AtmosphericDensity { get; set; }
		public float RayleighScattering { get; set; }
		public float MieScattering { get; set; }
		public bool ShowSun { get; set; }
		public float SunBrightness { get; set; }
		public float SunSize { get; set; }
		public float SunColorRed { get; set; }
		public float SunColorGreen { get; set; }
		public float SunColorBlue { get; set; }
		public float InverseWavelengthsX { get; set; }
		public float InverseWavelengthsY { get; set; }
		public float InverseWavelengthsZ { get; set; }
		public float Exposure { get; set; }

		public PhysicalSkyTextureProceduralNode(Shader shader) : this(shader, "a physical sky texture") { }
		public PhysicalSkyTextureProceduralNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal PhysicalSkyTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new PhysicalSkyTextureInputs(this);
			outputs = new PhysicalSkyTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_vec(Id, "SunDirection", SunDirectionX, SunDirectionY, SunDirectionZ);
			CSycles.shadernode_set_member_float(Id, "AtmosphericDensity", AtmosphericDensity);
			CSycles.shadernode_set_member_float(Id, "RayleighScattering", RayleighScattering);
			CSycles.shadernode_set_member_float(Id, "MieScattering", MieScattering);
			CSycles.shadernode_set_member_bool(Id, "ShowSun", ShowSun);
			CSycles.shadernode_set_member_float(Id, "SunBrightness", SunBrightness);
			CSycles.shadernode_set_member_float(Id, "SunSize", SunSize);
			CSycles.shadernode_set_member_vec(Id, "SunColor", SunColorRed, SunColorGreen, SunColorBlue);
			CSycles.shadernode_set_member_vec(Id, "InverseWavelengths", InverseWavelengthsX, InverseWavelengthsY, InverseWavelengthsZ);
			CSycles.shadernode_set_member_float(Id, "Exposure", Exposure);
		}
	}
}
