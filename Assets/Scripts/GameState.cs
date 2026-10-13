using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    //file selection and path related variables
    public string selectedFilePath;
    public List<string> selectedFolderPaths = new List<string>();
    public List<string> selectedFilePaths = new List<string>();

    //difficulty variables
    public int selectedDifficulty;

    //settings variables
    public float videoVolume;
    public float sfxVolume;
    public float bgmVolume;
    public bool fullscreen;
    public string resolution;
    
    //initializes the game state, stops it from being destroyed on a scene change, and prevents duplication
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

    //loads the saved settings from PlayerPrefs and applies them to the game
    //playerprefs persist after application close
    private void LoadSettings()
    {
        videoVolume = PlayerPrefs.GetFloat("Video Volume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGM Volume", 1f);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        resolution = PlayerPrefs.GetString("Resolution", Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        ApplyResolution();
        ApplyAudioSettings();
        MainMenuHandler.UpdateSettings();
    }

    //updates the settings variables and saves them to PlayerPrefs
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
        if (AudioManager.Instance != null) AudioManager.Instance.sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFX Volume", newVal);
        PlayerPrefs.Save();
    }

    public void UpdateBgmVolume(float newVal)
    {
        bgmVolume = newVal;
        if (BGMManager.Instance != null) BGMManager.Instance.bgmSource.volume = bgmVolume;
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

    //applies the current resolution and fullscreen settings to the game
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

    //applies the current audio settings to the game
    public void ApplyAudioSettings()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.sfxSource.volume = sfxVolume;
        if (BGMManager.Instance != null) BGMManager.Instance.bgmSource.volume = bgmVolume;
    }
}