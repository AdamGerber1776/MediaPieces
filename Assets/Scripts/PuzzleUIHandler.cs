using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PuzzleUIHandler : MonoBehaviour
{
    //initialize variable for full UI document
    private UIDocument _document;

    //initialize buttons
    private Button _skipButton;
    private Button _hintButton;
    private Button _menuButton;

    private void Awake()
    {
        //Gets primary ui document
        _document = GetComponent<UIDocument>();

        //gets ui buttons
        _skipButton = _document.rootVisualElement.Q("SkipButton") as Button;
        _hintButton = _document.rootVisualElement.Q("HintButton") as Button;
        _menuButton = _document.rootVisualElement.Q("MenuButton") as Button;

        //Registers events for clicking each button
        _skipButton.RegisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.RegisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.RegisterCallback<ClickEvent>(OnMenuButtonPress);
    }

    private void OnDisable()
    {
        //dissables button events when buttons are dissabled
        _skipButton.UnregisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.UnregisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.UnregisterCallback<ClickEvent>(OnMenuButtonPress);
    }

    //skips the current puzzle and loads the next one
    //if no more puzzles are available, returns to the main menu
    private void OnSkipButtonPress(ClickEvent evt)
    {
        Debug.Log("Skip Button Pressed");
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnHintButtonPress(ClickEvent evt)
    {
        Debug.Log("Hint Button Pressed");
    }

    private void OnMenuButtonPress(ClickEvent evt)
    {
        Debug.Log("Menu Button Pressed");
    }
}
