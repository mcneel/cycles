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
using System.Collections.Generic;
using System.Text;
using LeftRightFound = System.Tuple<ccl.ShaderNodes.ColorStop, ccl.ShaderNodes.ColorStop, bool>;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// ColorStop for ColorBand, comparable on position
	/// </summary>
	public class ColorStop : IComparable<ColorStop>
	{
		/// <summary>
		/// Position of the color stop
		/// </summary>
		public float Position;
		/// <summary>
		/// Color of the color stop
		/// </summary>
		public float4 Color;

		/// <summary>
		/// Default constructor
		/// </summary>
		public ColorStop() { }
		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public ColorStop(ColorStop other)
		{
			Position = other.Position;
			Color = new float4(other.Color);
		}

		/// <summary>
		/// Solely on position, as that is what we care about
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ColorStop other) => other == null ? 1 : Position.CompareTo(other.Position);
	}

	/// <summary>
	/// ColorBand is a collection of ColorStops and gives an evaluate function
	/// to get a color from a float based on the ColorStops and interpolation
	/// </summary>
	public class ColorBand
	{
		/// <summary>
		/// ColorBand interpolation types
		/// </summary>
		public enum Interpolations
		{
			/// <summary>
			/// Constant. Color changes on the stop
			/// </summary>
			Constant,
			/// <summary>
			/// Linear interpolation between stops
			/// </summary>
			Linear,
			/// <summary>
			/// Linear interpolation with easing between stops
			/// </summary>
			Ease
		};

		public Interpolations Interpolation = Interpolations.Linear;

		public List<ColorStop> Stops = new List<ColorStop>();


		/// <summary>
		/// Get a ColorStop on the left of pos, and on the right of pos.
		/// Third tuple member is true when no stop was found that actually
		/// matches pos
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public LeftRightFound GetLeftRight(float pos)
		{
			var needle = new ColorStop { Position = pos };
			ColorStop l = null;
			ColorStop r = null;

			var idx = Stops.BinarySearch(needle);
			var notfound = false;

			if (idx < 0)
			{
				idx = ~idx;
			}

			if (idx == Stops.Count) // there was no item larger than needle
			{
				notfound = true;
				// get the last two stops, or just last one if possible.
				r = new ColorStop(Stops[idx - 1]);
				if ((idx - 2) >= 0)
				{
					l = new ColorStop(Stops[idx - 2]);
				}
				else
				{
					// if there was only one stop we copy r to l, then set r to be on position 1.0f
					// so we can still interpolate
					l = new ColorStop(r);
					r.Position = 1.0f;
				}
			}
			else
			{
				// idx is the either precisely pos or the one right after.
				r = new ColorStop(Stops[idx]);

				// try to get the stop on the left of pos
				if ((idx - 1) >= 0)
				{
					l = new ColorStop(Stops[idx - 1]);
				}
				else
				{
					// or copy the one from the right,
					// but set its position to 0.0f so we can
					// still interpolate
					l = new ColorStop(r) { Position = 0.0f };
				}

			}

			// done, give it back.
			return new LeftRightFound(l, r, notfound);
		}

		/// <summary>
		/// Add a ColorStop at position with color
		/// </summary>
		/// <param name="color"></param>
		/// <param name="position"></param>
		public void InsertColorStop(float4 color, float position)
		{
			InsertColorStop(new ColorStop { Position = position, Color = color });
		}

		/// <summary>
		/// Add given ColorStop
		/// </summary>
		/// <param name="cstop"></param>
		public void InsertColorStop(ColorStop cstop)
		{
			if (Stops.Count == 0)
			{
				Stops.Add(cstop);
				return;
			}

			var idx = Stops.BinarySearch(cstop);

			if (idx < 0) idx = ~idx;

			if (idx == Stops.Count)
			{
				Stops.Add(cstop);
			}
			else
			{
				Stops.Insert(idx, cstop);
			}
		}


		/// <summary>
		/// Evaluate pos into a color
		/// </summary>
		/// <param name="pos">Value in range 0.0f-1.0f</param>
		/// <param name="color">Color will be interpolated into this</param>
		/// <returns></returns>
		public void evaluate(float pos, float4 color)
		{
			if (Stops.Count == 0)
			{
				color.x = 0.0f;
				color.y = 0.0f;
				color.z = 0.0f;
				color.w = 0.0f;
				return;
			}

			// stop1 = left of pos
			var left = Stops[0];
			// stop2 = right of pos
			ColorStop right = null;

			// if there is only one stop, or we're on the left of the very first stop
			// just copy the color, we're done.
			if (Stops.Count == 1 || pos <= left.Position)
			{
				color.Copy(left.Color);
			}
			else
			{
				// get left and right items
				var lr = GetLeftRight(pos);
				left = lr.Item1;
				right = lr.Item2;
				var last_item = lr.Item3;

				// if we're on the right of the right stop, copy
				// color, we're done.
				if (pos >= right.Position)
				{
					color.Copy(right.Color);
				}
				else
				{
					// if we're on constant interpolation, lets use
					// color on left of position.
					if (Interpolation == Interpolations.Constant)
					{
						color.Copy(left.Color);
					}
					else
					{
						// ok, so we need to interpolate
						float mfac;
						float fac;
						// get factor
						if (Math.Abs(left.Position - right.Position) > 0.0001)
						{
							fac = Math.Abs(pos - right.Position) / Math.Abs(left.Position - right.Position);
						}
						else
						{
							fac = last_item ? 1.0f : 0.0f;
						}

						// extra easing if ease
						if (Interpolation == Interpolations.Ease)
						{
							mfac = fac * fac;
							fac = 3.0f * mfac - 2.0f * mfac * fac;
						}

						// right color fac
						mfac = 1.0f - fac;

						// factor colors
						left.Color *= fac;
						right.Color *= mfac;
						// add factored colors to get new interpolated color
						var interpolated_color = left.Color + right.Color;
						// copy, done
						color.Copy(interpolated_color);
					}
				}
			}
		}
	}

	/// <summary>
	/// ColorRamp output sockets
	/// </summary>
	public class ColorRampOutputs : Outputs
	{
		/// <summary>
		/// ColorRamp interpolated Color output
		/// </summary>
		public ColorSocket Color { get; set; }
		/// <summary>
		/// ColorRamp interpolated Alpha output
		/// </summary>
		public FloatSocket Alpha { get; set; }

		internal ColorRampOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha", "alpha");
			AddSocket(Alpha);
		}
	}

	/// <summary>
	/// ColorRamp input sockets
	/// </summary>
	public class ColorRampInputs : Inputs
	{
		/// <summary>
		/// Factor value to interpolate to a color
		/// </summary>
		public FloatSocket Fac { get; set; }

		internal ColorRampInputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	/// <summary>
	/// ColorRampNode.
	///
	/// Interpolate input factor (0.0f-1.0f) to a color on the ColorBand of the node.
	/// </summary>
	[ShaderNode("rgb_ramp")]
	public class ColorRampNode : ShaderNode
	{
		/// <summary>
		/// ColorRamp input sockets
		/// </summary>
		public ColorRampInputs ins => (ColorRampInputs)inputs;

		/// <summary>
		/// ColorRamp output sockets
		/// </summary>
		public ColorRampOutputs outs => (ColorRampOutputs)outputs;

		/// <summary>
		/// Create a ColorRampNode
		/// </summary>
		public ColorRampNode(Shader shader) : this(shader, "a color ramp node")
		{
		}

		public ColorRampNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal ColorRampNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ColorRampInputs(this);
			outputs = new ColorRampOutputs(this);
			Interpolate = true;
			ColorBand = new ColorBand();
		}

		/// <summary>
		/// Cycles uses ramp table size of 256 float4s
		/// </summary>
		private const int RampTableSize = 256;

		/// <summary>
		/// Set ColorRamp member variable [IN] for ColorRampNode.
		/// </summary>
		public bool Interpolate { get; set; }

		/// <summary>
		/// The ColorBand for the ColorRamp to evaluate fac against
		/// </summary>
		public ColorBand ColorBand { get; set; }

		internal override void SetDirectMembers()
		{
			var val = Interpolate;
			var color = new float4();
			CSycles.shadernode_set_member_bool(Id, "interpolate", val);

			for (var i = 0; i < RampTableSize; i++)
			{
				ColorBand.evaluate((float)i / (float)(RampTableSize - 1), color);
				CSycles.shadernode_set_member_vec4_at_index(Id, "ramp", color.x, color.y, color.z, color.w, i);
			}
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
			bool interp = false;
			Utilities.Instance.get_bool(ref interp, xmlNode.GetAttribute("interpolate"));
			var interpolation = xmlNode.GetAttribute("interpolation");
			if (string.IsNullOrEmpty(interpolation))
			{
				ColorBand.Interpolations i;
				if (Enum.TryParse(interpolation, out i))
				{
					ColorBand.Interpolation = i;
				}
			}
			if (xmlNode.ReadToDescendant("stop"))
			{
				float pos = 0.0f;
				do
				{
					var color = new float4(0.0f);
					Utilities.Instance.get_float(ref pos, xmlNode.GetAttribute("position"));
					Utilities.Instance.get_float4(color, xmlNode.GetAttribute("color"));

					ColorBand.InsertColorStop(color, pos);
				} while (xmlNode.ReadToNextSibling("stop"));

			}
		}

		public override string CreateXmlAttributes()
		{
			var xml = new StringBuilder(1024);
			xml.Append($" interpolate=\"{Interpolate.ToString().ToLowerInvariant()}\" ");
			xml.Append($" interpolation=\"{ColorBand.Interpolation}\" ");
			return xml.ToString();
		}

		public override string CreateChildNodes()
		{
			var childNodes = new StringBuilder(1024);
			foreach (var stop in ColorBand.Stops)
			{
				childNodes.Append(
					$"<stop color=\"{stop.Color.x} {stop.Color.y} {stop.Color.z} {stop.Color.w}\" position=\"{stop.Position}\" />");
			}

			return childNodes.ToString();
		}

		public override string CreateCodeAttributes()
		{
			var attr = new StringBuilder(1024);
			foreach (var stop in ColorBand.Stops)
			{
				attr.Append(
					$" {VariableName}.ColorBand.Stops.Add(new ccl.ShaderNodes.ColorStop() {{Color=new {stop.Color}, Position={stop.Position}f}});");
			}

			return attr.ToString();
		}
	}
}
