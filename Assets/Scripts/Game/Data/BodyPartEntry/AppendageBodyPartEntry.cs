using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class AppendageBodyPartEntry : BodyPartEntry {

		// ------------------------------------------------------------------------- 

		public AppendageBodyPart BodyPart {
			get {
				return Database.Engine.GameData.Get<AppendageBodyPart>(BodyPartId);
			}
		}

		public int Grade {
			get {
				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = toLevel > 0
					? Mathf.Clamp(3f * (experience / (float) (toLevel * 3f)), 0, 3)
					: 0;

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}

		// -------------------------------------------------------------------------

	}
}
