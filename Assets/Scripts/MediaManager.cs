using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class MediaManager : MonoBehaviour
{
    //list of valid extensions
    private readonly string[] validImageExtensions = { ".png", ".jpg", ".jpeg" };
    private readonly string[] validAnimatedImageExtensions = { ".gif" };
    private readonly string[] validVideoExtensions = { ".mp4" };

    //variables to handle gif playing
    private bool gifPlaying = false;
    private float frameDelayTime;
    private int gifFrame;
    private List<UniGif.GifTexture> gifTextures;


    void Start()
    {
        //checks the file path for validity and determines what function to use to load the media
        if (GameState.Instance.selectedFilePath != "" && GameState.Instance.selectedFilePath != null)
        {
            string filePath = GameState.Instance.selectedFilePath;
            Debug.Log("Loaded file path: " + filePath);
            //checks if the file exists at the specified path
            if (System.IO.File.Exists(filePath))
            {
                Debug.Log("File exists");
                // Check if the file has a valid image extension
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();
                if (validImageExtensions.Contains(fileExtension))
                {
                    Debug.Log("File has a valid image extension: " + fileExtension);
                    LoadImage(filePath);
                }
                else if (validAnimatedImageExtensions.Contains(fileExtension))
                {
                    Debug.Log("File has a valid animated image extension: " + fileExtension);
                    LoadAnimatedImage(filePath);
                }
                else if (validVideoExtensions.Contains(fileExtension))
                {
                    Debug.Log("File has a valid video extension: " + fileExtension);
                    LoadVideo(filePath);
                }
                else
                {
                    Debug.LogWarning("File does not have a valid extension: " + fileExtension);
                }
            }
            else
            {
                Debug.LogWarning("File does not exist at path: " + filePath);
            }
        }
        else
        {
            Debug.LogWarning("GameState instance filepath is null or empty.");
        }
    }

    void Update()
    {
        if (gifPlaying)
        {
            frameDelayTime -= Time.deltaTime;
            if (frameDelayTime <= 0)
            {
                gifFrame++;

                if (gifFrame >= gifTextures.Count)
                    gifFrame = 0;

                PuzzleManager.Instance.puzzleMaterial.mainTexture = gifTextures[gifFrame].m_texture2d;

                frameDelayTime = gifTextures[gifFrame].m_delaySec;
            }
        }
    }
    private void LoadImage(string filePath)
    {
        Debug.Log("Loading image from file: " + filePath);
        byte[] imageData = System.IO.File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            Debug.Log("Image loaded successfully.");
            // set puzzle texture to image
            PuzzleManager.Instance.puzzleMaterial.mainTexture = texture;
            // start function to make puzzle from image
            PuzzleManager.CreatePuzzleFromImage(texture);
        }
        else
        {
            Debug.LogError("Failed to load image from file.");
        }
    }

    private void LoadAnimatedImage(string filePath)
    {
        Debug.Log("Loading animated image from file: " + filePath);
        // load the gif frames into list of frames and delay times
        StartCoroutine(LoadGif(filePath));
    }

    private void LoadVideo(string filePath)
    {
        Debug.Log("Loading video from file: " + filePath);
    }

    // to load gifs into a list of frames
    private IEnumerator LoadGif(string filePath)
    {
        byte[] gifBytes = File.ReadAllBytes(filePath);

        yield return StartCoroutine(
            UniGif.GetTextureListCoroutine(
                gifBytes,
                OnGifLoaded
            )
        );
    }

    private void OnGifLoaded(
    List<UniGif.GifTexture> textures,
    int loopCount,
    int width,
    int height)
    {
        Debug.Log("GIF loaded!");
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);
        Debug.Log("Loop count: " + loopCount);
        Debug.Log("Frame count: " + textures.Count);

        if (textures.Count > 0)
        {
            Debug.Log("First frame delay: " + textures[0].m_delaySec);
            Debug.Log("First frame texture: " + textures[0].m_texture2d);
            gifTextures = textures;
            frameDelayTime = Time.time + textures[0].m_delaySec;
            // set puzzle texture to first frame of animated image
            PuzzleManager.Instance.puzzleMaterial.mainTexture = gifTextures[0].m_texture2d;
            // start function to make puzzle from this first frame
            PuzzleManager.CreatePuzzleFromImage(gifTextures[0].m_texture2d);
            gifPlaying = true;
        }
    }
}
