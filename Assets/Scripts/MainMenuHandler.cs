using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuHandler : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private Button _optionsButton;
    private Button _exitButton;

    private void Awake()
    {
        //Gets primary ui document and buttons
        _document = GetComponent<UIDocument>();
        _startButton = _document.rootVisualElement.Q("StartButton") as Button;
        _optionsButton = _document.rootVisualElement.Q("OptionsButton") as Button;
        _exitButton = _document.rootVisualElement.Q("ExitButton") as Button;

        //Registers events for clicking each button
        _startButton.RegisterCallback<ClickEvent>(OnStartButtonPress);
        _optionsButton.RegisterCallback<ClickEvent>(OnOptionsButtonPress);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonPress);
    }

    private void OnDisable()
    {
        //dissables button events when buttons are dissabled
        _startButton.UnregisterCallback<ClickEvent>(OnStartButtonPress);
        _optionsButton.UnregisterCallback<ClickEvent>(OnStartButtonPress);
        _exitButton.UnregisterCallback<ClickEvent>(OnStartButtonPress);
    }

    private void OnStartButtonPress(ClickEvent evt)
    {
        Debug.Log("Start Button Pressed");
    }
    private void OnOptionsButtonPress(ClickEvent evt)
    {
        Debug.Log("Options Button Pressed");
    }
    private void OnExitButtonPress(ClickEvent evt)
    {
        Debug.Log("Exit Button Pressed");
        Application.Quit();
    }
}
