using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MediaManager : MonoBehaviour
{
    //list of valid extensions
    private readonly string[] validImageExtensions = { ".png", ".jpg", ".jpeg" };
    private readonly string[] validAnimatedImageExtensions = { ".gif" };
    private readonly string[] validVideoExtensions = { ".mp4" };

    //variables to handle gif playing
    public bool gifPlaying = false;
    private float frameDelayTime;
    private int gifFrame;
    private List<UniGif.GifTexture> gifTextures;

    //variables to handle video playing
    public GameObject videoPlayerObject;
    public VideoPlayer videoPlayer;

    public static MediaManager Instance;

    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //keeps the game state object from being destroyed when loading a new scene
    }

    void Start()
    {
        if(GameState.Instance.selectedFolderPaths.Count > 0)
        {
            //get all file paths
            foreach (string folderPath in GameState.Instance.selectedFolderPaths)
            {
                List<string> filepaths = GetAllFilePaths(folderPath);
                foreach (string filePath in filepaths)
                {
                    GameState.Instance.selectedFilePaths.Add(filePath);
                }
            }
            Debug.Log(GameState.Instance.selectedFilePaths.Count + " valid file paths saved in gamestate.");
            ChooseFilePath();
        }
        LoadPuzzleFile();
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
            PuzzleManager.CreatePuzzle(texture.width, texture.height);
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
        videoPlayerObject = new GameObject("VideoPlayer");

        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        videoPlayer.url = filePath;

        videoPlayer.renderMode = VideoRenderMode.APIOnly;

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

        videoPlayer.SetDirectAudioVolume(0, GameState.Instance.volume / 100f);

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
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
            frameDelayTime = textures[0].m_delaySec;
            // set puzzle texture to first frame of animated image
            PuzzleManager.Instance.puzzleMaterial.mainTexture = gifTextures[0].m_texture2d;
            // start function to make puzzle from this first frame
            PuzzleManager.CreatePuzzle(gifTextures[0].m_texture2d.width, gifTextures[0].m_texture2d.height);
            PuzzleUIHandler.CloseLoadingScreen();
            gifPlaying = true;
        }
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        Debug.Log("Video prepared!");
        Debug.Log("Video dimensions: " + player.width + " x " + player.height);
        
        PuzzleManager.CreatePuzzle((int)player.width, (int)player.height);
        PuzzleManager.Instance.puzzleMaterial.mainTexture = player.texture;
        PuzzleUIHandler.CloseLoadingScreen();
        player.Play();
    }

    private void OnVideoError(VideoPlayer player, string message)
    {
        Debug.Log("VideoPlayer error: " + message);
        Debug.Log("The video codec may not be supported.");

        if (GameState.Instance.selectedFilePaths.Count > 0)
        {
            PuzzleUIHandler.OpenLoadingScreen();
            PuzzleManager.ClearPuzzle();
            ChooseFilePath();
            LoadPuzzleFile();
        }
        else
        {
            Debug.Log("No more file paths to load. Returning to menu.");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    //search for all files in the folder and all subfolders
    private List<string> GetAllFilePaths(string folderPath)
    {
        List<string> filePaths = new List<string>();

        foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
        {
            string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();
            if (validImageExtensions.Contains(fileExtension) || 
                validAnimatedImageExtensions.Contains(fileExtension) || 
                validVideoExtensions.Contains(fileExtension))
            {
                filePaths.Add(filePath);
            }
        }

        return filePaths;
    }

    public void ChooseFilePath()
    {
        if (GameState.Instance.selectedFilePaths.Count > 0)
        {
            int randomIndex = Random.Range(0, GameState.Instance.selectedFilePaths.Count);
            string selectedPath = GameState.Instance.selectedFilePaths[randomIndex];
            GameState.Instance.selectedFilePath = selectedPath;
            GameState.Instance.selectedFilePaths.RemoveAt(randomIndex);
        }
        else
        {
            Debug.Log("No file paths remaining. Returning to Menu");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void LoadPuzzleFile()
    {
        //checks the file path for validity and determines what function to use to load the media
        if (GameState.Instance.selectedFilePath != "" && GameState.Instance.selectedFilePath != null)
        {
            string filePath = GameState.Instance.selectedFilePath;
            PuzzleUIHandler.PopulateFilePath(filePath);
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
                    PuzzleUIHandler.CloseLoadingScreen();
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
}
