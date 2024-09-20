using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

public class Cutscene_NewGame : Cutscene {

	// ---------------------------------------------------------------------------

	[Header("Globals")]
	[SerializeField] Player Player;
	[SerializeField] Camera Camera;
	[SerializeField] SpriteRenderer PlayerBody;
	[SerializeField] GameObject Autosave;

	[Header("Locals")]
	[SerializeField] Transform FirstMark;
	[SerializeField] Transform SecondMark;
	[SerializeField] Transform ThirdMark;

	// ---------------------------------------------------------------------------

	override protected IEnumerator Script() {
		Database.Engine.Mode = EngineMode.Cutscene;

		//
		PlayerBody.enabled = false;

		Camera.transform.SetParent(null);


		yield return Go.To(Camera.transform, FirstMark.position, 5f, Easing.SineOut01);
		yield return Wait.For(2f);

		Player.transform.position = SecondMark.position;
		PlayerBody.enabled = true;
		yield return Wait.For(0.5f);
		yield return Go.To(Player.transform, ThirdMark.position, 0.25f, Easing.SineOut01);
		yield return Dialogue.Scene.Speaks(
			"Lethia",
			"!",
			"What was that sound just now? An explosion?",
			"There's unnatural magical energies in the air.",
			"It's late, but I should investigate."
		);
		yield return Wait.For(0.25f);

		Camera.transform.SetParent(Player.transform);
		yield return Go.To(
			Camera.transform,
			Player.transform.TransformPoint(new Vector3(0, 0, -5)),
			2f,
			Easing.SineOut01
		);

		//
		Database.Engine.Profile.StoryPoints.Add(Game.StoryPointId.SawNewGameScene);
		Database.Engine.Mode = EngineMode.PlayerControl;
		Autosave.SetActive(true);
	}

	public void Skip(ReturnValue returnValue) {
		returnValue.Skipped =
				Database.Engine.Profile.StoryPoints.Has(Game.StoryPointId.SawNewGameScene);
	}

	// ---------------------------------------------------------------------------
}
