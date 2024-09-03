using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------

public class EditingCreature {

	// ---------------------------------------------------------------------------

	public bool IsNew;

	public Game.Creature Creature;
	public Game.Creature Original;

	public List<Game.HeadBodyPartEntry> AvailableHead;
	public List<Game.TorsoBodyPartEntry> AvailableTorso;
	public List<Game.TailBodyPartEntry> AvailableTail;
	public List<Game.AppendageBodyPartEntry> AvailableAppendage;

	// ---------------------------------------------------------------------------

	public bool Changed {
		get {
			return
				Creature.Name != Original.Name ||
				Creature.Head != Original.Head ||
				Creature.Torso != Original.Torso ||
				Creature.Tail != Original.Tail ||
				!Creature.Appendages.SequenceEqual(Original.Appendages) ||
				!Creature.Skills.SequenceEqual(Original.Skills);
		}
	}

	// ---------------------------------------------------------------------------

}
