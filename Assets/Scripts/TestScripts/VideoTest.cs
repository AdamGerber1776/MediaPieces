using UnityEngine;
using UnityEngine.Video;

public class VideoTest : MonoBehaviour
{
    [SerializeField]
    private string path;

    void Awake()
    {
        GameObject vidSpot = GameObject.Find("GifSpot 1");

        VideoPlayer videoPlayer = vidSpot.AddComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.url = path;

        videoPlayer.isLooping = true;
    }
}
