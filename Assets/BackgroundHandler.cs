using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

public class BackgroundHandler : MonoBehaviour
{

    public GameObject imageBackground;
    public GameObject videoBackground;

    public GameObject backgroundCanvas;


    string currentImagePath = "";
    string currentVideoPath = "";

    GameObject lastDeactivatedBackground;

    public static BackgroundHandler instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            Destroy(backgroundCanvas);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(backgroundCanvas);
        }
    }

    public static void ResetValues()
    {
        instance.currentImagePath = "";
        instance.currentVideoPath = "";
    }

    public static void DefaultBackground()
    {
        if (instance.lastDeactivatedBackground != null)
        {
            instance.lastDeactivatedBackground.SetActive(true);
        }
        instance.videoBackground.SetActive(false);
        instance.imageBackground.SetActive(false);
    }


    public static void UseAsBackground(string path)
    {
        
        Debug.Log("Trying to use a background");
        string[] videoFormats = { ".asf", ".avi", ".dv", ".m4v", ".mov", ".mp4", ".mpg",
            ".mpeg", ".ogv", ".vp8", ".webm", ".wmv" };

        bool isVideo = false;
        foreach (string s in videoFormats)
        {
            isVideo = isVideo || path.EndsWith(s);
        }
        if (isVideo)
        {
            UseVideoAsBackground(path);
        }
        else
        {
            UseImageAsBackground(path);
        }
    }
    static void UseImageAsBackground(string imagePath)
    {
        instance.StartCoroutine(instance.LoadImage(imagePath));
    }

    static void UseVideoAsBackground(string videoPath)
    {
        Debug.Log("It's a video: " + videoPath);
        instance.StartCoroutine(instance.LoadVideo(videoPath));
    }





    IEnumerator LoadImage(string path)
    {

        if(File.Exists(path) && path != currentImagePath)
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }

            Debug.Log($"About to get texture for: {path}");
            yield return null;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
            yield return null;
            Debug.Log($"About to send web request for: {path}");
            yield return www.SendWebRequest();

            Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");

            yield return null;


            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 1.0f);

            if (sprite != null)
            {
                currentImagePath = path;
                imageBackground.GetComponent<Image>().sprite = sprite;
                imageBackground.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / texture.height;
                
                videoBackground.SetActive(false);
                imageBackground.SetActive(true);
            }

            www.Dispose();
        }
        else if(File.Exists(currentImagePath))
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }
            videoBackground.SetActive(false);
            imageBackground.SetActive(true);
        }
        else if(File.Exists(currentVideoPath))
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }
            videoBackground.SetActive(true);
            imageBackground.SetActive(false);
        }
        else
        {
            if (lastDeactivatedBackground != null)
            {
                lastDeactivatedBackground.SetActive(true);
            }
            videoBackground.SetActive(false);
            imageBackground.SetActive(false);
            Debug.Log("It does not exists :" + path);
        }
    }

    IEnumerator LoadVideo(string path)
    {
        Debug.Log("Trying to load a video: " + path);
        if (File.Exists(path) && path !=currentVideoPath )
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }

            Debug.Log("It exists !");
            currentVideoPath = path;
            VideoPlayer videoPlayer = videoBackground.GetComponent<VideoPlayer>();
            videoPlayer.url = path;

            videoBackground.SetActive(true);
            Debug.Log("Preparing");

            if(!!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();
            }
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }
            Debug.Log("Prepared");
            if(videoPlayer.texture.height>0)
            {
                videoBackground.GetComponent<AspectRatioFitter>().aspectRatio = (float)videoPlayer.texture.width / (float)videoPlayer.texture.height;
            }
            Debug.Log("Setting default background to false");

            
            Debug.Log("Set default background to false");
            imageBackground.SetActive(false);
            videoBackground.SetActive(true);
        }
        else if(File.Exists(currentVideoPath))
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }
            imageBackground.SetActive(false);
            videoBackground.SetActive(true);
        }
        else if (File.Exists(currentImagePath))
        {
            GameObject fond = GameObject.Find("Fond");
            if (fond != null)
            {
                fond.SetActive(false);
                lastDeactivatedBackground = fond;
            }
            videoBackground.SetActive(false);
            imageBackground.SetActive(true);
        }
        else
        {
            if(lastDeactivatedBackground != null)
            {
                lastDeactivatedBackground.SetActive(true);
            }
            videoBackground.SetActive(false);
            imageBackground.SetActive(false);
            Debug.Log("It does not exists :" + path);
        }
    }

}
