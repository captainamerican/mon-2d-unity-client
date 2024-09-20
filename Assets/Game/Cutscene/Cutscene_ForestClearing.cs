using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class Cutscene_ForestClearing : Cutscene {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;
	[SerializeField] Player Player;

	[Header("Locals")]
	[SerializeField] GameObject FirstMark;
	[SerializeField] GameObject SecondMark;
	[SerializeField] GameObject ThirdMark;
	[SerializeField] GameObject FourthMark;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Engine.Mode = EngineMode.Cutscene;

		//
		bool collectedEnoughSpiritEnergy =
			Engine.Profile.Acquired.SpiritWisdom.Count >= 3;

		//
		Player.SetFacing(Game.PlayerDirection.Down);

		yield return Go.To(
			Player.transform,
			FirstMark.transform.position,
			1.5f,
			Easing.SineOut01
		);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"It seems these old contraptions have reached their end.",
			"Mother said they were powered by more than the sacrifices of our kind.",
			"She was a bit literal, if flowery. So I wonder..."
		);
		yield return Wait.For(1.5f);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			collectedEnoughSpiritEnergy
				? new string[2] {
					"I've collected enough energy to repower this device.",
					"Let me just—",
				} :
				new string[3] {
					"3 Spirits should do it.",
					"I haven't collected any spirits yet.",
					"I'll return later."
				}
		);

		//
		if (!collectedEnoughSpiritEnergy) {
			Player.SetFacing(Game.PlayerDirection.Up);
			yield return Go.To(
				Player.transform,
				ThirdMark.transform.position,
				0.45f,
				Easing.SineOut01
			);
			Exit();
			yield break;
		}

		//
		Player.transform.position = SecondMark.transform.position;

		yield return Wait.For(1f);
		yield return Go.To(
			Player.transform,
			FourthMark.transform.position,
			1f,
			Easing.SineInOut01
		);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"I might vomit."
		);

		//
		Engine.Profile.StoryPoints.Add(Game.StoryPointId.UnlockedFirstSpiritGate);
		Exit();
	}

	void Exit() {
		Engine.Mode = EngineMode.PlayerControl;
	}

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped = Engine.Profile.StoryPoints.Has(Game.StoryPointId.UnlockedFirstSpiritGate);
	}

	// ---------------------------------------------------------------------------
}
