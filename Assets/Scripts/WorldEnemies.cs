using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

// -----------------------------------------------------------------------------

namespace World {
	public class WorldEnemies : MonoBehaviour {

		// -------------------------------------------------------------------------

		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		[SerializeField]
		List<WorldEnemy.Enemy> Enemies = new();

		// -------------------------------------------------------------------------

		private void LateUpdate() {
			if (Engine.Mode != EngineMode.PlayerControl) {
				return;
			}

			for (int i = 0; i < Enemies.Count; i++) {
				WorldEnemy.Enemy enemy = Enemies[i];
				if (enemy == null || !enemy.gameObject.activeInHierarchy) {
					continue;
				}

				switch (enemy.Alertness) {
					case WorldEnemy.Alertness.Unaware:
						if (Vector3.Distance(Player.transform.position, enemy.BodyTransform.position) <= enemy.AwarenessRadius) {
							enemy.TargetEnteredAwarenessRange(Player.transform);
						}
						break;

					case WorldEnemy.Alertness.InBattle:
						break;

					default:
						if (Vector3.Distance(Player.transform.position, enemy.BodyTransform.position) > 3f) {
							continue;
						}

						enemy.GetComponentInChildren<NavMeshAgent>().isStopped = true;
						enemy.Alertness = WorldEnemy.Alertness.InBattle;
						enemy.Stop();
						Enemies.ForEach(enemy => {
							if (enemy == null || !enemy.gameObject.activeInHierarchy) {
								return;
							}

							enemy.Stop();
						});

						Engine.Mode = EngineMode.Battle;
						StartCoroutine(
							Combat.Scene.Load(new Combat.Battle {
								SpiritId = Game.SpiritId.None,
								Creature = enemy.RollAppearance().Creature,
								CantFlee = false,
								OnDone = PostBattle
							})
						);
						return;
				}
			}
		}

		// -------------------------------------------------------------------------

		void PostBattle(Combat.BattleResult result) {
			StartCoroutine(LeavingBattle());
		}

		IEnumerator LeavingBattle() {
			yield return Combat.Scene.Unload();

			Engine.Mode = EngineMode.PlayerControl;
		}

		// -------------------------------------------------------------------------

	}
}