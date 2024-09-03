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

public delegate void EngineModeChangedEvent(EngineMode mode);

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {

	// ---------------------------------------------------------------------------

	public event EngineModeChangedEvent ModeChanged;

	public EngineMode Mode;
	public NextScene NextScene = null;

	public SaveFile Profile;

	public GameData GameData;

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
		total += GameData.BodyParts.Count;
		total += GameData.Items.Count;
		total += GameData.Lore.Count;
		total += GameData.Skills.Count;
		total += GameData.SpiritWisdom.Count;
		total += GameData.Tags.Count;

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