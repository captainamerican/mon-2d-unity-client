using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace StartScreen {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "StartScreen";

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject InitialContainer;
		[SerializeField] Button ContinueButton;
		[SerializeField] Button LoadGameButton;
		[SerializeField] Button StartButton;

		[Header("Load Save")]
		[SerializeField] SaveFileStatus SaveFileStatus;
		[SerializeField] GameObject LoadGameContainer;
		[SerializeField] Button AutoSaveButton;
		[SerializeField] InformationButton AutoSaveInformationButton;
		[SerializeField] SaveFileButton AutoSaveGameButton;
		[SerializeField] GameObject SaveGameTemplate;

		[Header("Confirm Load")]
		[SerializeField] GameObject ConfirmLoadModal;
		[SerializeField] Button ConfirmLoadModalCancel;

		// -------------------------------------------------------------------------

		string[] saveFilePaths;

		InputAction Cancel;
		Action OnBack;

		int selectedFileIndex;
		List<Button> buttons = new();

		Game.SaveFile fileToLoad;

		// -------------------------------------------------------------------------

		IEnumerator Start() {
			if (Cancel != null) {
				Cancel.performed -= HandleOnBack;
			}
			Cancel = Game.Control.Get(PlayerInput, "Cancel");

			//
			LoadGameContainer.SetActive(false);

			//
			saveFilePaths = Directory
				.GetFiles(
					Application.persistentDataPath,
					"*.lethia1",
					SearchOption.TopDirectoryOnly
				);

			//
			bool hasSaveFiles = saveFilePaths.Length > 0;
			ContinueButton.gameObject.SetActive(hasSaveFiles);
			LoadGameButton.gameObject.SetActive(hasSaveFiles);

			//
			Engine.Mode = EngineMode.None;
			Engine.NextScene = null;

			//
			Game.Focus.This(hasSaveFiles ? ContinueButton : StartButton);

			//
			yield return Loader.Scene.Clear();
		}

		void HandleOnBack(InputAction.CallbackContext _) {
			OnBack?.Invoke();
			OnBack = null;
		}

		// -------------------------------------------------------------------------

		public void StartNewGame() {
			Game.SaveFile newSaveFile = new() {
				FileIndex = -1,
				IsAutoSave = false,
				Level = 1,
				Magic = 999,
				Wisdom = 5,
				Hunger = 1,
				Skills = new() {
					new() {
						SkillId = Game.SkillId.Scratch
					}
				},
				Creatures = new() {
					new() {
						Id = Game.Id.Generate(),
						Name = "Wolfie",
						Health = 999,
						Head = new() {
							BodyPartId = Game.BodyPartId.WolfHead
						},
						Torso = new() {
							BodyPartId = Game.BodyPartId.WolfTorso
						},
						Tail = new() {
							BodyPartId = Game.BodyPartId.WolfTail
						},
						Appendages = new() {
							new() {
								BodyPartId = Game.BodyPartId.WolfFrontLeg
							},
							new() {
								BodyPartId = Game.BodyPartId.WolfFrontLeg
							},
							new() {
								BodyPartId = Game.BodyPartId.WolfRearLeg
							},
							new() {
								BodyPartId = Game.BodyPartId.WolfRearLeg
							}
						},
						Skills = new() {
							Game.SkillId.Scratch
						},
					}
				},
			};
			newSaveFile.Acquired.Add(Game.MapId.Village);
			newSaveFile.Party = new() {
				newSaveFile.Creatures[0].Id
			};
			newSaveFile.Inventory.AdjustItem(Database.Engine.GameData.Get(Game.ItemId.CoffeeF), 5);
			newSaveFile.Inventory.AdjustItem(Database.Engine.GameData.Get(Game.ItemId.PotionF), 5);

			newSaveFile.Acquired.Add(Game.ItemId.CoffeeF);
			newSaveFile.Seen.Add(Game.ItemId.CoffeeF);
			newSaveFile.Acquired.Add(Game.ItemId.PotionF);
			newSaveFile.Seen.Add(Game.ItemId.PotionF);

			newSaveFile.Acquired.Add(newSaveFile.Creatures[0].Head.BodyPartId);
			newSaveFile.Seen.Add(newSaveFile.Creatures[0].Head.BodyPartId);
			newSaveFile.Acquired.Add(newSaveFile.Creatures[0].Torso.BodyPartId);
			newSaveFile.Seen.Add(newSaveFile.Creatures[0].Torso.BodyPartId);
			newSaveFile.Acquired.Add(newSaveFile.Creatures[0].Tail.BodyPartId);
			newSaveFile.Seen.Add(newSaveFile.Creatures[0].Tail.BodyPartId);
			newSaveFile.Acquired.Add(newSaveFile.Creatures[0].Appendages[0].BodyPartId);
			newSaveFile.Seen.Add(newSaveFile.Creatures[0].Appendages[0].BodyPartId);
			newSaveFile.Acquired.Add(newSaveFile.Creatures[0].Appendages[2].BodyPartId);
			newSaveFile.Seen.Add(newSaveFile.Creatures[0].Appendages[2].BodyPartId);

			newSaveFile.Acquired.Add(newSaveFile.Skills[0].SkillId);
			newSaveFile.Seen.Add(newSaveFile.Skills[0].SkillId);

			//
			Cancel.performed -= HandleOnBack;
			Loader.Scene.Load(new Game.NextScene {
				Name = Village.Scene.Name,
				Destination = Village.Scene.Location_Tree,
				SaveFile = newSaveFile,
				PlayerDirection = Game.PlayerDirection.Down
			});
		}

		public void ContinueGame() {
			Game.SaveFile saveFile = saveFilePaths.ToList()
				.Select(filePath => {
					string json = File.ReadAllText(filePath);

					//
					return JsonUtility.FromJson<Game.SaveFile>(json);
				})
				.OrderByDescending(saveFile => saveFile.SavedAt)
				.ToList()
				[0];

			//
			Cancel.performed -= HandleOnBack;
			Loader.Scene.Load(new Game.NextScene {
				Name = saveFile.SceneName,
				Destination = saveFile.CurrentLocation,
				SaveFile = saveFile
			});
		}

		public void LoadGame() {
			buttons.Remove(AutoSaveButton);
			buttons.ForEach(button => Destroy(button.gameObject));
			buttons.Clear();

			//
			Game.SaveFile autosave = saveFilePaths
				.Where(IsAutoSave)
				.Select(ReadSaveFile)
				.ToList()
				.FirstOrDefault();
			if (autosave != null) {
				AutoSaveGameButton.Configure(autosave);
				AutoSaveInformationButton.Configure(() => SaveFileStatus.Configure(autosave));

				AutoSaveButton.onClick.RemoveAllListeners();
				AutoSaveButton.onClick.AddListener(() => LoadSave(autosave));

				buttons.Add(AutoSaveButton);
			}

			AutoSaveButton.gameObject.SetActive(autosave != null);

			//
			saveFilePaths
				.Where(IsNotAutoSave)
				.ToList()
				.Select(ReadSaveFile)
				.OrderByDescending(saveFile => saveFile.SavedAt)
				.ToList()
				.ForEach(saveFile => {
					GameObject go = Instantiate(SaveGameTemplate, SaveGameTemplate.transform.parent);
					go.SetActive(true);

					go
						.GetComponent<SaveFileButton>()
						.Configure(saveFile);

					go
						.GetComponent<InformationButton>()
						.Configure(() => SaveFileStatus.Configure(saveFile));

					Button button = go.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => LoadSave(saveFile));

					buttons.Add(button);
				});

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;
				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;
			}

			//
			Game.Focus.This(buttons[0]);

			//
			LoadGameContainer.SetActive(true);
			OnBack = () => {
				LoadGameContainer.SetActive(false);
				Game.Focus.This(LoadGameButton);
			};
		}

		void LoadSave(Game.SaveFile saveFile) {
			fileToLoad = saveFile;

			//
			ConfirmLoadModal.SetActive(true);
			Game.Focus.This(ConfirmLoadModalCancel);
			OnBack = () => OnLoadSave(0);
		}

		public void OnLoadSave(int actionIndex) {
			ConfirmLoadModal.SetActive(false);

			//
			if (actionIndex < 1) {
				Game.Focus.This(buttons[selectedFileIndex]);
				return;
			}

			//
			StartCoroutine(LoadingGame());
		}

		IEnumerator LoadingGame() {
			Game.Focus.Nothing();

			//
			LoadGameContainer.SetActive(false);
			InitialContainer.SetActive(false);

			//
			yield return Wait.For(0.5f);

			//
			Loader.Scene.Load(new Game.NextScene {
				Name = fileToLoad.SceneName,
				Destination = fileToLoad.CurrentLocation,
				SaveFile = fileToLoad
			});
		}

		// -------------------------------------------------------------------------

		Game.SaveFile ReadSaveFile(string filePath) {
			string json = File.ReadAllText(filePath);

			//
			return JsonUtility.FromJson<Game.SaveFile>(json);
		}

		bool IsAutoSave(string filePath) {
			return filePath.EndsWith("autosave.lethia1");
		}
		bool IsNotAutoSave(string filePath) {
			return !filePath.EndsWith("autosave.lethia1");
		}

		// -------------------------------------------------------------------------
	}
}