using System;

using UnityEngine;
// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class TailBodyPartEntry : BodyPartEntry {

		// -------------------------------------------------------------------------

		public TailBodyPart BodyPart {
			get {
				return Database.Engine.GameData.Get<TailBodyPart>(BodyPartId);
			}
		}

		public int Grade {
			get {

				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * (experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}

		public float GradeAsAdjustment {
			get {
				return Mathf.Clamp(1 + (Grade * 0.333334f), 1, 2);
			}
		}

		// -------------------------------------------------------------------------

	}
}
