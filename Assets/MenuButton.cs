using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {
	[SerializeField] Image Image;
	[SerializeField] TextMeshProUGUI Label;

	Action OnFocus;

	public void Focus() {
		Image.color = Color.black;
		Label.color = Color.white;

		OnFocus?.Invoke();
	}

	public void Blur() {
		Image.color = Color.white;
		Label.color = Color.black;
	}

	public void SetFocus(Action onFocus) {
		OnFocus = onFocus;
	}
}
