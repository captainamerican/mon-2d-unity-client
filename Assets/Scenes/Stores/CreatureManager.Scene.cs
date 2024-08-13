using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CreatureManager {
	public class Scene : MonoBehaviour {
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] EventSystem LocalEventSystem;

		// Start is called before the first frame update
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}
	}
}