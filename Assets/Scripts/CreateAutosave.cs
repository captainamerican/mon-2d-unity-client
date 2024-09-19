using System;
using System.Collections;
using System.IO;

using UnityEngine;

// -----------------------------------------------------------------------------

public class CreateAutosave : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;

	[Header("Locals")]
	[SerializeField] Transform Loader;

	// ---------------------------------------------------------------------------

	Vector3 localEulerAngles;

	bool saveFileCreated;
	float timeUntilShutdown;

	// ---------------------------------------------------------------------------

	IEnumerator Start() {
		timeUntilShutdown = 2.5f;

		//
		yield return Wait.ForReal(0.1f);
		CreateTheAutosave();
	}

	void Update() {
		localEulerAngles = Loader.localEulerAngles;
		localEulerAngles.z += 66f * Time.unscaledDeltaTime;

		//
		Loader.localEulerAngles = localEulerAngles;

		//
		timeUntilShutdown -= Time.unscaledDeltaTime;

		//
		if (!saveFileCreated) {
			return;
		}

		if (timeUntilShutdown < 0) {
			Shutdown();
		}
	}

	// ---------------------------------------------------------------------------

	void CreateTheAutosave() {
		string oldId = Engine.Profile.Id;

		//
		Engine.Profile.Id = "autosave";
		Engine.Profile.SavedAt = DateTime.Now.Ticks;

		//
		string path = $"{Application.persistentDataPath}/autosave.lethia1";
		string json = JsonUtility.ToJson(Engine.Profile);
		File.WriteAllText(path, json);

		//
		Engine.Profile.Id = oldId;

		//
		saveFileCreated = true;
	}

	void Shutdown() {
		gameObject.SetActive(false);
	}

	// ---------------------------------------------------------------------------
}
