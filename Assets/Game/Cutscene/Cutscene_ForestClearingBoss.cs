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
	[SerializeField] GameObject FourthMark;
	[SerializeField] GameObject Fourth2Mark;
	[SerializeField] GameObject FifthMark;
	[SerializeField] GameObject SixthMark;

	[SerializeField] ForestClearing_ContinuePrompt ContinuePrompt;

	[SerializeField] Combat.Battle Battle;

	// ---------------------------------------------------------------------------

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped = Engine.Profile.StoryPoints.Has(Game.StoryPointId.BeatenForestBoss);
	}

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Engine.Mode = EngineMode.Cutscene;

		//
		bool hasASecondCreature = Engine.Profile.Party.Count > 1;

		//
		if (HasBeenHereBefore()) {
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

	IEnumerator HeardSomething() {
		Vector3 cameraOriginalPosition = Camera.transform.position;

		//
		Player.Stop();
		yield return MoveCharacterToFirstMark();
		yield return MoveCharacterToSecondMark();
		yield return Wait.For(1f);
		yield return MoveCameraToShowBoss();
		yield return Wait.For(3f);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"A mutant Honey Pot. Haven't seen one this large since I was young.",
			"Never seen them interfere with the teleporters before."
		);
		yield return MoveCameraBackToOriginal(cameraOriginalPosition);
	}

	IEnumerator BeenHereBeforeStillNoSecondCreature() {
		Player.Stop();
		yield return MoveCharacterToFirstMark();
		Player.SetFacing(Game.PlayerDirection.Down);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"Must not have been paying attention as I still don't have a second creature."
		);
		ReturnControlToPlayer();
	}

	IEnumerator ReadyToParty() {
		yield return MoveCharacterToFirstMark();
		yield return MoveCharacterToSecondMark();
		yield return Wait.For(1f);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"I'm as ready as I'll ever be."
		);
		yield return Wait.For(0.1f);

		bool choseToFight = false;
		yield return ContinuePrompt.Display(actionIndex => choseToFight = actionIndex > 0);

		yield return (choseToFight)
			? ThrowDowntheGauntlet()
			: StillNotReadyToFight();
	}

	IEnumerator NotReadyToParty() {
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"In any case, I need than one creature to dispatch this mutant.",
			"I can assemble a new one from the beasts of the forest."
		);
		yield return MoveCharacterBackToFirstMark();

		//
		Engine.Profile.StoryPoints.Add(Game.StoryPointId.SurveyedForestBoss);
		ReturnControlToPlayer();
	}

	IEnumerator StillNotReadyToFight() {
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"I've still things left undone."
		);
		ReturnControlToPlayer();
	}

	IEnumerator ThrowDowntheGauntlet() {
		Debug.LogWarning("TODO: Show the boss chewing on Emily.");
		Debug.LogWarning("TODO: To ramp up");

		//
		Player.GetComponent<CapsuleCollider2D>()
			.enabled = false;

		//
		yield return Go.To(
			Player.transform,
			FourthMark.transform.position,
			0.5f,
			Easing.EaseOutSine01
		);
		yield return Go.To(
			Player.transform,
			Fourth2Mark.transform.position,
			0.25f,
			Easing.EaseOutSine01
		);
		yield return Go.To(
			Player.transform,
			FifthMark.transform.position,
			1f,
			Easing.EaseOutSine01
		);

		//
		Player.GetComponent<CapsuleCollider2D>()
			.enabled = true;

		//
		yield return Wait.For(0.66f);
		yield return Go.To(
			Player.transform,
			SixthMark.transform.position,
			1.66f,
			Easing.EaseOutSine01
		);

		//
		Battle.OnDone = PostBattle;
		yield return Combat.Scene.Load(Battle);
	}

	void PostBattle(Combat.BattleResult _) {
		Engine.Mode = EngineMode.Cutscene;
		Engine.Profile.StoryPoints.Add(Game.StoryPointId.BeatenForestBoss);

		Debug.LogWarning("TODO: Post-battle scene");

		ReturnControlToPlayer();
	}

	// ---------------------------------------------------------------------------

	IEnumerator MoveCharacterToFirstMark() {
		Player.SetFacing(Game.PlayerDirection.Right);

		//
		yield return Go.To(
			Player.transform,
			FirstMark.transform.position,
			1f,
			Easing.EaseOutSine01
		);
	}

	IEnumerator MoveCharacterToSecondMark() {
		Player.SetFacing(Game.PlayerDirection.Down);

		//
		yield return Go.To(
			Player.transform,
			SecondMark.transform.position,
			2f,
			Easing.EaseOutSine01
		);
	}

	IEnumerator MoveCharacterBackToFirstMark() {
		Player.SetFacing(Game.PlayerDirection.Up);

		//
		yield return Go.To(
			Player.transform,
			FirstMark.transform.position,
			0.55f,
			Easing.EaseOutSine01
		);
	}

	IEnumerator MoveCameraToShowBoss() {
		yield return Go.To(
			Camera.transform,
			ThirdMark.transform.position,
			2f,
			Easing.EaseOutSine01
		);
	}

	IEnumerator MoveCameraBackToOriginal(Vector3 position) {
		yield return Go.To(
			Camera.transform,
			position,
			2f,
			Easing.EaseOutSine01
		);
	}

	// ---------------------------------------------------------------------------

	void ReturnControlToPlayer() {
		Engine.Mode = EngineMode.PlayerControl;
	}

	bool HasBeenHereBefore() {
		return Engine.Profile.StoryPoints
			.Has(Game.StoryPointId.SurveyedForestBoss);
	}

	// ---------------------------------------------------------------------------

}
