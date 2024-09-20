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

	// ---------------------------------------------------------------------------

	void OnEnable() {
		if (!Engine.Profile.StoryPoints.Has(Game.StoryPointId.SawNewGameScene)) {
			Shutdown();
			return;
		}

		//
		StartCoroutine(Saving());
	}

	IEnumerator Saving() {
		yield return Wait.ForReal(0.1f);
		CreateTheAutosave();
		yield return Wait.ForReal(2f);
		Shutdown();
	}

	void Update() {
		localEulerAngles = Loader.localEulerAngles;
		localEulerAngles.z += 66f * Time.unscaledDeltaTime;

		//
		Loader.localEulerAngles = localEulerAngles;
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
	}

	void Shutdown() {
		gameObject.SetActive(false);
	}

	// ---------------------------------------------------------------------------
}
