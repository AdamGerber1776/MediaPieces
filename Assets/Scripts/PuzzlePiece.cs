using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public Vector2Int correctGridPosition;
    public Vector2Int currentGridPosition;
    public bool isPlacedCorrectly => currentGridPosition == correctGridPosition;
}
