using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class Cutscene_ForestCantGetBack : Cutscene {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Engine Engine;
	[SerializeField] Player Player;

	[Header("Locals")]
	[SerializeField] GameObject FirstMark;
	[SerializeField] GameObject SecondMark;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Engine.Mode = EngineMode.Cutscene;

		//
		yield return MoveCharacterToFirstMark();

		//
		switch (Mathf.Round(Engine.Profile.StoryPoints.Get(
			Game.StoryPointId.ToldAboutPocketTeleporter
		))) {
			case 0:
				yield return Wait.For(1f);
				yield return Dialogue.Scene.Speaks(
						"Lethia",
						"There's no way across...",
						"I'll have to use my pocket porter to head back to the village.",
						"I keep it with my map."
					);
				break;

			case 2:
				yield return Dialogue.Scene.Speaks(
					"Lethia",
					"I can use my pocket porter—which I keep with my map—to return to the village."
				);
				break;

			case 1:
				yield return Dialogue.Scene.Speaks(
					"Lethia",
					"I can use my pocket porter to return to the village.",
					"It's very handily kept with my map.",
					"The map I have of... the world.",
					"Typically referred to as a World Map."
				);
				break;

			case 3:
				yield return Dialogue.Scene.Speaks(
					"Lethia",
					"I keep my picket porter in the World Map. Found in the main menu."
				);
				break;

			case 4:
				yield return Dialogue.Scene.Speaks(
					"Lethia",
					"Press [spacebar]. Select the World Map option. Move the cursor to the village. Press [e].",
					"That's all."
				);
				break;

			case 5:
				Player.SetFacing(Game.PlayerDirection.Down);
				yield return Wait.For(3f);
				yield return Dialogue.Scene.Speaks(
					"Lethia",
					"*sigh*"
				);
				yield return Wait.For(0.5f);
				Loader.Scene.Load(new Game.NextScene {
					Name = Village.Scene.Name,
					PlayerDirection = Game.PlayerDirection.Down
				});
				yield break;
		}

		//
		yield return MoveCharacterToSecondMark();

		//
		Engine.Profile.StoryPoints.Adjust(Game.StoryPointId.ToldAboutPocketTeleporter, 1);
		Engine.Mode = EngineMode.PlayerControl;
	}

	// ---------------------------------------------------------------------------

	public void Skip(ReturnValue returnValue) {
		Debug.Log(!Engine.Profile.StoryPoints.Has(Game.StoryPointId.SurveyedForestBoss) + " " +
			Engine.Profile.StoryPoints.Has(Game.StoryPointId.UsedPocketTeleporter) + " " +
			Engine.Profile.StoryPoints.Has(Game.StoryPointId.BeatenForestBoss));
		returnValue.Skipped =
			!Engine.Profile.StoryPoints.Has(Game.StoryPointId.SurveyedForestBoss) ||
			Engine.Profile.StoryPoints.Has(Game.StoryPointId.UsedPocketTeleporter) ||
			Engine.Profile.StoryPoints.Has(Game.StoryPointId.BeatenForestBoss);
	}

	// ---------------------------------------------------------------------------

	IEnumerator MoveCharacterToFirstMark() {
		Player.SetFacing(Game.PlayerDirection.Up);

		//
		yield return Go.To(
			Player.transform,
			FirstMark.transform.position,
			1f,
			Easing.EaseOutSine01
		);
		Player.Stop();
	}

	IEnumerator MoveCharacterToSecondMark() {
		Player.SetFacing(Game.PlayerDirection.Down);

		//
		yield return Go.To(
			Player.transform,
			SecondMark.transform.position,
			1f,
			Easing.EaseOutSine01
		);
		Player.Stop();
	}

	// ---------------------------------------------------------------------------

}
