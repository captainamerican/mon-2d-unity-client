using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Battle {
	public class Dialogue : MonoBehaviour {
		enum Phase {
			Disabled,
			WaitinForUserInput,
			TypingText,
		}

		[SerializeField]
		protected List<TextMeshProUGUI> Pages = new();

		static Dialogue Self;

		Phase phase;
		string[] pages;
		int page;
		Action onDone;

		void Start() {
			Self = this;

			phase = Phase.Disabled;
		}

		private void OnDestroy() {
			Self = null;
		}

		static public void Display(Action onDone, params string[] text) {
			if (Self != null) {
				Self.DisplayText(onDone, text);
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

		void DisplayText(Action onDone, params string[] pages) {
			this.onDone = onDone;
			this.pages = pages;
			page = 0;

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
			StopAllCoroutines();

			phase = Phase.Disabled;
			pages = null;
			page = 0;

			Pages[0].text = "";
			Pages[1].text = "";

			onDone();
		}
	}
}