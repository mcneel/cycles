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

using System;
using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
		#region integrator
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_tag_update(IntPtr sessionId);
		public static void integrator_tag_update(IntPtr sessionId)
		{
			cycles_integrator_tag_update(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_bounce(IntPtr sessionId, int value);
		public static void integrator_set_max_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_max_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_min_bounce(IntPtr sessionId, int value);
		public static void integrator_set_min_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_min_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_diffuse_bounce(IntPtr sessionId, int value);
		public static void integrator_set_max_diffuse_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_max_diffuse_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_glossy_bounce(IntPtr sessionId, int value);
		public static void integrator_set_max_glossy_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_max_glossy_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_transmission_bounce(IntPtr sessionId, int value);
		public static void integrator_set_max_transmission_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_max_transmission_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_volume_bounce(IntPtr sessionId, int value);
		public static void integrator_set_max_volume_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_max_volume_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_max_bounce(IntPtr sessionId, int value);
		public static void integrator_set_transparent_max_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_transparent_max_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_min_bounce(IntPtr sessionId, int value);
		public static void integrator_set_transparent_min_bounce(IntPtr sessionId, int value)
		{
			cycles_integrator_set_transparent_min_bounce(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_bounces(IntPtr sessionId, int value);
		public static void integrator_set_ao_bounces(IntPtr sessionId, int value)
		{
			cycles_integrator_set_ao_bounces(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_factor(IntPtr sessionId, float value);
		public static void integrator_set_ao_factor(IntPtr sessionId, float value)
		{
			cycles_integrator_set_ao_factor(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_distance(IntPtr sessionId, float value);
		public static void integrator_set_ao_distance(IntPtr sessionId, float value)
		{
			cycles_integrator_set_ao_distance(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_additive_factor(IntPtr sessionId, float value);
		public static void integrator_set_ao_additive_factor(IntPtr sessionId, float value)
		{
			cycles_integrator_set_ao_additive_factor(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_aa_samples(IntPtr sessionId, int value);
		public static void integrator_set_aa_samples(IntPtr sessionId, int value)
		{
			cycles_integrator_set_aa_samples(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_no_caustics(IntPtr sessionId, bool value);
		public static void integrator_set_no_caustics(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_no_caustics(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_caustics_reflective(IntPtr sessionId, bool value);
		public static void integrator_set_caustics_reflective(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_caustics_reflective(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_caustics_refractive(IntPtr sessionId, bool value);
		public static void integrator_set_caustics_refractive(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_caustics_refractive(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_filter_glossy(IntPtr sessionId, float value);
		public static void integrator_set_filter_glossy(IntPtr sessionId, float value)
		{
			cycles_integrator_set_filter_glossy(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_use_direct_light(IntPtr sessionId, bool value);
		public static void integrator_set_use_direct_light(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_use_direct_light(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_use_indirect_light(IntPtr sessionId, bool value);
		public static void integrator_set_use_indirect_light(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_use_indirect_light(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_step_rate(IntPtr sessionId, float value);
		public static void integrator_set_volume_step_rate(IntPtr sessionId, float value)
		{
			cycles_integrator_set_volume_step_rate(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_max_steps(IntPtr sessionId, int value);
		public static void integrator_set_volume_max_steps(IntPtr sessionId, int value)
		{
			cycles_integrator_set_volume_max_steps(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_seed(IntPtr sessionId, int value);
		public static void integrator_set_seed(IntPtr sessionId, int value)
		{
			cycles_integrator_set_seed(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sampling_pattern(IntPtr sessionId, uint value);
		public static void integrator_set_sampling_pattern(IntPtr sessionId, SamplingPattern value)
		{
			cycles_integrator_set_sampling_pattern(sessionId, (uint)value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_clamp_direct(IntPtr sessionId, float value);
		public static void integrator_set_sample_clamp_direct(IntPtr sessionId, float value)
		{
			cycles_integrator_set_sample_clamp_direct(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_clamp_indirect(IntPtr sessionId, float value);
		public static void integrator_set_sample_clamp_indirect(IntPtr sessionId, float value)
		{
			cycles_integrator_set_sample_clamp_indirect(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_light_sampling_threshold(IntPtr sessionId, float value);
		public static void integrator_set_light_sampling_threshold(IntPtr sessionId, float value)
		{
			cycles_integrator_set_light_sampling_threshold(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_use_light_tree(IntPtr sessionId, bool value);
		public static void integrator_set_use_light_tree(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_use_light_tree(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_use_adaptive_sampling(IntPtr sessionId, bool value);
		public static void integrator_set_use_adaptive_sampling(IntPtr sessionId, bool value)
		{
			cycles_integrator_set_use_adaptive_sampling(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_adaptive_min_samples(IntPtr sessionId, int value);
		public static void integrator_set_adaptive_min_samples(IntPtr sessionId, int value)
		{
			cycles_integrator_set_adaptive_min_samples(sessionId, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_adaptive_threshold(IntPtr sessionId, float value);
		public static void integrator_set_adaptive_threshold(IntPtr sessionId, float value)
		{
			cycles_integrator_set_adaptive_threshold(sessionId, value);
		}



		#endregion
	}
}
