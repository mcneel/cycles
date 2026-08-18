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
using System.Text;
using System.Xml;

/**
 * \defgroup cclshadernodes CSycles Shader Nodes
 */

/**
 * \ingroup cclshadernodes
 *  This pacakage contains the high-level <c>ccl.ShaderNode</c>s to be used in a <c>ccl.Shader</c>.
 *
 *  The classes give a clear and concise way of setting up <c>ccl.ShaderNode</c>s with a minimum of
 *  fuss.
 */

namespace ccl.ShaderNodes
{
	/// <summary>
	/// Attribute node input sockets
	/// </summary>
	public class AttributeInputs : Inputs
	{
	}

	/// <summary>
	/// Attribute node output sockets
	/// </summary>
	public class AttributeOutputs : Outputs
	{
		/// <summary>
		/// Attribute output socket
		/// </summary>
		public ColorSocket Color { get; set; }
		public VectorSocket Vector { get; set; }
		public FloatSocket Fac { get; set; }
		public FloatSocket Alpha { get; set; }

		internal AttributeOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
			Alpha = new FloatSocket(parentNode, "Alpha", "alpha");
			AddSocket(Alpha);
		}
	}

	/// <summary>
	/// An Add attribute.
	/// This attribute takes two inputs, <c>Closure1</c> and <c>Closure2</c>. These
	/// will be simply added together.
	///
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("attribute")]
	public class AttributeNode : ShaderNode
	{
		/// <summary>
		/// Attribute input sockets
		/// </summary>
		public AttributeInputs ins => (AttributeInputs)inputs;

		/// <summary>
		/// Attribute output sockets
		/// </summary>
		public AttributeOutputs outs => (AttributeOutputs)outputs;

		/// <summary>
		/// Create a new Add attribute.
		/// </summary>
		public AttributeNode(Shader shader) : this(shader, "An add attribute node")
		{
		}

		/// <summary>
		/// UiName of the attribute to use
		/// </summary>
		public string Attribute { get; set; }

		/// <summary>
		/// Create a new Add attribute with name
		/// </summary>
		/// <param name="name"></param>
		public AttributeNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal AttributeNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new AttributeInputs();
			outputs = new AttributeOutputs(this);
		}

		internal override void SetDirectMembers()
		{

			var val = Attribute;
			CSycles.shadernode_set_member_string(Id, "attribute", val);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			var attr = "";
			if (Utilities.Instance.read_string(ref attr, xmlNode.GetAttribute("Attribute")))
			{
				Attribute = attr;
			}
		}

		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" Attribute=\"{Attribute}\" ", 1024);
			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = $"{VariableName}.Attribute = {Attribute};";
			return code;
		}
	}
}
