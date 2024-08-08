using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerProfileMenu : AbstractMenu {
	[SerializeField] PlayerInput PlayerInput;

	InputAction Cancel;

	override public void Show(Action onDone) {
		base.Show(onDone);

		Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
		Cancel.performed += HandleCancel;

		EventSystem.current.SetSelectedGameObject(null);
	}

	void HandleCancel(InputAction.CallbackContext ctx) {
		Cancel.performed -= HandleCancel;
		Cancel = null;

		Exit();
	}
}
