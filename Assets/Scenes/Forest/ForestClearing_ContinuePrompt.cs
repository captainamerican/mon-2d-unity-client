using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class ForestClearing_ContinuePrompt : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] GameObject Dialog;
	[SerializeField] Button CancelButton;

	// ---------------------------------------------------------------------------

	bool actionSelected;
	int actionIndex;

	// ---------------------------------------------------------------------------

	void Start() {
		Dialog.SetActive(false);
	}

	// ---------------------------------------------------------------------------

	public IEnumerator Display(Action<int> onDone) {
		actionSelected = false;
		actionIndex = 0;

		//
		Dialog.SetActive(true);
		Game.Focus.This(CancelButton);
		yield return Wait.Until(() => actionSelected);
		Dialog.SetActive(false);
		onDone(actionIndex);
	}

	// ---------------------------------------------------------------------------

	public void ActionSelected(int index) {
		actionSelected = true;
		actionIndex = index;
	}

	// ---------------------------------------------------------------------------

}
