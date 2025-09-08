/**
Copyright 2014-2025 Robert McNeel and Associates

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
using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ccl.ShaderNodes
{
    using cclext;
    public class ShaderNode
    {
        /// <summary>
        /// Set a NodeTypeName for this node
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Get NodeTypeName that can be used as variable NodeTypeName
        /// </summary>
        public virtual string VariableName
        {
            get
            {
                var s = $"{Name}{Ptr}";
                return Extensions.FirstCharacterToLower(s);
            }
        }

        /// <summary>
        /// Get the node ID. This is set when created in Cycles.
        /// </summary>
        public IntPtr Ptr { get; internal set; }

        /// <summary>
        /// Get the XML NodeTypeName of the node type as string.
        /// </summary>
        virtual public string NodeTypeName
        {
            get
            {
                var t = GetType();
                var attr = t.GetCustomAttributes(typeof(NodeAttribute), false)[0] as NodeAttribute;
                return attr.Name;
            }
        }

        public string NodeTypeCodeName
        {
            get
            {
                var t = GetType();
                return t.Name;
            }
        }
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
        internal ShaderNode(Shader shader, string name)
        {
            ConstructShaderNode(shader, ShaderNodeTypeName, name);
        }

        internal ShaderNode(Shader shader, IntPtr shadernodePtr)
        {
            Ptr = shadernodePtr;
            Shader = shader;
        }

        public Shader Shader { get; private set; }
        internal void ConstructShaderNode(Shader shader, string shaderNodeTypeName, string shaderNodeName)
        {
            Ptr = CSycles.add_shader_node(shader.Ptr, shaderNodeTypeName, shaderNodeName);
            Shader = shader;
        }

        internal virtual void SetString(string name, string data) {}
        internal virtual void SetFloat(string name, float data) {}
        internal virtual void SetNormal(string name, float3 data) {}
        internal virtual void SetVector(string name, float3 data) {}
        internal virtual void SetColor(string name, float3 data) {}
        internal virtual void SetPoint(string name, float3 data) {}
        internal virtual void SetPoint2(string name, float2 data) {}
        internal virtual void SetBool(string name, bool data) {}
        internal virtual void SetInt(string name, int data) {}
        internal virtual void SetUint(string name, uint data) {}
        internal virtual void SetUint64(string name, ulong data) {}
        internal virtual void SetEnum(string name, object data) {}
        internal virtual void SetNode(string name, IntPtr data) {}
        internal virtual void SetTransform(string name, Transform data) {}
        internal virtual void SetIntArray(string name, List<int> data) {}
        internal virtual void SetFloatArray(string name, List<float> data) {}
        internal virtual void SetBooleanArray(string name, List<bool> data) {}
        internal virtual void SetColorArray(string name, List<float3> data) {}
        internal virtual void SetPointArray(string name, List<float3> data) {}
        internal virtual void SetVectorArray(string name, List<float3> data) {}
        internal virtual void SetPoint2Array(string name, List<float2> data) {}
        internal virtual void SetTransformArray(string name, List<Transform> data) {}

        internal virtual string GetString(string name) { return ""; }
        internal virtual float GetFloat(string name) { return float.MinValue; }
        internal virtual float3 GetNormal(string name) { return new float3(0.0f, 0.0f, 0.0f); }
        internal virtual float3 GetVector(string name) { return new float3(0.0f, 0.0f, 0.0f); }
        internal virtual float3 GetColor(string name) { return new float3(0.0f, 0.0f, 0.0f); }
        internal virtual float3 GetPoint(string name) { return new float3(0.0f, 0.0f, 0.0f); }
        internal virtual float2 GetPoint2(string name) { return new float2(0.0f, 0.0f); }
        internal virtual bool GetBool(string name) { return false; }
        internal virtual int GetInt(string name) { return int.MinValue; }
        internal virtual uint GetUint(string name) { return uint.MaxValue;}
        internal virtual ulong GetUint64(string name) { return ulong.MaxValue;}
        internal virtual object GetEnum(string name) { return 0;}
        internal virtual IntPtr GetNode(string name) { return IntPtr.Zero;}
        internal virtual ccl.Transform GetTransform(string name) { return ccl.Transform.Identity(); }
        internal virtual List<int> GetIntArray(string name) { return new List<int>(); }
        internal virtual List<float> GetFloatArray(string name) { return new List<float>(); }
        internal virtual List<bool> GetBooleanArray(string name) { return new List<bool>(); }
        internal virtual List<float3> GetColorArray(string name) { return new List<float3>(); }
        internal virtual List<float3> GetPointArray(string name) { return new List<float3>(); }
        internal virtual List<float3> GetVectorArray(string name) { return new List<float3>(); }
        internal virtual List<float2> GetPoint2Array(string name) { return new List<float2>(); }
        internal virtual List<Transform> GetTransformArray(string name) { return new List<Transform>(); }
    }
}
