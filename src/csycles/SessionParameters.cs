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

namespace ccl
{
	/// <summary>
	/// Wrapper for creating and setting session parameters; it cannot fetch them.
	/// </summary>
	public class SessionParameters
	{
		private Device _device;
		/// <summary>
		/// Get the ID for the session parameters.
		/// </summary>
		public IntPtr Id { get; }
		/// <summary>
		/// Create session parameters using <c>Device</c>
		/// </summary>
		/// <param name="dev">The device to create session parameters for</param>
		public SessionParameters(Device dev)
		{
			_device = dev;
			Id = CSycles.session_params_create((uint)dev.Id);
		}

		/// <summary>
		/// The device the session will be created on.
		/// </summary>
		public Device Device
		{
			get => _device;
			set
			{
				_device = value;
				CSycles.session_params_set_device(Id, (uint)_device.Id);
			}
		}

		/// <summary>
		/// Set to true if background rendering is wanted
		/// </summary>
		public bool Background
		{
			set
			{
				CSycles.session_params_set_background(Id, value);
			}
		}

		/// <summary>
		/// Set to true if experimental shading features should be used
		/// </summary>
		public bool Experimental
		{
			set
			{
				CSycles.session_params_set_experimental(Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples to render
		/// </summary>
		public int Samples
		{
			set
			{
				CSycles.session_params_set_samples(Id, value);
			}
		}

		/// <summary>
		/// Set the Size of a tile used during rendering
		/// </summary>
		public int TileSize
		{
			set
			{
				CSycles.session_params_set_tile_size(Id, (uint)value);
			}
		}

		/// <summary>
		/// The number of Cpu threads to render with; 0 sets it automatically from the logical cores.
		/// </summary>
		public uint Threads
		{
			set
			{
				CSycles.session_params_set_threads(Id, value);
			}
		}

		/// <summary>
		/// Set which ShadingSystem should be used; only SVM is supported.
		/// </summary>
		public ShadingSystem ShadingSystem
		{
			set
			{
				CSycles.session_params_set_shadingsystem(Id, value);
			}
		}

		/// <summary>
		/// Set the pixel size to use in this render session.
		/// </summary>
		public int PixelSize
		{
			set
			{
				CSycles.session_params_set_pixel_size(Id, (uint)value);
			}
		}

		/// <summary>
		/// Set to true to use resolution division during initial stages of raytracing
		/// </summary>
		public bool UseResolutionDivider
		{
			set
			{
				CSycles.session_params_set_use_resolution_divider(Id, value);
			}
		}
	}
}
