using UnityEngine;
using UnityEngine.EventSystems;

// -----------------------------------------------------------------------------

public class SecondaryScreenDebug : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] EventSystem LocalEventSystem;
	[SerializeField] Camera LocalCamera;
	[SerializeField] AudioListener LocalAudioListener;

	[SerializeField] Canvas Canvas;


	// --------------------------------------------------------------------------- 

	void Awake() {
		if (EventSystem.current == null) {
			LocalEventSystem.enabled = true;
		}

		if (Camera.main == null) {
			LocalCamera.enabled = true;
			LocalAudioListener.enabled = true;
		}

		//
		Canvas.worldCamera = Camera.main;
	}

	// ---------------------------------------------------------------------------

}
