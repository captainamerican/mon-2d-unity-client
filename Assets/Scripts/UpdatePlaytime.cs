using UnityEngine;

// -----------------------------------------------------------------------------

public class UpdatePlaytime : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;

	// ---------------------------------------------------------------------------

	void Update() {
		Engine.Profile.PlaytimeAsSeconds += Time.unscaledDeltaTime;
	}

	// ---------------------------------------------------------------------------
}
