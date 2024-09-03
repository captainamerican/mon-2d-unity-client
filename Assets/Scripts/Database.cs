using UnityEngine;

// -----------------------------------------------------------------------------

[ExecuteAlways]
public class Database : MonoBehaviour {

	// ---------------------------------------------------------------------------

	public static Engine Engine;

	[SerializeField] Engine Source;

	// ---------------------------------------------------------------------------

	void Awake() {
		if (Engine == null) {
			Engine = Source;
			Engine.GameData.BuildDictionaries();
		}
	}

	// ---------------------------------------------------------------------------

}
