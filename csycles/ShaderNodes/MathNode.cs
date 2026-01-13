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

using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using System;
using System.Text;
using System.Xml;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// MathNode input sockets
	/// </summary>
	public class MathInputs : Inputs
	{
		/// <summary>
		/// MathNode Value1 input socket
		/// </summary>
		public FloatSocket Value1 { get; set; }
		/// <summary>
		/// MathNode Value2 input socket
		/// </summary>
		public FloatSocket Value2 { get; set; }
		public FloatSocket Value3 { get; set; }

		/// <summary>
		/// Create MathNode input sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MathInputs(ShaderNode parentNode)
		{
			Value1 = new FloatSocket(parentNode, "Value1", "value1");
			AddSocket(Value1);
			Value2 = new FloatSocket(parentNode, "Value2", "value2");
			AddSocket(Value2);
			Value3 = new FloatSocket(parentNode, "Value2", "value3");
			AddSocket(Value3);
		}
	}

	/// <summary>
	/// MathNode output sockets
	/// </summary>
	public class MathOutputs : Outputs
	{
		/// <summary>
		/// The resulting value of the MathNode operation
		/// </summary>
		public FloatSocket Value { get; set; }

		/// <summary>
		/// Create MathNode output sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MathOutputs(ShaderNode parentNode)
		{
			Value = new FloatSocket(parentNode, "Value", "value");
			AddSocket(Value);
		}
	}

	/// <summary>
	/// Add a Math node, setting output Value with any of the following <c>Operation</c>s using Value1 and Value2
	///
	/// Note that some operations use only Value1
	/// </summary>
	[ShaderNode("math")]
	public class MathNode : ShaderNode
	{
		/// <summary>
		/// Math operations the MathNode can do.
		/// </summary>
		public enum Operations
		{
			/// <summary>
			/// Value = Value1 + Value2
			/// </summary>
			Add,
			/// <summary>
			/// Value = Value1 - Value2
			/// </summary>
			Subtract,
			/// <summary>
			/// Value = Value1 * Value2
			/// </summary>
			Multiply,
			/// <summary>
			/// Value = Value1 / Value2
			/// </summary>
			Divide,
			/// <summary>
			/// Value = sin(Value1). Value2 ignored
			/// </summary>
			Sine,
			/// <summary>
			/// Value = cos(Value1). Value2 ignored
			/// </summary>
			Cosine,
			/// <summary>
			/// Value = tan(Value1). Value2 ignored
			/// </summary>
			Tangent,
			/// <summary>
			/// Value = asin(Value1). Value2 ignored
			/// </summary>
			Arcsine,
			/// <summary>
			/// Value = acos(Value1). Value2 ignored
			/// </summary>
			Arccosine,
			/// <summary>
			/// Value = atan(Value1). Value2 ignored
			/// </summary>
			Arctangent,
			/// <summary>
			/// Value = Value1 ** Value2
			/// </summary>
			Power,
			/// <summary>
			/// Value = log(Value1) / log(Value2). 0.0f if either input is 0.0f
			/// </summary>
			Logarithm,
			/// <summary>
			/// Value = min(Value1, Value2)
			/// </summary>
			Minimum,
			/// <summary>
			/// Value = max(Value1, Value2)
			/// </summary>
			Maximum,
			/// <summary>
			/// Value = floor(Value1 + 0.5). Value2 ignored
			/// </summary>
			Round,
			/// <summary>
			/// Value = Value1 &lt; Value2
			/// </summary>
			Less_Than,
			/// <summary>
			/// Value = Value1 &gt; Value2
			/// </summary>
			Greater_Than,
			/// <summary>
			/// Value = mod(Value1, Value2)
			/// </summary>
			Modulo,
			/// <summary>
			/// Value = abs(Value1). Value2 ignored
			/// </summary>
			Absolute,
			/// <summary>
			/// Value = atan2f(Value1, Value2)
			/// </summary>
			Arctan2,
			/// <summary>
			/// Value = floorf(Value1)
			/// </summary>
			Floor,
			/// <summary>
			/// Value = ceilf(Value1)
			/// </summary>
			Ceil,
			/// <summary>
			/// Value = Value1 - floorf(Value1)
			/// </summary>
			Fract,
			/// <summary>
			/// Value = sqrtf(Value1)
			/// </summary>
			Sqrt,
			/// <summary>
			/// Value = inverse sqrtf(Value1)
			/// </summary>
			InverseSqrt,
			/// <summary>
			/// Value = 0.0f if Value1 == 0.0f, -1.0f if Value1 < 0.0f else 1.0f
			/// </summary>
			Sign,
			/// <summary>
			/// Value = expf(Value1)
			/// </summary>
			Exponent,
			/// <summary>
			/// Value = Value1 * (pi / 180.0f)
			/// </summary>
			Radians,
			/// <summary>
			/// Value = Value1 * (180.0f / pi)
			/// </summary>
			Degrees,
			/// <summary>
			/// Value = sinh(Value1)
			/// </summary>
			Sinh,
			/// <summary>
			/// Value = cosh(Value1)
			/// </summary>
			Cosh,
			/// <summary>
			/// Value = tanh(Value1)
			/// </summary>
			Tanh,
			/// <summary>
			/// Value = Value1 >= 0.0f ? floorf(Value1) : ceilf(Value1)
			/// </summary>
			Trunc,
			/// <summary>
			/// Value = floorf(safedivide(Value1, Value2)) * Value2
			/// </summary>
			Snap,
			/// <summary>
			///   float range = Value2 - Value3;
			///   Value = (range != 0.0f) ? Value1 - (range * floorf((Value1 - Value3) / range)) : Value3;
			/// </summary>
			Wrap,
			/// <summary>
			/// Value = (Value1 == Value2 || abs(Value1 - Value2) <= Value3) ? 1.0f : 0.0f
			/// </summary>
			Compare,
			/// <summary>
			/// Value = Value1 * Value2 + Value3
			/// </summary>
			MultiplyAdd,
			/// <summary>
			/// Value = (Value2 != 0.0f) ? fabsf(fractf((Value1 - Value2) / (Value2 * 2.0f)) * Value2 * 2.0f - Value2) : 0.0f;
			/// </summary>
			Pingpong,
			/// <summary>
			/// Value = smoothmin(Value1, Value2, Value3)
			/// </summary>
			SmoothMin,
			/// <summary>
			/// Value = smoothmin(-Value1, -Value2, Value3)
			/// </summary>
			SmoothMax,
		}

		/// <summary>
		/// MathNode input sockets
		/// </summary>
		public MathInputs ins => (MathInputs)inputs;

		/// <summary>
		/// MathNode output sockets
		/// </summary>
		public MathOutputs outs => (MathOutputs)outputs;

		/// <summary>
		/// Math node operates on float inputs (note, some operations use only Value1)
		/// </summary>
		public MathNode(Shader shader) :
			this(shader, "a mathnode")
		{

		}

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
			inputs = new MathInputs(this);
			outputs = new MathOutputs(this);

			Operation = Operations.Add;
			ins.Value1.Value = 0.0f;
			ins.Value2.Value = 0.0f;
			ins.Value3.Value = 0.0f;
		}

		/// <summary>
		/// The operation this node does on Value1 and Value2
		/// </summary>
		public Operations Operation { get; set; }

		private void SetOperation(string op)
		{
			op = op.Replace(" ", "_");
			Operation = (Operations)Enum.Parse(typeof(Operations), op, true);
		}

		/// <summary>
		/// Set to true [IN] if math output in Value should be clamped 0.0..1.0
		/// </summary>
		public bool UseClamp { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "operation", (int)Operation);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_bool(Id, "use_clamp", UseClamp);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Value1, xmlNode.GetAttribute("value1"));
			Utilities.Instance.get_float(ins.Value2, xmlNode.GetAttribute("value2"));
			var operation = xmlNode.GetAttribute("type");
			if (!string.IsNullOrEmpty(operation))
			{
				SetOperation(operation);
			}
			var use_clamp = xmlNode.GetAttribute("use_clamp");
			if (!string.IsNullOrEmpty(use_clamp))
			{
				UseClamp = use_clamp.ToLowerInvariant().Equals("true");
			}
		}
		public override string CreateXmlAttributes()
		{
			var codeattr = new StringBuilder(1024);

			codeattr.Append($" type=\"{Operation}\" ");
			codeattr.Append($" use_clamp=\"{UseClamp.ToString().ToLowerInvariant()}\" ");

			return codeattr.ToString();
		}

		public override string CreateCodeAttributes()
		{
			var codeattr = new StringBuilder(1024);

			codeattr.Append($" {VariableName}.Operation = MathNode.Operations.{Operation};");
			codeattr.Append($" {VariableName}.UseClamp = {UseClamp.ToString().ToLowerInvariant()};");

			return codeattr.ToString();
		}
	}

	[ShaderNode("math_add")]
	public class MathAdd : MathNode
	{
		public MathAdd(Shader shader) : this(shader, "an add mathnode") { }
		public MathAdd(Shader shader, string name) : base(shader, name) { Operation = Operations.Add; }
		internal MathAdd(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Add; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_subtract")]
	public class MathSubtract : MathNode
	{
		public MathSubtract(Shader shader) : this(shader, "an subtract mathnode") { }
		public MathSubtract(Shader shader, string name) : base(shader, name) { Operation = Operations.Subtract; }
		internal MathSubtract(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Subtract; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_multiply")]
	public class MathMultiply : MathNode
	{
		public MathMultiply(Shader shader) : this(shader, "an multiply mathnode") { }
		public MathMultiply(Shader shader, string name) : base(shader, name) { Operation = Operations.Multiply; }
		internal MathMultiply(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Multiply; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_multiply_add")]
	public class MathMultiplyAdd : MathNode
	{
		public MathMultiplyAdd(Shader shader) : this(shader, "an multiply add mathnode") { }
		public MathMultiplyAdd(Shader shader, string name) : base(shader, name) { Operation = Operations.MultiplyAdd; }
		internal MathMultiplyAdd(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.MultiplyAdd; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_divide")]
	public class MathDivide : MathNode
	{
		public MathDivide(Shader shader) : this(shader, "an divide mathnode") { }
		public MathDivide(Shader shader, string name) : base(shader, name) { Operation = Operations.Divide; }
		internal MathDivide(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Divide; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_sine")]
	public class MathSine : MathNode
	{
		public MathSine(Shader shader) : this(shader, "an sine mathnode") { }
		public MathSine(Shader shader, string name) : base(shader, name) { Operation = Operations.Sine; }
		internal MathSine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Sine; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_cosine")]
	public class MathCosine : MathNode
	{
		public MathCosine(Shader shader) : this(shader, "an cosine mathnode") { }
		public MathCosine(Shader shader, string name) : base(shader, name) { Operation = Operations.Cosine; }
		internal MathCosine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Cosine; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_tangent")]
	public class MathTangent : MathNode
	{
		public MathTangent(Shader shader) : this(shader, "an tangent mathnode") { }
		public MathTangent(Shader shader, string name) : base(shader, name) { Operation = Operations.Tangent; }
		internal MathTangent(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Tangent; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_sinh")]
	public class MathSinh : MathNode
	{
		public MathSinh(Shader shader) : this(shader, "an sinh mathnode") { }
		public MathSinh(Shader shader, string name) : base(shader, name) { Operation = Operations.Sinh; }
		internal MathSinh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Sinh; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_cosh")]
	public class MathCosh : MathNode
	{
		public MathCosh(Shader shader) : this(shader, "an cosh mathnode") { }
		public MathCosh(Shader shader, string name) : base(shader, name) { Operation = Operations.Cosh; }
		internal MathCosh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Cosh; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_tanh")]
	public class MathTanh : MathNode
	{
		public MathTanh(Shader shader) : this(shader, "an tanh mathnode") { }
		public MathTanh(Shader shader, string name) : base(shader, name) { Operation = Operations.Tanh; }
		internal MathTanh(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Tanh; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_arcsine")]
	public class MathArcsine : MathNode
	{
		public MathArcsine(Shader shader) : this(shader, "an arcsine mathnode") { }
		public MathArcsine(Shader shader, string name) : base(shader, name) { Operation = Operations.Arcsine; }
		internal MathArcsine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Arcsine; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_arccosine")]
	public class MathArccosine : MathNode
	{
		public MathArccosine(Shader shader) : this(shader, "an arccosine mathnode") { }
		public MathArccosine(Shader shader, string name) : base(shader, name) { Operation = Operations.Arccosine; }
		internal MathArccosine(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Arccosine; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_arctangent")]
	public class MathArctangent : MathNode
	{
		public MathArctangent(Shader shader) : this(shader, "an arctangent mathnode") { }
		public MathArctangent(Shader shader, string name) : base(shader, name) { Operation = Operations.Arctangent; }
		internal MathArctangent(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Arctangent; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_power")]
	public class MathPower : MathNode
	{
		public MathPower(Shader shader) : this(shader, "an power mathnode") { }
		public MathPower(Shader shader, string name) : base(shader, name) { Operation = Operations.Power; }
		internal MathPower(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Power; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_logarithm")]
	public class MathLogarithm : MathNode
	{
		public MathLogarithm(Shader shader) : this(shader, "an logarithm mathnode") { }
		public MathLogarithm(Shader shader, string name) : base(shader, name) { Operation = Operations.Logarithm; }
		internal MathLogarithm(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Logarithm; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_minimum")]
	public class MathMinimum : MathNode
	{
		public MathMinimum(Shader shader) : this(shader, "an minimum mathnode") { }
		public MathMinimum(Shader shader, string name) : base(shader, name) { Operation = Operations.Minimum; }
		internal MathMinimum(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Minimum; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_maximum")]
	public class MathMaximum : MathNode
	{
		public MathMaximum(Shader shader) : this(shader, "an maximum mathnode") { }
		public MathMaximum(Shader shader, string name) : base(shader, name) { Operation = Operations.Maximum; }
		internal MathMaximum(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Maximum; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_round")]
	public class MathRound : MathNode
	{
		public MathRound(Shader shader) : this(shader, "an round mathnode") { }
		public MathRound(Shader shader, string name) : base(shader, name) { Operation = Operations.Round; }
		internal MathRound(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Round; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_lessthan")]
	public class MathLess_Than : MathNode
	{
		public MathLess_Than(Shader shader) : this(shader, "an lessthan mathnode") { }
		public MathLess_Than(Shader shader, string name) : base(shader, name) { Operation = Operations.Less_Than; }
		internal MathLess_Than(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Less_Than; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_greaterthan")]
	public class MathGreater_Than : MathNode
	{
		public MathGreater_Than(Shader shader) : this(shader, "an greaterthan mathnode") { }
		public MathGreater_Than(Shader shader, string name) : base(shader, name) { Operation = Operations.Greater_Than; }
		internal MathGreater_Than(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Greater_Than; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_modulo")]
	public class MathModulo : MathNode
	{
		public MathModulo(Shader shader) : this(shader, "an modulo mathnode") { }
		public MathModulo(Shader shader, string name) : base(shader, name) { Operation = Operations.Modulo; }
		internal MathModulo(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Modulo; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_absolute")]
	public class MathAbsolute : MathNode
	{
		public MathAbsolute(Shader shader) : this(shader, "an absolute mathnode") { }
		public MathAbsolute(Shader shader, string name) : base(shader, name) { Operation = Operations.Absolute; }
		internal MathAbsolute(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Absolute; }
		public override string ShaderNodeTypeName => "math";
	}
	[ShaderNode("math_compare")]
	public class MathCompare : MathNode
	{
		public MathCompare(Shader shader) : this(shader, "an compare mathnode") { }
		public MathCompare(Shader shader, string name) : base(shader, name) { Operation = Operations.Compare; }
		internal MathCompare(Shader shader, IntPtr intPtr) : base(shader, intPtr) { Operation = Operations.Compare; }
		public override string ShaderNodeTypeName => "math";
	}
}
