using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

// -----------------------------------------------------------------------------

public class SecondaryScreenDebug : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;

	[SerializeField] EventSystem LocalEventSystem;
	[SerializeField] Camera LocalCamera;
	[SerializeField] AudioListener LocalAudioListener;

	[SerializeField] Canvas Canvas;
	[SerializeField] List<Canvas> Canvases;

	[SerializeField] bool ResetEngineMode;
	[SerializeField] EngineMode EngineModeReset;


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
		Canvases.ForEach(c => c.worldCamera = Camera.main);

		//
		if (ResetEngineMode) {
			Engine.Mode = EngineModeReset;
		}
	}

	// ---------------------------------------------------------------------------

}
