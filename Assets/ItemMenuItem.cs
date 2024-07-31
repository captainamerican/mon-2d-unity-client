using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMenuItem : MonoBehaviour, ISelectHandler {
	Action Selected;

	public void Configure(Action callback) {
		Selected = callback;
	}

	public void OnSelect(BaseEventData eventData) {
		Selected();
	}
}
