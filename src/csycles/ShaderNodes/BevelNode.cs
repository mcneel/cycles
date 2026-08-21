using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using System;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// BevelNode input sockets
	/// </summary>
	public class BevelInputs : Inputs
	{
		public VectorSocket Normal { get; set; }
		public FloatSocket Radius { get; set; }
		public IntSocket Samples { get; set; }

		public BevelInputs(ShaderNode parentNode)
		{
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
			Radius = new FloatSocket(parentNode, "Radius", "radius");
			AddSocket(Radius);
			Samples = new IntSocket(parentNode, "Samples", "samples");
			AddSocket(Samples);
		}
	}

	/// <summary>
	/// BevelNode output sockets
	/// </summary>
	public class BevelOutputs : Outputs
	{
		/// <summary>
		/// BevelNode new Normal
		/// </summary>
		public VectorSocket Normal { get; set; }

		internal BevelOutputs(ShaderNode parentNode)
		{
			/* The output's internal name is "bevel"; only the ui name is Normal, which
			 * the input socket also uses. */
			Normal = new VectorSocket(parentNode, "Normal", "bevel");
			AddSocket(Normal);
		}
	}

	/// <summary>
	/// BevelNode
	/// </summary>
	[ShaderNode("bevel")]
	public class BevelNode : ShaderNode
	{
		/// <summary>
		/// BevelNode input sockets
		/// </summary>
		public BevelInputs ins => (BevelInputs)inputs;

		/// <summary>
		/// BevelNode output sockets
		/// </summary>
		public BevelOutputs outs => (BevelOutputs)outputs;

		/// <summary>
		/// Create new BevelNode
		/// </summary>
		public BevelNode(Shader shader) : this(shader, "a bevel node") { }
		public BevelNode(Shader shader, string name) :
				base(shader, name)
		{
			FinalizeConstructor();
		}

		internal BevelNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new BevelInputs(this);
			outputs = new BevelOutputs(this);

			ins.Radius.Value = 0.1f;
			ins.Samples.Value = 4;
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Radius, xmlNode.GetAttribute("radius"));
			Utilities.Instance.get_int(ins.Samples, xmlNode.GetAttribute("samples"));
		}
	}
}
