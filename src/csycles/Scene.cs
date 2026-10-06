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
using System.Threading;

namespace ccl
{
	/// <summary>
	/// The Cycles scene representation
	/// </summary>
	public class Scene
	{
		/// <summary>
		/// Get the ID of the created Session as given by CCycles
		/// </summary>
		public IntPtr Id { get; private set; }

		/// <summary>
		/// Access to the Camera for this Session
		/// </summary>
		public Camera Camera { get; private set; }

		/// <summary>
		/// Access to the Integrator settings for this Session
		/// </summary>
		public Integrator Integrator { get; private set; }

		/// <summary>
		/// Access to the Bacground settings for this Session
		/// </summary>
		public Background Background { get; private set; }

		/// <summary>
		/// Access to the Film for this Session
		/// </summary>
		public Film Film { get; private set; }

		/// <summary>
		/// Access to the Device used for this Session
		/// </summary>
		public Device Device { get; private set; }

		/// <summary>
		/// Wrap the scene that the ccl::Session constructor created.
		/// </summary>
		/// <param name="session">The Session to create scene for</param>
		public Scene(Session session)
		{
			// for now use Session.Id as Session.Id too, since they are now tightly coupled in Cycles
			Id = session.Id;
			Camera = new Camera(session);
			Integrator = new Integrator(session);
			Film = new Film(session);
			Background = new Background(session);
		}

		/// <summary>
		/// Reset the scene, forcing update and device update in Cycles.
		/// </summary>
		public void Reset()
		{
			CSycles.scene_reset(Id);
		}

		/// <summary>
		/// Try aqcuire lock on scene mutex non-blocking.
		/// </summary>
		/// <returns>True if lock was acquired, false otherwise</returns>
		public bool TryLock()
		{
			return CSycles.scene_try_lock(Id);
		}

		/// <summary>
		/// Wait until lock on scene is acquired. While
		/// acquire fails sleep for 10 milliseconds and
		/// try again
		/// </summary>
		public void WaitUntilLocked()
		{
			while (!TryLock())
			{
				Thread.Sleep(10);
			}
		}

		/// <summary>
		/// Release lock.
		/// </summary>
		public void Unlock()
		{
			CSycles.scene_unlock(Id);
		}

		public void ClearClippingPlanes()
		{
			CSycles.scene_clear_clipping_planes(Id);
		}
	}
}
