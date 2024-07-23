using UnityEngine;

public class ExitStage : MonoBehaviour {

	[SerializeField]
	PlayerDirection Direction;

	[SerializeField]
	string SceneName;

	[SerializeField]
	Player Controller;



	private void OnTriggerEnter2D(Collider2D collision) {
		Controller.GetComponent<Animator>().enabled = false;
		Controller.enabled = false;
	}
}
