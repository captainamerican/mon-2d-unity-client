using UnityEngine;

public enum EngineMode {
	None,
	PlayerControl,
	Menu,
	Cutscene,
	NextScene
}

public delegate void EngineModeChangedEvent(EngineMode mode);

[CreateAssetMenu(fileName = "Engine", menuName = "MoN/Engine")]
public class Engine : ScriptableObject {
	public EngineMode Mode;
	public event EngineModeChangedEvent ModeChanged;

	public string NextScene = "Start";
	public Vector3 NextScenePosition;

	public void SetMode(EngineMode mode) {
		Mode = mode;
		ModeChanged?.Invoke(mode);
	}
}
