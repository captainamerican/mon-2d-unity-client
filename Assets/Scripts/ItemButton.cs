using TMPro;

using UnityEngine;

// -----------------------------------------------------------------------------

public class ItemButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] TextMeshProUGUI Name;
	[SerializeField] TextMeshProUGUI Quantity;

	// ---------------------------------------------------------------------------

	public void Configure(Game.InventoryEntry entry) {
		Name.text = entry.Item.Name;
		Quantity.text = $"x{entry.Amount}";
	}

	// ---------------------------------------------------------------------------

}