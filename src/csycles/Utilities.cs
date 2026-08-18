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

using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace cclext
{
  public static class Extensions
	{
		public static string FirstCharacterToLower(string str)
		{
			if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
				return str;
			return char.ToLowerInvariant(str[0]) + str.Substring(1);
		}
	}
}

namespace ccl
{

  public delegate void ConsoleWriteDelegate(string message);

	public class Utilities
	{
		public static Utilities g_utilities;

		public NumberFormatInfo NumberFormatInfo { get; set; }
		public Utilities()
		{
			NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
		}

		public static Utilities Instance => g_utilities ?? (g_utilities = new Utilities());

		private static ConsoleWriteDelegate m_consoleWriter = null;

		public static void RegisterConsoleWriter(ConsoleWriteDelegate consoleWriter)
		{
			m_consoleWriter = consoleWriter;
		}

		public static void ConsoleWrite(string message) {
			if(m_consoleWriter!=null) {
				m_consoleWriter(message);
			}
		}

		public float[] parse_floats(string floats)
		{
			floats = floats.Trim();
			floats = floats.Replace("  ", " ");
			floats = floats.Replace(",", "");
			var fs = floats.Split(' ');
			var realfloats = new float[fs.Length];
			for (var i = 0; i < fs.Length; i++)
			{
				realfloats[i] = float.Parse(fs[i], NumberFormatInfo);
			}

			return realfloats;
		}

		/// <summary>
		/// Get a transform from given float string
		/// </summary>
		/// <param name="transform">Transform to create new transform in</param>
		/// <param name="mat">string with 16 floats</param>
		/// <returns>true if a parsing was possible</returns>
		public bool get_transform(Transform transform, string mat)
		{
			if (string.IsNullOrWhiteSpace(mat)) return false;
			var matrix = parse_floats(mat);

			if (matrix.Length != 16) return false;

			transform.SetMatrix(matrix);

			return true;
		}

		/// <summary>
		/// Get a float4 from given string. This function creates a new float4
		/// </summary>
		/// <param name="f4">Will be assigned a new float4 when at least 3 floats are found</param>
		/// <param name="floats">String with 3 or more floats. At most 4 will be used</param>
		/// <returns>true if a parsing was possible</returns>
		public bool get_float4(float4 f4, string floats)
		{
			//f4 = new float4(0.0f);
			if (string.IsNullOrEmpty(floats)) return false;

			var vec = parse_floats(floats);
			if (vec.Length < 3) return false;

			f4.x = vec[0];
			f4.y = vec[1];
			f4.z = vec[2];

			if (vec.Length >= 4)
				f4.w = vec[3];
			return true;
		}

		/// <summary>
		/// Set the Value float4 for a Float4Socket from given string. This function creates a new float4
		/// </summary>
		/// <param name="socket">Will be assigned a new float4 in Value when at least 3 floats are found</param>
		/// <param name="floats">String with 3 or more floats. At most 4 will be used</param>
		public void get_float4(Float4Socket socket, string floats)
		{
			if (string.IsNullOrEmpty(floats)) return;

			var vec = parse_floats(floats);
			if (vec.Length < 3) return;

			socket.Value = new float4(vec[0], vec[1], vec[2]);
			if (vec.Length >= 4)
				socket.Value.w = vec[3];
		}
		public void get_float4(Float4Socket socket, XmlReader node)
		{
			var floats = node.GetAttribute(socket.XmlName);
			if (string.IsNullOrEmpty(floats)) return;

			var vec = parse_floats(floats);
			if (vec.Length < 3) return;

			socket.Value = new float4(vec[0], vec[1], vec[2]);
			if (vec.Length >= 4)
				socket.Value.w = vec[3];
		}

		/// <summary>
		/// Set the Value float for a FloatSocket from given string.
		/// </summary>
		/// <param name="socket">socket for which the Value will be set</param>
		/// <param name="nr">float string</param>
		public void get_float(FloatSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = float.Parse(nr, NumberFormatInfo);
		}
		public void get_float(FloatSocket socket, XmlReader node)
		{
			var nr = node.GetAttribute(socket.XmlName);
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = float.Parse(nr, NumberFormatInfo);
		}

		/// <summary>
		/// Set the Value float for a FloatSocket from given string.
		/// </summary>
		/// <param name="val">float to set</param>
		/// <param name="floatstring">float string</param>
		public bool get_float(ref float val, string floatstring)
		{
			if (string.IsNullOrEmpty(floatstring)) return false;

			val = float.Parse(floatstring, NumberFormatInfo);
			return true;
		}

		/// <summary>
		/// Set the Value int for an IntSocket from given string
		/// </summary>
		/// <param name="socket">IntSocket for wich Value is set</param>
		/// <param name="nr">int string</param>
		public void get_int(IntSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = int.Parse(nr);
		}

		public int[] parse_ints(string ints)
		{
			ints = ints.Trim();
			ints = ints.Replace("  ", " ");
			ints = ints.Replace(",", "");
			var fs = ints.Split(' ');
			var realints = new int[fs.Length];
			for (var i = 0; i < fs.Length; i++)
			{
				realints[i] = int.Parse(fs[i]);
			}

			return realints;
		}

		public bool get_bool(ref bool val, string booleanstring)
		{
			if (string.IsNullOrEmpty(booleanstring))
			{
				val = false;
				return false;
			}

			val = bool.Parse(booleanstring);
			return true;
		}

		public bool get_int(ref int val, string intstring)
		{
			if (string.IsNullOrEmpty(intstring)) return false;

			val = int.Parse(intstring);
			return true;
		}

		public bool read_string(ref string val, string stringstring)
		{
			if (string.IsNullOrEmpty(stringstring)) return false;

			val = stringstring;
			return true;
		}


		public void ReadNodeGraph(Shader shader, XmlReader xmlNode, bool finalize)
		{
			var nodes = new Dictionary<string, ShaderNode> {{"output", shader.Output}};

			while (xmlNode.Read())
			{
				ShaderNode shader_node = null;
				if (!xmlNode.IsStartElement()) continue;
				var nodename = xmlNode.GetAttribute("name");
				if (string.IsNullOrEmpty(nodename) && xmlNode.Name != "connect") continue;

				if (string.IsNullOrEmpty(nodename)) nodename = "";

				switch (xmlNode.Name)
				{
					case "connect":
						var fromstring = xmlNode.GetAttribute("from");
						var tostring = xmlNode.GetAttribute("to");
						if (fromstring != null && tostring != null)
						{
							var from = fromstring.Split(' ');
							var to = tostring.Split(' ');

							if (!nodes.ContainsKey(from[0]))
								throw new KeyNotFoundException($"'from' node [{@from[0]}] not defined prior to connection.");
							var fromnode = nodes[from[0]];
							var fromsocket = fromnode.outputs.Socket(from[1]);

							if (!nodes.ContainsKey(to[0]))
								throw new KeyNotFoundException($"'to' node [{to[0]}] not defined prior to connection.");
							var tonode = nodes[to[0]];
							var tosocket = tonode.inputs.Socket(to[1]);

							fromsocket.Connect(tosocket);
						}
						break;
					default:
						shader_node = CSycles.CreateShaderNode(shader, xmlNode.Name, nodename);
						break;
				}
				if (shader_node != null)
				{
					shader_node.ParseXml(xmlNode);
					nodes.Add(nodename, shader_node);
					shader.AddNode(shader_node);
				}
			}

			if(finalize) shader.WriteDataToNodes();
		}
	}
}
