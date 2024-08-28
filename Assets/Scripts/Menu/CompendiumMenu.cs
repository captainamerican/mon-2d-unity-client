using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Compendium2Menu : AbstractMenu {
	[SerializeField] PlayerInput PlayerInput;

	InputAction Cancel;

	private void OnDestroy() {
		if (Cancel != null) {
			Cancel.performed -= HandleCancel;
		}
	}

	override public void Show(Action onDone) {
		Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
		Cancel.performed += HandleCancel;

		//
		EventSystem.current.SetSelectedGameObject(null);

		//
		base.Show(onDone);
	}

	void HandleCancel(InputAction.CallbackContext ctx) {
		Cancel.performed -= HandleCancel;
		Cancel = null;

		Exit();
	}
}
