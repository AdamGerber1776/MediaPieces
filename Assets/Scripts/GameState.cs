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
    public float volume = 100f;
    public bool fullscreen = true;
    public string resolution = "1920x1080";
    
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
        DontDestroyOnLoad(gameObject);
    }
}
