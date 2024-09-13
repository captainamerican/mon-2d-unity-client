using UnityEngine;

// -----------------------------------------------------------------------------

public class LoreAction : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;
	[SerializeField] Game.LoreId LoreId;

	// ---------------------------------------------------------------------------

	bool isBeingTouched;
	bool isOpen;

	// ---------------------------------------------------------------------------

	void Update() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		if (!isBeingTouched) {
			return;
		}

		if (!Input.GetButtonDown("Submit")) {
			return;
		}

		if (isOpen) {
			LoreDisplay.Done();
			isOpen = false;
			return;
		}

		//
		GainLore();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player"))
			return;
		isBeingTouched = true;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player"))
			return;
		isBeingTouched = false;
	}

	public void GainLore() {
		isOpen = true;
		LoreDisplay.Display(LoreId);
	}

	// ---------------------------------------------------------------------------

}
