using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class TeleporterAction : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;
	[SerializeField] Player Player;

	[Header("Locals")]
	[SerializeField] Game.GamePointId RequiredGamePoint;
	[SerializeField] GameObject Destination;
	[SerializeField] Game.PlayerDirection ExitFacing;

	[TextArea(2, 10)]
	[SerializeField] string DialogueIfNotWorking = "The device isn't working...";

	// ---------------------------------------------------------------------------

	bool isBeingTouched;
	bool isTeleporting;
	bool isUnlocked;

	// ---------------------------------------------------------------------------

	void Start() {
		isBeingTouched = false;
		isTeleporting = false;
		isUnlocked = Engine.Profile.GamePoints.Contains(RequiredGamePoint);
	}

	void Update() {
		if (
			!isBeingTouched ||
			isTeleporting ||
			!Engine.PlayerHasControl() ||
			!Input.GetButtonDown("Submit")
		) {
			return;
		}

		if (!isUnlocked) {
			StartCoroutine(CantUseYet());
			return;
		}

		//
		Teleport();
	}

	// ---------------------------------------------------------------------------

	private void OnTriggerEnter2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		isBeingTouched = true;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		isBeingTouched = false;
	}

	// ---------------------------------------------------------------------------

	public void Teleport() {
		isTeleporting = true;

		Player.transform.position = Destination.transform.position;
		Player.SetFacing(ExitFacing);

		isTeleporting = false;
		isBeingTouched = false;
	}

	public IEnumerator CantUseYet() {
		yield return Dialogue.Scene.DisplayWithSpeaker("Lethia", DialogueIfNotWorking);
		Engine.Mode = EngineMode.PlayerControl;
	}

	// ---------------------------------------------------------------------------

}
