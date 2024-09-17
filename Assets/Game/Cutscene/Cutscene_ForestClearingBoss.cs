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

	[SerializeField] ForestClearing_ContinuePrompt ContinuePrompt;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Engine.Mode = EngineMode.Cutscene;

		//
		bool hasASecondCreature = Engine.Profile.Party.Count > 1;

		//
		if (BeenHereBefore()) {
			yield return hasASecondCreature
				? ReadyToParty()
				: BeenHereBeforeStillNoSecondCreature();
		} else {
			yield return HeardSomething();
			yield return (hasASecondCreature)
				? ReadyToParty()
				: NotReadyToParty();
		}
	}

	// ---------------------------------------------------------------------------

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped = Engine.Profile.GamePoints.Contains(Game.GamePointId.BeatenForestBoss);
	}

	// ---------------------------------------------------------------------------

	IEnumerator HeardSomething() {
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
		yield return MoveCameraToShowBoss();
		yield return Wait.For(3f);
		yield return Dialogue.Scene.Display(
			"Lethia",
			"A mutated Monk Trap. Very rare specimen. I want the head...",
			"Never heard of them interfering with teleporters before."
		);
		yield return NoveCameraBackToOriginal();
	}

	IEnumerator BeenHereBeforeStillNoSecondCreature() {
		yield return Dialogue.Scene.Display(
			"Lethia",
			"Must not have been paying attention as I still don't have a second creature."
		);
		ReturnControlToPlayer();
	}

	IEnumerator ReadyToParty() {
		Debug.LogWarning("TODO: Show the boss chewing on Emily.");
		yield return Dialogue.Scene.Display(
			"Lethia",
			"I'm as ready as I'll ever be."
		);

		bool choseToFight = false;
		yield return ContinuePrompt.Display(actionIndex => choseToFight = actionIndex > 0);

		if (choseToFight) {
			Debug.Log("Fight!");
		} else {
			Debug.Log("Never mind.");
		}
	}

	IEnumerator NotReadyToParty() {
		yield return Dialogue.Scene.Display(
			"Lethia",
			"In any case, I need than one creature to dispatch this mutant.",
			"I can assemble one from the beasts of the forest."
		);

		//
		Engine.Profile.GamePoints.Add(Game.GamePointId.SurveyedForestBoss);
		ReturnControlToPlayer();
	}

	// ---------------------------------------------------------------------------

	void ReturnControlToPlayer() {
		Engine.Mode = EngineMode.PlayerControl;
	}

	bool BeenHereBefore() {
		return Engine.Profile.GamePoints
			.Contains(Game.GamePointId.SurveyedForestBoss);
	}

	IEnumerator MoveCameraToShowBoss() {
		yield return Go.To(
			Camera.transform,
			ThirdMark.transform.position,
			2f,
			Easing.EaseOutSine01
		);
	}

	IEnumerator NoveCameraBackToOriginal() {
		yield return Go.To(
			Camera.transform,
			new Vector3(0, 0, -5),
			2f,
			Easing.EaseOutSine01
		);
	}

	// ---------------------------------------------------------------------------

}
