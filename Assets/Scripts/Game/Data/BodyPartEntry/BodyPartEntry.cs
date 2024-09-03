using System;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class BodyPartEntry {

		// -------------------------------------------------------------------------

		public string Id = Game.Id.Generate();
		public BodyPartId BodyPartId;
		public int Experience = 0;
		public float Quality = 1;

		// -------------------------------------------------------------------------

	}
}
