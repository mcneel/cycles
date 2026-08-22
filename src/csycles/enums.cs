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
		/* 5.x inserted HIPRT here. Leaving it out put Metal, OneApi and Dummy one
		 * short of their ccl::DeviceType values, so DeviceTypeMask.METAL was really
		 * HIPRT's bit and ONEAPI was Metal's. */
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
	/// <summary>
	/// Sampling patterns. Mirrors ccl::SamplingPattern in kernel/types.h.
	///
	/// The old members were Sobol = 0 and CMJ = 1. Correlated multi-jitter is
	/// long gone from Cycles, and both names were passed straight across the C
	/// API as their numbers - so "CMJ" was really asking for tabulated Sobol and
	/// had been for some time. The values are unchanged, only the names now say
	/// what they select.
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

	public enum BvhType : uint
	{
		Dynamic,
		Static
	}

	/// <summary>
	/// BVH layouts. Mirrors ccl::KernelBVHLayout in kernel/types.h (ccl::BVHLayout
	/// is an alias of it).
	///
	/// Cycles dropped the wide BVH builders, so Bvh4 and Bvh8 no longer exist.
	/// They used to sit on bits 1 and 2, which upstream now uses for Embree and
	/// OptiX - so the old Default, being Bvh8, was asking for BVH_LAYOUT_OPTIX,
	/// and the old OptiX was asking for MULTI_OPTIX_EMBREE. The only caller is
	/// behind #if LEGACY and is not compiled, so nothing was misbuilding.
	/// </summary>
	[Flags]
	public enum BvhLayout : uint
	{
		None = 0,

		Bvh2 = (1 << 0),
		Embree = (1 << 1),
		OptiX = (1 << 2),
		MultiOptiX = (1 << 3),
		MultiOptiXEmbree = (1 << 4),
		Metal = (1 << 5),
		MultiMetal = (1 << 6),
		MultiMetalEmbree = (1 << 7),
		Hiprt = (1 << 8),
		MultiHiprt = (1 << 9),
		MultiHiprtEmbree = (1 << 10),
		EmbreeGpu = (1 << 11),
		MultiEmbreeGpu = (1 << 12),
		MultiEmbreeGpuEmbree = (1 << 13),

		/* Default BVH layout to use for CPU. */
		Auto = Embree,
		All = Bvh2 | Embree | OptiX | Metal | Hiprt | MultiHiprt | MultiHiprtEmbree |
		      EmbreeGpu | MultiEmbreeGpu | MultiEmbreeGpuEmbree,
	}

	public enum CameraType : uint
	{
		Perspective,
		Orthographic,
		Panorama,
		Custom,
	}

	public enum PanoramaType : uint
	{
		Equirectangular,
		FisheyeEquidistant,
		FisheyeEquisolid,
		MirrorBall,
		FisheyeLensPolynomial,
		EquiangularCubemapFace,
		CentralCylindrical
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
	/// Object and background ray visibility. Mirrors ccl::PathRayVisibilityFlag in
	/// kernel/types.h.
	///
	/// Up to Cycles 3.x this enum doubled as the path flags, so it ran to 20 bits
	/// and mixed in Reflect, Singular, Transparent, Curve and the catcher/non-catcher
	/// shadow split. 5.x split visibility off into its own seven-bit enum and
	/// Object::visibility_for_tracing() now asserts that nothing outside
	/// AllVisibility is set, so the retired bits cannot simply be left in place.
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

		/* Denoising passes. These moved out of the data range in 4.x; csycles had
		 * them inline after Mist, which put every later entry on the wrong value. */
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
