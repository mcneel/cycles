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
	/// Cycles light representation.
	/// </summary>
	public class Light
	{
		/// <summary>
		/// Id of this light after creation.
		/// </summary>
		public IntPtr Id { get; internal set; }
		/// <summary>
		/// Reference to scene in which light was created.
		/// </summary>
		public Scene Scene { get; internal set; }
		/// <summary>
		/// Reference to client for which light was created.
		/// </summary>
		public Session Client { get; internal set; }
		public Shader Shader { get; set; }
		/// <summary>
		/// Create a new light.
		///
		/// This constructor creates a Cycles light right away. The Id
		/// is from Cycles
		/// </summary>
		/// <param name="client"></param>
		/// <param name="scene"></param>
		/// <param name="lightShader"></param>
		public Light(Session client, Scene scene, Shader lightShader)
		{
			Client = client;
			Scene = scene;
			Shader = lightShader;
			Id = CSycles.create_light(Scene.Id, lightShader.Id);
		}

		/// <summary>
		/// Tag light for update on device
		/// </summary>
		public void TagUpdate()
		{
			CSycles.light_tag_update(Scene.Id, Id);
		}

		/// <summary>
		/// Set type of light.
		/// </summary>
		public LightType Type
		{
			set
			{
				CSycles.light_set_type(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true to use multiple importance sampling.
		/// </summary>
		public bool UseMis
		{
			set
			{
				CSycles.light_set_use_mis(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this light should cast shadows.
		/// </summary>
		public bool CastShadow
		{
			set
			{
				CSycles.light_set_cast_shadow(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set sample count for light.
		/// </summary>
		public uint Samples
		{
			set
			{
				CSycles.light_set_samples(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces this light contributes per ray.
		/// </summary>
		public uint MaxBounces
		{
			set
			{
				CSycles.light_set_max_bounces(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the map resolution to be used with multiple importance sampling.
		/// </summary>
		public uint MapResolution
		{
			set
			{
				CSycles.light_set_map_resolution(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the soft shadow size (larger generally means fewer fireflies).
		/// </summary>
		public float Size
		{
			set
			{
				CSycles.light_set_size(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the angular diameter (used for sun/distant light).
		/// </summary>
		public float Angle
		{
			set
			{
				CSycles.light_set_angle(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the location.
		/// </summary>
		public float4 Location
		{
			set
			{
				CSycles.light_set_co(Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		/// <summary>
		/// Set the direction.
		/// </summary>
		public float4 Direction
		{
			set
			{
				CSycles.light_set_dir(Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		/// <summary>
		/// Set the U size (rectangular lights)
		/// </summary>
		public float SizeU
		{
			set
			{
				CSycles.light_set_sizeu(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the V size (rectangular lights)
		/// </summary>
		public float SizeV
		{
			set
			{
				CSycles.light_set_sizev(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the direction of the U axis (rectangular lights)
		/// </summary>
		public float4 AxisU
		{
			set
			{
				CSycles.light_set_axisu(Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		/// <summary>
		/// Set the direction of the V axis (rectangular lights)
		/// </summary>
		public float4 AxisV
		{
			set
			{
				CSycles.light_set_axisv(Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		/// <summary>
		/// Set the angle for spot light.
		/// </summary>
		public float SpotAngle
		{
			set
			{
				CSycles.light_set_spot_angle(Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the blend from light to shadow factor for spot light.
		/// </summary>
		public float SpotSmooth
		{
			set
			{
				CSycles.light_set_spot_smooth(Scene.Id, Id, value);
			}
		}
	}
}
