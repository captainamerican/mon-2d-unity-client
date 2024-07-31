using System.Collections.Generic;

using UnityEngine;

namespace World {

	public class WorldEnemies : MonoBehaviour {
		[SerializeField]
		Engine Engine;

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
				if (enemy == null) {
					continue;
				}

				switch (enemy.Alertness) {
					case WorldEnemy.Alertness.Unaware:
						if (Vector3.Distance(Player.transform.position, enemy.BodyTransform.position) <= enemy.AwarenessRadius) {
							enemy.TargetEnteredAwarenessRange(Player.transform);
						}
						break;

					default:
						if (Vector3.Distance(Player.transform.position, enemy.BodyTransform.position) > 3f) {
							continue;
						}

						Debug.Log(enemy);
						Battle.Begin(enemy);
						Enemies.ForEach(enemy => enemy.Stop());
						return;
				}


			}
		}
	}
}