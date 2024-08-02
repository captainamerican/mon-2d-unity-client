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


		Phase phase;
		string[] pages;
		int page;

		void Start() {
			phase = Phase.Disabled;
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

		public void Display(params string[] pages) {
			this.pages = pages;
			page = 0;

			Pages.ForEach(page => page.gameObject.SetActive(false));

			StartCoroutine(TypingOutText());
		}

		IEnumerator TypingOutText() {
			phase = Phase.TypingText;

			WaitForSecondsRealtime wait = new(0.025f);

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
		}

		public bool IsDone() {
			return phase == Phase.Disabled;
		}
	}
}