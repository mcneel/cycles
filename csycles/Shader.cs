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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ccl
{
	/// <summary>
	/// Representation of a Cycles shader
	/// </summary>
	public class Shader : IDisposable
	{
		/// <summary>
		/// Get the ID for this shader. This ID is given by CCycles
		/// </summary>
		public IntPtr Id { get; }
		private Scene Scene { get; }

		/// <summary>
		/// Print out debug information if set to true. Default false.
		/// </summary>
		public bool Verbose { get; set; }

		/// <summary>
		/// Get the output node for this shader.
		/// </summary>
		public OutputNode Output { get; internal set; }

		/// <summary>
		/// Create a new shader for session.
		/// </summary>
		/// <param name="session">Session ID for C[CS]ycles API.</param>
		/// <param name="type">The type of shader to create</param>
		public Shader(Scene scene)
		{
			Scene = scene;
			Id = CSycles.create_shader(scene.Id);
			CommonConstructor();
		}

		/// <summary>
		/// Create front for existing shader. Note that
		/// functionality through this can be limited.
		/// Generally used only for interal matters
		/// </summary>
		/// <param name="session"></param>
		/// <param name="type"></param>
		/// <param name="id"></param>
		internal Shader(Scene scene, IntPtr id)
		{
			Scene = scene;
			Id = id;
			CommonConstructor();
		}

		private void CommonConstructor()
		{
			int nodeCount = CSycles.shader_node_count(Id);
			for (int i = 0; i < nodeCount; i++)
			{
				IntPtr shn = CSycles.shader_node_get(Id, i);
				string name = CSycles.shadernode_get_name(shn);
				ShaderNode n = CSycles.CreateShaderNode(this, shn, name);
				AddNode(n);
			}
			Verbose = false;
		}

		public void DumpGraph(string filename)
		{
			CSycles.shader_dump_graph(Id, filename);
		}

		/*
		/// <summary>
		/// Create a shader outside of the Cycles system. Can be used to set up a
		/// shader graph for serialisation purposes.
		/// </summary>
		/// <param name="type"></param>
		public Shader(ShaderType type)
		{
			Type = type;
			Output = new OutputNode();
		}
		*/

		/// <summary>
		/// Clear the shader graph for this node, so it can be repopulated.
		/// </summary>
		public virtual void Recreate()
		{
			CSycles.shader_new_graph(Id);

			m_nodes.Clear();
			CommonConstructor();
		}

		/// <summary>
		/// Tag shader for device update
		/// </summary>
		public virtual void Tag()
		{
			Tag(true);
		}
		public virtual void Tag(bool use)
		{
			CSycles.scene_tag_shader(Scene.Id, Id, use);
		}

		readonly internal List<ShaderNode> m_nodes = new List<ShaderNode>();
		/// <summary>
		/// Add a ShaderNode to the shader. This will create the node in Cycles, set
		/// any values for sockets and direct members.
		/// </summary>
		/// <param name="node">ShaderNode to add</param>
		public virtual void AddNode(ShaderNode node)
		{
			m_nodes.Add(node);
		}

		public void CreateNode(string nodeTypeName)
		{
			// XXXX
		}

		/// <summary>
		/// Finalizes the graph by connecting all sockets in Cycles as specified
		/// through code.
		///
		/// This step also commits any values set to input sockets, enumerations
		/// and direct member variables.
		/// </summary>
		public virtual void WriteDataToNodes()
		{
			foreach (var node in m_nodes)
			{
				/* set enumerations */
				node.SetEnums();

				/* set direct member variables */
				node.SetDirectMembers();

				if (node.inputs == null) continue;

				node.SetSockets();
			}
		}

		/// <summary>
		/// Make the actual connection between nodes.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="fromout"></param>
		/// <param name="to"></param>
		/// <param name="toin"></param>
		public bool Connect(ShaderNode from, string fromout, ShaderNode to, string toin)
		{
			return CSycles.shader_connect_nodes(Id, from.Id, fromout, to.Id, toin);
		}

		/// <summary>
		/// Disconnect this node.
		/// </summary>
		/// <param name="node"></param>
		public void Disconnect(ShaderNode from, string fromout)
		{
			CSycles.shader_disconnect_node(Id, from.Id, fromout);
		}

		/// <summary>
		/// Set the name of the Shader
		/// </summary>
		public string Name
		{
			set
			{
				CSycles.shader_set_name(Id, value);
			}
			get
			{
				return CSycles.shader_get_name(Id);
			}
		}

		/// <summary>
		/// Set to true if multiple importance sampling is to be used
		/// </summary>
		public bool UseMis
		{
			set
			{
				if (Scene != null) CSycles.shader_set_use_mis(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set or get the pass index, used for the Material ID pass
		/// </summary>
		public int PassId
		{
			set
			{
				CSycles.shader_set_pass_id(Id, value);
			}
			get
			{
				return CSycles.shader_get_pass_id(Id);
			}
		}

		/// <summary>
		/// Set to true if this shader supports transparent shadows.
		/// </summary>
		public bool UseTransparentShadow
		{
			set
			{
				if (Scene != null) CSycles.shader_set_use_transparent_shadow(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this shader is used for a heterogeneous volume.
		/// </summary>
		public bool HeterogeneousVolume
		{
			set
			{
				if (Scene != null) CSycles.shader_set_heterogeneous_volume(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Create node graph in the given shader from the passed XML.
		///
		/// Note that you should call FinalizeConstructor on shader if you are not further
		/// changing the graph.
		/// </summary>
		/// <param name="shader">Shader to populate with nodes from the XML representation.</param>
		/// <param name="shaderXml">The XML representation for the shader.</param>
		/// <param name="finalize">Set to true if the shader should be finalized.</param>
		public static void ShaderFromXml(Shader shader, string shaderXml, bool finalize)
		{
			var xmlmem = Encoding.UTF8.GetBytes(shaderXml);
			using (var xmlstream = new MemoryStream(xmlmem))
			{
				var settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Fragment,
					IgnoreComments = true,
					IgnoreProcessingInstructions = true,
					IgnoreWhitespace = true
				};
				var reader = XmlReader.Create(xmlstream, settings);
				Utilities.Instance.ReadNodeGraph(shader, reader, finalize);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			// C# no longer owns shaders at any stage. No disposing of
			// unmanaged resources
		}

		public void Dispose()
		{
			// C# no longer owns shaders at any stage. No disposing of
			// unmanaged resources
		}
	}
}
