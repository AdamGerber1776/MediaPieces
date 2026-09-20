using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class GifTest : MonoBehaviour
{
    [SerializeField]
    private string gifPath;
    [SerializeField]
    private Material gifMaterial;
    private List<UniGif.GifTexture> testGifTextures;
    private bool running = false;
    private int gifTextureIndex = 0;
    private float frameDelayTime;

    private void Start()
    {
        StartCoroutine(LoadGif());
    }
    private void Update()
    {
        if (running)
        {
            frameDelayTime -= Time.deltaTime;
            if (frameDelayTime <= 0)
            {
                gifTextureIndex++;

                if (gifTextureIndex >= testGifTextures.Count)
                    gifTextureIndex = 0;

                gifMaterial.mainTexture =
                    testGifTextures[gifTextureIndex].m_texture2d;

                frameDelayTime = testGifTextures[gifTextureIndex].m_delaySec;
            }
        }
    }

    private IEnumerator LoadGif()
    {
        byte[] gifBytes = File.ReadAllBytes(gifPath);

        yield return StartCoroutine(
            UniGif.GetTextureListCoroutine(
                gifBytes,
                OnGifLoaded
            )
        );
    }

    private void OnGifLoaded(
    List<UniGif.GifTexture> gifTextures,
    int loopCount,
    int width,
    int height)
    {
        Debug.Log("GIF loaded!");
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);
        Debug.Log("Loop count: " + loopCount);
        Debug.Log("Frame count: " + gifTextures.Count);

        if (gifTextures.Count > 0)
        {
            Debug.Log("First frame delay: " + gifTextures[0].m_delaySec);
            Debug.Log("First frame texture: " + gifTextures[0].m_texture2d);
            testGifTextures = gifTextures;
            frameDelayTime = Time.time + gifTextures[0].m_delaySec;
            running = true;
        }
    }


}
