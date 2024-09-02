using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class WorldMapMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject WorldMapContent;

		[SerializeField] List<MapButton> MapButtons;
		[SerializeField] List<MapButton> VisualMapButtons;
		[SerializeField] List<SpriteRenderer> SpriteRenderers;

		[SerializeField] Sprite CurrentSprite;
		[SerializeField] TextMeshProUGUI LocationLabel;

		[SerializeField] TextMeshProUGUI TeleportNotice;

		[Header("Teleport Dialog")]
		[SerializeField] GameObject TeleportDialog;
		[SerializeField] Button TeleportCancelButton;

		[Header("Already Here Dialog")]
		[SerializeField] GameObject AlreadyHereDialog;
		[SerializeField] Button AlreadyHereCancelButton;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		float cameraSize;
		Vector3 cameraPosition;

		Dictionary<MapId, string> mapNames = new() {
			{
				MapId.Village,
				"Village"
			},
			{
				MapId.Forest01,
				"Forest 01"
			},
			{
				MapId.Forest02,
				"Forest 02"
			},
			{
				MapId.Forest03,
				"Spirit Gate"
			},
			{
				MapId.Forest04,
				"Clearing"
			},
			{
				MapId.ForestCave01,
				"Cave"
			},
			{
				MapId.ForestCave02,
				"Natural Tunnel"
			}
		};

		Dictionary<MapId, string> teleportLocations = new() {
			{
				MapId.Village,
				Village.Scene.Name
			},
		};

		MapId teleportLocation;
		Button focusedButton;

		// --------------------------------------------------------------------------

		void OnEnable() {
			RemoveInputCallbacks();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;

			TeleportDialog.SetActive(false);
			WorldMapContent.SetActive(true);

			//
			if (Camera.main.transform.parent != null && Camera.main.transform.parent.CompareTag("Player")) {
				cameraSize = Camera.main.orthographicSize;
				Camera.main.orthographicSize = 32;

				cameraPosition = Camera.main.transform.parent.position;
				Camera.main.transform.parent.position = Vector3.zero;
			}

			//
			MapId mapId = Engine.Profile.MapId;
			if (mapId == MapId.Other) {
				mapId = MapId.Village;
			}

			// 
			MapButtons.ForEach(mapButton => {
				MapId mapButtonMapId = mapButton.MapId;
				Button button = mapButton.GetComponent<Button>();

				//
				mapButton
				.GetComponent<InformationButton>()
					.Configure(() => HighlightLocation(mapButtonMapId));

				//
				if (Engine.Profile.TeleportUnlocked.Contains(mapButtonMapId)) {
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => {
						focusedButton = button;

						Teleport(mapButtonMapId);
					});
				}

				//
				if (mapButtonMapId == mapId) {
					Game.Focus.This(button);
				}
			});
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			if (Camera.main.transform.parent != null && Camera.main.transform.parent.CompareTag("Player")) {
				Camera.main.transform.parent.position = cameraPosition;
				Camera.main.orthographicSize = cameraSize;
			}

			//
			InitialMenu.gameObject.SetActive(true);

			//
			WorldMapContent.SetActive(false);
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------  

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void HighlightLocation(MapId mapId) {
			bool canTeleport = Engine.Profile.TeleportUnlocked.Contains(mapId);

			//
			TeleportNotice.gameObject.SetActive(canTeleport);
			SpriteRenderers.ForEach(s => s.sprite = null);

			//
			MapButton mapButton = VisualMapButtons.Find(mb => mb.MapId == mapId);
			if (mapButton == null) {
				return;
			}

			//
			mapButton.GetComponent<SpriteRenderer>().sprite = CurrentSprite;
			LocationLabel.text = mapNames[mapId];
		}

		void Teleport(MapId mapId) {
			if (Engine.Profile.MapId == mapId) {
				AlreadyHereDialog.SetActive(true);
				Game.Focus.This(AlreadyHereCancelButton);
				return;
			}

			//
			teleportLocation = mapId;

			//
			TeleportDialog.SetActive(true);
			Game.Focus.This(TeleportCancelButton);
		}

		public void OnTeleport(int action) {
			TeleportDialog.SetActive(false);
			AlreadyHereDialog.SetActive(false);

			if (action < 1) {
				Game.Focus.This(focusedButton);
				return;
			}

			//
			Engine.NextScene = new NextScene {
				Name = teleportLocations[teleportLocation]
			};

			SceneManager.LoadSceneAsync(Loader.Scene.Name, LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------
	}
}