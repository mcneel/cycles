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
	/// The background representation contains a background shader
	/// and settings to control Ambient Occlusion and path ray visibility
	/// </summary>
	public class Background
	{
		/// <summary>
		/// Reference to scene for which this the background is
		/// </summary>
		internal Session Session { get; set; }

		/// <summary>
		/// Create a background representation for scene. Generally this
		/// is handled by the Session, therefor this is an internally visible
		/// constructor
		/// </summary>
		/// <param name="scene">Session for which to create background</param>
		internal Background(Session scene)
		{
			Session = scene;
		}

		Shader bgShader;
		/// <summary>
		/// Get or set the background shader
		/// </summary>
		public Shader Shader
		{
			set
			{
				bgShader = value;
				CSycles.scene_set_background_shader(Session.Scene.Id, value.Id);
			}
			get
			{
				if (bgShader == null)
				{
					var shid = CSycles.scene_get_background_shader(Session.Scene.Id);
					bgShader = new Shader(Session.Scene, shid);
				}
				return bgShader;
			}
		}

		/// <summary>
		/// Set the rays to which the background is responsive
		/// </summary>
		public PathRay Visibility
		{
			set
			{
				CSycles.scene_set_background_visibility(Session.Scene.Id, value);
			}
		}

		/// <summary>
		/// Set to true if you want the output with bg transparent.
		/// </summary>
		public bool Transparent
		{
			set
			{
				CSycles.scene_set_background_transparent(Session.Scene.Id, value);
			}
		}
	}
}
