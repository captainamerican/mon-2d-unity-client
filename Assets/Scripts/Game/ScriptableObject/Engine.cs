using System.Collections.Generic;

using Game;

using UnityEngine;

// ---------------------------------------------------------------------------

public enum EngineMode {
	None,
	PlayerControl = 1,
	Menu = 10,
	Store = 20,
	Dialogue = 30,
	Cutscene = 40,
	Battle = 50,
	NextScene = 60
}

public enum MapId {
	Other,
	Village = 10,
	Forest01 = 11,
	ForestCave01 = 12,
	Forest02 = 13,
	ForestCave02 = 14,
	Forest03 = 15,
	Forest04 = 16
}

public class NextScene {
	public string Name;
	public Vector3 Destination;
	public object Data;
}

public delegate void EngineModeChangedEvent(EngineMode mode);

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {

	// ---------------------------------------------------------------------------

	public event EngineModeChangedEvent ModeChanged;

	public EngineMode Mode;
	public NextScene NextScene = null;

	public SaveFile Profile;

	[Header("Game Data")]
	public List<Item> CraftingEquipment = new();
	public List<Item> Items = new();
	public List<BodyPartBase> BodyParts = new();
	public List<Skill> Skills = new();
	public List<SpiritWisdom> SpiritWisdom = new();
	public List<Gameplay> Gameplay = new();
	public List<Tag> Tags = new();
	public List<Lore> Lore = new();

	// ---------------------------------------------------------------------------

	public void SetMode(EngineMode mode) {
		Mode = mode;
		ModeChanged?.Invoke(mode);
	}

	public bool PlayerHasControl() {
		return Mode == EngineMode.PlayerControl;
	}

	public CompletionData SaveFileCompletion() {
		return SaveFileCompletion(Profile);
	}

	public CompletionData SaveFileCompletion(SaveFile saveFile) {
		int total = 0;
		total += BodyParts.Count;
		total += Items.Count;
		total += Lore.Count;
		total += Skills.Count;
		total += SpiritWisdom.Count;
		total += Tags.Count;

		int current = 0;
		current += saveFile.Acquired.BodyPart.Count;
		current += saveFile.Acquired.Item.Count;
		current += saveFile.Acquired.Lore.Count;
		current += saveFile.Acquired.Skill.Count;
		current += saveFile.Acquired.SpiritWisdom.Count;
		current += saveFile.Acquired.Tag.Count;

		float ratio = total > 0 ? (float) current / (float) total : 0;

		//
		return new() {
			current = current,
			total = total,
			ratio = ratio
		};
	}

	// ---------------------------------------------------------------------------

}