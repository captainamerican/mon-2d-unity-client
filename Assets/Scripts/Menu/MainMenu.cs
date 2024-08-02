using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	enum Phase {
		Hidden,
		MainMenu,
		SubMenu,
	}

	[SerializeField]
	Engine Engine;

	[SerializeField]
	GameObject MainMenuObject;

	[SerializeField]
	List<Button> MainMenuButtons;

	[SerializeField]
	List<AbstractMenu> Menus;

	Phase phase;

	void Start() {
		ExitMainMenu();
	}

	void Update() {
		if (phase > Phase.MainMenu) {
			return;
		}

		switch (phase) {
			case Phase.Hidden:
				if (Input.GetButtonDown("Menu")) {
					OpenMainMenu();
				}
				break;

			case Phase.MainMenu:
				if (Input.GetButtonDown("Menu")) {
					ExitMainMenu();
				}
				break;
		}
	}

	void OpenMainMenu() {
		Time.timeScale = 0;
		Engine.SetMode(EngineMode.Menu);

		//
		MainMenuObject.SetActive(true);
		phase = Phase.MainMenu;
		EventSystem.current.sendNavigationEvents = true;
	}

	public void ExitMainMenu() {
		Menus.ForEach(menu => menu.Exit());
		MainMenuObject.SetActive(false);

		//
		phase = Phase.Hidden;

		//
		Engine.SetMode(EngineMode.PlayerControl);
		Time.timeScale = 1;
	}

	void OnSubMenuClosed(Button buttonToHighlight) {
		phase = Phase.MainMenu;
		EventSystem.current.sendNavigationEvents = true;
		buttonToHighlight.Select();
		buttonToHighlight.OnSelect(null);
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
