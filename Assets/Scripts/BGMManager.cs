using UnityEngine;

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

    private int loopsRemaining;

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
        if (MediaManager.Instance != null && MediaManager.Instance.videoPlaying && bgmSource.isPlaying) 
        {
            Debug.Log("Video is playing, stopping BGM.");
            loopsRemaining = 0;
            bgmSource.Stop();
            return;
        }

        if (!bgmSource.isPlaying)
        {
            loopsRemaining--;

            if (loopsRemaining > 0)
            {
                bgmSource.Play();
            }
            else
            {
                StartSong();
            }
        }
    }

    public void PlayMusic(AudioClip song, int playCount)
    {
        bgmSource.clip = song;
        bgmSource.loop = false;
        loopsRemaining = playCount;
        bgmSource.Play();
    }

    private void StartSong()
    {
        int songNum = Random.Range(0, 10);
        int playCount = 1;
        AudioClip selectedSong = null;
        switch (songNum)
        {
            case 0: 
                selectedSong = offroadAvenue; 
                playCount = 5;
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
                playCount = 4;
                break;
            case 9: 
                selectedSong = chillLofiPianoMusic; 
                break;
        }
        PlayMusic(selectedSong, playCount);
    }
}
