using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Battle {
	public class Encounter : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		Player Player;

		[SerializeField]
		Image Creature;

		[SerializeField]
		Image Enemy;

		[SerializeField]
		GameObject Cover;

		[SerializeField]
		GameObject CoverMask;

		[SerializeField]
		GameObject Content;

		[SerializeField]
		GameObject ItemsMenu;

		[SerializeField]
		GameObject ActionMenu;

		[SerializeField]
		GameObject MovesMenu;

		[SerializeField]
		GameObject CreaturesMenu;

		[SerializeField]
		GameObject Dialogue;

		static Encounter Self;
		WorldEnemy.Enemy enemy;

		static public void Begin(WorldEnemy.Enemy enemy) {
			Self.StartBattle(enemy);

		}

		void Start() {
			Self = this;
			Cover.SetActive(false);
			Content.SetActive(false);
		}

		void StartBattle(WorldEnemy.Enemy enemy) {
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
			yield return Wait.For(1f);

			Content.SetActive(true);
			Dialogue.SetActive(true);
			ItemsMenu.SetActive(false);
			ActionMenu.SetActive(false);
			MovesMenu.SetActive(false);
			CreaturesMenu.SetActive(false);
		}
	}
}