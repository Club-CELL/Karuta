using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScreenResize : MonoBehaviour
{

    void Awake()
    {
        int wPadding = 0;
        int hPadding = 146;

        float width = Screen.currentResolution.width;
        float height = Screen.currentResolution.height;

        float baseWidth = 1080;
        float baseHeight = 1920;

        int newWidth = Mathf.FloorToInt(Mathf.Min(width - wPadding, (height - hPadding) * baseWidth / baseHeight) );
        int newHeight = Mathf.FloorToInt(Mathf.Min(height - hPadding, width *  baseHeight / baseWidth) );

        Screen.SetResolution(newWidth, newHeight,false);
        Invoke("ChangeScene", 0.5f);
    }


    void ChangeScene()
    {

        SceneManager.LoadScene("ChoixJoueurs");
    }

}
