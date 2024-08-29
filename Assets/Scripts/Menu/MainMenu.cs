using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	[SerializeField] Engine Engine;
	[SerializeField] PlayerInput PlayerInput;
	[SerializeField] GameObject MainMenuObject;
	[SerializeField] List<Button> MainMenuButtons;
	[SerializeField] List<AbstractMenu> Menus;

	InputAction Menu;
	InputAction Cancel;

	void Start() {
		Menu = PlayerInput.currentActionMap.FindAction("Menu");
		Cancel = PlayerInput.currentActionMap.FindAction("Cancel");

		Menu.performed += ShouldIOpen;
	}

	void OnDestroy() {
		if (Menu != null) {
			Menu.performed -= ShouldIOpen;
			Menu.performed -= ShouldIClose;
			Cancel.performed -= ShouldIClose;
		}
	}

	void ShouldIOpen(InputAction.CallbackContext ctx) {
		if (!Menu.WasPressedThisFrame() || !Engine.PlayerHasControl()) {
			return;
		}

		OpenMainMenu();
	}

	void ShouldIClose(InputAction.CallbackContext ctx) {
		ExitMainMenu();
	}

	void OpenMainMenu() {
		StartCoroutine(Wait.Until(() => {
			Time.timeScale = 0;

			//
			Menu.performed -= ShouldIOpen;
			Menu.performed += ShouldIClose;
			Cancel.performed += ShouldIClose;

			// 
			Engine.SetMode(EngineMode.Store);

			//
			MainMenuObject.SetActive(true);

			if (MainMenuButtons[0] == null) {
				return false;
			}

			MainMenuButtons[0].Select();
			MainMenuButtons[0].OnSelect(null);

			return true;
		}));
	}

	public void ExitMainMenu() {
		MainMenuObject.SetActive(false);

		//  
		Engine.SetMode(EngineMode.PlayerControl);

		// 
		Time.timeScale = 1;

		Menu.performed += ShouldIOpen;
		Menu.performed -= ShouldIClose;
		Cancel.performed -= ShouldIClose;
	}

	void OnSubMenuClosed(Button buttonToHighlight) {
		buttonToHighlight.Select();
		buttonToHighlight.OnSelect(null);

		Menu.performed += ShouldIClose;
		Cancel.performed += ShouldIClose;
	}

	void OpenSubMenu(int index) {
		Menu.performed -= ShouldIClose;
		Cancel.performed -= ShouldIClose;

		EventSystem.current.SetSelectedGameObject(null);
		Menus[index].Show(() => OnSubMenuClosed(MainMenuButtons[index]));
	}

	public void OpenCompendiumMenu() {
		OpenSubMenu(0);
	}

	public void OpenCreaturesMenu() {
		OpenSubMenu(1);
		;
	}

	public void OpenItemsMenu() {
		OpenSubMenu(2);
	}

	public void OpenMapMenu() {
		OpenSubMenu(3);
	}

	public void OpenPlayerProfileMenu() {
		OpenSubMenu(4);
	}

	public void OpenSaveMenu() {
		OpenSubMenu(5);
	}

	public void OpenOptionsMenu() {
		OpenSubMenu(6);
	}
}
