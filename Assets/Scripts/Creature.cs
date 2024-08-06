using System.Collections.Generic;
using Battle;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "MoN/Creature")]
public class Creature : ScriptableObject {
	public string Name;

	public BodyPart Head;
	public BodyPart Torso;
	public BodyPart Tail;

	public BodyPart LeftAppendage1;
	public BodyPart LeftAppendage2;
	public BodyPart LeftAppendage3;

	public BodyPart RightAppendage1;
	public BodyPart RightAppendage2;
	public BodyPart RightAppendage3;

	public List<Skill> Skills = new();
}