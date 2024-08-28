using System.Collections.Generic;

using Game;

using UnityEngine;

public enum EngineMode {
	None,
	PlayerControl,
	Menu,
	Dialogue,
	Cutscene,
	Battle,
	NextScene
}

public delegate void EngineModeChangedEvent(EngineMode mode);

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {
	public EngineMode Mode;
	public event EngineModeChangedEvent ModeChanged;

	public NextScene NextScene = null;

	[Header("Game Data")]
	public List<Item> CraftingEquipment = new();
	public List<Item> AllItems = new();
	public List<BodyPartBase> AllBodyParts = new();
	public List<Skill> AllSkills = new();
	public List<SpiritWisdom> AllSpiritWisdom = new();
	public List<Gameplay> AllGameplay = new();
	public List<Tag> AllTags = new();

	[Header(("Current Profile"))]
	public SaveFile Profile;

	public void SetMode(EngineMode mode) {
		Mode = mode;
		ModeChanged?.Invoke(mode);
	}

	public bool PlayerHasControl() {
		return Mode == EngineMode.PlayerControl;
	}
}

public class NextScene {
	public string Name;
	public Vector3 Destination;
	public object Data;
}