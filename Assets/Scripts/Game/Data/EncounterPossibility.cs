using System;

namespace Game {
	[Serializable]
	public class EncounterPossibility {
		public int Weight = 100;
		public int Level = 1;
		public ConstructedCreature Creature;
	}
}
