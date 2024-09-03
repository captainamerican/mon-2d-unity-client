using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class HeadBodyPartEntry : BodyPartEntry {

		// -------------------------------------------------------------------------

		public HeadBodyPart BodyPart {
			get {
				return Database.Engine.GameData.Get<HeadBodyPart>(BodyPartId);
			}
		}

		public int MaxSkills {
			get {
				return 1 + Grade;
			}
		}

		public int Grade {
			get {
				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}

		// -------------------------------------------------------------------------

	}
}
