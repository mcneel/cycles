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

namespace ccl
{
	/// <summary>
	/// The Cycles session representation.
	///
	/// The Session is used to manage the render process at the highest level. Here one can set
	/// the different callbacks to retrieve render results and updates and statistics during a render.
	///
	/// A session is created using SessionParameters and a Session.
	///
	/// Through Session the render process is started and ended.
	/// </summary>
	public class Session : IDisposable
	{
		/// <summary>
		/// True if the session has already been destroyed.
		/// </summary>
		private bool Destroyed;

		private Scene sc;
		/// <summary>
		/// Get or set the Session used for this Session
		/// </summary>
		public Scene Scene
		{
			get { return sc; }
			private set
			{
				sc = value;
			}
		}
		/// <summary>
		/// Get the SessionParams used for this Session
		/// </summary>
		public SessionParameters SessionParams { get; private set; }
		/// <summary>
		/// Get the ID for this session
		/// </summary>
		public IntPtr Id { get; }

		/// <summary>
		/// Create a new session using the given SessionParameters and Session
		/// </summary>
		/// <param name="sessionParams">Previously created SessionParameters to create Session with</param>
		public Session(SessionParameters sessionParams)
		{
			SessionParams = sessionParams;
			Id = CSycles.session_create(sessionParams.Id);
			Scene = new Scene(this);
		}

		public void WaitUntilLocked()
		{
			if (Destroyed) return;
			Scene.WaitUntilLocked();
		}

		public void Unlock()
		{
			if (Destroyed) return;
			Scene.Unlock();
		}

		/// <summary>
		/// Start the rendering session. After this one should Wait() for
		/// the session to complete.
		/// </summary>
		public void Start()
		{
			if (Destroyed) return;
			CSycles.progress_reset(Id);
			CSycles.session_start(Id);
		}

		/// <summary>
		/// Sample one pass. Return -1 when no pass was rendered, zero or positive when something was rendered.
		/// </summary>
		public int Sample()
		{
			if (Destroyed) return -1;
			return CSycles.session_sample(Id);
		}

		/// <summary>
		/// Wait for the rendering session to complete
		/// </summary>
		public void Wait()
		{
			if (Destroyed) return;
			CSycles.session_wait(Id);
		}

		/// <summary>
		/// Cancel the currently in progress render
		/// </summary>
		/// <param name="cancelMessage"></param>
		public void Cancel(string cancelMessage)
		{
			if (Destroyed) return;
			CSycles.session_cancel(Id, cancelMessage);
		}

		public void QuickCancel()
		{
			if (Destroyed) return;
			CSycles.session_quickcancel(Id);
		}

		/// <summary>
		/// Destroy the session and all related.
		/// </summary>
		public void Destroy()
		{
			if (Destroyed) return;
			CSycles.session_destroy(Id);
		}

		public void GetPixelBuffer(PassType pt, ref IntPtr pixel_buffer)
		{
			if (Destroyed)
			{
				pixel_buffer = IntPtr.Zero;
			}
			else
			{
				CSycles.session_get_float_buffer(Id, pt, ref pixel_buffer);
			}
		}

		public void RetainPixelBuffer(PassType pt, int width, int height, ref IntPtr pixel_buffer, ref int pixel_size_from_cycles)
		{
			if (Destroyed)
			{
				pixel_buffer = IntPtr.Zero;
				pixel_size_from_cycles = 0;
			}
			else
			{
				CSycles.session_retain_float_buffer(Id, pt, width, height, ref pixel_buffer, ref pixel_size_from_cycles);
			}
		}

		public void ReleasePixelBuffer(PassType pt)
		{
			if (!Destroyed)
			{
				CSycles.session_release_float_buffer(Id, pt);
			}
		}


		/// <summary>
		/// Reset a Session
		/// </summary>
		/// <param name="width">Width of the resolution to reset with</param>
		/// <param name="height">Height of the resolutin to reset with</param>
		/// <param name="samples">The amount of samples to reset with</param>
		/// <returns>0 on success. -1 when the session is already destroyed. -13 when a crash happened.</returns>
		public int Reset(int width, int height, int samples, int full_x, int full_y, int full_width, int full_height, int pixel_size)
		{
			if (Destroyed) return -1;
			CSycles.progress_reset(Id);
			return CSycles.session_reset(Id, width, height, samples, full_x, full_y, full_width, full_height, pixel_size);
		}

		/// <summary>
		/// Pause or un-pause a render session.
		/// </summary>
		/// <param name="pause"></param>
		public void SetPause(bool pause)
		{
			if (Destroyed) return;
			CSycles.session_set_pause(Id, pause);
		}

		/// <summary>
		/// Set sample count for session to render. This can be used to increase the sample
		/// count for an interactive render session.
		/// </summary>
		/// <param name="samples"></param>
		public void SetSamples(int samples)
		{
			if (Destroyed) return;
			CSycles.session_set_samples(Id, samples);
		}


		public IEnumerable<PassType> Passes
		{
			get
			{
				foreach (var pass in _passes)
				{
					yield return pass;
				}
			}
		}
		private List<PassType> _passes = new List<PassType>();

		/// <summary>
		/// Add given pass to output layers
		/// </summary>
		/// <param name="pass"></param>
		public void AddPass(PassType pass)
		{
			if (Destroyed) return;
			CSycles.session_add_pass(Id, pass);
			if (!_passes.Contains(pass))
			{
				_passes.Add(pass);
			}
		}

		/// <summary>
		/// Clear all passes for session.
		/// </summary>
		public void ClearPasses()
		{
			if (Destroyed) return;
			_passes.Clear();
			CSycles.session_clear_passes(Id);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!Destroyed)
			{
				QuickCancel();
				Destroy();
				Destroyed = true;
			}
		}

		~Session()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
