using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    //initialization of the materials and components related to puzzle creation and interaction
    [SerializeField] private Material cellMaterial;
    [SerializeField] private Material gridMaterial;
    [SerializeField] public Material puzzleMaterial;
    [SerializeField] private Transform puzzle;
    [SerializeField] private Material highlightMaterial;

    public static PuzzleManager Instance;

    //variables holding information about the puzzle's dimensions, difficulty, and grid structure
    private static float pieceWidth;
    private static float pieceHeight;
    private static float boardWidth;
    private static float boardHeight;
    private static int difficulty;
    private static int columns;
    private static int rows;

    //keeps track of which grid positions are currently occupied by puzzle pieces
    public static int[,] occupiedGridPositions;

    //for handling highlighting and unhighlighting puzzle pieces and board pieces
    private static GameObject highlightPieceObject;
    private static GameObject highlightBoardPieceObject;

    //ensures the puzzle manager is not duplicated and configures difficulty and highlight material
    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        difficulty = GameState.Instance.selectedDifficulty;
        ConfigureHighlightMaterial(highlightMaterial);
    }

    //creates the puzzle based on the specified width and height, determining the number of columns and rows based on difficulty and aspect ratio
    public static void CreatePuzzle(int width, int height)
    {
        Debug.Log("Creating puzzle from image of size: " + width + "x" + height);
        Debug.Log("Creating puzzle of difficulty " + difficulty);

        //determines the number of columns and rows based on the difficulty and aspect ratio of the image
        float aspectRatio = (float)width / height;
        int maxPiecesInOneDimension = difficulty * 2;
        if (aspectRatio >= 1f)
        {
            columns = Mathf.RoundToInt(difficulty * aspectRatio);
            columns = Mathf.Clamp(columns, 1, maxPiecesInOneDimension);
            rows = difficulty;
        }
        else
        {
            columns = difficulty;
            rows = Mathf.RoundToInt(difficulty / aspectRatio);
            rows = Mathf.Clamp(rows, 1, maxPiecesInOneDimension);
        }

        occupiedGridPositions = new int[columns, rows];
        pieceWidth = width / 10f / columns;
        pieceHeight = height / 10f / rows;
        float pieceWidthPercent = 1.0f / columns;
        float pieceHeightPercent = 1.0f / rows;
        Instance.BuildBoard();

        //variables to determine piece random placement location around the board
        float boardLeft   = -boardWidth / 2f;
        float boardRight  =  boardWidth / 2f;
        float piecesPadding = 1f; //padding to prevent pieces from being placed too close to the board edges
        float piecesSpace = pieceWidth * 2f; //size of the space in which the pieces can be placed on either side of the board
        float pieceZ = 0;

        //creates the puzzle pieces and places them around the board
        //this utilizes uv mapping so that each piece uses a piece of one texture rather than all having their own separate textures
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject piece = new GameObject("PuzzlePiece " + "x:" + x + " y:" + y);
                piece.transform.SetParent(Instance.puzzle);
                PuzzlePiece puzzlePiece = piece.AddComponent<PuzzlePiece>();
                puzzlePiece.correctGridPosition = new Vector2Int(x, y);
                puzzlePiece.currentGridPosition = new Vector2Int(-1, -1);
                Mesh mesh = new Mesh();
                Vector3[] vertices =
                {
                    new Vector3(-pieceWidth / 2f, -pieceHeight / 2f, 0),
                    new Vector3( pieceWidth / 2f, -pieceHeight / 2f, 0),
                    new Vector3(-pieceWidth / 2f,  pieceHeight / 2f, 0),
                    new Vector3( pieceWidth / 2f,  pieceHeight / 2f, 0)
                };
                mesh.vertices = vertices;

                int[] triangles =
                {
                    0, 2, 1,
                    2, 3, 1
                };

                mesh.triangles = triangles;
                Vector2[] uv =
                {
                    new Vector2(pieceWidthPercent*x, pieceHeightPercent*y),
                    new Vector2(pieceWidthPercent*(x+1), pieceHeightPercent*y),
                    new Vector2(pieceWidthPercent*x, pieceHeightPercent*(y+1)),
                    new Vector2(pieceWidthPercent*(x+1), pieceHeightPercent*(y+1))
                };
                mesh.uv = uv;
                MeshFilter filter = piece.AddComponent<MeshFilter>();
                MeshRenderer renderer = piece.AddComponent<MeshRenderer>();
                filter.mesh = mesh;
                renderer.material = Instance.puzzleMaterial;

                //randomly places the puzzle pieces on the board
                int randomSide = UnityEngine.Random.Range(0, 2);
                switch (randomSide)
                {
                    case 0: //left side
                        piece.transform.position = new Vector3(
                            UnityEngine.Random.Range(
                                boardLeft - piecesSpace - piecesPadding - pieceWidth / 2f,
                                 boardLeft - piecesPadding - pieceWidth / 2f),
                            UnityEngine.Random.Range(
                                -boardHeight / 2f,
                                 boardHeight / 2f),
                            pieceZ
                        );
                        break;
                    case 1: //right side
                        piece.transform.position = new Vector3(
                            UnityEngine.Random.Range(
                                boardRight + piecesPadding + pieceWidth / 2f,
                                boardRight + piecesSpace + piecesPadding + pieceWidth / 2f),
                            UnityEngine.Random.Range(
                                -boardHeight / 2f,
                                boardHeight / 2f),
                            pieceZ
                        );
                        break;
                }
                pieceZ -= 0.0001f;
                piece.AddComponent<BoxCollider2D>();
            }
        }
    }

    //builds the puzzle board grid and the individual board pieces
    //includes calculating the grid gap based on the piece dimensions
    //also zooms the camera to fit the board size to ensure the entire puzzle is visible
    private void BuildBoard()
    {
        boardWidth = pieceWidth * columns;
        boardHeight = pieceHeight * rows;
        float gridGap;

        //gap between pieces of the board to create the grid
        if (pieceWidth > pieceHeight)
        {
            gridGap = pieceWidth * 0.01f; 
        }
        else
        {
            gridGap = pieceHeight * 0.01f; 
        }
        
        int[] triangles =
        {
            0, 2, 1,
            2, 3, 1
        };

        //Creates the board grid
        GameObject boardGrid = new GameObject("PuzzleBoardGrid");
        boardGrid.transform.SetParent(puzzle);
        Mesh gridMesh = new Mesh();
        Vector3[] gridVertices =
        {
            new Vector3(-boardWidth / 2f - gridGap / 2f, -boardHeight / 2f - gridGap / 2f, 0),
            new Vector3( boardWidth / 2f + gridGap / 2f, -boardHeight / 2f - gridGap / 2f, 0),
            new Vector3(-boardWidth / 2f - gridGap / 2f,  boardHeight / 2f + gridGap / 2f, 0),
            new Vector3( boardWidth / 2f + gridGap / 2f,  boardHeight / 2f + gridGap / 2f, 0)
        };

        gridMesh.vertices = gridVertices;
        gridMesh.triangles = triangles;

        MeshFilter gridFilter = boardGrid.AddComponent<MeshFilter>();
        MeshRenderer gridRenderer = boardGrid.AddComponent<MeshRenderer>();

        gridFilter.mesh = gridMesh;
        gridRenderer.material = PuzzleManager.Instance.gridMaterial;

        boardGrid.transform.position = new Vector3(
            0,
            0,
            2
        );

        //creates the puzzle board
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject boardPiece = new GameObject("PuzzleBoardPiece " + "x:" + x + " y:" + y);
                boardPiece.transform.SetParent(puzzle);
                PuzzleBoardPiece boardPieceData = boardPiece.AddComponent<PuzzleBoardPiece>();
                boardPieceData.gridPosition = new Vector2Int(x, y);

                Mesh mesh = new Mesh();
                Vector3[] vertices =
                {
                    new Vector3(-pieceWidth / 2f + gridGap / 2f, -pieceHeight / 2f + gridGap / 2f, 0),
                    new Vector3( pieceWidth / 2f - gridGap / 2f, -pieceHeight / 2f + gridGap / 2f, 0),
                    new Vector3(-pieceWidth / 2f + gridGap / 2f,  pieceHeight / 2f - gridGap / 2f, 0),
                    new Vector3( pieceWidth / 2f - gridGap / 2f,  pieceHeight / 2f - gridGap / 2f, 0)
                };

                mesh.vertices = vertices;
                mesh.triangles = triangles;

                MeshFilter filter = boardPiece.AddComponent<MeshFilter>();
                MeshRenderer renderer = boardPiece.AddComponent<MeshRenderer>();

                filter.mesh = mesh;
                renderer.material = PuzzleManager.Instance.cellMaterial;

                Vector3 boardPosition = GetBoardPosition(x, y);
                boardPiece.transform.position = new Vector3(
                    boardPosition.x,
                    boardPosition.y,
                    1
                );
            }
        }

        //zooms the camera to fit the puzzle board
        CameraHandler.Instance.FrameBoard(boardWidth, boardHeight);
    }

    //function to get the world position of a piece based on its grid position
    public static Vector3 GetBoardPosition(int x, int y)
    {
        float posX = -boardWidth / 2f + pieceWidth / 2f + x * pieceWidth;
        float posY = -boardHeight / 2f + pieceHeight / 2f + y * pieceHeight;

        return new Vector3(posX, posY, 0f);
    }

    //function to get the grid position of a piece based on its world position
    //this is a rounded variable so it will round to the nearest grid position, which is useful for snapping puzzle pieces into place
    public static Vector2Int GetPieceGridPosition(Vector3 piecePosition)
    {
        float relativeX = piecePosition.x - (-boardWidth / 2f + pieceWidth / 2f);
        float relativeY = piecePosition.y - (-boardHeight / 2f + pieceHeight / 2f);

        float gridX = relativeX / pieceWidth;
        float gridY = relativeY / pieceHeight;

        int x = Mathf.RoundToInt(gridX);
        int y = Mathf.RoundToInt(gridY);
        
        if (x < 0 || x >= columns)
        {
            x = -1;
        }
        if (y < 0 || y >= rows)
        {
            y = -1;
        }

        return new Vector2Int(x, y);
    }

    //checks if all pieces are in their correct positions
    public static void CheckPuzzleCompletion()
    {
        PuzzlePiece[] pieces = GameObject.FindObjectsByType<PuzzlePiece>();
        bool allCorrect = true;
        foreach (PuzzlePiece piece in pieces)
        {
            if (!piece.isPlacedCorrectly) allCorrect = false;
            else if (piece.highlighted) 
            {
                Debug.Log("Clear Highlight");
                ClearHighlight();
            }
        }
        if (!allCorrect) return;
        Debug.Log("Puzzle Completed!");
        ClearHighlight();
        PuzzleUIHandler.OnPuzzleCompletion();
        AudioManager.Instance.PlayCompletePuzzle();
    }

    //function to get a random piece that is not yet placed correctly, used for providing hints to the player
    public static PuzzlePiece GetHelpPiece()
    {
        PuzzlePiece[] pieces = FindObjectsByType<PuzzlePiece>(FindObjectsSortMode.None);
        List<PuzzlePiece> availablePieces = new List<PuzzlePiece>();

        foreach (PuzzlePiece piece in pieces)
        {
            if (!piece.isPlacedCorrectly)
            {
                availablePieces.Add(piece);
            }
        }

        if (availablePieces.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, availablePieces.Count);

        return availablePieces[index];
    }

    //function to configure the highlight material used for highlighting puzzle pieces and board pieces
    private static void ConfigureHighlightMaterial(Material material)
    {
        material.SetFloat("_Surface", 1f); // Transparent
        material.SetFloat("_AlphaClip", 0f); // Disable alpha clipping

        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        material.SetFloat("_SrcBlendAlpha", (float)UnityEngine.Rendering.BlendMode.One);
        material.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        material.SetFloat("_ZWrite", 0f);

        material.SetColor("_BaseColor", new Color(1f, 1f, 0f, 0.35f));
    }

    //function to highlight a piece and its correct board location
    public static void HighlightPiece(PuzzlePiece piece)
    {
        // Remove previous highlight
        ClearHighlight();

        if (piece == null) return;

        // Create highlight object for puzzle piece
        highlightPieceObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        highlightPieceObject.name = "PuzzlePieceHighlight";
        highlightPieceObject.transform.localScale = new Vector3(pieceWidth, pieceHeight, 1f);

        highlightPieceObject.transform.SetParent(piece.transform);
        highlightPieceObject.transform.localPosition = new Vector3(0f, 0f, -0.1f);

        Renderer pieceRenderer = highlightPieceObject.GetComponent<Renderer>();
        pieceRenderer.material = Instance.highlightMaterial;
        Destroy(highlightPieceObject.GetComponent<Collider>());
        piece.highlighted = true;

        //find board piece and create highlight object for it
        PuzzleBoardPiece[] boardPieces = FindObjectsByType<PuzzleBoardPiece>(FindObjectsSortMode.None);

        foreach (PuzzleBoardPiece boardPiece in boardPieces)
        {
            if (boardPiece.gridPosition == piece.correctGridPosition)
            {
                highlightBoardPieceObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                highlightBoardPieceObject.name = "BoardPieceHighlight";
                highlightBoardPieceObject.transform.localScale = new Vector3(pieceWidth, pieceHeight, 1f);

                highlightBoardPieceObject.transform.position = 
                new Vector3(
                    boardPiece.transform.position.x, 
                    boardPiece.transform.position.y, 
                    boardPiece.transform.position.z - 1.1f
                    );

                Renderer boardPieceRenderer = highlightBoardPieceObject.GetComponent<Renderer>();
                boardPieceRenderer.material = Instance.highlightMaterial;
                Destroy(highlightBoardPieceObject.GetComponent<Collider>());
                boardPiece.highlighted = true;
            }
        }
    }

    //function to clear highlight on a puzzle piece and board piece
    public static void ClearHighlight()
    {
        PuzzlePiece[] pieces = GameObject.FindObjectsByType<PuzzlePiece>();
        foreach (PuzzlePiece piece in pieces)
        {
            if (piece.highlighted) piece.highlighted = false;
        }
        if (highlightPieceObject != null)
        {
            Destroy(highlightPieceObject);

            highlightPieceObject = null;
        }
        if (highlightBoardPieceObject != null)
        {
            Destroy(highlightBoardPieceObject);

            highlightBoardPieceObject = null;
        }
    }

    //function to clear the entire puzzle, including all pieces and media objects
    public static void ClearPuzzle()
    {
        foreach (Transform child in Instance.puzzle)
        {
            Destroy(child.gameObject);
        }

        if (MediaManager.Instance.videoPlayerObject != null)
        {
            Destroy(MediaManager.Instance.videoPlayerObject);
            MediaManager.Instance.videoPlayer = null;
            MediaManager.Instance.videoPlaying = false;
        }

        MediaManager.Instance.gifPlaying = false;
    }
}
