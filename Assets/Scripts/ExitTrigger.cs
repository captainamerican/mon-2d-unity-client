using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitStage : MonoBehaviour {

	[SerializeField]
	PlayerDirection Direction;

	[SerializeField]
	Vector3 Destination;

	[SerializeField]
	string SceneName;

	[SerializeField]
	Player Controller;

	[SerializeField]
	Engine Engine;


	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.GetComponent<Player>() != Controller) {
			return;
		}

		LoadNextScene();
	}

	void LoadNextScene() {
		Engine.NextScene = new NextScene { Name = SceneName, Destination = Destination };

		SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Additive);
	}
}
