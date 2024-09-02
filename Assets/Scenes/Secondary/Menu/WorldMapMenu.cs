using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
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

		// --------------------------------------------------------------------------

		void OnEnable() {
			RemoveInputCallbacks();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;

			WorldMapContent.SetActive(true);

			//
			if (Camera.main.transform.parent != null && Camera.main.transform.parent.CompareTag("Player")) {
				cameraSize = Camera.main.orthographicSize;
				Camera.main.orthographicSize = 32;

				cameraPosition = Camera.main.transform.parent.position;
				Camera.main.transform.parent.position = Vector3.zero;
			}

			//
			MapButtons.ForEach(mapButton => {
				mapButton.GetComponent<InformationButton>()
					.Configure(() => HighlightLocation(mapButton.MapId));
			});

			//
			MapId mapId = Engine.MapId;
			if (mapId == MapId.Other) {
				mapId = MapId.Village;
			}

			Button button = MapButtons
				.Find(mb => mb.MapId == mapId)
				?.GetComponent<Button>();
			if (button == null) {
				return;
			}

			Game.Btn.Select(button);
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

		// -------------------------------------------------------------------------
	}
}