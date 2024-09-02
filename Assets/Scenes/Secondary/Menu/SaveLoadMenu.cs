using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class SaveLoadMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum Phase {
			Initial,
			FileMenu,
			Confirm
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] ScrollView ScrollView;

		[Header("Which Modal")]
		[SerializeField] GameObject WhichMenu;
		[SerializeField] List<Button> WhichMenuButtons;

		[Header("File List")]
		[SerializeField] GameObject FileList;
		[SerializeField] GameObject SaveFileTemplate;
		[SerializeField] SaveFileButton AutosaveFile;
		[SerializeField] Button AutosaveButton;
		[SerializeField] Button CreateNew;

		[Header("Overwrite Modal")]
		[SerializeField] GameObject ConfirmOverwriteModal;
		[SerializeField] Button ConfirmOverwriteCancel;

		[Header("Confirm Load Modal")]
		[SerializeField] GameObject ConfirmLoadModal;
		[SerializeField] Button ConfirmLoadCancel;

		[Header("Confirm Delete Modal")]
		[SerializeField] GameObject ConfirmDeleteModal;
		[SerializeField] Button ConfirmDeleteCancel;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;
		int selectedInitialIndex;
		int selectedFileIndex;

		readonly List<Button> fileButtons = new();
		readonly List<Game.SaveFile> saveFiles = new();
		Game.SaveFile autosave;

		// --------------------------------------------------------------------------

		void OnEnable() {
			selectedInitialIndex = 0;

			//
			ConfigureCancel();
			LoadSaveFiles();
			ShowWhich();
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			switch (phase) {
				case Phase.Initial:
					InitialMenu.gameObject.SetActive(true);

					//
					gameObject.SetActive(false);
					break;

				case Phase.FileMenu:
					ShowWhich();
					break;

				case Phase.Confirm:
					phase = Phase.FileMenu;
					ConfirmOverwriteModal.SetActive(false);
					ConfirmLoadModal.SetActive(false);
					ConfirmDeleteModal.SetActive(false);
					FileList.SetActive(true);
					HighlightFileButton();
					break;
			}
		}

		// -------------------------------------------------------------------------  

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void ShowWhich() {
			phase = Phase.Initial;
			ConfirmOverwriteModal.SetActive(false);
			ConfirmLoadModal.SetActive(false);
			ConfirmDeleteModal.SetActive(false);
			FileList.SetActive(false);
			WhichMenu.SetActive(true);
			HighlightWhichButton();
		}

		public void ShowSaveList() {
			selectedInitialIndex = 0;
			selectedFileIndex = 0;

			//
			ClearList();
			GenerateSaveFileList(false);
			AddCreateNewIfPossible();
			UpdateFileButtonNavigation(SaveFile);
			ShowFileList();
		}

		public void ShowLoadList() {
			selectedInitialIndex = 0;
			selectedFileIndex = 0;

			//
			ClearList();
			GenerateSaveFileList(true);
			UpdateFileButtonNavigation(LoadFile);
			ShowFileList();
		}

		void AddCreateNewIfPossible() {
			if (saveFiles.Count < 100) {
				CreateNew.gameObject.SetActive(true);
				fileButtons.Insert(0, CreateNew);
			}
		}

		void GenerateSaveFileList(bool includeAutosave) {
			saveFiles.ForEach(saveFile => {
				if (saveFile.IsAutoSave && saveFile == saveFiles[0]) {
					return;
				}

				//
				GameObject saveFileGO = Instantiate(SaveFileTemplate, SaveFileTemplate.transform.parent);
				saveFileGO.SetActive(true);

				SaveFileButton saveFileButton = saveFileGO.GetComponent<SaveFileButton>();
				saveFileButton.Configure(saveFile);

				//
				fileButtons.Add(saveFileButton.GetComponent<Button>());
			});

			//
			AutosaveFile.gameObject.SetActive(false);
			if (includeAutosave && autosave != null) {
				AutosaveFile.gameObject.SetActive(true);
				AutosaveFile.Configure(autosave);

				//
				fileButtons.Insert(0, AutosaveButton);
			}
		}

		void ShowFileList() {
			phase = Phase.FileMenu;

			//
			ConfirmOverwriteModal.SetActive(false);
			ConfirmLoadModal.SetActive(false);
			ConfirmDeleteModal.SetActive(false);
			WhichMenu.SetActive(false);
			FileList.SetActive(true);
			ScrollView.UpdateVisibleButtonRange(fileButtons, selectedFileIndex);
			HighlightFileButton();
		}

		void UpdateFileButtonNavigation(UnityAction onClick) {
			for (int i = 0; i < fileButtons.Count; i++) {
				int up = i == 0 ? fileButtons.Count - 1 : i - 1;
				int down = i == fileButtons.Count - 1 ? 0 : i + 1;

				Button button = fileButtons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = fileButtons[up];
				navigation.selectOnDown = fileButtons[down];

				button.navigation = navigation;

				//
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => {
						selectedFileIndex = j;
						ScrollView.UpdateVisibleButtonRange(fileButtons, j);
					});

				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(onClick);
			}
		}

		void LoadSaveFiles() {
			saveFiles.Clear();

			// save_00.letha1 (autosave)
			// save_98.letha1
			// save_99.letha1
			// I don't want to rename files so I will count up
			// from the maximum number of save files
			Directory
				.GetFiles(
					Application.persistentDataPath,
					"*.lethia1",
					SearchOption.TopDirectoryOnly
				)
				.OrderBy(filePath => filePath)
				.ToList()
				.ForEach(filePath => {
					string json = File.ReadAllText(filePath);

					var saveFile = JsonUtility.FromJson<Game.SaveFile>(json);
					saveFiles.Add(saveFile);
				});

			//
			if (saveFiles.Count > 0 && saveFiles[0].IsAutoSave && saveFiles[0].FileIndex == 0) {
				Debug.Log("autsoave");
				autosave = saveFiles[0];
				saveFiles.Remove(autosave);
			}
		}

		void ClearList() {
			AutosaveFile.gameObject.SetActive(false);
			CreateNew.gameObject.SetActive(false);

			//
			fileButtons.Remove(AutosaveButton);
			fileButtons.Remove(CreateNew);

			fileButtons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			fileButtons.Clear();
		}

		void ConfigureCancel() {
			RemoveInputCallbacks();

			//
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;
		}

		void HighlightWhichButton() {
			Game.Focus.This(WhichMenuButtons[selectedInitialIndex]);
		}

		void HighlightFileButton() {
			if (fileButtons.Count > 0) {
				Game.Focus.This(fileButtons[selectedFileIndex]);
			} else {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		public void SaveFile() {
			if (fileButtons[selectedFileIndex] == CreateNew) {
				ConfirmSave(1);
				return;
			}

			//
			phase = Phase.Confirm;
			ConfirmOverwriteModal.SetActive(true);
			Game.Focus.This(ConfirmOverwriteCancel);
		}

		public void LoadFile() {

			//
			phase = Phase.Confirm;
			ConfirmLoadModal.SetActive(true);
			Game.Focus.This(ConfirmLoadCancel);

		}

		public void DeleteFile() {
		}

		public void ConfirmSave(int action) {
			if (action < 1) {
				ShowFileList();
				return;
			}

			//
			Engine.Profile.FileIndex = (fileButtons[selectedFileIndex] == CreateNew)
				? 100 - (saveFiles.Count + 1)
				: saveFiles[selectedFileIndex].FileIndex;

			string path = $"{Application.persistentDataPath}/save_{Engine.Profile.FileIndex:d2}.lethia1";
			string json = JsonUtility.ToJson(Engine.Profile);

			//
			File.WriteAllText(path, json);

			//
			var newSaveFile = JsonUtility.FromJson<Game.SaveFile>(json);
			if (fileButtons[selectedFileIndex] == CreateNew) {
				saveFiles.Insert(0, newSaveFile);
			} else {
				saveFiles[selectedFileIndex] = newSaveFile;
			}

			//
			int selected = selectedFileIndex;
			ShowSaveList();
			selectedFileIndex = selected;

			ScrollView.UpdateVisibleButtonRange(fileButtons, selected);
			HighlightFileButton();
		}

		public void ConfirmLoad(int action) {
			if (action < 1) {
				ShowFileList();
				return;
			}

			//
			var saveFile = (autosave != null && fileButtons[selectedFileIndex] == AutosaveButton)
				? autosave
				: saveFiles[autosave != null ? selectedFileIndex - 1 : selectedFileIndex];

			Debug.Log(saveFile);
		}

		public void ConfirmDelete(int action) {
		}

		// -------------------------------------------------------------------------
	}
}