using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class Cutscene_ForestClearingBoss : Cutscene {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;
	[SerializeField] Player Player;
	[SerializeField] Camera Camera;

	[Header("Locals")]
	[SerializeField] GameObject FirstMark;
	[SerializeField] GameObject SecondMark;
	[SerializeField] GameObject ThirdMark;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Engine.Mode = EngineMode.Cutscene;

		//
		bool hasASecondCreature = Engine.Profile.Party.Count > 1;
		bool beenHereBefore = Engine.Profile.GamePoints.Contains(Game.GamePointId.SurveyedForestBoss);

		//
		//Engine.Profile.GamePoints.Add(Game.GamePointId.SurveyedForestBoss);

		//
		if (beenHereBefore) {
			yield return hasASecondCreature
				? ReadyToParty()
				: BeenHereBeforeStillNoSecondCreature();
			Exit();
			yield break;
		}

		//
		Player.Stop();
		yield return Wait.For(1.5f);

		Player.SetFacing(Game.PlayerDirection.Right);
		yield return Go.To(
			Player.transform,
			FirstMark.transform.position,
			1f,
			Easing.EaseOutSine01
		);
		Player.SetFacing(Game.PlayerDirection.Down);
		yield return Go.To(
			Player.transform,
			SecondMark.transform.position,
			2f,
			Easing.EaseOutSine01
		);

		yield return Wait.For(1f);

		Vector3 cameraPosition = Camera.transform.position;
		yield return Go.To(
			Camera.transform,
			ThirdMark.transform.position,
			2f,
			Easing.EaseOutSine01
		);
		yield return Wait.For(3f);
		yield return Dialogue.Scene.Display(
			new string[2] {
				"A mutated Monk Trap. A very large specimen.",
				"I never heard of them interfering with the teleporters before."
			},
			"Lethia"
		);
		yield return Go.To(
			Camera.transform,
			cameraPosition,
			2f,
			Easing.EaseOutSine01
		);

		//
		if (hasASecondCreature) {
			yield return ReadyToParty();
		} else {

			yield return Dialogue.Scene.Display(
				new string[2] {
					"In any case, I'm going to need more than one creature to dispatch this mutant.",
					"I can assemble one from the creatures of the forest."
				},
				"Lethia"
			);
		}

		//
		Exit();
	}

	void Exit() {
		Engine.Mode = EngineMode.PlayerControl;
	}

	// ---------------------------------------------------------------------------

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped = Engine.Profile.GamePoints.Contains(Game.GamePointId.BeatenForestBoss);
	}

	// ---------------------------------------------------------------------------

	IEnumerator BeenHereBeforeStillNoSecondCreature() {

		yield break;
	}

	IEnumerator ReadyToParty() {

		yield break;
	}

	// ---------------------------------------------------------------------------
}
