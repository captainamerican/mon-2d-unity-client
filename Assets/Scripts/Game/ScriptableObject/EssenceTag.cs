using UnityEngine;

[CreateAssetMenu(fileName = "Essence Tag", menuName = "MoN/Essence Tag")]
public class EssenceTag : ScriptableObject {
	public Game.EssenceTagId Id;
	public string Name;

	[TextArea(2, 10)]
	public string Description;
}