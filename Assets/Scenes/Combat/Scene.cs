using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Combat {
	[Serializable]
	public class Battle {
		public Game.Creature Creature;
		public Game.SpiritId SpiritId;
		public Action OnDone;
	}

	// ---------------------------------------------------------------------------

	public class Scene : MonoBehaviour {

		static readonly public string Name = "Combat";
		static Battle Battle = null;

		static public IEnumerator Load(Battle battle) {
			Battle = battle;

			//
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------

#if UNITY_EDITOR
		[Header("Debug")]
		[SerializeField] Battle DebugBattle;
#endif

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject Spirit;
		[SerializeField] RectTransform PlayerCreatureContainer;
		[SerializeField] CanvasGroup PlayerCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen PlayerCombatant;
		[SerializeField] RectTransform EnemyCreatureContainer;
		[SerializeField] CanvasGroup EnemyCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen EnemyCombatant;
		[SerializeField] GameObject ActionListContainer;
		[SerializeField] GameObject ItemListContainer;
		[SerializeField] GameObject MoveListContainer;
		[SerializeField] GameObject ConfirmFleeDialog;
		[SerializeField] GameObject TargetsListContainer;

		// -------------------------------------------------------------------------

		IEnumerator Start() {
#if UNITY_EDITOR
			if (Battle == null) {
				Battle = DebugBattle;
			}
#endif

			//
			if (Spirit != null)
				Spirit.SetActive(false);
			PlayerCreatureContainer.gameObject.SetActive(false);
			EnemyCreatureContainer.gameObject.SetActive(false);
			ActionListContainer.SetActive(false);
			ItemListContainer.SetActive(false);
			MoveListContainer.SetActive(false);
			ConfirmFleeDialog.SetActive(false);
			TargetsListContainer.SetActive(false);

			//
			yield return Dialogue.Scene.Load();
#if UNITY_EDITOR
			yield return Wait.For(1f);
#endif

			//
			ConfigureCombatant(PlayerCombatant, Engine.Profile.GetPartyCreature(0));
			ConfigureCombatant(EnemyCombatant, Battle.Creature);

			PlayerCombatant.HideBars();
			EnemyCombatant.HideBars();

			//
			yield return ShowAndThenPositionOpponent();

			// prepare for combat
		}

		void ConfigureCombatant(CombatantOnScreen combatantOnScreen, Game.Creature creature) {
			combatantOnScreen.Configure(creature);
		}

		IEnumerator ShowAndThenPositionOpponent() {
			Vector3 enemyStart = Vector3.zero;
			Vector3 enemyEnd = EnemyCreatureContainer.anchoredPosition;

			EnemyCreatureCanvasGroup.alpha = 0;
			EnemyCreatureContainer.anchoredPosition = enemyStart;
			EnemyCreatureContainer.gameObject.SetActive(true);


			//
			yield return Do.For(0.25f, ratio => EnemyCreatureCanvasGroup.alpha = ratio);
			yield return Wait.For(1.25f);
			yield return Do.For(
				0.5f,
				ratio => EnemyCreatureContainer.anchoredPosition = Vector3.Lerp(enemyStart, enemyEnd, ratio),
				Easing.EaseOutSine01
			);
			yield return Wait.For(0.25f);

			//
			Vector3 playerEnd = PlayerCreatureContainer.anchoredPosition;
			Vector3 playerStart = playerEnd;
			playerStart.y -= 10;

			PlayerCreatureCanvasGroup.alpha = 0;
			PlayerCreatureContainer.anchoredPosition = playerStart;
			PlayerCreatureContainer.gameObject.SetActive(true);

			// 
			yield return Do.For(
				0.33f,
				ratio => {
					PlayerCreatureCanvasGroup.alpha = ratio;
					PlayerCreatureContainer.anchoredPosition = Vector3.Lerp(playerStart, playerEnd, ratio);
				},
				Easing.EaseOutSine01
			);
			yield return Wait.For(0.25f);

			// 
			EnemyCombatant.ShowBars();
			PlayerCombatant.ShowBars();

			yield return Do.For(2f, ratio => {
				PlayerCombatant.FancyFillUp(ratio, Engine.Profile.Magic, Engine.Profile.MagicTotal);
				EnemyCombatant.FancyFillUp(ratio);
			});
		}

		// -------------------------------------------------------------------------

	}
}