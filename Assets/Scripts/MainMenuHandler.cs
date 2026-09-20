using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using UnityEngine.SceneManagement;

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

    //initialize visual elements
    private VisualElement _fileSelectionPopup;
    private VisualElement _difficultySelectionPopup;
    private VisualElement _loadingScreen;

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

        //gets ui visual elements (primarily for popup screens)
        _fileSelectionPopup = _document.rootVisualElement.Q("FileSelectionPopup") as VisualElement;
        _difficultySelectionPopup = _document.rootVisualElement.Q("DifficultySelectionPopup") as VisualElement;
        _loadingScreen = _document.rootVisualElement.Q("LoadingScreen") as VisualElement;

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
    public void SelectFile()
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
}