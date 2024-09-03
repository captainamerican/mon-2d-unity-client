using System.Collections;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace StartScreen {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "StartScreen";

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] Button ContinueButton;
		[SerializeField] Button LoadGameButton;
		[SerializeField] Button StartButton;

		// -------------------------------------------------------------------------

		string[] saveFilePaths;

		// -------------------------------------------------------------------------

		IEnumerator Start() {
			saveFilePaths = Directory
				.GetFiles(
					Application.persistentDataPath,
					"*.lethia1",
					SearchOption.TopDirectoryOnly
				);
			bool hasSaveFiles = saveFilePaths.Length > 0;

			//
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
						}
					}
				},
			};
			newSaveFile.Acquired.Add(Game.MapId.Village);
			newSaveFile.Party = new() {
				newSaveFile.Creatures[0].Id
			};

			//
			Loader.Scene.Load(new Game.NextScene {
				Name = Village.Scene.Name,
				Destination = Village.Scene.Location_Tree,
				SaveFile = newSaveFile
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
			Loader.Scene.Load(new Game.NextScene {
				Name = saveFile.SceneName,
				Destination = saveFile.CurrentLocation,
				SaveFile = saveFile
			});
		}

		public void LoadGame() {
		}

		// -------------------------------------------------------------------------
	}
}