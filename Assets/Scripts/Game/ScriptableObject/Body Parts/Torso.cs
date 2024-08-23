using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Torso Body Part", menuName = "MoN/Body Part/Torso")]
public class TorsoBodyPart : BodyPartBase {

	// ---------------------------------------------------------------------------

	public Game.TypeOfAppendages TypeOfAppendages;

	//

	public static new string Label {
		get {
			return "Torso";
		}
	}

	public string LocomotionLabel {
		get {
			return TypeOfAppendages switch {
				Game.TypeOfAppendages.NoLowerTwoUpper => "Serpentine",
				Game.TypeOfAppendages.OneLowerNoUpper or Game.TypeOfAppendages.OneLowerTwoUpper => "Uniped",
				Game.TypeOfAppendages.TwoLowerNoUpper or Game.TypeOfAppendages.TwoLowerTwoUpper => "Biped",
				Game.TypeOfAppendages.FourLower or Game.TypeOfAppendages.FourLowerTwoUpper => "Quadraped",
				Game.TypeOfAppendages.SixLower => "Sexaped",
				//
				_ => "Nulped",
			};
		}
	}

	public int HowManyAppendages {
		get {
			return TypeOfAppendages switch {
				Game.TypeOfAppendages.OneLowerNoUpper => 1,
				Game.TypeOfAppendages.TwoLowerNoUpper => 2,
				Game.TypeOfAppendages.NoLowerTwoUpper => 2,
				Game.TypeOfAppendages.OneLowerTwoUpper => 3,
				Game.TypeOfAppendages.TwoLowerTwoUpper => 4,
				Game.TypeOfAppendages.FourLower => 4,
				Game.TypeOfAppendages.FourLowerTwoUpper => 6,
				Game.TypeOfAppendages.SixLower => 6,
				_ => 0,
			};
		}
	}

	// ---------------------------------------------------------------------------

}