using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    //class held by each puzzle piece
    //stores the correct and current grid positions of the puzzle piece, as well as its highlight status
    //includes a property to check if the piece is currently placed correctly
    public Vector2Int correctGridPosition;
    public Vector2Int currentGridPosition;
    public bool isPlacedCorrectly => currentGridPosition == correctGridPosition;
    public bool highlighted = false;
}
