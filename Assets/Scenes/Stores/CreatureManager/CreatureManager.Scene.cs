using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CreatureManager {
	public class Scene : MonoBehaviour {
		public const string Name = "CreatureManager";

		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Debug")]
		[SerializeField] EventSystem DebugEventSystem;
		[SerializeField] Camera DebugCamera;

		[Header("Locals")]
		[SerializeField] Canvas Canvas;
		[SerializeField] StartMenu StartMenu;
		[SerializeField] List<GameObject> MenusToDisableOnLoad;

		static public Action OnDone = () => Debug.Log("Close menu");

		static public IEnumerator Load(Action onDone) {
			OnDone = onDone;

			//
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}

		void Awake() {
			if (EventSystem.current == null) {
				DebugEventSystem.enabled = true;
			}

			if (Camera.main == null) {
				DebugCamera.enabled = true;
			}
		}

		void Start() {
			Canvas.worldCamera = Camera.main;

			//
			MenusToDisableOnLoad.ForEach(menu => menu.SetActive(false));

			StartMenu.gameObject.SetActive(true);
			StartMenu.Configure(OnDone);
		}
	}
}