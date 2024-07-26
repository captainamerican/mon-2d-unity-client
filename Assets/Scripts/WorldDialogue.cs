using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class WorldDialogue : MonoBehaviour {
	enum Phase {
		Hidden,
		WaitinForUserInput,
		TypingText,
	}

	[SerializeField]
	Engine Engine;

	[SerializeField]
	GameObject Panel;

	[SerializeField]
	protected List<TextMeshProUGUI> Pages = new();

	static WorldDialogue Self;

	Phase phase;
	string[] pages;
	int page;


	void Start() {
		Self = this;

		phase = Phase.Hidden;
		Panel.SetActive(false);
	}

	private void OnDestroy() {
		Self = null;
	}

	static public void Display(params string[] text) {
		if (Self != null) {
			Self.DisplayText(text);
		}
	}

	void Update() {
		switch (phase) {
			case Phase.TypingText:
				if (Input.GetButtonDown("Submit")) {
					StopAllCoroutines();

					phase = Phase.WaitinForUserInput;
					Pages[0].text = pages[page];
					page += 1;
				}
				break;

			case Phase.WaitinForUserInput:
				if (Input.GetButtonDown("Submit")) {
					if (page >= pages.Length) {
						Exit();
					} else {
						GoToNextPage();
					}
				}
				break;

			default:
				break;
		}
	}

	void DisplayText(params string[] pages) {
		Time.timeScale = 0;
		Engine.Mode = EngineMode.Dialogue;

		this.pages = pages;
		page = 0;

		Panel.SetActive(true);
		Pages.ForEach(page => page.gameObject.SetActive(false));

		StartCoroutine(TypingOutText());
	}

	IEnumerator TypingOutText() {
		phase = Phase.TypingText;


		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.025f);

		TextMeshProUGUI text = Pages[0];
		text.transform.localPosition = Vector3.zero;
		text.gameObject.SetActive(true);
		text.text = "";

		string letters = pages[page];
		for (int i = 0; i < letters.Length; i++) {
			text.text += letters[i];
			yield return wait;
		}

		phase = Phase.WaitinForUserInput;
		page += 1;
	}


	void GoToNextPage() {
		Pages.Reverse();

		var FirstPage = Pages[0];
		var SecondPage = Pages[1];

		FirstPage.text = "";

		SecondPage.gameObject.SetActive(false);

		StartCoroutine(TypingOutText());
	}

	void Exit() {
		Panel.SetActive(false);

		pages = null;
		page = 0;
		phase = Phase.Hidden;

		StopAllCoroutines();
		StartCoroutine(Do.AfterReal(0.1f, () => {
			Engine.Mode = EngineMode.PlayerControl;
			Time.timeScale = 1;
		}));
	}
}
