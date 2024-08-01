using UnityEngine;

public enum Part {
	None,
	Head,
	Torso,
	Tail,
	LeftArm,
	RightArm,
	LeftLeg,
	RightLeg
}

[CreateAssetMenu(fileName = "Body Part", menuName = "MoN/Body Part")]
public class BodyPart : ScriptableObject {
	public Part Part;
	public string Name;
}