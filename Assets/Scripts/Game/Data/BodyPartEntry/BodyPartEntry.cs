using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class BodyPartEntry {

		// -------------------------------------------------------------------------

		public string Id = Game.Id.Generate();
		public BodyPartId BodyPartId;
		public int Experience = 0;

		[Range(0, 1)]
		public float Quality = 1;

		// -------------------------------------------------------------------------

	}
}
