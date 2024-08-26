using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class ScrollView : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] GameObject Scrollbar;
	[SerializeField] RectTransform ScrollbarThumb;
	[SerializeField] int totalVisibleButtons = 8;

	// ---------------------------------------------------------------------------

	int visibleButtonRangeMin = 999999;
	int visibleButtonRangeMax = -999999;

	// ---------------------------------------------------------------------------

	public void UpdateVisibleButtonRange(List<Button> buttons, int index) {
		if (index < visibleButtonRangeMin) {
			visibleButtonRangeMin = index;
			visibleButtonRangeMax = index + totalVisibleButtons;
		} else if (index > visibleButtonRangeMax) {
			visibleButtonRangeMax = index;
			visibleButtonRangeMin = index - totalVisibleButtons;
		}

		//
		UpdateVisibleButtons(buttons);
		UpdateScrollbarThumb(buttons, index);
	}

	void UpdateVisibleButtons(List<Button> buttons) {
		for (int i = 0; i < buttons.Count; i++) {
			bool enabled = i >= visibleButtonRangeMin && i <= visibleButtonRangeMax;
			var button = buttons[i];
			var buttonGO = button.gameObject;
			if (buttonGO == null) {
				continue;
			}

			RectTransform rt = buttonGO.GetComponent<RectTransform>();

			Vector2 sizeDelta = rt.sizeDelta;
			sizeDelta.y = enabled
				? 10
				: 1;

			rt.sizeDelta = sizeDelta;

			foreach (Transform transform in buttonGO.transform) {
				transform.gameObject.SetActive(enabled);
			}
		}
	}

	void UpdateScrollbarThumb(List<Button> buttons, int index) {
		ScrollbarThumb.gameObject.SetActive(buttons.Count > 0);

		if (buttons.Count > 0) {
			var parent = ScrollbarThumb.parent.GetComponent<RectTransform>();

			float parentHeight = Mathf.Ceil(parent.rect.height);
			float rawButtonHeight = buttons.Count > 1 ? parentHeight / buttons.Count : parentHeight;
			float buttonHeight = Mathf.Round(Mathf.Clamp(rawButtonHeight, 1f, parentHeight));
			float track = parentHeight - buttonHeight;
			float offset = buttons.Count > 1 ? Mathf.Ceil(track * ((float) index / ((float) (buttons.Count - 1)))) : 0;

			ScrollbarThumb.anchoredPosition = new Vector2(0, -offset);
			ScrollbarThumb.sizeDelta = new Vector2(4, buttonHeight);
		}
	}

	// ---------------------------------------------------------------------------

}
