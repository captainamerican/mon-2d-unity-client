using UnityEngine;

[CreateAssetMenu(fileName = "Lore", menuName = "MoN/Lore")]
public class Lore : ScriptableObject {
	public Game.LoreId Id;
	public string Name;

	[TextArea(2, 20)]
	public string Text;
}