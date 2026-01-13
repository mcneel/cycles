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
		#region devices
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_devices();
		/// <summary>
		/// Get the number of available render devices.
		/// </summary>
		/// <returns>number of available render devices.</returns>
		public static uint number_devices()
		{
			return cycles_number_devices();
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_multidevices();
		/// <summary>
		/// Get the number of available render multidevices.
		/// </summary>
		/// <returns>number of available render multidevices.</returns>
		public static uint number_multidevices()
		{
			return cycles_number_multidevices();
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_multi_subdevices(int i);
		/// <summary>
		/// Get the number of available render multi_subdevices.
		/// </summary>
		/// <param name="i">ID of multi-device being queried</param>
		/// <returns>number of available render multi_subdevices.</returns>
		public static uint number_multi_subdevices(int i)
		{
			return cycles_number_multi_subdevices(i);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_get_multidevice_subdevice_id(int i, int j);
		/// <summary>
		/// Get the get of available render multidevice_subdevice_id.
		/// </summary>
		/// <param name="i">ID of multi-device being queried</param>
		/// <param name="j">index of subdevice in multi subdevices to query.</param>
		/// <returns>The index of the sub-device in the global device list</returns>
		public static uint get_multidevice_subdevice_id(int i, int j)
		{
			return cycles_get_multidevice_subdevice_id(i, j);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_devices_by_type(ccl.DeviceType device_type);
		/// <summary>
		/// Get the number of available Cuda devices.
		/// </summary>
		/// <returns>number of available Cuda devices.</returns>
		public static uint number_devices_by_type(ccl.DeviceType device_type)
		{
			return cycles_number_devices_by_type(device_type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_device_capabilities();
		/// <summary>
		/// Get the device capabilities for all devices Cycles can see.
		/// </summary>
		/// <returns>The device capabilities.</returns>
		public static string device_capabilities()
		{
			return Marshal.PtrToStringAnsi(cycles_device_capabilities());
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_device_description(int i);
		/// <summary>
		/// Get the device description for specified device.
		/// </summary>
		/// <param name="i">Device ID to get description of.</param>
		/// <returns>The device description.</returns>
		public static string device_decription(int i)
		{
			return Marshal.PtrToStringAnsi(cycles_device_description(i));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_device_id(int i);
		/// <summary>
		/// Get the device ID string
		/// </summary>
		/// <param name="i">Device ID to get the device ID string for.</param>
		/// <returns>Device ID string.</returns>
		public static string DeviceId(int i)
		{
			return Marshal.PtrToStringAnsi(cycles_device_id(i));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_device_num(int i);
		/// <summary>
		/// Return device enumeration number
		/// </summary>
		/// <param name="i">Device ID to get the device enumeration number for.</param>
		/// <returns>Device enumeration number</returns>
		public static uint device_num(int i)
		{
			return cycles_device_num(i);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_device_type(int i);
		/// <summary>
		/// Get the <c>DeviceType</c> of th specified device
		/// </summary>
		/// <param name="i">Device ID to get the <c>DeviceType</c> for</param>
		/// <returns><c>DeviceType</c></returns>
		public static DeviceType device_type(int i)
		{
			return (DeviceType)cycles_device_type(i);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_device_display_device(int i);
		/// <summary>
		/// Query if device is used as display device.
		/// </summary>
		/// <param name="i">Device ID to query</param>
		/// <returns>True if the device is used as display device.</returns>
		public static bool device_display_device(int i)
		{
			return cycles_device_display_device(i);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern int cycles_create_multidevice(int count, int* idxbuffer);
		/// <summary>
		///  create a multi-device with given device ids.( Device.num)
		/// </summary>
		/// <param name="count">Number of device ids in idxBuffer</param>
		/// <param name="idxBuffer">int buffer containing device ids.</param>
		/// <returns>A number 100000 or larger if successful, or -1 otherwise.</returns>
		public static int create_multidevice(int count, ref int[] idxBuffer)
		{
			unsafe
			{
				fixed (int* pidxBuffer = idxBuffer)
				{
					return cycles_create_multidevice(count, pidxBuffer);
				}
			}
		}
		#endregion
	}
}
