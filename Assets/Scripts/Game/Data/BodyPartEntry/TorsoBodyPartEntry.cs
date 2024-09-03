using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class TorsoBodyPartEntry : BodyPartEntry {

		// -------------------------------------------------------------------------

		public TorsoBodyPart BodyPart {
			get {
				return Database.Engine.GameData.Get<TorsoBodyPart>(BodyPartId);
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
