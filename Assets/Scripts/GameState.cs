using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    //file path to be used in puzzle scene
    public string selectedFilePath;
    public int selectedDifficulty;
    public List<string> selectedFolderPaths = new List<string>();
    public List<string> selectedFilePaths = new List<string>();
    public float volume;
    public bool fullscreen;
    public string resolution;
    
    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadSettings();
        //keeps the game state object from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    private void LoadSettings()
    {
        volume = PlayerPrefs.GetFloat("Volume", 1f);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        resolution = PlayerPrefs.GetString("Resolution", Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        ApplyResolution();
        MainMenuHandler.UpdateSettings();
    }

    public void UpdateVolume(float newVal)
    {
        volume = newVal;
        MediaManager.Instance.videoPlayer.SetDirectAudioVolume(0, GameState.Instance.volume);
        PlayerPrefs.SetFloat("Volume", newVal);
        PlayerPrefs.Save();
    }

    public void UpdateFullscreenToggle(bool newVal)
    {
        fullscreen = newVal;
        PlayerPrefs.SetInt("Fullscreen", newVal ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void UpdateResolutionDropdown(string newVal)
    {
        resolution = newVal;
        PlayerPrefs.SetString("Resolution", newVal);
        PlayerPrefs.Save();

        ApplyResolution();
    }

    public void ApplyResolution()
    {
        string[] dimensions = resolution.Split('x');

        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        if (fullscreen)
        {
            Screen.SetResolution(
                width,
                height,
                FullScreenMode.FullScreenWindow
            );
        }
        else
        {
            Screen.SetResolution(
                width,
                height,
                FullScreenMode.Windowed
            );
        }
    }
}
