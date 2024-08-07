using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Party {
	public List<Creature> Creatures = new();

	public int AvailableToFight {
		get {
			return Creatures.Sum(creature => creature.Health > 0 ? 1 : 0);
		}
	}
}
