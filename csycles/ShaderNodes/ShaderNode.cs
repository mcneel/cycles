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
using System.Linq;
using System.Text;
using System.Xml;

namespace ccl.ShaderNodes
{
	using cclext;
	/// <summary>
	/// Base class for shader nodes.
	/// </summary>
	[ShaderNode("shadernode base", true)]
	public class ShaderNode
	{

		/// <summary>
		/// Set a shaderNodeTypeName for this node
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Get shaderNodeTypeName that can be used as variable shaderNodeTypeName
		/// </summary>
		public virtual string VariableName
		{
			get
			{
				var s = $"{Name}{Id}";
				return Extensions.FirstCharacterToLower(s);
			}
		}

		/// <summary>
		/// Get the node ID. This is set when created in Cycles.
		/// </summary>
		public IntPtr Id { get; internal set; }

		/// <summary>
		/// Get the XML shaderNodeTypeName of the node type as string.
		/// </summary>
		virtual public string ShaderNodeTypeName
		{
			get
			{
				var t = GetType();
				var attr = t.GetCustomAttributes(typeof(ShaderNodeAttribute), false)[0] as ShaderNodeAttribute;
				return attr.Name;
			}
		}

		public string ShaderNodeTypeCodeName
		{
			get
			{
				var t = GetType();
				return t.Name;
			}
		}

		/// <summary>
		/// Generic access to input sockets.
		/// </summary>
		public Inputs inputs { get; set; }
		/// <summary>
		/// Generic access to output sockets.
		/// </summary>
		public Outputs outputs { get; set; }

		public virtual ClosureSocket GetClosureSocket()
		{
			throw new NotImplementedException($"Should implement GetClosureSocket for this node {ShaderNodeTypeName}");
		}

#if OLDSTUFF
		/// <summary>
		/// Create node of type ShaderNodeType type
		/// </summary>
		/// <param name="type"></param>
		internal ShaderNode(ShaderNodeType type) : this(type, String.Empty)
		{
		}

		/// <summary>
		/// Create node of type ShaderNodeType and with given name
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		internal ShaderNode(ShaderNodeType type, string name)
		{
			Type = type;
			Name = name;
		}
#endif

		internal ShaderNode(Shader shader, string name)
		{
			ConstructShaderNode(shader, ShaderNodeTypeName, name);
		}

		internal ShaderNode(Shader shader, IntPtr shadernodePtr)
		{
			Id = shadernodePtr;
			Shader = shader;
		}

		public Shader Shader { get; private set; }


