using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class AbstractMenu : MonoBehaviour {
	[SerializeField] protected GameObject Menu;

	Action OnDone;

	public virtual void Show(Action onDone) {
		OnDone = onDone;

		Menu.SetActive(true);
	}

	public virtual void Exit() {
		EventSystem.current.SetSelectedGameObject(null);

		Menu.SetActive(false);

		OnDone?.Invoke();
		OnDone = null;
	}

	public void Close() {
		Menu.SetActive(false);
	}
}
