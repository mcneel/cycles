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
	/// Film representation in a Cycles session.
	/// </summary>
	public class Film
	{
		/// <summary>
		/// Reference to the session in which this film is contained.
		/// </summary>
		internal Session Session { get; set; }

		/// <summary>
		/// Create a new film representation for Session.
		/// </summary>
		/// <param name="session"></param>
		internal Film(Session session)
		{
			Session = session;
		}

		/// <summary>
		/// Set film exposure
		/// </summary>
		public float Exposure
		{
			set
			{
				CSycles.film_set_exposure(Session.Id, value);
			}
		}

		/// <summary>
		/// Set film filter type and width
		/// </summary>
		/// <param name="filterType">Box or Gaussian</param>
		/// <param name="filterWidth">for proper Box use 1.0f</param>
		public void SetFilter(FilterType filterType, float filterWidth)
		{
			CSycles.film_set_filter(Session.Id, filterType, filterWidth);
		}

		/// <summary>
		/// Tag the film for update.
		/// </summary>
		public void Update()
		{
			CSycles.film_tag_update(Session.Id);
		}
	}
}
