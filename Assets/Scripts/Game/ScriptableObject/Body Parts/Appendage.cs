using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Appendage Body Part", menuName = "MoN/Body Part/Appendage")]
public class AppendageBodyPart : BodyPartBase {

	// ---------------------------------------------------------------------------

	static public new string Label(Game.TypeOfAppendages type, int index) {
		return type switch {
			Game.TypeOfAppendages.OneLowerNoUpper => index switch {
				0 => "Lower",
				_ => "???",
			},
			Game.TypeOfAppendages.TwoLowerNoUpper => index switch {
				0 => "Left Leg",
				1 => "Reft Leg",
				_ => "???",
			},
			Game.TypeOfAppendages.NoLowerTwoUpper => index switch {
				0 => "Left Arm",
				1 => "Reft Arm",
				_ => "???",
			},
			Game.TypeOfAppendages.OneLowerTwoUpper => index switch {
				0 => "Left Arm",
				1 => "Reft Arm",
				2 => "Lower Leg",
				_ => "???",
			},
			Game.TypeOfAppendages.TwoLowerTwoUpper => index switch {
				0 => "Left Arm",
				1 => "Reft Arm",
				2 => "Left Leg",
				3 => "Right Leg",
				_ => "???",
			},
			Game.TypeOfAppendages.FourLower => index switch {
				0 => "Left Front Leg",
				1 => "Right Front Leg",
				2 => "Left Rear Leg",
				3 => "Right Rear Leg",
				_ => "???",
			},
			Game.TypeOfAppendages.FourLowerTwoUpper => index switch {
				0 => "Left Arm",
				1 => "Right Arm",
				2 => "Left Front Leg",
				3 => "Right Front Leg",
				4 => "Left Rear Leg",
				5 => "Right Rear Leg",
				_ => "???",
			},
			Game.TypeOfAppendages.SixLower => index switch {
				0 => "Left Front Leg",
				1 => "Right Front Leg",
				2 => "Left Middle Leg",
				3 => "Right Middle Leg",
				4 => "Left Rear Leg",
				5 => "Right Rear Leg",
				_ => "???",
			},
			_ => "???",
		};
	}

	// ---------------------------------------------------------------------------
}