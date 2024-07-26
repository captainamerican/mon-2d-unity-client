using System;

using UnityEngine;

public class AbstractMenu : MonoBehaviour {
	[SerializeField]
	protected GameObject Menu;

	Action OnDone;

	public virtual void Show(Action onDone) {
		OnDone = onDone;

		Menu.SetActive(true);
		enabled = true;
	}

	public virtual void Exit() {
		Menu.SetActive(false);
		enabled = false;

		OnDone?.Invoke();
	}

	void Update() {
		if (Input.GetButtonDown("Cancel")) {
			Exit();
			return;
		}
	}
}
