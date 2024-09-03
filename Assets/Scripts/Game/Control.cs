using UnityEngine.InputSystem;

namespace Game {
	static public class Control {
		static public InputAction Get(PlayerInput playerInput, string action) {
			return playerInput.currentActionMap.FindAction(action);
		}
	}
}
