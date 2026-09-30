using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip pieceSnapSound;
    [SerializeField] private AudioClip buttonSound;

    public AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPieceSnap()
    {
        audioSource.PlayOneShot(pieceSnapSound, GameState.Instance.sfxVolume);
    }

    public void PlayButtonPress()
    {
        audioSource.PlayOneShot(buttonSound, GameState.Instance.sfxVolume);
    }
}
