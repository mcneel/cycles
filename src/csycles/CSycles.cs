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

using ccl.ShaderNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

/** \namespace ccl
 * \brief Namespace containing the low-level wrapping API of ccycles.dll and a set of higher-level classes.
 */
namespace ccl
{
	/// <summary>
	/// CSycles wraps the ccycles.dll, providing a very low-level API into the
	/// render engine Cycles.
	/// </summary>
	public partial class CSycles
	{
		#region misc
		private static bool g_ccycles_loaded;

		/// <summary>
		/// Load the ccycles DLL.
		/// </summary>
		private static void LoadCCycles()
		{
			if (g_ccycles_loaded) return;

			LoadShaderNodes();
			g_ccycles_loaded = true;
		}

		private static readonly Dictionary<string, Type> g_registered_shadernodes = new Dictionary<string, Type>();

		/// <summary>
		/// Load all shader nodes from the assembly using reflection
		/// </summary>
		private static void LoadShaderNodes()
		{
			Assembly ccass = Assembly.GetExecutingAssembly();
			var shaderAndNameConstructor = new Type[2];
			shaderAndNameConstructor[0] = typeof(ccl.Shader);
			shaderAndNameConstructor[1] = typeof(string);

			var internalConstructor = new Type[2];
			internalConstructor[0] = typeof(ccl.Shader);
			internalConstructor[1] = typeof(IntPtr);

			var exported_types = ccass.GetExportedTypes();
			var shadernode_type = typeof(ShaderNodes.ShaderNode);
			for (int i = 0; i < exported_types.Length; i++)
			{
				var exported_type = exported_types[i];
				if (!exported_type.IsSubclassOf(shadernode_type))
					continue;
				var attr = exported_type.GetCustomAttributes(typeof(Attributes.ShaderNodeAttribute), false);
				if (attr.Length < 1)
				{
					throw new NotImplementedException(String.Format("Class {0} must include a ShaderNode attribute", exported_type));
				}
				var shnattr = attr[0] as Attributes.ShaderNodeAttribute;
				if (shnattr == null || shnattr.IsBase)
					continue;

				var constructor = exported_type.GetConstructor(shaderAndNameConstructor);
				if (constructor == null)
				{
					throw new NotImplementedException(String.Format("Class {0} must include a constructor that takes a shader and a name", exported_type));
				}

				constructor = exported_type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, internalConstructor, null);
				if (constructor == null)
				{
					throw new NotImplementedException(String.Format("Class {0} must include an internal constructor that takes a shader and an IntPtr to a shadernode", exported_type));
				}

				if (!g_registered_shadernodes.ContainsKey(shnattr.Name))
				{
					g_registered_shadernodes.Add(shnattr.Name, exported_type);
				}
			}
		}

		/// <summary>
		/// Create a ShaderNode based on XML name. This name has been registered
		/// using the ShaderNodeAttribute on each ShaderNode derived class
		/// </summary>
		/// <param name="xmlName"></param>
		/// <param name="nodeName"></param>
		/// <returns>a new ShaderNode if xmlName is registered, null otherwise</returns>
		internal static ShaderNodes.ShaderNode CreateShaderNode(Shader shader, string xmlName, string nodeName)
		{
			if (xmlName.Equals("shader")) return null;

			if (g_registered_shadernodes.ContainsKey(xmlName))
			{
				var constructTypes = new Type[2];
				constructTypes[0] = typeof(Shader);
				constructTypes[1] = typeof(string);
				var shnt = g_registered_shadernodes[xmlName];
				var constructor = shnt.GetConstructor(constructTypes);
				var param = new object[2];
				param[0] = shader;
				param[1] = nodeName;

				return constructor.Invoke(param) as ShaderNodes.ShaderNode;
			}

			throw new InvalidDataException($"Node type '{xmlName}' not found.");
		}
		internal static ShaderNodes.ShaderNode CreateShaderNode(Shader shader, IntPtr shaderNodePtr, string xmlName)
		{
			if (xmlName.Equals("shader")) return null;

			if (g_registered_shadernodes.ContainsKey(xmlName))
			{
				var constructTypes = new Type[2];
				constructTypes[0] = typeof(Shader);
				constructTypes[1] = typeof(IntPtr);
				var shnt = g_registered_shadernodes[xmlName];
				var constructor = shnt.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructTypes, new ParameterModifier[] { });
				var param = new object[2];
				param[0] = shader;
				param[1] = shaderNodePtr;

				ShaderNode shader_node = constructor.Invoke(param) as ShaderNode;

				if (shader_node is OutputNode output_node)
					shader.Output = output_node;

				return shader_node;
			}

