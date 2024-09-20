using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class Cutscene_Forest_TeleportersAreBroken : Cutscene {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Player Player;

	[Header("Locals")]
	[SerializeField] Transform FirstMark;
	[SerializeField] Transform SecondMark;
	[SerializeField] Transform ThirdMark;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Database.Engine.Mode = EngineMode.Cutscene;

		//
		Player.Stop();
		yield return Go.To(Player.transform, FirstMark.position, 1.5f, Easing.SineOut01);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"Whatever caused that noise has annoyed the forest creatures.",
			"I should avoid them for now."
		);
		yield return Go.To(Player.transform, SecondMark.position, 1.5f, Easing.SineOut01);
		yield return Go.To(Player.transform, ThirdMark.position, 3f, Easing.SineOut01);
		yield return Wait.For(1.5f);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"This device isn't responding to magic.",
			"I don't know how to fix them...",
			"Hopefully investigating that noise leads to a solution.",
			"Unfortunately, this means I have to go through the forest.",
			"Thankfully I already assembled a creature yesterday."
		);
		yield return Go.To(Player.transform, SecondMark.position, 3f, Easing.SineOut01);

		//
		Database.Engine.Profile.StoryPoints.Add(Game.StoryPointId.InspectedBrokenTeleporter);
		Exit();
	}

	void Exit() {
		Database.Engine.Mode = EngineMode.PlayerControl;
	}

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped =
			Database.Engine.Profile.StoryPoints.Has(Game.StoryPointId.InspectedBrokenTeleporter) ||
			Database.Engine.Profile.StoryPoints.Has(Game.StoryPointId.UnlockedFirstSpiritGate) ||
			Database.Engine.Profile.StoryPoints.Has(Game.StoryPointId.BeatenForestBoss);
	}

	// ---------------------------------------------------------------------------
}
