using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class PlayVideo : MonoBehaviour
{
    RawImage rawImage;
    VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void OnEnable()
    {
        LoadVideo();
    }
    public void LoadVideo()
    {
        StartCoroutine(LoadVideoTexture());
    }
    IEnumerator LoadVideoTexture()
    {
        if(!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
        }
        while(!videoPlayer.isPrepared)
        {
            yield return null;
        }
        rawImage.texture = videoPlayer.texture;

        AspectRatioFitter aspectRatioFitter = rawImage.GetComponent<AspectRatioFitter>();
        if(aspectRatioFitter != null)
        {
            aspectRatioFitter.aspectRatio = (float)videoPlayer.texture.width / (float)videoPlayer.texture.height;
        }
    }

}