			throw new InvalidDataException($"Node type '{xmlName}' not found.");
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_initialise(uint mask);

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_failed_gpus_mask();

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_gpu_init_error(uint device_type);

		/// <summary>
		/// Initialise the Cycles render engine.
		///
		/// This will ensure ccycles.dll is loaded. The initialisation will also prepare
		/// the devices list for use with Cycles.
		///
		/// Note: call <c>set_kernel_path</c> before initialising CSycles, otherwise default
		/// kernel path "lib" will be used.
		/// </summary>
		/// <returns></returns>
		public static void initialise(DeviceTypeMask mask)
		{
			LoadCCycles();
			cycles_initialise((uint)mask);
		}

		/// <summary>
		/// Bitmask (DeviceTypeMask) of GPUs whose init threw during the most
		/// recent <see cref="initialise"/> call. Zero if everything succeeded.
		/// </summary>
		public static DeviceTypeMask failed_gpus_mask()
		{
			return (DeviceTypeMask)cycles_failed_gpus_mask();
		}

		/// <summary>
		/// Error message captured for the given GPU during the most recent
		/// <see cref="initialise"/> call. Empty if that GPU did not fail.
		/// </summary>
		public static string gpu_init_error(DeviceType type)
		{
			return Marshal.PtrToStringAnsi(cycles_gpu_init_error((uint)type)) ?? string.Empty;
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_path_init(string path, string userPath);
		/// <summary>
		/// Set the paths for Cycles to look for pre-compiled or cached kernels, or kernel code
		///
		/// Note: to have any effect needs to be called before <c>initialise</c>.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="userPath"></param>
		public static void path_init([MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPStr)] string userPath)
		{
			LoadCCycles();
			cycles_path_init(path, userPath);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shutdown();
		/// <summary>
		/// Clean up CSycles, CCycles and Cycles.
		/// </summary>
		/// <returns></returns>
		public static void shutdown()
		{
			cycles_shutdown();
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		/** <summary>
		 * Signature for a logger callback.
		 *
		 * CCycles no longer takes a logger callback; RhinoCycles still declares one.
		 * </summary>
		 */
		public delegate void LoggerCallback([MarshalAsAttribute(UnmanagedType.LPStr)] string msg);

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_log_to_stdout(int stdout);
		/**
		 * Set to true if logger output should be sent to std::cout as well.
		 *
		 * Note that this is global to the logger.
		 */
		public static void log_to_stdout(bool stdOut)
		{
			cycles_log_to_stdout(stdOut ? 1 : 0);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_set_rhino_perlin_noise_table(IntPtr data, uint count);
		public static void set_rhino_perlin_noise_table(IntPtr data, uint count)
		{
			LoadCCycles();
			cycles_set_rhino_perlin_noise_table(data, count);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_set_rhino_impulse_noise_table(IntPtr data, uint count);
		public static void set_rhino_impulse_noise_table(IntPtr data, uint count)
		{
			LoadCCycles();
			cycles_set_rhino_impulse_noise_table(data, count);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_set_rhino_vc_noise_table(IntPtr data, uint count);
		public static void set_rhino_vc_noise_table(IntPtr data, uint count)
		{
			LoadCCycles();
			cycles_set_rhino_vc_noise_table(data, count);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_set_rhino_aaltonen_noise_table(IntPtr data, uint count);
		public static void set_rhino_aaltonen_noise_table(IntPtr data, uint count)
		{
			LoadCCycles();
			cycles_set_rhino_aaltonen_noise_table(data, count);
		}
		#endregion
	}
}
