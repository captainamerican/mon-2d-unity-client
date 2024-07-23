using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour {
	[SerializeField]
	UnityEvent Callback;

	private void OnTriggerEnter2D(Collider2D collision) {
		Callback?.Invoke();
	}
}
