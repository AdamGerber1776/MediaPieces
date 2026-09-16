using UnityEngine;
using System.Linq;

public class MediaManager : MonoBehaviour
{
    //list of valid extensions
    private readonly string[] validImageExtensions = { ".png", ".jpg", ".jpeg" };
    private readonly string[] validAnimatedImageExtensions = { ".gif" };
    private readonly string[] validVideoExtensions = { ".mp4" };

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

    private void LoadImage(string filePath)
    {
        Debug.Log("Loading image from file: " + filePath);
        byte[] imageData = System.IO.File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            Debug.Log("Image loaded successfully.");
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
    }

    private void LoadVideo(string filePath)
    {
        Debug.Log("Loading video from file: " + filePath);
    }
}
