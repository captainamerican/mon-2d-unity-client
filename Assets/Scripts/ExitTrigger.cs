using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

public class ExitStage : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;
	[SerializeField] Player Controller;

	[SerializeField] string SceneName;
	[SerializeField] Vector3 Destination;
	[SerializeField] Game.PlayerDirection Direction;

	// ---------------------------------------------------------------------------

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.GetComponent<Player>() != Controller) {
			return;
		}

		LoadNextScene();
	}

	// ---------------------------------------------------------------------------

	void LoadNextScene() {
		Engine.NextScene = new Game.NextScene {
			Name = SceneName,
			Destination = Destination
		};

		//
		SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Additive);
	}

	// ---------------------------------------------------------------------------
}
