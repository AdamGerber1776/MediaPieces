using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class PuzzleUIHandler : MonoBehaviour
{
    //initialize variable for full UI document
    private UIDocument _document;

    //initialize buttons
    private Button _skipButton;
    private Button _hintButton;
    private Button _menuButton;
    private Button _nextPuzzleButton;

    //initialize visual elements
    private static VisualElement _puzzleCompletePopup;

    //handling variables for moving PuzzleCompletePopup
    public static bool isDragging = false;
    private Vector3 pointerStartPosition;
    private Vector2 popupStartPosition;

    private void Awake()
    {
        //Gets primary ui document
        _document = GetComponent<UIDocument>();

        //gets ui buttons
        _skipButton = _document.rootVisualElement.Q("SkipButton") as Button;
        _hintButton = _document.rootVisualElement.Q("HintButton") as Button;
        _menuButton = _document.rootVisualElement.Q("MenuButton") as Button;
        _nextPuzzleButton = _document.rootVisualElement.Q("NextPuzzleButton") as Button;

        //gets ui visual elemetns
        _puzzleCompletePopup = _document.rootVisualElement.Q("PuzzleCompletePopup") as VisualElement;

        //Registers events for clicking each button
        _skipButton.RegisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.RegisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.RegisterCallback<ClickEvent>(OnMenuButtonPress);
        _nextPuzzleButton.RegisterCallback<ClickEvent>(OnNextPuzzleButtonPress);

        //registers events for moving around the popup screen
        _puzzleCompletePopup.RegisterCallback<PointerDownEvent>(OnPointerDown);
        _puzzleCompletePopup.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _puzzleCompletePopup.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void OnDisable()
    {
        //dissables button events when buttons are dissabled
        _skipButton.UnregisterCallback<ClickEvent>(OnSkipButtonPress);
        _hintButton.UnregisterCallback<ClickEvent>(OnHintButtonPress);
        _menuButton.UnregisterCallback<ClickEvent>(OnMenuButtonPress);
        _nextPuzzleButton.UnregisterCallback<ClickEvent>(OnNextPuzzleButtonPress);
        _puzzleCompletePopup.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        _puzzleCompletePopup.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        _puzzleCompletePopup.UnregisterCallback<PointerUpEvent>(OnPointerUp);
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
        PuzzleManager.HighlightPiece(PuzzleManager.GetHelpPiece());
    }

    private void OnMenuButtonPress(ClickEvent evt)
    {
        Debug.Log("Menu Button Pressed");
    }

    public static void OnPuzzleCompletion()
    {
        _puzzleCompletePopup.style.display = DisplayStyle.Flex;
    }

    private void OnNextPuzzleButtonPress(ClickEvent evt)
    {
        Debug.Log("Next puzzle button pressed");
        SceneManager.LoadScene("MainMenuScene");
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
}
