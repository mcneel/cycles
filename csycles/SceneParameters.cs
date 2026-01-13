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

namespace ccl
{
	/// <summary>
	/// Wrapper for creating scene parameters.
	/// 
	/// Note that this is only used for *setting*, not fetching parameters.
	/// 
	/// BVH stands for Bound Volume Hierarchy (https://en.wikipedia.org/wiki/Bounding_volume_hierarchy)
	/// </summary>
	public class SceneParameters
	{
		/// <summary>
		/// Get the Id for these scene parameters.
		/// </summary>
		public uint Id { get; }

		private Client Client { get; }

		/// <summary>
		/// Create scene parameters object with given parameters
		/// </summary>
		/// <param name="client">Reference to the client</param>
		/// <param name="shadingSystem">ShadingSystem to use</param>
		/// <param name="bvhType">BvhType to use (dynamic or static)</param>
		/// <param name="bvhCache">True if BVH should be cached</param>
		/// <param name="bvhSpatialSplit">True if BVH spatial splits should be used</param>
		/// <param name="bvhLayout">BvhLayout to use. Default means Bvh8</param>
		/// <param name="persistentData">True if data should be persisted</param>
		public SceneParameters(Client client, ShadingSystem shadingSystem, BvhType bvhType, bool bvhSpatialSplit, BvhLayout bvhLayout, bool persistentData)
		{
			Client = client;
			Id = CSycles.scene_params_create(shadingSystem, bvhType, bvhSpatialSplit, bvhLayout, persistentData);
		}

		/// <summary>
		/// Set the <c>BvhType</c> to be used
		/// </summary>
		public BvhType BvhType
		{
			set
			{
				CSycles.scene_params_set_bvh_type(Id, value);
			}
		}

		/// <summary>
		/// Set to true if BVH should use spatial splits.
		/// </summary>
		public bool BvhSpatialSplit
		{
			set
			{
				CSycles.scene_params_set_bvh_spatial_split(Id, value);
			}
		}

		/// <summary>
		/// Set to true if QBVH should be used.
		/// </summary>
		public bool QBvh
		{
			set
			{
				CSycles.scene_params_set_qbvh(Id, value);
			}
		}

		/// <summary>
		/// Set the <c>ShadingSystem</c> to use.
		/// 
		/// Note that currently only <c>ShadingSystem.SVM</c> is supported.
		/// </summary>
		public ShadingSystem ShadingSystem
		{
			set
			{
				CSycles.scene_params_set_shadingsystem(Id, value);
			}
		}

		/// <summary>
		/// Set to true if BVH should be persisted between frames.
		/// </summary>
		public bool PersistentData
		{
			set
			{
				CSycles.scene_params_set_persistent_data(Id, value);
			}
		}
	}
}
