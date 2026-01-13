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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ccl
{
	/// <summary>
	/// Representation of a Cycles rendering device
	/// </summary>
	public class Device
	{
		/// <summary>
		/// Get the numerical ID for this device
		/// </summary>
		public uint Id { get; private set; }
		/// <summary>
		/// Get the Cycles description for the device
		/// </summary>
		public string Description { get; private set; }
		/// <summary>
		/// The name for the device
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Give a nice name for UI usage. PCI bus and other redundancy is removed.
		///
		/// For a multidevice the type of multi device and its subdevices is given.
		/// </summary>
		public string NiceName
		{
			get
			{
				if (IsMulti)
				{
					var n = string.Join(", ", (from sd in Subdevices select sd.NiceName).ToList());
					var multiType = FirstFromMulti.Type;
					if(multiType == DeviceType.Optix) {
						n = n.Replace(" (Optix)", "");
					}
					return $"Multi ({multiType}): {n}";
				}
				if (IsCuda || IsHip || IsMetal)
				{
					return Name.Split('_')[1];
				}
				if (IsOptix)
					return $"{Name.Split('_')[1]} (Optix)";
				return Name;
			}
		}

		public string NiceNameSha
		{
			get
			{
				using (SHA256 sha = SHA256.Create())
				{
					var nicenamebytes = System.Text.Encoding.UTF8.GetBytes(NiceName);
					var hash = sha.ComputeHash(nicenamebytes);
					var strhash = string.Concat(hash.Select(b => b.ToString("x2")));
					return strhash;
				}
			}
		}
		/// <summary>
		/// Get the Cycles num for the device
		/// </summary>
		public uint Num { get; private set; }
		/// <summary>
		/// Get the device type
		/// </summary>
		public DeviceType Type { get; private set; }
		/// <summary>
		/// True if this device is used as a display device
		/// </summary>
		public bool DisplayDevice { get; private set; }
		/// <summary>
		/// True if this is a Cuda device
		/// </summary>
		public bool IsCuda => Type == DeviceType.Cuda;

		/// <summary>
		/// True if this device is a Cpu
		/// </summary>
		public bool IsCpu => Type == DeviceType.Cpu;

		/// <summary>
		/// True if this device is an OptiX device
		/// </summary>
		public bool IsOptix => Type == DeviceType.Optix;

		/// <summary>
		/// True if this device is a Metal device
		/// </summary>
		public bool IsMetal => Type == DeviceType.Metal;

		/// <summary>
		/// True if this device is a HIP device
		/// </summary>
		public bool IsHip => Type == DeviceType.Hip;

		/// <summary>
		/// True if this device is a OneAPI device
		/// </summary>
		public bool IsOneApi => Type == DeviceType.OneApi;

		/// <summary>
		/// True if this device is a GPU
		/// </summary>
		public bool IsGpu => !IsCpu;

		/// <summary>
		/// True if this device is a Multi device
		/// </summary>
		public bool IsMulti => Type == DeviceType.Multi;

		/// <summary>
		/// True if this is a multi device that also uses CPU
		/// </summary>
		public bool MultiWithCpu => Subdevices.Where((Device d) => d.Type == DeviceType.Cpu).Any();

		public Device FirstFromMulti  {
			get
			{
				var l = Subdevices.Where((Device d) => d.Type != DeviceType.Cpu);
				if(l.Any()) {
					return l.First();	
				}

				return FirstCpu;
			}
		}

		/// <summary>
		/// True if this is a Multi Cuda device
		/// </summary>
		public bool IsMultiCuda => Type == DeviceType.Multi && Subdevices.Where((Device d) => d.Type == DeviceType.Cuda).Any();


		/// <summary>
		/// True if this is a Multi Optix device
		/// </summary>
		public bool IsMultiOptix => Type == DeviceType.Multi && Subdevices.Where((Device d) => d.Type == DeviceType.Optix).Any();

		/// <summary>
		/// True if this is a Multi Metal device
		/// </summary>
		public bool IsMultiMetal => Type == DeviceType.Multi && Subdevices.Where((Device d) => d.Type == DeviceType.Metal).Any();

		/// <summary>
		/// True if this is a Multi Hip device
		/// </summary>
		public bool IsMultiHip => Type == DeviceType.Multi && Subdevices.Where((Device d) => d.Type == DeviceType.Hip).Any();

		/// <summary>
		/// True if this is a Multi OneApi device
		/// </summary>
		public bool IsMultiOneApi => Type == DeviceType.Multi && Subdevices.Where((Device d) => d.Type == DeviceType.OneApi).Any();

		/// <summary>
		/// String representation of this device
		/// </summary>
		/// <returns>String representation of this device</returns>
		public override string ToString() => $"{base.ToString()}: {Description} ({Type}), Id {Id} Num {Num} UiName {Name} DisplayDevice {DisplayDevice}";

		/// <summary>
		/// Get the default device (Cpu)
		/// </summary>
		/// <returns>The default device</returns>
		static public Device Default => FirstCpu;

		/// <summary>
		/// Get capabilities of all devices that Cycles can see.
		/// </summary>
		static public string Capabilities => CSycles.device_capabilities();

		/// <summary>
		/// Get a device by using GetDevice(int idx). Constructor is private.
		/// </summary>
		private Device() { }

		/// <summary>
		/// Test if given ID is for this device
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool HasId(int id)
		{
			if (Type != DeviceType.Multi) return (int)Id == id;

			foreach (var sd in Subdevices)
			{
				if ((int)sd.Id == id) return true;
			}

			return false;
		}

		/// <summary>
		/// Test if given ID is for this device
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool HasId(uint id)
		{
			return HasId((int)id);
		}

		public override bool Equals(object obj)
		{
			var dev = obj as Device;
			if (dev == null) return false;

			return dev.Id == Id && dev.SubdeviceCount == SubdeviceCount && dev.Subdevices.SequenceEqual(Subdevices);
		}

		public override int GetHashCode()
		{
			return DeviceString.GetHashCode();
		}

		static public bool operator ==(Device a, Device b)
		{
			if (!(a is null) && b is null) return false;
			if (a is null && !(b is null)) return false;
			if (a is null && b is null) return true;
			return a.Equals(b);
		}
		static public bool operator !=(Device a, Device b)
		{
			if (!(a is null) && b is null) return true;
			if (a is null && !(b is null)) return true;
			if (a is null && b is null) return false;
			return !a.Equals(b);
		}

		/// <summary>
		/// Get the number of available Cycles render devices
		/// </summary>
		/// <returns></returns>
		static public uint Count => CSycles.number_devices();

		/// <summary>
		/// Get the number of available Cycles render multi-devices
		/// </summary>
		static public uint MultiCount => CSycles.number_multidevices();

		/// <summary>
		/// Amount of sub-devices for this (multi-)device.
		/// </summary>
		public uint SubdeviceCount => CSycles.number_multi_subdevices((int)Id);

		/// <summary>
		/// Enumerator over subdevices of this multi devices.
		/// </summary>
		public IEnumerable<Device> Subdevices
		{
			get
			{
				for (var i = 0; i < SubdeviceCount; i++)
				{
					var j = CSycles.get_multidevice_subdevice_id((int)Id, i);
					yield return GetDevice((int)j);
				}
			}
		}

		/// <summary>
		/// Enumerate over sub-devices, returning a Tuple&lt;int, Device&gt; wher
		/// the int is index into global regular device list.
		/// </summary>
		public IEnumerable<Tuple<int, Device>> SubdevicesIndex
		{
			get
			{
				for (var i = 0; i < SubdeviceCount; i++)
				{
					var j = CSycles.get_multidevice_subdevice_id((int)Id, i);
					yield return new Tuple<int, Device>((int)j, GetDevice((int)j));
				}
			}
		}

		/// <summary>
		/// True if any of the available devices is a Cuda device
		/// </summary>
		/// <returns></returns>
		static public bool CudaAvailable()
		{
			return CSycles.number_devices_by_type(ccl.DeviceType.Cuda) > 0;
		}

		static public bool OptixAvailable()
		{
			return CSycles.number_devices_by_type(ccl.DeviceType.Optix) > 0;
		}

		static public bool HipAvailable()
		{
			return CSycles.number_devices_by_type(ccl.DeviceType.Hip) > 0;
		}

		static public bool OneApiAvailable()
		{
			return CSycles.number_devices_by_type(ccl.DeviceType.OneApi) > 0;
		}

		static public bool MetalAvailable()
		{
			return CSycles.number_devices_by_type(ccl.DeviceType.Metal) > 0;
		}

		public const int MultiOffset = 100000;
		/// <summary>
		/// Enumerate over available devices.
		/// </summary>
		static public IEnumerable<Device> Devices
		{
			get
			{
				for (var i = 0; i < Count; i++)
				{
					yield return GetDevice(i);
				}

				for (var j = 0; j < MultiCount; j++)
				{
					yield return GetDevice(j + MultiOffset);
				}
			}
		}


		/// <summary>
		/// Returns the first cuda device if it exists,
		/// the default rendering device (Cpu) if not.
		/// </summary>
		static public Device FirstCuda
		{
			get
			{
				var d = (from device in Devices
								 where device.IsCuda || device.IsMultiCuda
								 select device).FirstOrDefault();
				return d ?? Default;
			}
		}


		/// <summary>
		/// Get the first GPU if one exists and is ready,
		/// the default rendering device (Cpu) if not.
		/// </summary>
		static public Device FirstGpu
		{
			get
			{
				var d = (from device in Devices
								 where device.IsGpu
								 select device).FirstOrDefault();
				return d ?? Default;

			}
		}

		/// <summary>
		/// Get the first Cpu if one exists,
		/// the default rendering device (Cpu) if not.
		/// </summary>
		static public Device FirstCpu
		{
			get
			{
				var d = (from device in Devices
								 where device.IsCpu
								 select device).FirstOrDefault();
				return d ?? null;

			}
		}


		/// <summary>
		/// Get the device with specified index. For a multi-device the
		/// ID starts at 100000
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		static public Device GetDevice(int idx)
		{
			Device d = new Device
			{
				Id = (uint)idx,
				Description = CSycles.device_decription(idx),
				Name = CSycles.DeviceId(idx),
				Num = CSycles.device_num(idx),
				Type = CSycles.device_type(idx),
				DisplayDevice = CSycles.device_display_device(idx),
			};

			if(d.Description == "Multi Device")
			{
				d.Type = DeviceType.Multi;
			}

			return d;
		}


		/// <summary>
		/// Create a multi-device from the given Device list.
		/// </summary>
		/// <param name="idx">List of Devices</param>
		/// <returns>Device representing the multi-device</returns>
		static public Device CreateMultiDevice(List<Device> idx)
		{
			idx.Sort((Device a, Device b) =>
			{
				if (a.Id == b.Id) return 0;
				return a.Id < b.Id ? -1 : 1;
			});
			int[] sortedidx = idx.ConvertAll<int>((Device d) => ((int)d.Id)).ToArray();
			var id = CSycles.create_multidevice(sortedidx.Length, ref sortedidx);
			if (id < 0) return null;
			return GetDevice(id);
		}

		/// <summary>
		/// Parse given string into a list of integers.
		///
		/// The integers should correspond to indices of devices in the
		/// global list of regular render devices.
		/// </summary>
		/// <param name="res"></param>
		/// <returns>
		/// Sorted List of indices into global list of regular render devices.
		/// </returns>
		static public List<int> IdListFromString(string res)
		{
			var l = IdSetFromString(res).ToList();
			l.Sort();
			return l;
		}

		/// <summary>
		/// Tell if given device id string constitutes a valid combo (do all device ids exist).
		/// </summary>
		/// <param name="res"></param>
		/// <returns></returns>
		static public string ValidDeviceString(string res)
		{
			if (res.Equals("-1")) return res;
			var l = IdListFromString(res);

			var existing_l = from dev in Devices select dev.Id;

			var x = from id in l where existing_l.Contains((uint)id) select id;

			return string.Join(",", x.ToList());
		}

		/// <summary>
		/// Generate string from device ID or IDs (in case of multidevice).
		///
		/// The resulting string can be used as input to DeviceFromString().
		/// </summary>
		public string DeviceString
		{
			get
			{
				if (!IsMulti) return $"{Id}";
				return string.Join(", ", (from sd in Subdevices select sd.Id).ToList());
			}
		}
		/// <summary>
		/// Parse given string into a set of integers.
		/// </summary>
		/// <param name="res"></param>
		/// <returns>
		/// HashSet of indices into global list of regular render devices.
		/// </returns>
		static public HashSet<int> IdSetFromString(string res)
		{
			var cleaned = res.Trim().ToLowerInvariant().Split(',');
			HashSet<int> set = new HashSet<int>();
			foreach (var item in cleaned)
			{
				if (int.TryParse(item, out int x))
				{
					if (x > -1)
					{
						set.Add(x);
					}
				}
			}
			return set;
		}

		/// <summary>
		/// Convert a list of id integers to a Device list.
		///
		/// The ids are indices into the global list of regular devices,
		/// i.e. used with GetDevice(int idx);
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		static public List<Device> DeviceListFromIntList(List<int> ids)
		{
			// TODO filter out first all ids that do not
			// match an existing device. This can happen due to hardware
			// configuration changes where an ID no longer is valid
			// after an ID was selected
			return ids.ConvertAll((int id) => GetDevice(id));
		}

		/// <summary>
		/// Get the device for given string. If the string doesn't parse correctly
		/// the first Cuda device (or Cpu if no Cuda device was found) is picked.
		/// </summary>
		/// <param name="res"></param>
		/// <returns></returns>
		static public Device DeviceFromString(string res)
		{
			var l = IdListFromString(res);

			if (l.Count == 0) return FirstGpu;
			if (l.Count == 1)
			{
				return l[0] == -1 ? FirstCuda : GetDevice(l[0]);
			}

			return CreateMultiDevice(DeviceListFromIntList(l));
		}
	}
}
