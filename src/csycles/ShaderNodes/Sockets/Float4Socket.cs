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
	/// Float4Socket is used to communicate vectors between nodes.
	/// </summary>
	public class Float4Socket : SocketBase<float4>
	{
		/// <summary>
		/// Create socket for parentNode. The name has to correspond to the socket name in Cycles.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="name"></param>
		public Float4Socket(ShaderNode parentNode, string name, string internalname)
			: base(parentNode, name, internalname)
		{
			Value = new float4();
		}
		public override string ToString()
		{
			var nfi = Utilities.Instance.NumberFormatInfo;
			return $"new {SetValueCode ?? Value.ToString()}";
		}
	}

	public class ColorSocket : Float4Socket {
		public ColorSocket(ShaderNode parentNode, string name, string internalname)
			: base(parentNode, name, internalname)
		{
		}
	}
	public class VectorSocket : Float4Socket {
		public VectorSocket(ShaderNode parentNode, string name, string internalname) : base(parentNode, name, internalname)
		{
		}
	}
}
