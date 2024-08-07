using UnityEngine;

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part")]
public class BodyPart : ScriptableObject {
	public CreatureBase Base;
	public Game.BodyPart Part;
	public string Name;
}