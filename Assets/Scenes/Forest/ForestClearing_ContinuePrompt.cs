using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class ForestClearing_ContinuePrompt : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Canvas Canvas;
	[SerializeField] Button CancelButton;

	// ---------------------------------------------------------------------------

	bool actionSelected;
	int actionIndex;

	// ---------------------------------------------------------------------------

	void Start() {
		Canvas.enabled = false;
	}

	// ---------------------------------------------------------------------------

	public IEnumerator Display(Action<int> onDone) {
		actionSelected = false;
		actionIndex = 0;

		//
		Canvas.enabled = true;
		Game.Focus.This(CancelButton);
		yield return Wait.Until(() => actionSelected);
		Canvas.enabled = false;
		onDone(actionIndex);
	}

	// ---------------------------------------------------------------------------

	public void ActionSelected(int index) {
		actionSelected = true;
		actionIndex = index;
	}

	// ---------------------------------------------------------------------------

}
