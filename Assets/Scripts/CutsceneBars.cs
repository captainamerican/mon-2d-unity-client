using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class CutsceneBars : MonoBehaviour {

	// ---------------------------------------------------------------------------

	static CutsceneBars Self;

	static public void Show() {
		if (Self != null) {
			Self.ShowBars();
		}
	}

	static public void Hide() {
		if (Self != null) {
			Self.HideBars();
		}
	}

	// ---------------------------------------------------------------------------

	[SerializeField] RectTransform TopBar;
	[SerializeField] RectTransform BottomBar;

	// ---------------------------------------------------------------------------

	void Awake() {
		Self = this;

		TopBar.gameObject.SetActive(false);
		BottomBar.gameObject.SetActive(false);
	}

	public void ShowBars() {
		StopAllCoroutines();
		StartCoroutine(ResizeBars(0, 16f, 0.5f, false));
	}

	public void HideBars() {
		StopAllCoroutines();
		StartCoroutine(ResizeBars(16f, 0, 0.5f, true));
	}

	IEnumerator ResizeBars(float from, float to, float duration, bool hideAtEnd) {
		TopBar.gameObject.SetActive(true);
		BottomBar.gameObject.SetActive(true);

		//
		Vector2 sizeDelta = new(0, from);

		yield return Do.ForReal(duration, ratio => {
			sizeDelta.y = Mathf.Lerp(from, to, ratio);
			TopBar.sizeDelta = sizeDelta;
			BottomBar.sizeDelta = sizeDelta;
		});

		//
		if (hideAtEnd) {
			TopBar.gameObject.SetActive(false);
			BottomBar.gameObject.SetActive(false);
		}
	}

	// ---------------------------------------------------------------------------

}
