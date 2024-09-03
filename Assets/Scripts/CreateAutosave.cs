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

	void Start() {
		timeUntilShutdown = 3f;

		//
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
		Engine.Profile.FileIndex = 0;
		Engine.Profile.IsAutoSave = true;

		string path = $"{Application.persistentDataPath}/save_00.lethia1";
		string json = JsonUtility.ToJson(Engine.Profile);
		File.WriteAllText(path, json);

		//
		saveFileCreated = true;
	}

	void Shutdown() {
		gameObject.SetActive(false);
	}

	// ---------------------------------------------------------------------------
}
