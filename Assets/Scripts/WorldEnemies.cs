using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace World {
	[Serializable]
	public class Respawn {
		public WorldEnemy.Enemy Enemy;
		public float TimeUntilRespawn;
	}

	public class WorldEnemies : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] GameObject Player;
		[SerializeField] List<WorldEnemy.Enemy> Enemies = new();
		[SerializeField] List<Respawn> Respawns = new();

		// -------------------------------------------------------------------------

		WorldEnemy.Enemy ActiveEnemy;

		List<Respawn> respawnsToRemove = new();

		// -------------------------------------------------------------------------

		private void Update() {
			foreach (Respawn respawn in Respawns) {
				respawn.TimeUntilRespawn -= Time.deltaTime;
				if (respawn.TimeUntilRespawn < 0) {
					respawn.Enemy.gameObject.SetActive(true);
					respawn.Enemy.Alertness = WorldEnemy.Alertness.Unaware;
					respawn.Enemy.GetComponentInChildren<SpriteRenderer>().color = Color.white;
					respawn.Enemy.GetComponentInChildren<NavMeshAgent>().isStopped = false;

					respawnsToRemove.Add(respawn);
				}
			}

			if (respawnsToRemove.Count > 0) {
				respawnsToRemove.ForEach(respawn => Respawns.Remove(respawn));
				respawnsToRemove.Clear();
			}
		}

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

						//
						ActiveEnemy = enemy;

						enemy.GetComponentInChildren<NavMeshAgent>().isStopped = true;
						enemy.Alertness = WorldEnemy.Alertness.InBattle;
						enemy.Stop();
						Enemies.ForEach(enemy => {
							if (enemy == null || !enemy.gameObject.activeInHierarchy) {
								return;
							}

							enemy.Stop();
						});

						//
						Game.EncounterPossibility encounter = enemy.RollAppearance();
						encounter.Creature.Health = 999;

						StartCoroutine(
							Combat.Scene.Load(new Combat.Battle {
								SpiritId = Game.SpiritId.None,
								Creature = encounter.Creature,
								PossibleLoot = encounter.PossibleLoot,
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
			switch (result) {
				case Combat.BattleResult.Won:
					ActiveEnemy.Stop();
					ActiveEnemy.gameObject.SetActive(false);
					Respawns.Add(new Respawn() {
						Enemy = ActiveEnemy,
						TimeUntilRespawn = 300
					});
					break;

				case Combat.BattleResult.Fled:
					ActiveEnemy.Stop();
					ActiveEnemy.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
					Respawns.Add(new Respawn() {
						Enemy = ActiveEnemy,
						TimeUntilRespawn = 5
					});
					break;
			}

			//
			ActiveEnemy = null;

			//
			StartCoroutine(LeavingBattle());
		}

		IEnumerator LeavingBattle() {
			yield return Combat.Scene.Unload();

			Engine.Mode = EngineMode.PlayerControl;
		}

		// -------------------------------------------------------------------------

	}
}