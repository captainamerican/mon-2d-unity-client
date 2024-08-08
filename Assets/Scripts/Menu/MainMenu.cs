using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	enum Phase {
		Hidden,
		MainMenu,
		SubMenu,
	}

	[SerializeField] Engine Engine;
	[SerializeField] PlayerInput PlayerInput;

	[SerializeField] GameObject MainMenuObject;

	[SerializeField] List<Button> MainMenuButtons;

	[SerializeField] List<AbstractMenu> Menus;

	Phase phase;

	InputAction Menu;
	InputAction Cancel;

	void Start() {
		Menu = PlayerInput.currentActionMap.FindAction("Menu");
		Cancel = PlayerInput.currentActionMap.FindAction("Cancel");

		ExitMainMenu();
	}

	void Update() {
		switch (phase) {
			case Phase.Hidden:
				//Debug.Log("Hidden");
				if (Menu.WasPressedThisFrame() && Engine.PlayerHasControl()) {
					OpenMainMenu();
				}
				break;

			case Phase.MainMenu:
				//Debug.Log("MainMenu");
				if ((Menu.WasPressedThisFrame() || Cancel.WasPressedThisFrame()) && Engine.Mode == EngineMode.Menu) {
					ExitMainMenu();
				}
				break;
		}
	}

	void OpenMainMenu() {
		Time.timeScale = 0;

		//
		phase = Phase.MainMenu;
		Engine.SetMode(EngineMode.Menu);

		//
		MainMenuObject.SetActive(true);

		MainMenuButtons[0].Select();
		MainMenuButtons[0].OnSelect(null);

		EventSystem.current.sendNavigationEvents = true;
	}

	public void ExitMainMenu() {
		Menus.ForEach(menu => menu.Exit());
		MainMenuObject.SetActive(false);

		// 
		phase = Phase.Hidden;
		Engine.SetMode(EngineMode.PlayerControl);

		//
		EventSystem.current.sendNavigationEvents = true;
		Time.timeScale = 1;
	}

	void OnSubMenuClosed(Button buttonToHighlight) {
		phase = Phase.MainMenu;

		buttonToHighlight.Select();
		buttonToHighlight.OnSelect(null);

		EventSystem.current.sendNavigationEvents = true;
	}

	void OpenSubMenu(int index) {
		phase = Phase.SubMenu;

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.sendNavigationEvents = false;
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
