using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip pieceSnapSound;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip completePuzzleSound;
    [SerializeField] private AudioClip hintSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip skipSound;

    [SerializeField] public AudioSource sfxSource;

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
