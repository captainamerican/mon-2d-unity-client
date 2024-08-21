using UnityEngine;

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part")]
public class BodyPart : ScriptableObject {
	public CreatureBase Base;
	public Game.PartOfBody Part;
	public string Name;
	public int ExperienceToLevel;

	public string TypeName {
		get {
			return NameOfType(Part);
		}
	}

	static public string NameOfType(Game.PartOfBody partOfBody) {
		switch (partOfBody) {
			case Game.PartOfBody.Head:
				return "Head";
			case Game.PartOfBody.Torso:
				return "Torso";
			case Game.PartOfBody.Tail:
				return "Tail";
			case Game.PartOfBody.Appendage:
				return "Appendage";
			default:
				return "???";
		}
	}
}