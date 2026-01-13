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

namespace ccl.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public sealed class ShaderNodeAttribute : System.Attribute
	{
		public ShaderNodeAttribute(string name, bool base_class)
		{
			Name = name;
			IsBase = base_class;
		}

		/// <summary>
		/// Shadernode attribute with given name. Non-base class.
		/// </summary>
		/// <param name="name"></param>
		public ShaderNodeAttribute(string name)
		{
			Name = name;
			IsBase = false;
		}

		public ShaderNodeAttribute() { }

		public string Name { get; set; }

		public bool IsBase { get; set; }
	}
}
