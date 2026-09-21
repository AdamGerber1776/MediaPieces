using UnityEngine;
using UnityEngine.Video;

//AI generated for testing to see if this method is efficient enough for use in the applicaiton for large videos
public class VideoPuzzleTest : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private string videoPath;

    [Header("Puzzle")]
    [SerializeField] private int rows = 7;
    [SerializeField] private int columns = 7;
    [SerializeField] private float pieceWidth = 1.5f;
    [SerializeField] private float pieceHeight = 1.5f;
    [SerializeField] private float pieceSpacing = 0.02f;

    private VideoPlayer videoPlayer;

    private void Start()
    {
        CreateVideoPlayer();
        CreatePuzzle();
    }

    private void CreateVideoPlayer()
    {
        GameObject videoObject = new GameObject("VideoPlayer");

        videoPlayer = videoObject.AddComponent<VideoPlayer>();

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;

        videoPlayer.renderMode = VideoRenderMode.APIOnly;

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        Debug.Log("Video prepared!");
        Debug.Log("Video dimensions: " +
                  player.width + " x " + player.height);

        player.Play();
    }

    private void CreatePuzzle()
    {
        float totalWidth = columns * pieceWidth;
        float totalHeight = rows * pieceHeight;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                CreatePiece(x, y, totalWidth, totalHeight);
            }
        }
    }

    private void CreatePiece(
        int x,
        int y,
        float totalWidth,
        float totalHeight)
    {
        GameObject piece = new GameObject(
            "VideoPiece_" + x + "_" + y
        );

        piece.transform.SetParent(transform);

        // Position the piece.
        float worldX =
            x * pieceWidth
            - totalWidth / 2f
            + pieceWidth / 2f;

        float worldY =
            y * pieceHeight
            - totalHeight / 2f
            + pieceHeight / 2f;

        piece.transform.localPosition =
            new Vector3(worldX, worldY, 0);

        // Mesh components.
        MeshFilter meshFilter =
            piece.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer =
            piece.AddComponent<MeshRenderer>();

        // Create mesh.
        Mesh mesh = new Mesh();

        float halfWidth = (pieceWidth - pieceSpacing) / 2f;
        float halfHeight = (pieceHeight - pieceSpacing) / 2f;

        mesh.vertices = new Vector3[]
        {
            new Vector3(-halfWidth, -halfHeight, 0),
            new Vector3( halfWidth, -halfHeight, 0),
            new Vector3(-halfWidth,  halfHeight, 0),
            new Vector3( halfWidth,  halfHeight, 0)
        };

        mesh.triangles = new int[]
        {
            0, 2, 1,
            2, 3, 1
        };

        // Determine which portion of the video
        // this piece should display.
        float uMin = (float)x / columns;
        float uMax = (float)(x + 1) / columns;

        float vMin = (float)y / rows;
        float vMax = (float)(y + 1) / rows;

        mesh.uv = new Vector2[]
        {
            new Vector2(uMin, vMin),
            new Vector2(uMax, vMin),
            new Vector2(uMin, vMax),
            new Vector2(uMax, vMax)
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        // Create material.
        Material material =
            new Material(Shader.Find("Sprites/Default"));

        meshRenderer.material = material;

        // We can't assign the video texture yet.
        // VideoPlayer needs to prepare first.
        videoPlayer.prepareCompleted +=
            (VideoPlayer player) =>
            {
                meshRenderer.material.mainTexture =
                    player.texture;
            };
    }
}
