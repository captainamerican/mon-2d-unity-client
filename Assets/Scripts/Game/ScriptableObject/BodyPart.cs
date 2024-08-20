using UnityEngine;

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part")]
public class BodyPart : ScriptableObject {
	public CreatureBase Base;
	public Game.PartOfBody Part;
	public string Name;
	public int ExperienceToLevel;

	public string TypeName {
		get {
			switch (Part) {
				case Game.PartOfBody.Head:
					return "Head";
				case Game.PartOfBody.Torso:
					return "Torso";
				case Game.PartOfBody.Tail:
					return "Tail";
				case Game.PartOfBody.FrontLeftAppendage:
					return "L. Front";
				case Game.PartOfBody.MiddleLeftAppendage:
					return "L. Middle";
				case Game.PartOfBody.RearLeftAppendage:
					return "L. Rear";
				case Game.PartOfBody.FrontRightAppendage:
					return "R. Front";
				case Game.PartOfBody.MiddleRightAppendage:
					return "R. Middle";
				case Game.PartOfBody.RearRightAppendage:
					return "R. Rear";
				default:
					return "???";
			}
		}
	}
}