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

    public class MathNodeInputs : Inputs
    {
        public BoolSocket UseClamp { get; private set; }
        public FloatSocket Value3 { get; private set; }
        public FloatSocket Value1 { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket Value2 { get; private set; }

        public MathNodeInputs(ShaderNode parentNode)
        {
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            Value3 = new FloatSocket(parentNode, "Value3", "value3", true);
            AddSocket(Value3);
            Value1 = new FloatSocket(parentNode, "Value1", "value1", true);
            AddSocket(Value1);
            Type = new EnumSocket(parentNode, "Type", "math_type", true);
            AddSocket(Type);
            Value2 = new FloatSocket(parentNode, "Value2", "value2", true);
            AddSocket(Value2);
        }
    }
    public class MathNodeOutputs : Outputs
    {
        public FloatSocket Value { get; private set; }

        public MathNodeOutputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
        }
    }

    [ShaderNode(name: "math")]
    public class MathNode : ShaderNode
    {
        public enum MathNodeType : uint {
            Add = ccl.NodeMathType.NODE_MATH_ADD,
            Subtract = ccl.NodeMathType.NODE_MATH_SUBTRACT,
            Multiply = ccl.NodeMathType.NODE_MATH_MULTIPLY,
            Divide = ccl.NodeMathType.NODE_MATH_DIVIDE,
            Sine = ccl.NodeMathType.NODE_MATH_SINE,
            Cosine = ccl.NodeMathType.NODE_MATH_COSINE,
            Tangent = ccl.NodeMathType.NODE_MATH_TANGENT,
            Arcsine = ccl.NodeMathType.NODE_MATH_ARCSINE,
            Arccosine = ccl.NodeMathType.NODE_MATH_ARCCOSINE,
            Arctangent = ccl.NodeMathType.NODE_MATH_ARCTANGENT,
            Power = ccl.NodeMathType.NODE_MATH_POWER,
            Logarithm = ccl.NodeMathType.NODE_MATH_LOGARITHM,
            Minimum = ccl.NodeMathType.NODE_MATH_MINIMUM,
            Maximum = ccl.NodeMathType.NODE_MATH_MAXIMUM,
            Round = ccl.NodeMathType.NODE_MATH_ROUND,
            LessThan = ccl.NodeMathType.NODE_MATH_LESS_THAN,
            GreaterThan = ccl.NodeMathType.NODE_MATH_GREATER_THAN,
            Modulo = ccl.NodeMathType.NODE_MATH_MODULO,
            Absolute = ccl.NodeMathType.NODE_MATH_ABSOLUTE,
            Arctan2 = ccl.NodeMathType.NODE_MATH_ARCTAN2,
            Floor = ccl.NodeMathType.NODE_MATH_FLOOR,
            Ceil = ccl.NodeMathType.NODE_MATH_CEIL,
            Fraction = ccl.NodeMathType.NODE_MATH_FRACTION,
            Sqrt = ccl.NodeMathType.NODE_MATH_SQRT,
            Inversesqrt = ccl.NodeMathType.NODE_MATH_INV_SQRT,
            Sign = ccl.NodeMathType.NODE_MATH_SIGN,
            Exponent = ccl.NodeMathType.NODE_MATH_EXPONENT,
            Radians = ccl.NodeMathType.NODE_MATH_RADIANS,
            Degrees = ccl.NodeMathType.NODE_MATH_DEGREES,
            Sinh = ccl.NodeMathType.NODE_MATH_SINH,
            Cosh = ccl.NodeMathType.NODE_MATH_COSH,
            Tanh = ccl.NodeMathType.NODE_MATH_TANH,
            Trunc = ccl.NodeMathType.NODE_MATH_TRUNC,
            Snap = ccl.NodeMathType.NODE_MATH_SNAP,
            Wrap = ccl.NodeMathType.NODE_MATH_WRAP,
            Compare = ccl.NodeMathType.NODE_MATH_COMPARE,
            MultiplyAdd = ccl.NodeMathType.NODE_MATH_MULTIPLY_ADD,
            Pingpong = ccl.NodeMathType.NODE_MATH_PINGPONG,
            Smoothmin = ccl.NodeMathType.NODE_MATH_SMOOTH_MIN,
            Smoothmax = ccl.NodeMathType.NODE_MATH_SMOOTH_MAX,
            FlooredModulo = ccl.NodeMathType.NODE_MATH_FLOORED_MODULO,
        }
        public MathNodeInputs ins => (MathNodeInputs)inputs;
        public MathNodeOutputs outs => (MathNodeOutputs)outputs;
        public MathNode(Shader shader) : this(shader, "a math node") { }

        public MathNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MathNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MathNodeInputs(this);
            outputs = new MathNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mathnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "value3":
                    /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value3', 'ui_name': 'Value3'} */
                    {
                    CSycles.mathnode_set_value3(this.Ptr, data);
                    }
                    break;
            case "value1":
                    /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value1', 'ui_name': 'Value1'} */
                    {
                    CSycles.mathnode_set_value1(this.Ptr, data);
                    }
                    break;
            case "value2":
                    /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value2', 'ui_name': 'Value2'} */
                    {
                    CSycles.mathnode_set_value2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mathnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mathnode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "math_type":
                    /* mathnode . {'datatype': 'ENUM', 'default_value': 'NODE_MATH_ADD', 'default_value_type': 'NodeMathType', 'is_input': True, 'member_name': 'math_type', 'ui_name': 'Type'} */
                    {
                    CSycles.mathnode_set_math_type(this.Ptr, (ccl.NodeMathType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "value3":
                /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value3', 'ui_name': 'Value3'} */
                {
                    return CSycles.mathnode_get_value3(this.Ptr);
                }
            case "value1":
                /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value1', 'ui_name': 'Value1'} */
                {
                    return CSycles.mathnode_get_value1(this.Ptr);
                }
            case "value2":
                /* mathnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value2', 'ui_name': 'Value2'} */
                {
                    return CSycles.mathnode_get_value2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mathnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mathnode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "math_type":
                /* mathnode . {'datatype': 'ENUM', 'default_value': 'NODE_MATH_ADD', 'default_value_type': 'NodeMathType', 'is_input': True, 'member_name': 'math_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.mathnode_get_math_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MathNode (getter)");
            }
        }

#endregion
    }
    /* code from MathNode.cs.post */

    [ShaderNode("math_add")]
    public class MathAdd : MathNode
    {
        public MathAdd(Shader shader) : this(shader, "an add mathnode") { }
        public MathAdd(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Add; }
        internal MathAdd(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Add; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_subtract")]
    public class MathSubtract : MathNode
    {
        public MathSubtract(Shader shader) : this(shader, "an subtract mathnode") { }
        public MathSubtract(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Subtract; }
        internal MathSubtract(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Subtract; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_multiply")]
    public class MathMultiply : MathNode
    {
        public MathMultiply(Shader shader) : this(shader, "an multiply mathnode") { }
        public MathMultiply(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Multiply; }
        internal MathMultiply(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Multiply; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_multiply_add")]
    public class MathMultiplyAdd : MathNode
    {
        public MathMultiplyAdd(Shader shader) : this(shader, "an multiply add mathnode") { }
        public MathMultiplyAdd(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.MultiplyAdd; }
        internal MathMultiplyAdd(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.MultiplyAdd; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_divide")]
    public class MathDivide : MathNode
    {
        public MathDivide(Shader shader) : this(shader, "an divide mathnode") { }
        public MathDivide(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Divide; }
        internal MathDivide(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Divide; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_sine")]
    public class MathSine : MathNode
    {
        public MathSine(Shader shader) : this(shader, "an sine mathnode") { }
        public MathSine(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Sine; }
        internal MathSine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Sine; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_cosine")]
    public class MathCosine : MathNode
    {
        public MathCosine(Shader shader) : this(shader, "an cosine mathnode") { }
        public MathCosine(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Cosine; }
        internal MathCosine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Cosine; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_tangent")]
    public class MathTangent : MathNode
    {
        public MathTangent(Shader shader) : this(shader, "an tangent mathnode") { }
        public MathTangent(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Tangent; }
        internal MathTangent(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Tangent; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_sinh")]
    public class MathSinh : MathNode
    {
        public MathSinh(Shader shader) : this(shader, "an sinh mathnode") { }
        public MathSinh(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Sinh; }
        internal MathSinh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Sinh; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_cosh")]
    public class MathCosh : MathNode
    {
        public MathCosh(Shader shader) : this(shader, "an cosh mathnode") { }
        public MathCosh(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Cosh; }
        internal MathCosh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Cosh; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_tanh")]
    public class MathTanh : MathNode
    {
        public MathTanh(Shader shader) : this(shader, "an tanh mathnode") { }
        public MathTanh(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Tanh; }
        internal MathTanh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Tanh; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_arcsine")]
    public class MathArcsine : MathNode
    {
        public MathArcsine(Shader shader) : this(shader, "an arcsine mathnode") { }
        public MathArcsine(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Arcsine; }
        internal MathArcsine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Arcsine; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_arccosine")]
    public class MathArccosine : MathNode
    {
        public MathArccosine(Shader shader) : this(shader, "an arccosine mathnode") { }
        public MathArccosine(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Arccosine; }
        internal MathArccosine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Arccosine; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_arctangent")]
    public class MathArctangent : MathNode
    {
        public MathArctangent(Shader shader) : this(shader, "an arctangent mathnode") { }
        public MathArctangent(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Arctangent; }
        internal MathArctangent(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Arctangent; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_power")]
    public class MathPower : MathNode
    {
        public MathPower(Shader shader) : this(shader, "an power mathnode") { }
        public MathPower(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Power; }
        internal MathPower(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Power; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_logarithm")]
    public class MathLogarithm : MathNode
    {
        public MathLogarithm(Shader shader) : this(shader, "an logarithm mathnode") { }
        public MathLogarithm(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Logarithm; }
        internal MathLogarithm(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Logarithm; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_minimum")]
    public class MathMinimum : MathNode
    {
        public MathMinimum(Shader shader) : this(shader, "an minimum mathnode") { }
        public MathMinimum(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Minimum; }
        internal MathMinimum(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Minimum; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_maximum")]
    public class MathMaximum : MathNode
    {
        public MathMaximum(Shader shader) : this(shader, "an maximum mathnode") { }
        public MathMaximum(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Maximum; }
        internal MathMaximum(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Maximum; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_round")]
    public class MathRound : MathNode
    {
        public MathRound(Shader shader) : this(shader, "an round mathnode") { }
        public MathRound(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Round; }
        internal MathRound(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Round; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_lessthan")]
    public class MathLess_Than : MathNode
    {
        public MathLess_Than(Shader shader) : this(shader, "an lessthan mathnode") { }
        public MathLess_Than(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.LessThan; }
        internal MathLess_Than(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.LessThan; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_greaterthan")]
    public class MathGreater_Than : MathNode
    {
        public MathGreater_Than(Shader shader) : this(shader, "an greaterthan mathnode") { }
        public MathGreater_Than(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.GreaterThan; }
        internal MathGreater_Than(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.GreaterThan; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_modulo")]
    public class MathModulo : MathNode
    {
        public MathModulo(Shader shader) : this(shader, "an modulo mathnode") { }
        public MathModulo(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Modulo; }
        internal MathModulo(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Modulo; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_absolute")]
    public class MathAbsolute : MathNode
    {
        public MathAbsolute(Shader shader) : this(shader, "an absolute mathnode") { }
        public MathAbsolute(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Absolute; }
        internal MathAbsolute(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Absolute; }
        public override string ShaderNodeTypeName => "math";
    }
    [ShaderNode("math_compare")]
    public class MathCompare : MathNode
    {
        public MathCompare(Shader shader) : this(shader, "an compare mathnode") { }
        public MathCompare(Shader shader, string name) : base(shader, name) { ins.Type.Value = MathNode.MathNodeType.Compare; }
        internal MathCompare(Shader shader, IntPtr intPtr) : base(shader, intPtr) { ins.Type.Value = MathNode.MathNodeType.Compare; }
        public override string ShaderNodeTypeName => "math";
    }

    /* end manual post class code */
}