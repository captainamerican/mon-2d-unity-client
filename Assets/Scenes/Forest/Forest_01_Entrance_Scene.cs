using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace World {

	public class Forest_01_Entrance_Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		[SerializeField]
		List<WorldEnemy.Enemy> Enemies = new();

		IEnumerator Start() {
			Player.transform.position = Engine.GetNextScenePosition();
			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}

		private void LateUpdate() {
			for (int i = 0; i < Enemies.Count; i++) {
				WorldEnemy.Enemy enemy = Enemies[i];
				if (enemy.Alertness != WorldEnemy.Alertness.Unaware) {
					continue;
				}

				if (Vector3.Distance(Player.transform.position, enemy.transform.position) > enemy.AwarenessRadius) {
					continue;
				}

				enemy.TargetEnteredAwarenessRange(Player.transform);
			}
		}
	}
}