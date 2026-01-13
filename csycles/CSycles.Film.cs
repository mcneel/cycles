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
		#region film
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_filter(IntPtr sessionId, uint filter_type, float filter_width);
		public static void film_set_filter(IntPtr sessionId, FilterType filter_type, float filter_width)
		{
			cycles_film_set_filter(sessionId, (uint)filter_type, filter_width);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_exposure(IntPtr sessionId, float exposure);
		public static void film_set_exposure(IntPtr sessionId, float exposure)
		{
			cycles_film_set_exposure(sessionId, exposure);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_use_sample_clamp(IntPtr sessionId, bool use_sample_clamp);
		public static void film_set_use_sample_clamp(IntPtr sessionId, bool use_sample_clamp)
		{
			cycles_film_set_use_sample_clamp(sessionId, use_sample_clamp);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_tag_update(IntPtr sessionId);
		public static void film_tag_update(IntPtr sessionId)
		{
			cycles_film_tag_update(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_use_approximate_shadow_catcher(IntPtr sessionId, bool use_approximate_shadow_catcher);
		public static void film_set_use_approximate_shadow_catcher(IntPtr sessionId, bool use_approximate_shadow_catcher)
		{
			cycles_film_set_use_approximate_shadow_catcher(sessionId, use_approximate_shadow_catcher);
		}

		#endregion
	}
}
