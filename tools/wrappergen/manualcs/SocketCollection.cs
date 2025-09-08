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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ccl
{
	/// <summary>
	/// Base helper class for managing input and output sockets for shader nodes.
	/// </summary>
	public class SocketCollection
	{
		readonly List<ISocket> m_socketlist = new List<ISocket>();

		/// <summary>
		/// Add socket so it can actually be linked to and from.
		/// </summary>
		/// <param name="sock"></param>
		internal void AddSocket(ISocket sock)
		{
			m_socketlist.Add(sock);
		}

		static string XmlSocketName(string name, bool underscore)
		{
			return underscore ? name.Replace(" ", "_") : name.Replace(" ", "");
		}

		/// <summary>
		/// Get socket based on name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ISocket Socket(string name)
		{
			foreach (var socket in Sockets)
			{
				if (XmlSocketName(socket.UiName, false).ToLowerInvariant().Equals(name.ToLowerInvariant())) return socket;
				if (XmlSocketName(socket.UiName, true).ToLowerInvariant().Equals(name.ToLowerInvariant())) return socket;
			}

			throw new ArgumentException($"Socket {name} doesn't exist", nameof(name));
		}

		/// <summary>
		/// Iterate over the available property names
		/// </summary>
		public IEnumerable<string> PropertyNames
		{
			get
			{
				var p = TypeDescriptor.GetProperties(this);

				for (var i = 0; i < p.Count; i++)
				{
					if (p[i].Name.Equals("PropertyNames")) continue;

					yield return p[i].Name;
				}
			}
		}

		/// <summary>
		/// True if the requested property exists.
		/// 
		/// Case insensitive.
		/// </summary>
		/// <param name="n">UiName of property to search for</param>
		/// <returns></returns>
		public bool HasSocket(string n)
		{
			return PropertyNames.Any(pname => pname.ToLowerInvariant().Equals(n.ToLowerInvariant()));
		}

		/// <summary>
		/// Get socket from collection based on index.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		public ISocket this[int idx]
		{
			get
			{
				if (idx >= 0 && idx < m_socketlist.Count)
				{
					return m_socketlist[idx];
				}
				throw new IndexOutOfRangeException($"SocketCollection: {idx} not a valid index");
			}
		}

		/// <summary>
		/// Get an IEnumerable over sockets.
		/// </summary>
		public IEnumerable<ISocket> Sockets => m_socketlist;
	}
}
