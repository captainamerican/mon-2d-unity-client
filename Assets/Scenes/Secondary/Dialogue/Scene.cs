using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Dialogue {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "Dialogue";

		static Scene Self;

		static public IEnumerator Load() {
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			if (Self != null) {
				yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			}
		}

		static public IEnumerator Display(string[] pages, string speaker = null) {
			Debug.Assert(Self != null, "Dialogue scene wasn't loaded!");

			//
			return Self.DisplayText(pages, speaker);
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject Canvas;

		[SerializeField] GameObject SpeakerContainer;
		[SerializeField] TextMeshProUGUI SpeakerLabel;

		[SerializeField] GameObject TextContainer;
		[SerializeField] List<TextMeshProUGUI> Pages;
		[SerializeField] List<RectTransform> PageRectTransforms;

		// -------------------------------------------------------------------------

		InputAction Submit;

		string[] pages;
		TextMeshProUGUI currentPage;
		int currentPageIndex;
		int currentTextIndex;
		string currentText;
		float delayUntilNextKeyStroke;
		Coroutine slideUp;

		readonly WaitForEndOfFrame waitForEndOfFrame = new();
		readonly Vector2 a1 = new(0, -30);
		readonly Vector2 b1 = new(0, 0);
		readonly Vector2 a2 = new(0, 0);
		readonly Vector2 b2 = new(0, 30);

		// -------------------------------------------------------------------------

		void Awake() {
			Self = this;
		}

		void Start() {
			Canvas.SetActive(false);
			SpeakerContainer.SetActive(false);
			TextContainer.SetActive(false);

			//
			Submit = PlayerInput.currentActionMap.FindAction("Submit");
		}

		// -------------------------------------------------------------------------

		public IEnumerator DisplayText(string[] pages, string speaker = null) {
			Engine.Mode = EngineMode.Dialogue;
			Time.timeScale = 0;

			// usually the player is holding down the submit button
			// when a dialogue window is launched.
			yield return Wait.Until(() => !Submit.WasPressedThisFrame());

			//
			this.pages = pages;
			currentPageIndex = 0;

			//
			ResetPages();
			ConfigureSpeaker(speaker);

			TextContainer.SetActive(true);
			Canvas.SetActive(true);

			//
			for (currentPageIndex = 0; currentPageIndex < pages.Length; currentPageIndex++) {
				GetCurrentPage();
				yield return TypeOutPage();
				yield return WaitForUserInput();
				if (currentPageIndex < pages.Length - 1) {
					slideUp = StartCoroutine(SwapAndSlipPagesUp());
				}
			}

			//
			SpeakerContainer.SetActive(false);
			TextContainer.SetActive(false);
			Canvas.SetActive(false);

			//
			Engine.Mode = EngineMode.Dialogue;
			Time.timeScale = 1;
		}

		// -------------------------------------------------------------------------

		void ResetPages() {
			Pages.ForEach(page => {
				page.text = "";
				page.gameObject.SetActive(true);
			});
		}

		void ConfigureSpeaker(string speaker) {
			SpeakerLabel.text = speaker ?? "";
			SpeakerContainer.SetActive(speaker != null);
		}

		void CompletePage() {
			if (slideUp != null) {
				StopCoroutine(slideUp);
				slideUp = null;
			}

			//
			currentTextIndex = currentText.Length;
			currentPage.text = currentText;

			PageRectTransforms[0].anchoredPosition = b1;
			PageRectTransforms[1].anchoredPosition = b2;
		}

		IEnumerator DelayNextLetter() {
			SetDelay();

			//
			while (delayUntilNextKeyStroke > 0) {
				delayUntilNextKeyStroke -= Time.unscaledDeltaTime;

				if (Submit.WasPressedThisFrame()) {
					CompletePage();
					break;
				}

				yield return waitForEndOfFrame;
			}
		}

		void SetDelay() {
			delayUntilNextKeyStroke = 0.25f * Engine.Profile.Options.DialogueSpeed;
		}

		IEnumerator TypeOutPage() {
			for (currentTextIndex = 0; currentTextIndex < currentText.Length; currentTextIndex++) {
				currentPage.text += currentText[currentTextIndex];
				yield return DelayNextLetter();
			}
		}

		void FlipPages() {
			Pages.Reverse();
			PageRectTransforms.Reverse();
		}

		void GetCurrentPage() {
			currentPage = Pages[0];
			currentText = pages[currentPageIndex];
			currentTextIndex = 0;
		}

		IEnumerator WaitForUserInput() {
			// in case they hit the button to skip the typewriter effect
			yield return Wait.Until(() => !Submit.WasPressedThisFrame());
			yield return Wait.Until(() => Submit.WasPressedThisFrame());
			yield return waitForEndOfFrame;
		}

		IEnumerator SwapAndSlipPagesUp() {
			var oldPage = currentPage;
			var oldRectTransform = PageRectTransforms[0];

			FlipPages();

			currentPage = Pages[0];
			var currentRectTransform = PageRectTransforms[0];

			//
			currentRectTransform.anchoredPosition = a1;

			//
			yield return Do.ForReal(3f, i => {
				currentRectTransform.anchoredPosition = Vector2.Lerp(a1, b1, i);
				oldRectTransform.anchoredPosition = Vector2.Lerp(a2, b2, i);
			});

			//
			slideUp = null;
		}

		// -------------------------------------------------------------------------

	}
}