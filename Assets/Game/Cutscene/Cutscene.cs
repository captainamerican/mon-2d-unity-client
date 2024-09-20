using System.Collections;

using UnityEngine;


// -----------------------------------------------------------------------------

public class Cutscene : MonoBehaviour {

	// ---------------------------------------------------------------------------

	static public bool IsActivated = false;

	// ---------------------------------------------------------------------------

	protected void OnEnable() {
		if (!IsActivated) {
			enabled = false;
			return;
		}

		//
		Stop();
		Play();
	}

	public void Play() {
		StartCoroutine(Playing());
	}

	public IEnumerator Playing() {
		CutsceneBars.Show();
		yield return Script();
		CutsceneBars.Hide();

		//
		IsActivated = false;
		enabled = false;
	}

	public void Stop() {
		StopAllCoroutines();
	}

	virtual protected IEnumerator Script() {
		yield break;
	}


	// ---------------------------------------------------------------------------

}
