using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

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

    [SerializeField] public AudioSource bgmSource;
    
    bool waitingToStartSong = false;
    private int lastSongNum = -1;

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

    public void PlayMusic(AudioClip song)
    {
        bgmSource.clip = song;
        bgmSource.loop = false;
        bgmSource.Play();
    }

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
