using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	Canvas Canvas;

	[SerializeField]
	GameObject MainMenu;

	[SerializeField]
	GameObject CompendiumMenu;

	[SerializeField]
	GameObject ItemMenu;

	[SerializeField]
	GameObject PlayerMenu;

	[SerializeField]
	GameObject SaveMenu;

	[SerializeField]
	GameObject OptionMenu;

	[SerializeField]
	EventSystem EventSystem;

	[SerializeField]
	Button FirstButton;

	bool isOpen;

	// Start is called before the first frame update
	void Start() {
		ExitMenu();
	}

	// Update is called once per frame
	void Update() {
		if (!isOpen && Input.GetButtonDown("Menu")) {
			OpenMenu();
		}
	}

	void OpenMenu() {
		Canvas.gameObject.SetActive(true);
		MainMenu.SetActive(true);

		//
		EventSystem.SetSelectedGameObject(FirstButton.gameObject);

		//
		Engine.SetMode(EngineMode.Menu);
		isOpen = true;
	}

	public void ExitMenu() {
		CompendiumMenu.SetActive(false);
		ItemMenu.SetActive(false);
		PlayerMenu.SetActive(false);
		SaveMenu.SetActive(false);
		OptionMenu.SetActive(false);
		MainMenu.SetActive(false);
		Canvas.gameObject.SetActive(false);

		//
		Engine.SetMode(EngineMode.PlayerControl);
		isOpen = false;
	}

	void HandleMouseClick() {

	}
}
