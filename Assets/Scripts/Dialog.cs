using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class Dialog : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] List<Button> Actions;
	[SerializeField] UnityEvent<int> OnAction;

	// ---------------------------------------------------------------------------

	void Start() {
		Do.ForEach(Actions, (action, index) => {
			action.onClick.RemoveAllListeners();
			action.onClick.AddListener(() => OnAction?.Invoke(index));
		});
		Game.Focus.This(Actions[0]);
	}

	// ---------------------------------------------------------------------------

}
