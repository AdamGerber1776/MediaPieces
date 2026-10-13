using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //Sound Effect Variables
    [SerializeField] private AudioClip pieceSnapSound;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip completePuzzleSound;
    [SerializeField] private AudioClip hintSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip skipSound;

    //Audio source that plays the sound effects
    [SerializeField] public AudioSource sfxSource;

    //initializes the audiomanager and ensures it persists across scenes
    private void Awake()
    {
        //helps prevent the possibility of having two competing instances of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (GameState.Instance != null) sfxSource.volume = GameState.Instance.sfxVolume;
        //keeps the game state object from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    //Following functions play the respective sound effects
    public void PlayPieceSnap()
    {
        sfxSource.PlayOneShot(pieceSnapSound, GameState.Instance.sfxVolume);
    }

    public void PlayButtonPress()
    {
        sfxSource.PlayOneShot(buttonSound, GameState.Instance.sfxVolume);
    }

    public void PlayCompletePuzzle()
    {
        sfxSource.PlayOneShot(completePuzzleSound, GameState.Instance.sfxVolume);
    }

    public void PlayHint()
    {
        sfxSource.PlayOneShot(hintSound, GameState.Instance.sfxVolume);
    }

    public void PlayError()
    {
        sfxSource.PlayOneShot(errorSound, GameState.Instance.sfxVolume);
    }

    public void PlaySkip()
    {
        sfxSource.PlayOneShot(skipSound, GameState.Instance.sfxVolume);
    }
}
