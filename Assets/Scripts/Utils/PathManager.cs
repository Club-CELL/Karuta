using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PathManager : MonoBehaviour
{

    public static string MainPath
    {
        get
        {
            if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
            {
                string directory = Path.Combine(Application.dataPath, "..", "Data");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                return directory;
            }
            else
            {
                return Application.persistentDataPath;
            }
        }
    }
}
