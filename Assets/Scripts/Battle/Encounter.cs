using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Battle {
	enum Phase {
		NoBattle,
		BattleStart,
		BattleTurnStart,
		BattlePlayerActionSelection,
		BattleAction,
		BattleTurnEnd,
		BattleEnd
	}

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
		Dialogue Dialogue;

		[SerializeField]
		Button FirstAction;

		static Encounter Self;
		WorldEnemy.Enemy enemy;

		Phase phase;

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
			CoverMask.transform.localScale = Vector3.one * 2;

			Player.GetComponent<SpriteRenderer>().sortingOrder = 11;
			enemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = 11;

			yield return Wait.For(1f);
			yield return Do.For(1f, ratio => {
				CoverMask.transform.localScale = (1 - ratio) * 2 * Vector3.one;
			});
			yield return Wait.For(1f);

			Content.SetActive(true);
			ItemsMenu.SetActive(false);
			ActionMenu.SetActive(false);
			MovesMenu.SetActive(false);
			CreaturesMenu.SetActive(false);

			Dialogue.Display("An angry spirit emerges.");
			yield return Wait.Until(Dialogue.IsDone);

			TurnStart();
		}

		void TurnStart() {
			ActionMenu.SetActive(true);
			FirstAction.Select();
			FirstAction.OnSelect(null);
		}

		void TurnAction() {
		}

		void TurnEnd() {

		}

		void BattleEnd() {
		}

		public void ShowMoves() {
			Debug.Log("Moves");
		}

		public void ShowItems() {
			Debug.Log("Items");
		}

		public void ShowCreatures() {
			Debug.Log("Creatures");
		}

		public void Run() {
			Debug.Log("Run");
		}
	}
}