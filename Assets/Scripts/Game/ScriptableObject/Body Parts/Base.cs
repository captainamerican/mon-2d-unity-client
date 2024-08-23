using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part/Base")]
public class BodyPartBase : ScriptableObject {

	// ---------------------------------------------------------------------------

	public string Name;
	public int ExperienceToLevel;
	public int Strength = 1;
	public int Endurance = 1;
	public int Dexterity = 1;
	public int Intelligence = 1;
	public int Wisdom = 1;
	public int Luck = 1;

	public List<Game.BodyPartTag> Tags = new();

	// ---------------------------------------------------------------------------

	public static string Label {
		get {
			return "???";
		}
	}

	// ---------------------------------------------------------------------------
}