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

		readonly Dictionary<Game.MapId, string> mapNames = new() {
			{
				Game.MapId.Village,
				"Village"
			},
			{
				Game.MapId.ForestEntrance,
				"Forest 01"
			},
			{
				Game.MapId.ForestCliffs,
				"Forest 02"
			},
			{
				Game.MapId.ForestSpiritGate,
				"Spirit Gate"
			},
			{
				Game.MapId.ForestClearing,
				"Clearing"
			},
			{
				Game.MapId.ForestCave,
				"Cave"
			},
			{
				Game.MapId.ForestTunnel,
				"Natural Tunnel"
			}
		};

		readonly Dictionary<Game.MapId, string> teleportLocations = new() {
			{
				Game.MapId.Village,
				Village.Scene.Name
			},
		};

		Game.MapId teleportLocation;
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
			Game.MapId mapId = Engine.Profile.MapId;
			if (mapId == Game.MapId.Other) {
				mapId = Game.MapId.Village;
			}

			// 
			MapButtons.ForEach(mapButton => {
				Game.MapId mapButtonMapId = mapButton.MapId;
				Button button = mapButton.GetComponent<Button>();

				//
				mapButton
				.GetComponent<InformationButton>()
					.Configure(() => HighlightLocation(mapButtonMapId));

				//
				if (Engine.Profile.Acquired.Has(mapButtonMapId)) {
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

		void HighlightLocation(Game.MapId mapId) {
			bool canTeleport = Engine.Profile.Acquired.Has(mapId);

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

		void Teleport(Game.MapId mapId) {
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
			Loader.Scene.Load(new Game.NextScene {
				Name = teleportLocations[teleportLocation],
				PlayerDirection = Game.PlayerDirection.Down
			});
		}

		// -------------------------------------------------------------------------
	}
}