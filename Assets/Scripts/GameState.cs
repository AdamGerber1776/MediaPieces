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
    public float videoVolume;
    public float sfxVolume;
    public float bgmVolume;
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
        videoVolume = PlayerPrefs.GetFloat("Video Volume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGM Volume", 1f);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        resolution = PlayerPrefs.GetString("Resolution", Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        ApplyResolution();
        MainMenuHandler.UpdateSettings();
    }

    public void UpdateVideoVolume(float newVal)
    {
        videoVolume = newVal;
        if (MediaManager.Instance != null && MediaManager.Instance.videoPlayer != null) MediaManager.Instance.videoPlayer.SetDirectAudioVolume(0, videoVolume);
        PlayerPrefs.SetFloat("Video Volume", newVal);
        PlayerPrefs.Save();
    }

    public void UpdateSfxVolume(float newVal)
    {
        sfxVolume = newVal;
        AudioManager.Instance.sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFX Volume", newVal);
        PlayerPrefs.Save();
    }

    public void UpdateBgmVolume(float newVal)
    {
        bgmVolume = newVal;
        BGMManager.Instance.bgmSource.volume = bgmVolume;
        PlayerPrefs.SetFloat("BGM Volume", newVal);
        PlayerPrefs.Save();
    }

    public void UpdateFullscreenToggle(bool newVal)
    {
        fullscreen = newVal;
        PlayerPrefs.SetInt("Fullscreen", newVal ? 1 : 0);
        PlayerPrefs.Save();

        ApplyResolution();
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