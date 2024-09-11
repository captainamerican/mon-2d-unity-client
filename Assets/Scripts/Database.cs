using UnityEngine;

// -----------------------------------------------------------------------------

public class Database : MonoBehaviour {

	// ---------------------------------------------------------------------------

	public static Engine Engine;

	[SerializeField] Engine Source;

	// ---------------------------------------------------------------------------

	void Awake() {
		Engine = Source;
		Engine.GameData.BuildDictionaries();
	}

	// ---------------------------------------------------------------------------

}
