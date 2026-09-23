using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public class MainMenuHandler : MonoBehaviour
{
    //initialize variable for full UI document
    private UIDocument _document;

    //initialize buttons
    private Button _startButton;
    private Button _optionsButton;
    private Button _exitButton;
    private Button _selectFileButton;
    private Button _selectFolderButton;
    private Button _exitSelectionPopupButton;
    private Button _easyButton;
    private Button _normalButton;
    private Button _hardButton;
    private Button _exitDifficultyPopupButton;
    private Button _addFolderButton;
    private Button _folderSelectionContinueButton;
    private Button _exitSettingsMenuButton;
    private static Button _errorPopupExitButton;

    //initialize visual elements
    private VisualElement _fileSelectionPopup;
    private VisualElement _folderSelectionPopup;
    private VisualElement _difficultySelectionPopup;
    private VisualElement _loadingScreen;
    private VisualElement _settingsMenu;
    private static VisualElement _errorPopup;

    //initialize settings menu items
    private static Slider _volumeSlider;
    private static Toggle _fullscreenToggle;
    private static DropdownField _resolutionDropdown;

    private static Label _errorPopupInfo;

    private void Awake()
    {
        //Gets primary ui document
        _document = GetComponent<UIDocument>();

        //gets ui buttons
        _startButton = _document.rootVisualElement.Q("StartButton") as Button;
        _optionsButton = _document.rootVisualElement.Q("OptionsButton") as Button;
        _exitButton = _document.rootVisualElement.Q("ExitButton") as Button;
        _selectFileButton = _document.rootVisualElement.Q("SelectFileButton") as Button;
        _selectFolderButton = _document.rootVisualElement.Q("SelectFolderButton") as Button;
        _exitSelectionPopupButton = _document.rootVisualElement.Q("ExitSelectionPopupButton") as Button;
        _easyButton = _document.rootVisualElement.Q("EasyButton") as Button;
        _normalButton = _document.rootVisualElement.Q("NormalButton") as Button;
        _hardButton = _document.rootVisualElement.Q("HardButton") as Button;
        _exitDifficultyPopupButton = _document.rootVisualElement.Q("ExitDifficultyPopupButton") as Button;
        _addFolderButton = _document.rootVisualElement.Q("AddFolderButton") as Button;
        _folderSelectionContinueButton = _document.rootVisualElement.Q("FolderSelectionContinueButton") as Button;
        _exitSettingsMenuButton = _document.rootVisualElement.Q("ExitSettingsMenuButton") as Button;
        _errorPopupExitButton = _document.rootVisualElement.Q("ErrorPopupExitButton") as Button;

        //gets ui visual elements (primarily for popup screens)
        _fileSelectionPopup = _document.rootVisualElement.Q("FileSelectionPopup") as VisualElement;
        _difficultySelectionPopup = _document.rootVisualElement.Q("DifficultySelectionPopup") as VisualElement;
        _loadingScreen = _document.rootVisualElement.Q("LoadingScreen") as VisualElement;
        _folderSelectionPopup = _document.rootVisualElement.Q("FolderSelectionPopup") as VisualElement;
        _settingsMenu = _document.rootVisualElement.Q("SettingsMenu") as VisualElement;
        _errorPopup = _document.rootVisualElement.Q("ErrorPopup") as VisualElement;

        //get settings menu items
        _volumeSlider = _document.rootVisualElement.Q("VolumeSlider") as Slider;
        _fullscreenToggle = _document.rootVisualElement.Q("FullscreenToggle") as Toggle;
        _resolutionDropdown = _document.rootVisualElement.Q("ResolutionDropdown") as DropdownField;

        _errorPopupInfo = _document.rootVisualElement.Q("ErrorPopupInfo") as Label;

        //Registers events for clicking each button
        _startButton.RegisterCallback<ClickEvent>(OnStartButtonPress);
        _optionsButton.RegisterCallback<ClickEvent>(OnOptionsButtonPress);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonPress);
        _selectFileButton.RegisterCallback<ClickEvent>(OnSelectFileButtonPress);
        _selectFolderButton.RegisterCallback<ClickEvent>(OnSelectFolderButtonPress);
        _exitSelectionPopupButton.RegisterCallback<ClickEvent>(OnExitSelectionPopupButtonPress);
        _easyButton.RegisterCallback<ClickEvent>(OnEasyButtonPress);
        _normalButton.RegisterCallback<ClickEvent>(OnNormalButtonPress);
        _hardButton.RegisterCallback<ClickEvent>(OnHardButtonPress);
        _exitDifficultyPopupButton.RegisterCallback<ClickEvent>(OnExitDifficultyPopupButtonPress);
        _addFolderButton.RegisterCallback<ClickEvent>(OnAddFolderButtonPress);
        _folderSelectionContinueButton.RegisterCallback<ClickEvent>(OnFolderSelectionContinueButtonPress);
        _exitSettingsMenuButton.RegisterCallback<ClickEvent>(OnExitSettingsMenuButtonPress);
        _errorPopupExitButton.RegisterCallback<ClickEvent>(OnErrorPopupExitButtonPress);

        //Register events for settings menu items
        _volumeSlider.RegisterValueChangedCallback(OnVolumeSliderChanged);
        _fullscreenToggle.RegisterValueChangedCallback(OnFullscreenToggleChanged);
        _resolutionDropdown.RegisterValueChangedCallback(OnResolutionDropdownChanged);

        //gives the resolution dropdown the viable resolutions for the users device
        PopulateResolutionDropdown();
    }

    private void OnDisable()
    {
        //dissables button events when buttons are dissabled
        _startButton.UnregisterCallback<ClickEvent>(OnStartButtonPress);
        _optionsButton.UnregisterCallback<ClickEvent>(OnOptionsButtonPress);
        _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonPress);
        _selectFileButton.UnregisterCallback<ClickEvent>(OnSelectFileButtonPress);
        _selectFolderButton.UnregisterCallback<ClickEvent>(OnSelectFolderButtonPress);
        _exitSelectionPopupButton.UnregisterCallback<ClickEvent>(OnExitSelectionPopupButtonPress);
        _easyButton.UnregisterCallback<ClickEvent>(OnEasyButtonPress);
        _normalButton.UnregisterCallback<ClickEvent>(OnNormalButtonPress);
        _hardButton.UnregisterCallback<ClickEvent>(OnHardButtonPress);
        _exitDifficultyPopupButton.UnregisterCallback<ClickEvent>(OnExitDifficultyPopupButtonPress);
        _addFolderButton.UnregisterCallback<ClickEvent>(OnAddFolderButtonPress);
        _folderSelectionContinueButton.UnregisterCallback<ClickEvent>(OnFolderSelectionContinueButtonPress);
        _exitSettingsMenuButton.UnregisterCallback<ClickEvent>(OnExitSettingsMenuButtonPress);
        _errorPopupExitButton.UnregisterCallback<ClickEvent>(OnErrorPopupExitButtonPress);

        //dissables settings menu items events when they are dissabled
        _volumeSlider.UnregisterValueChangedCallback(OnVolumeSliderChanged);
        _fullscreenToggle.UnregisterValueChangedCallback(OnFullscreenToggleChanged);
        _resolutionDropdown.UnregisterValueChangedCallback(OnResolutionDropdownChanged);
    }

    //pulls up the file selection popup to determine if users want to select a file or a folder
    private void OnStartButtonPress(ClickEvent evt)
    {
        Debug.Log("Start Button Pressed");
        _fileSelectionPopup.style.display = DisplayStyle.Flex;
    }

    //pulls up the settings menu
    private void OnOptionsButtonPress(ClickEvent evt)
    {
        Debug.Log("Options Button Pressed");
        _settingsMenu.style.display = DisplayStyle.Flex;
    }

    //exits the game
    private void OnExitButtonPress(ClickEvent evt)
    {
        Debug.Log("Exit Button Pressed");
        Application.Quit();
    }

    //pulls up the windows file browser for a user to select a file
    private void OnSelectFileButtonPress(ClickEvent evt)
    {
        Debug.Log("Select file Button Pressed");
        SelectFile();
    }

    //pulls up windows file browser for a user to select a folder
    private void OnSelectFolderButtonPress(ClickEvent evt)
    {
        Debug.Log("Select folder Button Pressed");
        SelectFolder();
        _folderSelectionPopup.style.display = DisplayStyle.Flex;
    }

    //returns to the main menu
    private void OnExitSelectionPopupButtonPress(ClickEvent evt)
    {
        Debug.Log("Exit selection popup Button Pressed");
        _fileSelectionPopup.style.display = DisplayStyle.None;
    }

    //sets difficulty to easy and loads the puzzle scene
    private void OnEasyButtonPress(ClickEvent evt)
    {
        Debug.Log("Easy Button Pressed");
        GameState.Instance.selectedDifficulty = 3;
        _loadingScreen.style.display = DisplayStyle.Flex;
        SceneManager.LoadScene("PuzzleScene");
    }

    //sets difficulty to normal and loads the puzzle scene
    private void OnNormalButtonPress(ClickEvent evt)
    {
        Debug.Log("Normal Button Pressed");
        GameState.Instance.selectedDifficulty = 5;
        _loadingScreen.style.display = DisplayStyle.Flex;
        SceneManager.LoadScene("PuzzleScene");
    }

    //sets difficulty to hard and loads the puzzle scene
    private void OnHardButtonPress(ClickEvent evt)
    {
        Debug.Log("Hard Button Pressed");
        GameState.Instance.selectedDifficulty = 7;
        _loadingScreen.style.display = DisplayStyle.Flex;
        SceneManager.LoadScene("PuzzleScene");
    }

    //returns to the selection popup menu
    private void OnExitDifficultyPopupButtonPress(ClickEvent evt)
    {
        Debug.Log("Exit difficulty popup Button Pressed");
        _difficultySelectionPopup.style.display = DisplayStyle.None;
    }

    //function to open the file browser for a user to select a file
    //base structure of this function was generated by AI then heavily modified
    private void SelectFile()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "Select a File",//display text in file browser
            "",//starting directory (uses the default or current location without an argument)
            "",//file filters for the file browser selection
            false //determines if the player can select multiple files
        );


        if (paths.Length > 0)
        {
            string path = paths[0];
            Debug.Log("Selected file path: " + path);
            //save the path to the game state for future use
            GameState.Instance.selectedFilePath = path;
            Debug.Log("Saved file path: " + GameState.Instance.selectedFilePath);
            _difficultySelectionPopup.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.Log("Error, No Path Selected");
        }
    }

    private void SelectFolder()
    {
        string[] folderPath = StandaloneFileBrowser.OpenFolderPanel(
            "Select Folder",
            "",
            false
        );

        if (!string.IsNullOrEmpty(folderPath[0]))
        {
            Debug.Log("Selected folder: " + folderPath);
            string newFolderPath = Path.GetFullPath(folderPath[0]).TrimEnd(Path.DirectorySeparatorChar);
            if (GameState.Instance.selectedFolderPaths != null)
            {
                //removes all existing paths within the new path
                GameState.Instance.selectedFolderPaths.RemoveAll(path =>
                {
                    string existingPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);

                    return existingPath.StartsWith(newFolderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
                });

                // Checks new path against existing paths
                foreach (string existingPath in GameState.Instance.selectedFolderPaths)
                {
                    string existingFolderPath = Path.GetFullPath(existingPath).TrimEnd(Path.DirectorySeparatorChar);

                    // if it is the same folder
                    if (string.Equals(newFolderPath, existingFolderPath, StringComparison.OrdinalIgnoreCase))
                    {
                        OpenErrorPopup("Folder already selected: " + newFolderPath);
                        Debug.Log("Folder already selected: " + newFolderPath);
                        return;
                    }

                    // if the new folder is inside an existing folder
                    if (newFolderPath.StartsWith(existingFolderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    {
                        OpenErrorPopup("Folder is already covered by an existing folder: " + existingFolderPath);
                        Debug.Log("Folder is already covered by an existing folder: " + existingFolderPath);
                        return;
                    }
                }
                
            }
            GameState.Instance.selectedFolderPaths.Add(newFolderPath);
            Debug.Log("Saved folder paths: " + GameState.Instance.selectedFolderPaths[0] + ", ...");
            Debug.Log(GameState.Instance.selectedFolderPaths.Count + " valid folder paths saved in gamestate.");
        }
        else
        {
            Debug.Log("Error no folder path selected");
        }
    }

    private void OnAddFolderButtonPress(ClickEvent evt)
    {
        SelectFolder();
    }

    private void OnFolderSelectionContinueButtonPress(ClickEvent evt)
    {
        _difficultySelectionPopup.style.display = DisplayStyle.Flex;
    }

    private void OnExitSettingsMenuButtonPress(ClickEvent evt)
    {
        _settingsMenu.style.display = DisplayStyle.None;
    }

    private void OnVolumeSliderChanged(ChangeEvent<float> evt)
    {
        GameState.Instance.UpdateVolume(evt.newValue);
    }

    private void OnFullscreenToggleChanged(ChangeEvent<bool> evt)
    {
        GameState.Instance.UpdateFullscreenToggle(evt.newValue);
    }

    private void OnResolutionDropdownChanged(ChangeEvent<string> evt)
    {
        GameState.Instance.UpdateResolutionDropdown(evt.newValue);
    }

    public static void UpdateSettings()
    {
        _volumeSlider.value = GameState.Instance.volume;
        _fullscreenToggle.value = GameState.Instance.fullscreen;
        _resolutionDropdown.index = _resolutionDropdown.choices.IndexOf(GameState.Instance.resolution);
    }

    private void PopulateResolutionDropdown()
    {
        _resolutionDropdown.choices.Clear();

        foreach (Resolution resolution in Screen.resolutions)
        {
            string option =
                resolution.width + "x" + resolution.height;

            if (!_resolutionDropdown.choices.Contains(option))
            {
                _resolutionDropdown.choices.Add(option);
            }
        }
    }

    public static void OpenErrorPopup(string text)
    {
        _errorPopupInfo.text = text;
        _errorPopup.style.display = DisplayStyle.Flex;
    }

    private void OnErrorPopupExitButtonPress(ClickEvent evt)
    {
        _errorPopup.style.display = DisplayStyle.None;
    }
}