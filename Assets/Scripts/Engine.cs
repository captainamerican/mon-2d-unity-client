using System.Collections.Generic;

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

public enum ChestId {
	None,
	ForestEntranceFirst,
	ForestEntranceSecond
}

public delegate void EngineModeChangedEvent(EngineMode mode);

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {
	public EngineMode Mode;
	public event EngineModeChangedEvent ModeChanged;

	public Profile Profile;

	public NextScene NextScene = null;

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