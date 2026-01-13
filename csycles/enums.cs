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
	/// Device types that Cycles can support.
	///
	/// Note that currently focus is on Cpu and Cuda
	/// and Cuda Multi support, but others can be
	/// added when needed and possible
	/// </summary>
	public enum DeviceType : uint
	{
		None,
		Cpu,
		Cuda,
		Multi,
		Optix,
		Hip,
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
	/// Shading systems available in Cycles.
	///
	/// Note that currently only SVM is supported
	/// in C[CS]?ycles
	/// </summary>
	public enum ShadingSystem : uint
	{
		OSL,
		SVM
	}

	/// <summary>
	/// Integration method used for ray casting.
	/// </summary>
	public enum IntegratorMethod : int
	{
		/// <summary>
		/// On each hit rays get split up for all possible equivalents
		/// </summary>
		BranchedPath = 0,
		Path
	}

	/// <summary>
	/// Sampling patterns available in
	/// Cycles.
	/// </summary>
	public enum SamplingPattern : uint
	{
		Sobol = 0,
		CMJ
	}

	public enum BvhType : uint
	{
		Dynamic,
		Static
	}

	public enum BvhLayout
	{

		Bvh2 = (1 << 0),
		Bvh4 = (1 << 1),
		Bvh8 = (1 << 2),

		Embree = (1 << 3),
		OptiX = (1 << 4),

		Default = Bvh8,
	}

	public enum CameraType : uint
	{
		Perspective,
		Orthographic,
		Panorama
	}

	public enum PanoramaType : uint
	{
		Equirectangular,
		FisheyeEquidistant,
		FisheyeEquisolid
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


	[Flags]
	public enum PathRay : uint
	{
		Hidden = 0,
		Camera = 1 << 0,
		Reflect = 1 << 1,
		Transmit = 1 << 2,
		Diffuse = 1 << 3,
		Glossy = 1 << 4,
		Singular = 1 << 5,
		Transparent = 1 << 6,

		ShadowOpaqueNonCatcher = 1 << 7,
		ShadowOpaqueCatcher = 1 << 8,
		ShadowOpaque = (ShadowOpaqueNonCatcher | ShadowOpaqueCatcher),
		ShadowTransparentNonCatcher = 1 << 9,
		ShadowTransparentCatcher = 1 << 10,
		ShadowTransparent = (ShadowTransparentNonCatcher | ShadowTransparentCatcher),
		ShadowNonCatcher = (ShadowOpaqueNonCatcher | ShadowTransparentNonCatcher),
		Shadow = (ShadowOpaque | ShadowTransparent),

		Curve = 1 << 11, /* visibility flag to define curve segments*/
		VolumeScatter = 1 << 12,

		NodeUnaligned = 1 << 13,

		/* note that these can use maximum 12 bits, the other are for layers */

		AllVisibility = ((1 << 14) - 1),

		MisSkip = 1 << 15,
		DiffuseAncestor = 1 << 16,
		SinglePassDone = 1 << 17,
		ShadowCatcher = 1 << 18,
		StoreShadowInfo = 1 << 19,

		/* we need layer member flags to be the 20 upper bits */
		LayerShift = (32 - 20)
	}

	public enum PassType : int
	{
		None = 0,

		/* Light Passes */
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
		DenoisingNormal,
		DenoisingAlbedo,
		DenoisingDepth,
		DenoisingPrevious,
		ShadowCatcher,
		ShadowCatcherSampleCount,
		ShadowCatcherMatte,
		GuidingColor,
		GuidingProbability,
		GuidingAvgRoughness,
		CategoryDataEnd = 63,

		BakePrimitive,
		BakeDifferential,
		CategoryBakeEnd = 95,

		Num
	}
}
