using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    //Background Music Variables
    [SerializeField] private AudioClip offroadAvenue;
    [SerializeField] private AudioClip chillLofiKeepinThePace;
    [SerializeField] private AudioClip lofiLostStars;
    [SerializeField] private AudioClip lofiMorningBreeze;
    [SerializeField] private AudioClip coolGuitarBackgroundMusicThing;
    [SerializeField] private AudioClip vanillaLofiBeat;
    [SerializeField] private AudioClip easyGoingMusicLoop;
    [SerializeField] private AudioClip lofiBeatLoopSchmoop;
    [SerializeField] private AudioClip moodyBeat;
    [SerializeField] private AudioClip chillLofiPianoMusic;

    //Audio source that plays the background music
    [SerializeField] public AudioSource bgmSource;
    
    //flags to indicate when a song is waiting to start and what song previously played
    bool waitingToStartSong = false;
    private int lastSongNum = -1;

    //initializes the bgmmanager and ensures it persists across scenes
    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (GameState.Instance != null) bgmSource.volume = GameState.Instance.bgmVolume;
        //keeps the game state object from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    //Makes checks for video playing and song status
    //This determines whether the background music should be playing or not and if it needs to start a new song.
    private void Update()
    {
        if (MediaManager.Instance != null && MediaManager.Instance.videoPlaying) 
        {
            if (bgmSource.isPlaying) 
            {
                Debug.Log("Video is playing, stopping BGM.");
                bgmSource.Stop();
            }
            waitingToStartSong = false;
            return;
        }

        if (!bgmSource.isPlaying && !waitingToStartSong)
        {
            waitingToStartSong = true;
            StartCoroutine(WaitToStartSong());
        }
    }

    //Starts playing the selected song
    public void PlayMusic(AudioClip song)
    {
        bgmSource.clip = song;
        bgmSource.loop = false;
        bgmSource.Play();
    }

    //Selects a random song and starts playing it
    private void StartSong()
    {
        int songNum = Random.Range(0, 10);
        while (songNum == lastSongNum)
        {
            songNum = Random.Range(0, 10);
        }
        lastSongNum = songNum;
        AudioClip selectedSong = null;
        switch (songNum)
        {
            case 0: 
                selectedSong = offroadAvenue; 
                break;
            case 1: 
                selectedSong = chillLofiKeepinThePace; 
                break;
            case 2: 
                selectedSong = lofiLostStars; 
                break;
            case 3: 
                selectedSong = lofiMorningBreeze; 
                break;
            case 4: 
                selectedSong = coolGuitarBackgroundMusicThing; 
                break;
            case 5: 
                selectedSong = vanillaLofiBeat; 
                break;
            case 6: 
                selectedSong = easyGoingMusicLoop; 
                break;
            case 7: 
                selectedSong = lofiBeatLoopSchmoop; 
                break;
            case 8: 
                selectedSong = moodyBeat; 
                break;
            case 9: 
                selectedSong = chillLofiPianoMusic; 
                break;
        }
        PlayMusic(selectedSong);
    }

    //Waits for a short duration before starting a new song, ensuring no video is playing
    private IEnumerator WaitToStartSong()
    {
        yield return new WaitForSeconds(1f);

        // Check again in case a video started during the wait.
        if (MediaManager.Instance == null || !MediaManager.Instance.videoPlaying)
        {
            StartSong();
        }

        waitingToStartSong = false;
    }
}
