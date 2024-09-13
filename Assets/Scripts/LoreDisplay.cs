using UnityEngine;
using TMPro;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class LoreDisplay : MonoBehaviour {

	// ---------------------------------------------------------------------------

	static LoreDisplay Self;

	static public void Display(Game.LoreId loreId) {
		Self.ShowLore(loreId);
	}

	static public void Done() {
		Self.Close();
	}

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;
	[SerializeField] GameObject Dialog;
	[SerializeField] TextMeshProUGUI Lore;
	[SerializeField] Button Button;

	// ---------------------------------------------------------------------------

	void Awake() {
		Self = this;

		Dialog.SetActive(false);
	}

	public void ShowLore(Game.LoreId loreId) {
		Lore lore = Database.Engine.GameData.Get(loreId);
		if (lore == null) {
			return;
		}

		//
		Engine.Profile.Acquired.Add(loreId);

		//
		Button.interactable = true;
		Lore.text = lore.Text;

		//
		Game.Focus.This(Button);
		Dialog.SetActive(true);
		Engine.Mode = EngineMode.Dialogue;
	}

	public void Close() {
		Button.interactable = false;
		Lore.text = "";

		//
		Dialog.SetActive(false);
		Game.Focus.Nothing();
		Engine.Mode = EngineMode.PlayerControl;
	}

	// ---------------------------------------------------------------------------

}
