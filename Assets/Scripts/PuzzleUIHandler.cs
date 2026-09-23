using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PuzzleUIHandler : MonoBehaviour
{
    //initialize variable for full UI document
    private UIDocument _document;

    //initialize buttons
    private Button _skipButton;
    private Button _hintButton;
    private Button _menuButton;
    private Button _nextPuzzleButton;
    private Button _exitSettingsButton;
    private Button _returnToMenuButton;

    //initialize visual elements
    private static VisualElement _puzzleCompletePopup;
    private static VisualElement _loadingScreen;
    private static VisualElement _settingsMenu;

    //handling variables for moving PuzzleCompletePopup
    public static bool isDragging = false;
    private Vector3 pointerStartPosition;
    private Vector2 popupStartPosition;

    //Initialize settings menu Items
    private Slider _volumeSlider;
    private Toggle _fullscreenToggle;
    private DropdownField _resolutionDropdown;
    private static Label _filePathText;

    private void Awake()
    {
        //Gets primary ui document
        _document = GetComponent<UIDocument>();

        //gets ui buttons
        _skipButton = _document.rootVisualElement.Q("SkipButton") as Button;
        _hintButton = _document.rootVisualElement.Q("HintButton") as Button;
        _menuButton = _document.rootVisualElement.Q("MenuButton") as Button;
        _nextPuzzleButton = _document.rootVisualElement.Q("NextPuzzleButton") as Button;
        _exitSettingsButton = _document.rootVisualElement.Q("ExitSettingsMenuButton") as Button;
        _returnToMenuButton = _document.rootVisualElement.Q("ReturnToMenuButton") as Button;

        //gets ui visual elemetns
        _puzzleCompletePopup = _document.rootVisualElement.Q("PuzzleCompletePopup") as VisualElement;
        _loadingScreen = _document.rootVisualElement.Q("LoadingScreen") as VisualElement;
        _settingsMenu = _document.rootVisualElement.Q("SettingsMenu") as VisualElement;

        //get settings menu items
        _filePathText = _document.rootVisualElement.Q("FilePathText") as Label;
        _volumeSlider = _document.rootVisualElement.Q("VolumeSlider") as Slider;
        _fullscreenToggle = _document.rootVisualElement.Q("FullscreenToggle") as Toggle;
        _resolutionDropdown = _document.rootVisualElement.Q("ResolutionDropdown") as DropdownField;

        //Registers events for clicking each button
        _skipButton.RegisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.RegisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.RegisterCallback<ClickEvent>(OnMenuButtonPress);
        _nextPuzzleButton.RegisterCallback<ClickEvent>(OnNextPuzzleButtonPress);
        _exitSettingsButton.RegisterCallback<ClickEvent>(OnExitSettingsMenuButtonPress);
        _returnToMenuButton.RegisterCallback<ClickEvent>(OnReturnToMenuButtonPress);

        //registers events for moving around the popup screen
        _puzzleCompletePopup.RegisterCallback<PointerDownEvent>(OnPointerDown);
        _puzzleCompletePopup.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _puzzleCompletePopup.RegisterCallback<PointerUpEvent>(OnPointerUp);

        //Register events for settings menu items
        _volumeSlider.RegisterValueChangedCallback(OnVolumeSliderChanged);
        _fullscreenToggle.RegisterValueChangedCallback(OnFullscreenToggleChanged);
        _resolutionDropdown.RegisterValueChangedCallback(OnResolutionDropdownChanged);

        //gives the resolution dropdown the viable resolutions for the users device
        PopulateResolutionDropdown();
        UpdateSettings();
    }

    private void OnDisable()
    {
        //dissables button events when buttons are dissabled
        _skipButton.UnregisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.UnregisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.UnregisterCallback<ClickEvent>(OnMenuButtonPress);
        _nextPuzzleButton.UnregisterCallback<ClickEvent>(OnNextPuzzleButtonPress);
        _exitSettingsButton.UnregisterCallback<ClickEvent>(OnExitSettingsMenuButtonPress);
        _returnToMenuButton.UnregisterCallback<ClickEvent>(OnReturnToMenuButtonPress);

        //dissables popup events when popup is dissabled
        _puzzleCompletePopup.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        _puzzleCompletePopup.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        _puzzleCompletePopup.UnregisterCallback<PointerUpEvent>(OnPointerUp);

        //dissables settings menu items events when they are dissabled
        _volumeSlider.UnregisterValueChangedCallback(OnVolumeSliderChanged);
        _fullscreenToggle.UnregisterValueChangedCallback(OnFullscreenToggleChanged);
        _resolutionDropdown.UnregisterValueChangedCallback(OnResolutionDropdownChanged);
    }

    //skips the current puzzle and loads the next one
    //if no more puzzles are available, returns to the main menu
    private void OnSkipButtonPress(ClickEvent evt)
    {
        Debug.Log("Skip Button Pressed");
        if (GameState.Instance.selectedFilePaths.Count > 0)
        {
            OpenLoadingScreen();
            PuzzleManager.ClearHighlight();
            PuzzleManager.ClearPuzzle();
            MediaManager.Instance.ChooseFilePath();
            MediaManager.Instance.LoadPuzzleFile();
        }
        else
        {
            Debug.Log("No more file paths to load. Returning to menu.");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    private void OnHintButtonPress(ClickEvent evt)
    {
        Debug.Log("Hint Button Pressed");
        PuzzleManager.HighlightPiece(PuzzleManager.GetHelpPiece());
    }

    private void OnMenuButtonPress(ClickEvent evt)
    {
        Debug.Log("Menu Button Pressed");
        _settingsMenu.style.display = DisplayStyle.Flex;
    }

    public static void OnPuzzleCompletion()
    {
        _puzzleCompletePopup.style.display = DisplayStyle.Flex;
    }

    private void OnNextPuzzleButtonPress(ClickEvent evt)
    {
        Debug.Log("Next puzzle button pressed");
        if (GameState.Instance.selectedFilePaths.Count > 0)
        {
            OpenLoadingScreen();
            PuzzleManager.ClearHighlight();
            PuzzleManager.ClearPuzzle();
            MediaManager.Instance.ChooseFilePath();
            MediaManager.Instance.LoadPuzzleFile();
            _puzzleCompletePopup.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.Log("No more file paths to load. Returning to menu.");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    //when popup clicked
    private void OnPointerDown(PointerDownEvent evt)
    {
        isDragging = true;

        pointerStartPosition = evt.position;
        popupStartPosition = new Vector2(
            _puzzleCompletePopup.resolvedStyle.left,
            _puzzleCompletePopup.resolvedStyle.top
        );

        _puzzleCompletePopup.CapturePointer(evt.pointerId);
    }

    //when mouse is moved
    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging)
            return;
        
        Vector2 delta = evt.position - pointerStartPosition;
        _puzzleCompletePopup.style.left = popupStartPosition.x + delta.x;
        _puzzleCompletePopup.style.top = popupStartPosition.y + delta.y;
    }

    //when popup is no longer clicked
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging)
            return;

        isDragging = false;

        _puzzleCompletePopup.ReleasePointer(evt.pointerId);
    }

    public static void CloseLoadingScreen()
    {
        _loadingScreen.style.display = DisplayStyle.None;
    }

    public static void OpenLoadingScreen()
    {
        _loadingScreen.style.display = DisplayStyle.Flex;
    }

    private void OnExitSettingsMenuButtonPress(ClickEvent evt)
    {
        _settingsMenu.style.display = DisplayStyle.None;
    }

    private void OnReturnToMenuButtonPress(ClickEvent evt)
    {
        GameState.Instance.selectedFilePath = "";
        GameState.Instance.selectedFolderPaths = new List<string>();
        GameState.Instance.selectedFilePaths = new List<string>();
        SceneManager.LoadScene("MainMenuScene");
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

    private void UpdateSettings()
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

    public static void PopulateFilePath(string path)
    {
        _filePathText.text = path;
    }
}
