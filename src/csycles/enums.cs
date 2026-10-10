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

	public static class Constants
	{
		public const string ccycles = "ccycles";
	}
	/// <summary>
	/// Device types; mirrors ccl::DeviceType.
	/// </summary>
	public enum DeviceType : uint
	{
		None,
		Cpu,
		Cuda,
		Multi,
		Optix,
		Hip,
		/* 5.x inserted HIPRT; without it every later value is one short of ccl::DeviceType. */
		Hiprt,
		Metal,
		OneApi,
		Dummy,
	}

	/// <summary>
	///  Device type mask used for Cycles initialisation
	/// </summary>
	public enum DeviceTypeMask : uint
	{
		CPU = (1 << (int)DeviceType.Cpu),
		CUDA = (1 << (int)DeviceType.Cuda),
		OPTIX = (1 << (int)DeviceType.Optix),
		HIP = (1 << (int)DeviceType.Hip),
		METAL = (1 << (int)DeviceType.Metal),
		ONEAPI = (1 << (int)DeviceType.OneApi),
		All = 0xFFFFFFFF
	}

	/// <summary>
	/// Why OptiX offered no devices after initialisation (cycles_optix_init_result).
	/// </summary>
	public enum OptixInitResult
	{
		/// <summary>OptiX started, or was never tried because there is no NVIDIA driver.</summary>
		Ok = 0,
		/// <summary>The NVIDIA driver is older than the OptiX SDK ccycles was built with supports.</summary>
		DriverTooOld = 1,
		/// <summary>OptiX failed to start for another reason.</summary>
		Failed = 2,
	}

	/// <summary>
	/// Shading systems available in Cycles; C[CS]?ycles supports only SVM.
	/// </summary>
	public enum ShadingSystem : uint
	{
		OSL,
		SVM
	}

	/// <summary>
	/// Sampling patterns; mirrors ccl::SamplingPattern in kernel/types.h. Formerly Sobol = 0 and
	/// CMJ = 1: same values, renamed to what they select.
	/// </summary>
	public enum SamplingPattern : uint
	{
		SobolBurley = 0,
		TabulatedSobol = 1,
		BlueNoisePure = 2,
		BlueNoiseFirst = 3,
		BlueNoiseRound = 4,
		Automatic = 5,
	}

	public enum CameraType : uint
	{
		Perspective,
		Orthographic,
		Panorama,
		Custom,
	}

	public enum FilterType : uint
	{
		Box = 0,
		Gaussian = 1,
		BlackmanHarris = 2,
	}

	public enum LightType : uint
	{
		Point = 0,
		Distant,
		Background,
		Area,
		Spot,
		Triangle,
	}

	public enum InterpolationType : int
	{
		None = -1,
		Linear = 0,
		Closest = 1,
		Cubic = 2,
		Smart = 3,
	}

	public enum DecalDirection
	{
		Both = 0,
		Forward = 1,
		Backward = 2,
	}


	/// <summary>
	/// Object and background ray visibility; mirrors ccl::PathRayVisibilityFlag in kernel/types.h.
	/// 5.x split it from the 3.x path flags into seven bits, and Object::visibility_for_tracing()
	/// asserts that nothing outside AllVisibility is set.
	/// </summary>
	[Flags]
	public enum PathRay : uint
	{
		Hidden = 0,

		Camera = 1 << 0,
		Transmit = 1 << 1,
		Diffuse = 1 << 2,
		Glossy = 1 << 3,
		VolumeScatter = 1 << 4,

		ShadowOpaque = 1 << 5,
		ShadowTransparent = 1 << 6,
		Shadow = (ShadowOpaque | ShadowTransparent),

		AllVisibility = ((1 << 7) - 1),

		/* Only ever set on a BVH node, never on an object or the background. */
		NodeUnaligned = 1 << 15,
	}

	/// <summary>
	/// Render passes. Mirrors ccl::PassType in kernel/types.h - the numbering is
	/// what crosses the C API, so it has to match entry for entry.
	/// </summary>
	public enum PassType : int
	{
		None = 0,

		/* Light passes */
		Combined = 1,
		Emission,
		Background,
		Ao,
		Diffuse,
		DiffuseDirect,
		DiffuseIndirect,
		Glossy,
		GlossyDirect,
		GlossyIndirect,
		Transmission,
		TransmissionDirect,
		TransmissionIndirect,
		Volume,
		VolumeDirect,
		VolumeIndirect,
		VolumeScatter,
		VolumeTransmit,
		CategoryLightEnd = 31,

		/* Data passes */
		Depth = 32,
		Position,
		Normal,
		Roughness,
		Uv,
		ObjectId,
		MaterialId,
		Motion,
		MotionWeight,
		CryptoMatte,
		AovColor,
		AovValue,
		AdaptiveAuxBuffer,
		SampleCount,
		ShadowCatcherTransparentSampleCount,
		ShadowCatcherBackgroundSampleCount,
		DiffuseColor,
		GlossyColor,
		TransmissionColor,
		Mist,
		RenderTime,
		ShadowCatcher,
		ShadowCatcherSampleCount,
		ShadowCatcherMatte,
		GuidingColor,
		GuidingProbability,
		GuidingAvgRoughness,
		VolumeMajorant,
		VolumeMajorantSampleCount,
		CategoryDataEnd = 63,

		/* Denoising passes; out of the data range since 4.x. */
		DenoisingAlbedo = 64,
		DenoisingSpecularAlbedo,
		DenoisingNormal,
		DenoisingRoughness,
		DenoisingDepth,
		DenoisingBackwardMotion,
		CategoryDenoisingEnd = 95,

		BakePrimitive = 96,
		BakeSeed,
		BakeDifferential,
		CategoryBakeEnd = 127,

		DenoisingPrevious,

		Num
	}
}
