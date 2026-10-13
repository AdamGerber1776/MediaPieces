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
    private readonly string[] validVideoExtensions = { ".mp4" , ".asf", ".avi", ".dv", ".m4v", ".mov", ".mpg", ".mpeg", ".ogv", ".vp8", ".webm", ".wmv"};

    //variables to handle gif playing
    public bool gifPlaying = false;
    private float frameDelayTime;
    private int gifFrame;
    private List<UniGif.GifTexture> gifTextures;

    //variables to handle video playing and stalling
    private int videoFailureNumber = 0;
    public bool videoPlaying = false;
    public GameObject videoPlayerObject;
    public VideoPlayer videoPlayer;
    private Coroutine videoWatchdog;
    private bool puzzleCreatedForCurrentVideo = false;
    private const float FreezeTimeout = 5f;
    private const int MaxRecoveryAttempts = 2;
    private int recoveryAttempts = 0;

    public static MediaManager Instance;

    //ensures only one instance of MediaManager exists
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

    //initializes the MediaManager and loads the first media file
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

    //updates the gif animation if it is playing
    //plays gifs using the loaded frames and delay times
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

    //loads an image from the specified file path and creates a puzzle from it
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
            PuzzleUIHandler.OpenErrorPopup("Failed to load image from file.");
            PuzzleUIHandler.CloseLoadingScreen();
        }
    }

    //loads an animated image (GIF) from the specified file path
    private void LoadAnimatedImage(string filePath)
    {
        Debug.Log("Loading animated image from file: " + filePath);
        // load the gif frames into list of frames and delay times
        StartCoroutine(LoadGif(filePath));
    }

    //loads a video from the specified file path
    private void LoadVideo(string filePath)
    {
        Debug.Log("Loading video from file: " + filePath);
        videoPlayerObject = new GameObject("VideoPlayer");
        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        //configure the video player settings
        videoPlayer.url = filePath;
        puzzleCreatedForCurrentVideo = false;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.SetDirectAudioVolume(0, GameState.Instance.videoVolume);

        //add callbacks for video preparation and error handling
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;

        //prepare the video player to load the video
        videoPlayer.Prepare();
    }

    //loads gifs into a list of frames and delay times
    //utilizes the unigif plugin for gif decoding functionality
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

    //callback function invoked when a GIF is successfully loaded
    //this function receives the loaded GIF textures, loop count, and dimensions
    //creates a puzzle with the first frame of the GIF
    //and starts the playing of the gif animation
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

    //callback function invoked when a video is successfully prepared
    //this function creates a puzzle using the prepared videoplayer, plays the video, and starts a function to watch the video for errors causing it to stop playback
    private void OnVideoPrepared(VideoPlayer player)
    {
        Debug.Log("Video prepared!");
        Debug.Log("Video dimensions: " + player.width + " x " + player.height);
        videoFailureNumber = 0;

        //creates a puzzle from the video
        if (!puzzleCreatedForCurrentVideo)
        {
            PuzzleManager.CreatePuzzle((int)player.width, (int)player.height);
            PuzzleUIHandler.CloseLoadingScreen();
            puzzleCreatedForCurrentVideo = true;
        }
        PuzzleManager.Instance.puzzleMaterial.mainTexture = player.texture;
        videoPlaying = true;
        player.Play();
        
        //start monitoring the video playback to see if it stops unexpectedly
        if (videoWatchdog == null)
        {
            videoWatchdog = StartCoroutine(MonitorVideoPlayback());
        }
    }

    //callback function invoked when a video loading error occurs
    //this function handles the error by incrementing the failure count, displaying an error popup, and attempting to load the next video if available
    private void OnVideoError(VideoPlayer player, string message)
    {
        Debug.Log("VideoPlayer error: " + message);
        Debug.Log("The video codec may not be supported.");
        videoFailureNumber++;
        PuzzleUIHandler.OpenErrorPopup(videoFailureNumber + " videos Failed to play. The video codec may not be supported. VideoPlayer error: " + message);

        if (GameState.Instance.selectedFilePaths.Count > 0)
        {
            PuzzleUIHandler.OpenLoadingScreen();
            PuzzleManager.ClearPuzzle();
            ChooseFilePath();
            LoadPuzzleFile();
        }
        else
        {
            PuzzleUIHandler.OpenErrorPopup(videoFailureNumber + " videos Failed to play. The video codec may not be supported. VideoPlayer error: " +
                                            message + " No more file paths to load. Return to the menu to select more.");
            PuzzleUIHandler.CloseLoadingScreen();
        }
    }

    //search for all files in the folder and all subfolders
    //returns a list of valid file paths based on the supported extensions
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

    //chooses a random file path from the list of selected file paths
    //removes the chosen file path from the list
    //if no file paths remain, displays an error popup and closes the loading screen
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
            PuzzleUIHandler.OpenErrorPopup("No file paths remaining. Return to Menu");
            PuzzleUIHandler.CloseLoadingScreen();
        }
    }

    //loads the puzzle file based on the selected file path
    //determines the type of media (image, animated image, video) and calls the appropriate loading function
    //if the file path is invalid it displays an error popup and closes the loading screen
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

                //determines the type of media based on the file extension
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
                    PuzzleUIHandler.OpenErrorPopup("File does not have a valid extension: " + fileExtension);
                    PuzzleUIHandler.CloseLoadingScreen();
                }
            }
            else
            {
                Debug.LogWarning("File does not exist at path: " + filePath);
                PuzzleUIHandler.OpenErrorPopup("File does not exist at path: " + filePath);
                PuzzleUIHandler.CloseLoadingScreen();
            }
        }
        else
        {
            Debug.LogWarning("GameState instance filepath is null or empty.");
            PuzzleUIHandler.OpenErrorPopup("GameState instance filepath is null or empty. Try again or select a different file.");
            PuzzleUIHandler.CloseLoadingScreen();
        }
    }

    //monitors the video playback to detect if it stalls or freezes
    //attempts recovery by restarting the video if it appears frozen
    //if the video remains frozen after multiple recovery attempts, it stops playback and logs an error
    private IEnumerator MonitorVideoPlayback()
    {
        float previousTime = (float)videoPlayer.time;
        float stalledTime = 0f;

        while (videoPlayer != null)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (videoPlayer == null || !videoPlaying)
                break;

            float currentTime = (float)videoPlayer.time;
            if (Mathf.Abs(currentTime - previousTime) > 0.05f)
            {
                // Playback is advancing normally, including a loop
                // that jumps from the end back to the beginning.
                stalledTime = 0f;
                recoveryAttempts = 0;
            }
            else
            {
                stalledTime += 1f;
            }
            previousTime = currentTime;
            if (stalledTime >= FreezeTimeout)
            {
                if (recoveryAttempts >= MaxRecoveryAttempts)
                {
                    Debug.LogError(
                        "Video remains frozen after recovery attempts."
                    );
                    videoPlaying = false;
                    break;
                }
                recoveryAttempts++;
                Debug.LogWarning(
                    $"Video appears frozen. Attempting recovery " +
                    $"({recoveryAttempts}/{MaxRecoveryAttempts})."
                );

                videoPlayer.Stop();
                videoPlayer.isLooping = true;
                videoPlayer.Prepare();

                stalledTime = 0f;
                previousTime = 0f;
            }
        }
        videoWatchdog = null;
    }
}
