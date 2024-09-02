using System.Collections.Generic;

using Game;

using UnityEngine;

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

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {
	public EngineMode Mode;
	public event EngineModeChangedEvent ModeChanged;

	public MapId MapId;

	public NextScene NextScene = null;

	[Header("Game Data")]
	public List<Item> CraftingEquipment = new();
	public List<Item> AllItems = new();
	public List<BodyPartBase> AllBodyParts = new();
	public List<Skill> AllSkills = new();
	public List<SpiritWisdom> AllSpiritWisdom = new();
	public List<Gameplay> AllGameplay = new();
	public List<Tag> AllTags = new();
	public List<Lore> AllLore = new();

	[Header(("Current Profile"))]
	public SaveFile Profile;

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
		total += AllBodyParts.Count;
		total += AllItems.Count;
		total += AllLore.Count;
		total += AllSkills.Count;
		total += AllSpiritWisdom.Count;
		total += AllTags.Count;

		int current = 0;
		current += saveFile.AcquiredBodyPart.Count;
		current += saveFile.AcquiredItem.Count;
		current += saveFile.AcquiredLore.Count;
		current += saveFile.AcquiredSkills.Count;
		current += saveFile.AcquiredSpiritWisdom.Count;
		current += saveFile.AcquiredTags.Count;

		float ratio = total > 0 ? (float) current / (float) total : 0;

		//
		return new() {
			current = current,
			total = total,
			ratio = ratio
		};
	}
}