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
namespace ccl.ShaderNodes.Sockets
{

  /// <summary>
  /// Interface for Socket implementations.
  /// </summary>
  public interface ISocket
	{
		/// <summary>
		/// Get or set the UI Name for this socket.
		/// </summary>
		string UiName { get; set; }
		/// <summary>
		/// True when Cycles no longer has this socket. The socket stays on the C# node
		/// so callers keep compiling, but nothing is pushed to Cycles and connecting it
		/// is a no-op rather than a failed lookup.
		/// </summary>
		bool Retired { get; }
		/// <summary>
		/// Get or set the Internal Name for this socket.
		/// </summary>
		string InternalName { get; set; }

		/// <summary>
		/// Get the XML-valid uiName for this socket.
		/// </summary>
		string XmlName {get;}

		/// <summary>
		/// Get the XML-valid connect tag for this socket.
		/// </summary>
		string ConnectTag{get;}
		/// <summary>
		/// Get the code uiName for this socket.
		/// </summary>
		string CodeName {get;}
		/// <summary>
		/// Get the C# code for this socket connection.
		/// </summary>
		string ConnectCode {get;}
		/// <summary>
		/// Connect this socket to the given socket.
		/// </summary>
		void Connect(ISocket to);
		/// <summary>
		/// The connection from which is connected to this socket.
		/// </summary>
		ISocket ConnectionFrom { get; set; }
		/// <summary>
		/// The parent node for this socket.
		/// </summary>
		ShaderNode Parent { get; set; }
		/// <summary>
		/// A path to this socket.
		/// </summary>
		string Path {get;}
		/// <summary>
		/// Clear any existing connections.
		/// </summary>
		void ClearConnections();
		/// <summary>
		/// Set value code.
		/// </summary>
		string SetValueCode { get; set; }
	}


	/// <summary>
	/// Generic base class for sockets.
	/// </summary>
	public class SocketBase<T> : ISocket
	{
		public T Value { get; set; }
		public ShaderNode Parent { get; set; }

		public string UiName { get; set; }
		public string InternalName { get; set; }

		/* Upstream retires sockets between releases - the 4.x principled BSDF rework
		 * dropped Subsurface Color and Transmission Roughness outright. RhinoCycles
		 * still assigns them, so removing the properties would break a lot of calling
		 * code for no gain. Marking them retired keeps that code compiling and makes
		 * both the value push and the connect skip them, instead of asking Cycles for
		 * a socket that is not there. */
		public bool Retired { get; internal set; }

		public string XmlName => UiName.Replace(' ', '_').ToLowerInvariant();
		public string CodeName => UiName.Replace(" ", string.Empty);

		public void Connect(ISocket to)
		{
			if (Retired || to.Retired)
			{
				return;
			}

#if DEBUG
			if (!
#endif
				Parent.Shader.Connect(Parent, UiName, to.Parent, to.UiName)
#if DEBUG
			)
			{
				/* Connect returns false for a socket that was already linked AND for a
				 * socket name that does not exist on the node, so naming only the first
				 * was actively misleading: a socket renamed by a Cycles upgrade reported
				 * itself as a double connection. ccycles prints which of the two it was,
				 * and to which socket, via OutputDebugString - so point at that rather
				 * than guessing here. */
				System.Diagnostics.Debug.Assert(false,
					$"Could not connect {Parent.Name}:{UiName} to {to.Parent.Name}:{to.UiName} - " +
					"either the target is already connected or one of those socket names does not " +
					"exist on the node. See the ccycles: line in the Output window for which.");
			}
#else
				;
#endif
				to.ConnectionFrom = this;
		}

		internal SocketBase(ShaderNode parentNode, string uiName, string internalName = "UNSET")
		{
			Parent = parentNode;
			UiName = uiName;
			InternalName = internalName;
		}

		public ISocket ConnectionFrom { get; set; }

		public string SetValueCode { get; set; }

		/// <summary>
		/// Get string containing node uiName, type and socket uiName
		/// </summary>
		public string Path => $"{Parent.Name}({Parent.ShaderNodeTypeName}):{UiName}";

		/// <summary>
		/// Get the C# connection code into this socket
		/// </summary>
		public string ConnectCode => ConnectionFrom != null ? $"{ConnectionFrom.Parent.VariableName}.outs.{ConnectionFrom.CodeName}.Connect({Parent.VariableName}.ins.{CodeName});" : "";

		public string ConnectTag => ConnectionFrom != null ? $"<connect to=\"{Parent.VariableName} {XmlName}\" from=\"{ConnectionFrom.Parent.VariableName} {ConnectionFrom.XmlName}\" />": "";

		/// <summary>
		/// Remove connections
		/// </summary>
		public void ClearConnections()
		{
			Parent.Shader.Disconnect(Parent, UiName);
		}

		public override string ToString()
		{
			return "";
		}
	}
}
