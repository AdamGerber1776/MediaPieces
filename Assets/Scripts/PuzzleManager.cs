using Unity.Mathematics;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private Material cellMaterial;
    [SerializeField] private Material gridMaterial;

    public static PuzzleManager Instance;
    private static float pieceWidth;
    private static float pieceHeight;
    private static float boardWidth;
    private static float boardHeight;
    private static int difficulty = 3; //temporarily set difficulty value. to be determined by selection later

    public static int[,] occupiedGridPositions;

    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void CreatePuzzleFromImage(Texture2D image)
    {
        Debug.Log("Creating puzzle from image of size: " + image.width + "x" + image.height);

        occupiedGridPositions = new int[difficulty, difficulty];
        pieceWidth = image.width / 10f / difficulty;
        pieceHeight = image.height / 10f / difficulty;
        float pieceWidthPercent = 1.0f / difficulty;
        float pieceHeightPercent = 1.0f / difficulty;
        Instance.BuildBoard();

        //variables to determine piece random placement location
        float boardLeft   = -boardWidth / 2f;
        float boardRight  =  boardWidth / 2f;
        float piecesPadding = 1f; //padding to prevent pieces from being placed too close to the board edges
        float piecesSpace = pieceWidth * 2f; //size of the space in which the pieces can be placed on either side of the board

        //creates the puzzle pieces and places them around the board
        for (int x = 0; x < difficulty; x++)
        {
            for (int y = 0; y < difficulty; y++)
            {
                GameObject piece = new GameObject("PuzzlePiece " + "x:" + x + " y:" + y);
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
                Material material = new Material(Shader.Find("Sprites/Default"));
                renderer.material = material;
                material.mainTexture = image;

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
                            0
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
                            0
                        );
                        break;
                }
                Collider2D collider = piece.AddComponent<BoxCollider2D>();
            }
        }
    }

    private void BuildBoard()
    {
        boardWidth = pieceWidth * difficulty;
        boardHeight = pieceHeight * difficulty;
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
        for (int x = 0; x < difficulty; x++)
        {
            for (int y = 0; y < difficulty; y++)
            {
                GameObject boardPiece = new GameObject("PuzzleBoardPiece " + "x:" + x + " y:" + y);
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

    //to get the world position of a piece based on its grid position
    public static Vector3 GetBoardPosition(int x, int y)
    {
        float posX = -boardWidth / 2f + pieceWidth / 2f + x * pieceWidth;
        float posY = -boardHeight / 2f + pieceHeight / 2f + y * pieceHeight;

        return new Vector3(posX, posY, 0f);
    }

    //to get the grid position of a piece based on its world position
    //this is a rounded variable so it will round to the nearest grid position, which is useful for snapping pieces into place
    public static Vector2Int GetPieceGridPosition(Vector3 piecePosition)
    {
        float relativeX = piecePosition.x - (-boardWidth / 2f + pieceWidth / 2f);
        float relativeY = piecePosition.y - (-boardHeight / 2f + pieceHeight / 2f);

        float gridX = relativeX / pieceWidth;
        float gridY = relativeY / pieceHeight;

        int x = Mathf.RoundToInt(gridX);
        int y = Mathf.RoundToInt(gridY);
        
        if (x < 0 || x >= difficulty)
        {
            x = -1;
        }
        if (y < 0 || y >= difficulty)
        {
            y = -1;
        }

        return new Vector2Int(x, y);
    }

    //checks if all pieces are in their correct positions
    public static void CheckPuzzleCompletion()
    {
        PuzzlePiece[] pieces = GameObject.FindObjectsByType<PuzzlePiece>();
        foreach (PuzzlePiece piece in pieces)
        {
            if (!piece.isPlacedCorrectly)
            {
                return;
            }
        }
        Debug.Log("Puzzle Completed!");
        PuzzleUIHandler.OnPuzzleCompletion();
    }
}
