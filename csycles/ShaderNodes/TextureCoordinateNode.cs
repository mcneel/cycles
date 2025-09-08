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

----------------------------------------------------------------------
NOTE: Do NOT modify this file directly, it is automatically generated.

Code generated at: 2025-12-02 03:24:08 UTC
----------------------------------------------------------------------

**/

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;
namespace ccl.ShaderNodes
{
    using cclext;

    public class TextureCoordinateNodeInputs : Inputs
    {
        public TransformSocket ObjectTransform { get; private set; }
        public BoolSocket UseTransform { get; private set; }
        public BoolSocket FromDupli { get; private set; }

        public TextureCoordinateNodeInputs(ShaderNode parentNode)
        {
            ObjectTransform = new TransformSocket(parentNode, "Object Transform", "ob_tfm", true);
            AddSocket(ObjectTransform);
            UseTransform = new BoolSocket(parentNode, "Use Transform", "use_transform", true);
            AddSocket(UseTransform);
            FromDupli = new BoolSocket(parentNode, "From Dupli", "from_dupli", true);
            AddSocket(FromDupli);
        }
    }
    public class TextureCoordinateNodeOutputs : Outputs
    {
        public PointSocket Window { get; private set; }
        public PointSocket Object { get; private set; }
        public NormalSocket Normal { get; private set; }
        public NormalSocket Reflection { get; private set; }
        public PointSocket Camera { get; private set; }
        public PointSocket UV { get; private set; }
        public PointSocket Generated { get; private set; }

        public TextureCoordinateNodeOutputs(ShaderNode parentNode)
        {
            Window = new PointSocket(parentNode, "Window", "window", false);
            AddSocket(Window);
            Object = new PointSocket(parentNode, "Object", "object", false);
            AddSocket(Object);
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
            Reflection = new NormalSocket(parentNode, "Reflection", "reflection", false);
            AddSocket(Reflection);
            Camera = new PointSocket(parentNode, "Camera", "camera", false);
            AddSocket(Camera);
            UV = new PointSocket(parentNode, "UV", "UV", false);
            AddSocket(UV);
            Generated = new PointSocket(parentNode, "Generated", "generated", false);
            AddSocket(Generated);
        }
    }

    [ShaderNode(name: "texture_coordinate")]
    public class TextureCoordinateNode : ShaderNode
    {
        public TextureCoordinateNodeInputs ins => (TextureCoordinateNodeInputs)inputs;
        public TextureCoordinateNodeOutputs outs => (TextureCoordinateNodeOutputs)outputs;
        public TextureCoordinateNode(Shader shader) : this(shader, "a texture_coordinate node") { }

        public TextureCoordinateNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal TextureCoordinateNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new TextureCoordinateNodeInputs(this);
            outputs = new TextureCoordinateNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.texturecoordinatenode_get_node_type();
        }
#region Setters

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_transform":
                    /* texturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transform', 'ui_name': 'Use Transform'} */
                    {
                    CSycles.texturecoordinatenode_set_use_transform(this.Ptr, data);
                    }
                    break;
            case "from_dupli":
                    /* texturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'From Dupli'} */
                    {
                    CSycles.texturecoordinatenode_set_from_dupli(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TextureCoordinateNode (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "ob_tfm":
                    /* texturecoordinatenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ob_tfm', 'ui_name': 'Object Transform'} */
                    {
                    CSycles.texturecoordinatenode_set_ob_tfm(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TextureCoordinateNode (setter)");
            }
        }

#endregion
#region Getters

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_transform":
                /* texturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transform', 'ui_name': 'Use Transform'} */
                {
                    return CSycles.texturecoordinatenode_get_use_transform(this.Ptr);
                }
            case "from_dupli":
                /* texturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'From Dupli'} */
                {
                    return CSycles.texturecoordinatenode_get_from_dupli(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TextureCoordinateNode (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "ob_tfm":
                /* texturecoordinatenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ob_tfm', 'ui_name': 'Object Transform'} */
                {
                    return CSycles.texturecoordinatenode_get_ob_tfm(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TextureCoordinateNode (getter)");
            }
        }

#endregion
    }

}