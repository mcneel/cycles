
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

using ccl;
using ccl.ShaderNodes.Sockets;

namespace ccl.NodeSockets
{

    /// <summary>
    /// Interface for Socket implementations.
    /// </summary>
    public interface INodeSocket
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
        /// Get the code uiName for this socket.
        /// </summary>
        string CodeName {get;}
        /// <summary>
        /// The parent node for this socket.
        /// </summary>
        Node Parent { get; set; }
        /// <summary>
        /// A path to this socket.
        /// </summary>
        string Path {get;}
        /// <summary>
        /// Set value code.
        /// </summary>
        string SetValueCode { get; set; }
    }


    /// <summary>
    /// Generic base class for sockets.
    /// </summary>
    public class NodeSocketBase<T> : INodeSocket
    {
        public T Value {
            get {
                Type t = this.GetType();

                if (typeof(BoolNodeSocket) == t)
                {
                    return (dynamic)Parent.GetBool(InternalName);
                }
                else if (typeof(BooleanArrayNodeSocket) == t)
                {
                    return (dynamic)Parent.GetBooleanArray(InternalName);
                }
                else if (typeof(ClosureNodeSocket) == t)
                {
                    return (dynamic)Parent.GetClosure(InternalName);
                }
                else if (typeof(ColorNodeSocket) == t)
                {
                    return (dynamic)Parent.GetColor(InternalName);
                }
                else if (typeof(EnumNodeSocket) == t)
                {
                    return (dynamic)Parent.GetEnum(InternalName);
                }
                else if (typeof(FloatNodeSocket) == t)
                {
                    return (dynamic)Parent.GetFloat(InternalName);
                }
                else if (typeof(IntNodeSocket) == t)
                {
                    return (dynamic)Parent.GetInt(InternalName);
                }
                else if (typeof(NodeNodeSocket) == t)
                {
                    return (dynamic)Parent.GetNode(InternalName);
                }
                else if (typeof(NormalNodeSocket) == t)
                {
                    return (dynamic)Parent.GetNormal(InternalName);
                }
                else if (typeof(PointNodeSocket) == t)
                {
                    return (dynamic)Parent.GetPoint(InternalName);
                } else if (typeof(PointArrayNodeSocket) == t)
                {
                    return (dynamic)Parent.GetPointArray(InternalName);
                }
                else if (typeof(StringNodeSocket) == t)
                {
                    return (dynamic)Parent.GetString(InternalName);
                }
                else if (typeof(VectorNodeSocket) == t)
                {
                    return (dynamic)Parent.GetVector(InternalName);
                }
                else
                {
                    throw new NotImplementedException($"Socket type {t.Name} not implemented in NodeSocketBase Get Value");
                }
                //return default;
            }
            set
            {
                if(!IsInput) return;
                Type t = this.GetType();
                if (typeof(BoolNodeSocket) == t)
                {
                    Parent.SetBool(InternalName, (dynamic)value);
                }
                else if (typeof(BooleanArrayNodeSocket) == t)
                {
                    Parent.SetBooleanArray(InternalName, (dynamic)value);
                }
                else if (typeof(ColorNodeSocket) == t)
                {
                    Parent.SetColor(InternalName, (dynamic)value);
                }
                else if (typeof(EnumNodeSocket) == t)
                {
                    Parent.SetEnum(InternalName, (dynamic)value);
                }
                else if (typeof(FloatNodeSocket) == t)
                {
                    Parent.SetFloat(InternalName, (dynamic)value);
                }
                else if (typeof(FloatArrayNodeSocket) == t)
                {
                    Parent.SetFloatArray(InternalName, (dynamic)value);
                }
                else if (typeof(IntNodeSocket) == t)
                {
                    Parent.SetInt(InternalName, (dynamic)value);
                }
                else if (typeof(IntArrayNodeSocket) == t)
                {
                    Parent.SetIntArray(InternalName, (dynamic)value);
                }
                else if (typeof(NodeNodeSocket) == t)
                {
                    Parent.SetNode(InternalName, (dynamic)value);
                }
                else if (typeof(NormalNodeSocket) == t)
                {
                    Parent.SetNormal(InternalName, (dynamic)value);
                }
                else if (typeof(PointNodeSocket) == t)
                {
                    Parent.SetPoint(InternalName, (dynamic)value);
                }
                else if (typeof(PointArrayNodeSocket) == t)
                {
                    Parent.SetPointArray(InternalName, (dynamic)value);
                }
                else if (typeof(Point2NodeSocket) == t)
                {
                    Parent.SetPoint2(InternalName, (dynamic)value);
                }
                else if (typeof(Point2ArrayNodeSocket) == t)
                {
                    Parent.SetPoint2Array(InternalName, (dynamic)value);
                }
                else if (typeof(StringNodeSocket) == t)
                {
                    Parent.SetString(InternalName, (dynamic)value);
                }
                else if (typeof(TransformNodeSocket) == t)
                {
                    Parent.SetTransform(InternalName, (dynamic)value);
                }
                else if (typeof(TransformArrayNodeSocket) == t)
                {
                    Parent.SetTransformArray(InternalName, (dynamic)value);
                }
                else if (typeof(UintNodeSocket) == t)
                {
                    Parent.SetUint(InternalName, (dynamic)value);
                }
                else if (typeof(Uint64NodeSocket) == t)
                {
                    Parent.SetUint64(InternalName, (dynamic)value);
                }
                else if (typeof(VectorNodeSocket) == t)
                {
                    Parent.SetVector(InternalName, (dynamic)value);
                }
                else
                {
                    throw new NotImplementedException($"Socket type {t.Name} not implemented in NodeSocketBase Set Value");
                }
            }
        }

        public Node Parent { get; set; }

        public string UiName { get; set; }
        public string InternalName { get; set; }

        public string XmlName => UiName.Replace(' ', '_').ToLowerInvariant();
        public string CodeName => UiName.Replace(" ", string.Empty);

        public bool IsInput { get; private set; }


        internal NodeSocketBase(Node parentNode, string uiName, string internalName = "UNSET", bool isInput = true)
        {
            Parent = parentNode;
            UiName = uiName;
            InternalName = internalName;
            IsInput = isInput;
        }

        public string SetValueCode { get; set; }

        /// <summary>
        /// Get string containing node uiName, type and socket uiName
        /// </summary>
        public string Path => $"{Parent.Name}({Parent.NodeTypeName}):{UiName}";


        public override string ToString()
        {
            return "";
        }
    }
}
