using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Head Body Part", menuName = "MoN/Body Part/Head")]
public class HeadBodyPart : BodyPartBase {

	// ---------------------------------------------------------------------------

	public List<Skill> InnateSkills;

	public static new string Label {
		get {
			return "Head";
		}
	}

	// ---------------------------------------------------------------------------

}