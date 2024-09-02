using System.Collections;

using UnityEngine;

namespace World {

	public class Scene : MonoBehaviour {
		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] MapId MapId;
		[SerializeField] string MapName;
		[SerializeField] GameObject Player;

		IEnumerator Start() {
			NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
			}
			Engine.NextScene = null;

			//
			Engine.Profile.MapId = MapId;
			Engine.Profile.CurrentLocation = Player.transform.position;

			//
			yield return Dialogue.Scene.Load();
			yield return Menu.Scene.Load();
			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}
	}
}