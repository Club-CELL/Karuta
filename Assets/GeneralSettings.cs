using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    public int targetFrameRate = 60;
    public int vsyncCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = vsyncCount;
        Application.targetFrameRate = targetFrameRate;
    }
}