		internal void ConstructShaderNode(Shader shader, string shaderNodeTypeName, string shaderNodeName)
		{
			Id = CSycles.add_shader_node(shader.Id, shaderNodeTypeName, shaderNodeName);
			Shader = shader;
			Shader.AddNode(this);
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has enumerations that need to be committed to Cycles
		/// </summary>
		/// <param shaderNodeTypeName="clientId"></param>
		/// <param shaderNodeTypeName="sceneId"></param>
		/// <param shaderNodeTypeName="shaderId"></param>
		virtual internal void SetEnums()
		{
			// do nothing
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has direct members that need to be committed to Cycles
		/// </summary>
		/// <param shaderNodeTypeName="clientId"></param>
		/// <param shaderNodeTypeName="sceneId"></param>
		/// <param shaderNodeTypeName="shaderId"></param>
		virtual internal void SetDirectMembers()
		{
			// do nothing
		}

		internal void SetSockets()
		{
			/* set node attributes */
			if (inputs != null)
			{
				foreach (var socket in inputs.Sockets)
				{
					if (socket is FloatSocket float_socket)
					{
						CSycles.shadernode_set_attribute_float(Id, float_socket.InternalName, float_socket.Value);
					}
					if (socket is IntSocket int_socket)
					{
						CSycles.shadernode_set_attribute_int(Id, int_socket.InternalName, int_socket.Value);
					}
					if (socket is Float4Socket float4_socket)
					{
						CSycles.shadernode_set_attribute_vec(Id, float4_socket.InternalName, float4_socket.Value);
					}
					if (socket is BoolSocket bool_socket)
					{
						CSycles.shadernode_set_attribute_bool(Id, bool_socket.InternalName, bool_socket.Value);
					}
					if (socket is StringSocket string_socket)
					{
						if (string.IsNullOrEmpty(string_socket.Value)) continue;
						CSycles.shadernode_set_attribute_string(Id, string_socket.InternalName, string_socket.Value);
					}
				}
			}
		}

		public override string ToString()
		{
			var str = $"{Name} ({ShaderNodeTypeName})";
			return str;
		}

		/// <summary>
		/// Implement ParseXml to support proper XMl support.
		/// </summary>
		/// <param shaderNodeTypeName="xmlNode"></param>
		virtual internal void ParseXml(XmlReader xmlNode)
		{
		}

		public virtual string CreateXmlAttributes()
		{
			return "";
		}

		public virtual string CreateChildNodes()
		{
			return "";
		}

		public virtual string CreateXml()
		{
			var nfi = Utilities.Instance.NumberFormatInfo;
			var xml = new StringBuilder($"<{ShaderNodeTypeName} shaderNodeTypeName=\"{VariableName}\" ", 1024);

			foreach (var inp in inputs.Sockets)
			{
				if (inp.ConnectionFrom != null) continue;

				if (inp is FloatSocket fs)
				{
					xml.AppendFormat(nfi, " {0}=\"{1}\"", fs.XmlName, fs.Value);
					continue;
				}
				if (inp is IntSocket ints)
				{
					xml.AppendFormat(nfi, " {0}=\"{1}\"", ints.XmlName, ints.Value);
					continue;
				}
				if (inp is ColorSocket cols)
				{
					xml.AppendFormat(nfi, " {0}=\"{1} {2} {3} {4}\"", cols.XmlName, cols.Value.x, cols.Value.y, cols.Value.z, cols.Value.w);
					continue;
				}
				if (inp is VectorSocket vec)
				{
					xml.AppendFormat(nfi, " {0}=\"{1} {2} {3} {4}\"", vec.XmlName, vec.Value.x, vec.Value.y, vec.Value.z, vec.Value.w);
					continue;
				}
				if (inp is Float4Socket f4s)
				{
					xml.AppendFormat(nfi, " {0}=\"{1} {2} {3} {4}\"", f4s.XmlName, f4s.Value.x, f4s.Value.y, f4s.Value.z, f4s.Value.w);
					continue;
				}
				if (inp is StringSocket strs)
				{
					xml.AppendFormat(nfi, " {0}=\"{1}\"", strs.XmlName, strs.Value);
				}
			}

			xml.Append(CreateXmlAttributes());

			var childNodes = CreateChildNodes();
			if (String.IsNullOrEmpty(childNodes))
			{
				xml.Append(" />");
			}
			else
			{
				xml.Append(">");
				xml.Append(childNodes);
				xml.Append($"</{ShaderNodeTypeName}>");
			}

			return xml.ToString();
		}

		public virtual string CreateConnectXml()
		{
			var sb = inputs.Sockets.Aggregate(new StringBuilder("", 1024), (current, inp) => current.Append($"{inp.ConnectTag}\n"));
			return sb.ToString();
		}

		public virtual string CreateCodeAttributes()
		{
			return "";
		}

		public virtual string CreateCode()
		{
			var cs = new StringBuilder($"var {VariableName} = new {ShaderNodeTypeCodeName}(\"{Name}\");", 1024);

			var nfi = Utilities.Instance.NumberFormatInfo;
			if (inputs.Sockets.Any())
			{
				foreach (var inp in inputs.Sockets.Where(inp => !(inp is ClosureSocket)))
				{
					if (inp.ConnectionFrom != null) continue;

					cs.AppendFormat(nfi, " {0}.ins.{1}.Value = {2};", VariableName, inp.CodeName, inp);
				}
				cs.AppendLine();
			}

			cs.Append(CreateCodeAttributes());

			return cs.ToString();
		}

		public virtual string CreateConnectCode()
		{
			var sb = inputs.Sockets.Aggregate(new StringBuilder("", 1024), (current, inp) => current.Append($"{inp.ConnectCode}\n"));
			return sb.ToString();
		}

		public virtual string CreateAddToShaderCode(string shader)
		{
			var sb = new StringBuilder(1024);

			sb.Append($"{shader}.AddNode({VariableName});");

			return sb.ToString();
		}
	}
}
