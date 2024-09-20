using UnityEngine;
using UnityEngine.Events;

using S = UnityEngine.SerializeField;

// -----------------------------------------------------------------------------

public class ReturnValue {
	public bool Skipped;
}

public class CutSceneTrigger : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[S] Cutscene Cutscene;
	[S] bool TriggerOnce;

	[S] UnityEvent<ReturnValue> Skip;

	// ---------------------------------------------------------------------------

	bool wasTriggered;

	// ---------------------------------------------------------------------------

	private void OnTriggerEnter2D(Collider2D collision) {
		if (!Database.Engine.PlayerHasControl())
			return;
		if (collision.gameObject.GetComponent<Player>() == null)
			return;
		if (TriggerOnce && wasTriggered)
			return;

		//
		ReturnValue was = new();
		Skip?.Invoke(was);

		if (was.Skipped)
			return;

		//
		wasTriggered = true;

		//
		Cutscene.IsActivated = true;
		Cutscene.enabled = true;
	}

	// ---------------------------------------------------------------------------
}
