using UnityEngine;

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part")]
public class BodyPart : ScriptableObject {
	public CreatureBase Base;
	public Game.PartOfBody Part;
	public string Name;
}