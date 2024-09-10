using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game {
	[Serializable]
	public class EncounterPossibility {
		public int Weight = 100;
		public Creature Creature;
		public List<WeightedLootDrop> PossibleLoot = new();
	}
}
