using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using S = UnityEngine.SerializeField;

// -----------------------------------------------------------------------------

namespace StartScreen {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		public static string Name = "StartScreen";

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[S] Engine Engine;
		[S] PlayerInput PlayerInput;

		[Header("Locals")]
		[S] GameObject InitialContainer;
		[S] Button ContinueButton;
		[S] Button LoadGameButton;
		[S] Button StartButton;

		[Header("Load Save")]
		[S] ScrollView ScrollView;
		[S] SaveFileStatus SaveFileStatus;
		[S] GameObject LoadGameContainer;
		[S] Button AutoSaveButton;
		[S] InformationButton AutoSaveInformationButton;
		[S] SaveFileButton AutoSaveGameButton;
		[S] GameObject SaveGameTemplate;

		[Header("Confirm Load")]
		[S] GameObject ConfirmLoadModal;
		[S] Button ConfirmLoadModalCancel;

		// -------------------------------------------------------------------------

		string[] saveFilePaths;

		InputAction Cancel;
		Action OnBack;

		int selectedFileIndex;
		readonly List<Button> buttons = new();

		Game.SaveFile fileToLoad;

		// -------------------------------------------------------------------------

		void OnDestroy() {
			if (Cancel != null) {
				Cancel.performed -= HandleOnBack;
			}
		}

		void OnEnable() {
			OnDestroy();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += HandleOnBack;
		}

		IEnumerator Start() {
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
				Id = Game.Id.Generate(),
				PlaytimeAsSeconds = 0,
				Level = 1,
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
			newSaveFile.Magic = newSaveFile.MagicTotal;
			newSaveFile.Acquired.Add(Game.MapId.Village);
			newSaveFile.Party = new() {
				newSaveFile.Creatures[0].Id
			};

			newSaveFile.Creatures[0].Adjustment = 1;
			newSaveFile.Creatures[0].PrepareForBattle();
			newSaveFile.Creatures[0].Heal();

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
				SaveFile = newSaveFile,
				PlayerDirection = Game.PlayerDirection.Down,
				ExecuteOnLoad = "New Game"
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
				AutoSaveGameButton.Configure(autosave, true);
				AutoSaveInformationButton.Configure(() => {
					selectedFileIndex = 0;
					ScrollView.UpdateVisibleButtonRange(buttons, selectedFileIndex);
					SaveFileStatus.Configure(autosave);
				});

				AutoSaveButton.onClick.RemoveAllListeners();
				AutoSaveButton.onClick.AddListener(() => LoadSave(autosave));

				buttons.Add(AutoSaveButton);
			}

			AutoSaveButton.gameObject.SetActive(autosave != null);

			//
			Do.ForEach(saveFilePaths
				.Where(IsNotAutoSave)
				.ToList()
				.Select(ReadSaveFile)
				.OrderByDescending(saveFile => saveFile.SavedAt)
				.ToList(),
				(saveFile, index) => {
					GameObject go = Instantiate(SaveGameTemplate, SaveGameTemplate.transform.parent);
					go.SetActive(true);

					go
						.GetComponent<SaveFileButton>()
						.Configure(saveFile, false);

					int j = index;
					go
						.GetComponent<InformationButton>()
						.Configure(() => {
							selectedFileIndex = autosave != null ? j + 1 : j;
							ScrollView.UpdateVisibleButtonRange(buttons, selectedFileIndex);
							SaveFileStatus.Configure(saveFile);
						});

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
				OnBack = () => {
					LoadGameContainer.SetActive(false);
					Game.Focus.This(LoadGameButton);
				};
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
			Cancel.performed -= HandleOnBack;
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