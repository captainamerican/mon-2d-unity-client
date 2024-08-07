using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

namespace World {

	public class WorldEnemies : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		Battle.Encounter Encounter;

		[SerializeField]
		GameObject Player;

		[SerializeField]
		List<WorldEnemy.Enemy> Enemies = new();

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

						Encounter.StartBattle(enemy);
						Enemies.ForEach(enemy => {
							if (enemy == null || !enemy.gameObject.activeInHierarchy) {
								return;
							}

							enemy.Stop();
						});
						return;
				}
			}
		}
	}
}