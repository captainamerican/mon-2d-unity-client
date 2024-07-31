using System.Collections;

using UnityEngine;

public class Battle : MonoBehaviour {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	GameObject Cover;

	[SerializeField]
	GameObject CoverMask;

	[SerializeField]
	Player Player;

	static Battle Self;
	WorldEnemy.Enemy enemy;

	static public void Begin(WorldEnemy.Enemy enemy) {
		Self.StartBattle(enemy);

	}

	void Start() {
		Self = this;
		Cover.SetActive(false);
	}

	void Update() {

	}

	void StartBattle(WorldEnemy.Enemy enemy) {

		Debug.Log("Start Battle");

		Engine.Mode = EngineMode.Battle;

		this.enemy = enemy;

		Player.Stop();

		StartCoroutine(StartingBatle());
	}

	IEnumerator StartingBatle() {
		Cover.SetActive(true);
		Cover.transform.position = Player.transform.position + new Vector3(0, 0, 20f);
		CoverMask.transform.localScale = Vector3.one;

		Player.GetComponent<SpriteRenderer>().sortingOrder = 11;
		enemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = 11;

		yield return Wait.For(1f);
		yield return Do.For(1f, ratio => {
			CoverMask.transform.localScale = Vector3.one * (1 - ratio);
		});


	}
}
