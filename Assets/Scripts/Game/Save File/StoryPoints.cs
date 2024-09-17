using System;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class StoryPoint {
		public StoryPointId Id;
		public float Value;
	}

	[Serializable]
	public class StoryPoints {

		// -------------------------------------------------------------------------

		[SerializeField] List<StoryPoint> storyPoints = new();

		Dictionary<StoryPointId, StoryPoint> storyPointsById = new();

		void Rebuild() {
			storyPointsById.Clear();
			storyPoints.ForEach(storyPoint => {
				storyPointsById.Add(storyPoint.Id, storyPoint);
			});
		}

		void RebuildIfNecessary() {
			if (storyPointsById.Count > 0) {
				return;
			}

			//
			Rebuild();
		}

		public bool Has(StoryPointId id) {
			RebuildIfNecessary();

			//
			return storyPointsById.ContainsKey(id);
		}

		public float Get(StoryPointId id) {
			RebuildIfNecessary();

			//
			return Has(id) ? storyPointsById[id].Value : 0;
		}

		StoryPoint GetRaw(StoryPointId id) {
			RebuildIfNecessary();

			//
			return Has(id)
				? storyPointsById[id]
				: new() {
					Id = id
				};
		}

		public float Adjust(StoryPointId id, float value) {
			StoryPoint storyPoint = GetRaw(id);

			//
			storyPoint.Value += value;
			storyPointsById[id] = storyPoint;

			//
			if (!storyPoints.Contains(storyPoint)) {
				storyPoints.Add(storyPoint);
			}

			//
			return storyPoint.Value;
		}

		public float Set(StoryPointId id, float value) {
			StoryPoint storyPoint = GetRaw(id);

			//
			storyPoint.Value = value;
			storyPointsById[id] = storyPoint;

			//
			if (!storyPoints.Contains(storyPoint)) {
				storyPoints.Add(storyPoint);
			}

			//
			return storyPoint.Value;
		}

		// just a boolean
		public void Add(StoryPointId id) {
			Set(id, 1);
		}

		// -------------------------------------------------------------------------

	}
}