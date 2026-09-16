using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static void CreatePuzzleFromImage(Texture2D image)
    {
        Debug.Log("Creating puzzle from image of size: " + image.width + "x" + image.height);

        int difficulty = 3; //temporarily set difficulty value. to be determined by selection later
        int pieceWidth = image.width / 10 / difficulty;
        int pieceHeight = image.height / 10 / difficulty;
        float pieceWidthPercent = 1.0f / difficulty;
        float pieceHeightPercent = 1.0f / difficulty;
        int[] triangles =
        {
            0, 2, 1,
            2, 3, 1
        };

        for (int x = 0; x < difficulty; x++)
        {
            for (int y = 0; y < difficulty; y++)
            {
                GameObject piece = new GameObject("PuzzlePiece");
                Mesh mesh = new Mesh();
                Vector3[] vertices =
                {
                    new Vector3(-pieceWidth, -pieceHeight, 0),
                    new Vector3(pieceWidth, -pieceHeight, 0),
                    new Vector3(-pieceWidth,  pieceHeight, 0),
                    new Vector3(pieceWidth,  pieceHeight, 0)
                };
                mesh.vertices = vertices;
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
                CameraHandler.Instance.FrameObject(piece);
            }
        }

        /*testing code for here
        GameObject piece = new GameObject("PuzzlePiece");
        Mesh mesh = new Mesh();
        Vector3[] vertices =
        {
            new Vector3(-image.width / 2, -image.height / 2, 0),
            new Vector3(image.width / 2, -image.height / 2, 0),
            new Vector3(-image.width / 2,  image.height / 2, 0),
            new Vector3(image.width / 2,  image.height / 2, 0)
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
            new Vector2(0.0f, 0.0f),
            new Vector2(0.5f, 0.0f),
            new Vector2(0.0f, 0.5f),
            new Vector2(0.5f, 0.5f)
        };
        mesh.uv = uv;
        MeshFilter filter = piece.AddComponent<MeshFilter>();
        MeshRenderer renderer = piece.AddComponent<MeshRenderer>();
        filter.mesh = mesh;
        Material material = new Material(Shader.Find("Sprites/Default"));
        renderer.material = material;
        material.mainTexture = image;
        CameraHandler.Instance.FrameObject(piece);*/
    }
}
