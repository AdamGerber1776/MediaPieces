using UnityEngine;

public class PuzzleBoardPiece : MonoBehaviour
{
    //class held by all pieces of the puzzle board
    //contains information about the piece's position on the grid and whether it is currently highlighted
    public Vector2Int gridPosition;
    public bool highlighted = false;
}
