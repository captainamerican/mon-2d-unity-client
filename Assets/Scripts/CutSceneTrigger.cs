using UnityEngine;
using UnityEngine.Events;

// -----------------------------------------------------------------------------

public class ReturnValue {
	public bool Skipped;
}

public class CutSceneTrigger : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Cutscene Cutscene;
	[SerializeField] bool TriggerOnce;

	[SerializeField] UnityEvent<ReturnValue> Skip;

	// ---------------------------------------------------------------------------

	bool wasTriggered;

	// ---------------------------------------------------------------------------

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.GetComponent<Player>() == null) {
			return;
		}

		if (TriggerOnce && wasTriggered) {
			return;
		}

		//
		ReturnValue was = new();
		Skip?.Invoke(was);

		if (was.Skipped == true) {
			return;
		}

		//
		wasTriggered = true;

		//
		Cutscene.IsActivated = true;
		Cutscene.enabled = true;
	}

	// ---------------------------------------------------------------------------
}
