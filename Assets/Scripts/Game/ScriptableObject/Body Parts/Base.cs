using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part/Base")]
public class BodyPartBase : ScriptableObject {

	// ---------------------------------------------------------------------------

	public Game.BodyPartId Id;
	public string Name;
	public int ExperienceToLevel;
	public int Strength = 0;
	public int Endurance = 0;
	public int Dexterity = 0;
	public int Intelligence = 0;
	public int Wisdom = 0;
	public int Luck = 0;
	public bool Obtainable = true;

	public List<Game.EssenceTagId> Tags = new();

	// ---------------------------------------------------------------------------

	public static string Label {
		get {
			return "???";
		}
	}

	// ---------------------------------------------------------------------------
}