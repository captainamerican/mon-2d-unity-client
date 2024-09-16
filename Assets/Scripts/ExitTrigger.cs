using UnityEngine;

// -----------------------------------------------------------------------------

public class ExitStage : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;

	[Header("Locals")]
	[SerializeField] string SceneName;
	[SerializeField] Vector3 Destination;
	[SerializeField] Game.PlayerDirection Direction;

	// ---------------------------------------------------------------------------

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.GetComponent<Player>() == null) {
			return;
		}

		LoadNextScene();
	}

	// ---------------------------------------------------------------------------

	void LoadNextScene() {
		Loader.Scene.Load(new Game.NextScene {
			Name = SceneName,
			Destination = Destination,
			PlayerDirection = Direction
		});
	}

	// ---------------------------------------------------------------------------
}
