
using System;

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
        public T Value {
            get {
                Type t = this.GetType();

                if (typeof(BoolSocket) == t)
                {
                    return (dynamic)Parent.GetBool(InternalName);
                }
                else if (typeof(ColorSocket) == t)
                {
                    return (dynamic)Parent.GetColor(InternalName);
                }
                else if (typeof(ColorArraySocket) == t)
                {
                    return (dynamic)Parent.GetColorArray(InternalName);
                }
                else if (typeof(EnumSocket) == t)
                {
                    return (dynamic)Parent.GetEnum(InternalName);
                }
                else if(typeof(FloatSocket) == t)
                {
                    return (dynamic)Parent.GetFloat(InternalName);
                }
                else if (typeof(IntSocket) == t)
                {
                    return (dynamic)Parent.GetInt(InternalName);
                }
                else if (typeof(NormalSocket) == t)
                {
                    return (dynamic)Parent.GetNormal(InternalName);
                }
                else if (typeof(PointSocket) == t)
                {
                    return (dynamic)Parent.GetPoint(InternalName);
                }
                else if (typeof(PointArraySocket) == t)
                {
                    return (dynamic)Parent.GetPointArray(InternalName);
                }
                else if (typeof(StringSocket) == t)
                {
                    return (dynamic)Parent.GetString(InternalName);
                }
                else if (typeof(TransformSocket) == t)
                {
                    return (dynamic)Parent.GetTransform(InternalName);
                }
                else if (typeof(VectorSocket) == t)
                {
                    return (dynamic)Parent.GetVector(InternalName);
                }
                else
                {
                    throw new NotImplementedException($"Socket type {t.Name} not implemented in SocketBase Get Value");
                }
            }
            set
            {
                if(!IsInput) return;
                Type t = this.GetType();
                if (typeof(BoolSocket) == t)
                {
                    Parent.SetBool(InternalName, (dynamic)value);
                }
                else if (typeof(ColorSocket) == t)
                {
                    Parent.SetColor(InternalName, (dynamic)value);
                }
                else if (typeof(ColorArraySocket) == t)
                {
                    Parent.SetColorArray(InternalName, (dynamic)value);
                }
                else if (typeof(EnumSocket) == t)
                {
                    Parent.SetEnum(InternalName, (dynamic)value);
                }
                else if (typeof(FloatSocket) == t)
                {
                    Parent.SetFloat(InternalName, (dynamic)value);
                }
                else if (typeof(IntSocket) == t)
                {
                    Parent.SetInt(InternalName, (dynamic)value);
                }
                else if (typeof(NormalSocket) == t)
                {
                    Parent.SetNormal(InternalName, (dynamic)value);
                }
                else if (typeof(PointSocket) == t)
                {
                    Parent.SetPoint(InternalName, (dynamic)value);
                }
                else if (typeof(PointArraySocket) == t)
                {
                    Parent.SetPointArray(InternalName, (dynamic)value);
                }
                else if (typeof(StringSocket) == t)
                {
                    Parent.SetString(InternalName, (dynamic)value);
                }
                else if (typeof(TransformSocket) == t)
                {
                    Parent.SetTransform(InternalName, (dynamic)value);
                }
                else if (typeof(VectorSocket) == t)
                {
                    Parent.SetVector(InternalName, (dynamic)value);
                }
                else
                {
                    throw new NotImplementedException($"Socket type {t.Name} not implemented in SocketBase Set Value");
                }
            }
        }

        public ShaderNode Parent { get; set; }

        public string UiName { get; set; }
        public string InternalName { get; set; }

        public string XmlName => UiName.Replace(' ', '_').ToLowerInvariant();
        public string CodeName => UiName.Replace(" ", string.Empty);

        public void Connect(ISocket to)
        {
#if DEBUG
            if (!
#endif
                    Parent.Shader.Connect(Parent, UiName, to.Parent, to.UiName)
#if DEBUG
               )
            {
                System.Diagnostics.Debug.Assert(false, $"Trying to connect to {to.Parent.Name}:{to.UiName} which has already a connection");
            }
#else
            ;
#endif
            to.ConnectionFrom = this;
        }

        public bool IsInput { get; private set; }

        internal SocketBase(ShaderNode parentNode, string uiName, string internalName = "UNSET", bool isInput = true)
        {
            Parent = parentNode;
            UiName = uiName;
            InternalName = internalName;
            IsInput = isInput;
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
