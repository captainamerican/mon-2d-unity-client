using TMPro;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class BlinkingCursor : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Image Cursor;

	// ---------------------------------------------------------------------------

	float timeToSwitch;
	Color nextColor;
	bool hidden;

	const float time = 0.66f;
	Color off = new(0, 0, 0, 0);
	Color on = new(0, 0, 0, 1);

	// ---------------------------------------------------------------------------

	void Start() {
		timeToSwitch = time;
		Cursor.color = on;
	}

	void Update() {
		timeToSwitch -= Time.unscaledDeltaTime;
		if (timeToSwitch > 0) {
			return;
		}

		timeToSwitch = time;
		Cursor.color = nextColor;
		hidden = !hidden;
		nextColor = hidden ? off : on;
	}

	// ---------------------------------------------------------------------------

}
