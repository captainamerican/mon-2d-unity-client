using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CompendiumMenu : AbstractMenu {
	[SerializeField] PlayerInput PlayerInput;

	InputAction Cancel;

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
