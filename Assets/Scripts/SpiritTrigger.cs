using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

public class SpiritTrigger : MonoBehaviour {

	// ---------------------------------------------------------------------------

	enum Direction {
		Up,
		Down,
		Left,
		Right,
	}

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;

	[Header("Locals")]
	[SerializeField] Direction Facing;
	[SerializeField] SpiritWisdom SpiritWisdom;
	[SerializeField] Transform SpiritTransform;
	[SerializeField] SpriteRenderer SpriteRenderer;
	[SerializeField] BoxCollider2D BoxCollider2D;
	[SerializeField] GameObject Alert;

	[Header("Battle")]
	[SerializeField] Game.Creature Creature;
	[SerializeField] List<Game.LootDrop> Loot;

	// ---------------------------------------------------------------------------

	void Start() {
		Alert.SetActive(false);

		//
		if (Engine.Profile.Acquired.Has(SpiritWisdom.Id)) {
			SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
			BoxCollider2D.enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player == null) {
			return;
		}

		//
		Engine.Mode = EngineMode.Cutscene;
		Time.timeScale = 0;

		StartCoroutine(ApproachPlayer(player));
	}

	// ---------------------------------------------------------------------------

	IEnumerator ApproachPlayer(Player player) {
		Game.PlayerDirection directionToFace = Game.PlayerDirection.None;

		Vector3 a = SpiritTransform.position;
		Vector3 b = player.transform.position;

		switch (Facing) {
			case Direction.Up:
			case Direction.Down:
				directionToFace = a.y > b.y
					? Game.PlayerDirection.Up
					: Game.PlayerDirection.Down;
				b.y = a.y < b.y
					? b.y - 2.5f
					: b.y + 2.5f;
				break;
			case Direction.Left:
			case Direction.Right:
				directionToFace = a.x > b.x
					? Game.PlayerDirection.Left
					: Game.PlayerDirection.Right;
				b.x = a.x < b.x
					? b.x - 2.5f
					: b.x + 2.5f;
				break;
		}

		//
		float duration = Vector3.Distance(a, b) * 0.1f;

		//
		Alert.SetActive(true);
		yield return Wait.ForReal(0.66f);
		player.SetFacing(directionToFace);
		player.Stop();
		Alert.SetActive(false);
		yield return Do.ForReal(duration, ratio => {
			SpiritTransform.position = Vector3.Lerp(a, b, ratio);
		});
		yield return Dialogue.Scene.Display(
			new string[1] { SpiritWisdom.BattleStart },
			"Spirit"
		);
		yield return Combat.Scene.Load(new() {
			Creature = Creature,
			SpiritWisdom = SpiritWisdom,
			Loot = Loot,
			OnDone = PostBattle,
			CantFlee = true,
			DontGiveBodyPart = true
		});
	}

	void PostBattle(Combat.BattleResult result) {
		Engine.Profile.Acquired.Add(SpiritWisdom.Id);
		Engine.Profile.Seen.Add(SpiritWisdom.Id);

		//
		SpiritTransform.gameObject.SetActive(false);

		//
		StartCoroutine(ExitingBattle());
	}

	IEnumerator ExitingBattle() {
		yield return Combat.Scene.Unload();

		//
		Engine.Mode = EngineMode.PlayerControl;
		Time.timeScale = 1;

		//
		gameObject.SetActive(false);
	}

	// ---------------------------------------------------------------------------

}
