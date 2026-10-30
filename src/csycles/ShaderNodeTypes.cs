/**
Copyright 2026 Robert McNeel and Associates

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

using System;
using System.Collections.Generic;

namespace ccl
{
	/// <summary>
	/// A socket of a shader node type, as Cycles registers it.
	/// </summary>
	public sealed class ShaderNodeSocketInfo
	{
		/* Cycles SocketType::Flags */
		private const int LinkableFlag = 1 << 0;
		private const int InternalFlags = (1 << 2) | (1 << 3);

		internal ShaderNodeSocketInfo(string name, string uiName, string type, int flags, string defaultValue, KeyValuePair<string, int>[] enumValues)
		{
			Name = name;
			UiName = uiName;
			Type = type;
			Flags = flags;
			Default = defaultValue;
			EnumValues = enumValues;
		}

		/// <summary>
		/// Socket name as XML and the C API use it, e.g. "base_color".
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Display name, e.g. "Base Color".
		/// </summary>
		public string UiName { get; }

		/// <summary>
		/// Cycles socket type name, e.g. "float", "color", "vector", "closure" or "enum".
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// Cycles SocketType flags.
		/// </summary>
		public int Flags { get; }

		/// <summary>
		/// True if the socket can be connected; false for settings such as enums.
		/// </summary>
		public bool IsLinkable => (Flags & LinkableFlag) != 0;

		/// <summary>
		/// True for sockets only Cycles itself uses; keep them out of a UI.
		/// </summary>
		public bool IsInternal => (Flags & InternalFlags) != 0;

		/// <summary>
		/// Default value in XML attribute form. Empty for outputs, closures and arrays.
		/// </summary>
		public string Default { get; }

		/// <summary>
		/// Enum value names and numbers, sorted by number. Empty unless Type is "enum".
		/// </summary>
		public IReadOnlyList<KeyValuePair<string, int>> EnumValues { get; }

		public override string ToString()
		{
			return $"{Name} ({Type})";
		}
	}

	/// <summary>
	/// A shader node type with its sockets, as Cycles registers it.
	/// </summary>
	public sealed class ShaderNodeTypeInfo
	{
		internal ShaderNodeTypeInfo(string name, ShaderNodeSocketInfo[] inputs, ShaderNodeSocketInfo[] outputs)
		{
			Name = name;
			Inputs = inputs;
			Outputs = outputs;
		}

		/// <summary>
		/// Node type name as XML and the C API use it, e.g. "principled_bsdf".
		/// </summary>
		public string Name { get; }

		public IReadOnlyList<ShaderNodeSocketInfo> Inputs { get; }

		public IReadOnlyList<ShaderNodeSocketInfo> Outputs { get; }

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// Every shader node type Cycles can create, read once from its node registry.
	/// Needs ccycles to be loadable, but no session or device.
	/// </summary>
	public static class ShaderNodeTypes
	{
		private static readonly Lazy<ShaderNodeTypeInfo[]> g_all = new Lazy<ShaderNodeTypeInfo[]>(Load);
		private static readonly Lazy<Dictionary<string, ShaderNodeTypeInfo>> g_by_name =
			new Lazy<Dictionary<string, ShaderNodeTypeInfo>>(() =>
			{
				var byName = new Dictionary<string, ShaderNodeTypeInfo>(StringComparer.Ordinal);
				foreach (var nodeType in g_all.Value)
					byName[nodeType.Name] = nodeType;
				return byName;
			});

		/// <summary>
		/// All shader node types, sorted by name.
		/// </summary>
		public static IReadOnlyList<ShaderNodeTypeInfo> All => g_all.Value;

		/// <summary>
		/// Find a shader node type by name, or null.
		/// </summary>
		public static ShaderNodeTypeInfo Find(string name)
		{
			if (name == null) return null;
			return g_by_name.Value.TryGetValue(name, out var nodeType) ? nodeType : null;
		}

		private static ShaderNodeTypeInfo[] Load()
		{
			var count = CSycles.shader_node_type_count();
			var nodeTypes = new ShaderNodeTypeInfo[count];
			for (var i = 0; i < count; i++)
			{
				var name = CSycles.shader_node_type_name(i);
				nodeTypes[i] = new ShaderNodeTypeInfo(name, LoadSockets(name, false), LoadSockets(name, true));
			}
			return nodeTypes;
		}

		private static ShaderNodeSocketInfo[] LoadSockets(string nodeType, bool output)
		{
			var count = Math.Max(0, CSycles.shader_node_type_socket_count(nodeType, output));
			var sockets = new List<ShaderNodeSocketInfo>(count);
			for (var i = 0; i < count; i++)
			{
				if (!CSycles.shader_node_type_socket(nodeType, output, i, out var name, out var uiName, out var type, out var flags))
					continue;

				var defaultValue = output ? string.Empty : CSycles.shader_node_type_socket_default(nodeType, i);

				var enumCount = output ? 0 : CSycles.shader_node_type_socket_enum_count(nodeType, i);
				var enumValues = new List<KeyValuePair<string, int>>(enumCount);
				for (var e = 0; e < enumCount; e++)
				{
					if (CSycles.shader_node_type_socket_enum(nodeType, i, e, out var enumName, out var enumValue))
						enumValues.Add(new KeyValuePair<string, int>(enumName, enumValue));
				}

				sockets.Add(new ShaderNodeSocketInfo(name, uiName, type, flags, defaultValue, enumValues.ToArray()));
			}
			return sockets.ToArray();
		}
	}
}
