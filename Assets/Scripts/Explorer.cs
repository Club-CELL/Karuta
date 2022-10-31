using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Explorer : MonoBehaviour
{



	public string path;
	public string startDirectory;
	public GameObject folderObject;
    public GameObject exceptionBox;
    public GameObject choicePanel;
	float height;
    float treshold;
    public float marginDown;
    public float back_speed;
    Vector2 fingerStart = Vector2.zero;
    Vector2 fingerEnd = Vector2.zero;
    float y_start_touch;
    float x_start_touch;
    // Use this for initialization
    void Start () {
        path = GetExtDir();
        //path = "/sdcard";
        //path = Directory.GetCurrentDirectory();
        //path = @"C:\RIDE";
        height = folderObject.GetComponent<RectTransform>().rect.height;
		ChangeDirectory (startDirectory);
	}
    private void OnEnable()
    {
        Start();
    }

    // Update is called once per frame
    void Update () {
        swipeDetect();
	}

    public Vector2 swipeDetect()
    {
        //bool hasMoved;
        
        if (Input.touches.Length == 0)
        {
            LetGo();
        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                //hasMoved = false;
                fingerStart = touch.position;
                fingerEnd = touch.position;
                x_start_touch = transform.position.x;
                y_start_touch = transform.position.y;

            }
            if (touch.phase == TouchPhase.Moved)
            {
                //hasMoved = true;
                fingerEnd = touch.position;
                float delta_x = fingerEnd.x - fingerStart.x;
                float delta_y = fingerEnd.y - fingerStart.y;
                
                if(delta_y!=0)
                {
                    Hold(delta_y);
                    foreach (Folder f in GetComponentsInChildren<Folder>())
                    {
                        f.activated = false;
                    }
                    
                }
                
            }
            if (touch.phase == TouchPhase.Ended)
            {
                float delta_y = fingerStart.y - fingerEnd.y;
                LetGo();
            }
        }
        return new Vector2(0, 0);
    }


    public void Hold(float dy)
    {
        Vector2 pos = transform.position;
        transform.position = new Vector2(pos.x, y_start_touch + dy);
    }
    public void LetGo()
    {
        float dy = 0;
        Vector2 pos = transform.position;
        int nbFolder = GetComponentsInChildren<Folder>().Length;

        
        int hh = Screen.height;
        float size = (nbFolder) * height;
        float max = Math.Max(hh, size + marginDown);
        float min = Math.Min(size + marginDown, hh);
        if (pos.y < min)
        {
            dy = back_speed * (min - pos.y);
        }
        if (pos.y > max)
        {
            dy = -back_speed * (pos.y - max);
        }
        pos = new Vector2(pos.x, pos.y + dy);
        GetComponent<RectTransform>().position = pos;
    }
    public void ChangeDirectory(string folder)
	{
        
        
        Folder[] folds = GetComponentsInChildren<Folder>(false);

        string startPath = path;
        //path = @"C:\RIDE";
        Debug.Log("Path: " + path);
        Debug.Log("Folder: " + folder);
        //Debug.Log("Combine: " + Path.Combine(path, folder));
        if(folder == "<--")
        {
            path = Directory.GetParent(path).ToString();
        }
        else if (path.EndsWith("/") || path.EndsWith("\\"))
        {
            //path += folder;
            path=Path.Combine(path, folder);
        }
        else
        {
            //path = path + "\\" + folder;
            path = Path.Combine(path, folder);
        }
		
        Global.mainPath =path + "/";
        try
        {
            string[] folders = Directory.GetDirectories(path);
            string name;
            //string[] pathFolders;
            int hh = Screen.height;
            int last = 0;
            if (Directory.GetParent(path) != null)
            {
                createFolderObject("<--", 0, 0);//createFolderObject("<--", 0, hh);
                last = 1;

            }
            for (int i = 0; i < folders.Length; i++)
            {
                //pathFolders = folders[i].Split('/','\\');

                //name = pathFolders[pathFolders.Length - 1];
                name = Path.GetFileName(folders[i].TrimEnd(Path.DirectorySeparatorChar));
                createFolderObject(name, 0,  - (i + last) * height);//createFolderObject(name, 0, hh - (i + last) * height);

            }
            for (int i = 0; i < folds.Length; i++)
            {
                if (folds[i].gameObject.GetInstanceID() != GetInstanceID())
                {
                    Destroy(folds[i].gameObject);

                }

            }
            GameObject choiceCopy = Instantiate(choicePanel) as GameObject;
            Destroy(choicePanel);
            choicePanel = choiceCopy;
            choicePanel.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>().parent, false);

        }
        catch(UnauthorizedAccessException e)
        {
            path = startPath;
            GameObject excBox = Instantiate(exceptionBox) as GameObject;
            Destroy(exceptionBox);
            exceptionBox = excBox;
            exceptionBox.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>(), false);
            Debug.Log("BUGGGGG !!!" + e.ToString());
            exceptionBox.GetComponentInChildren<Text>().text="Déso' pas d'accès à ce dossier :'(";
            exceptionBox.SetActive(true);
        }
        catch(Exception e)
        {
            Debug.Log("BUGGGGG !!!" + e.ToString());
        }
        

	}

	GameObject createFolderObject(string text, float x, float y)
	{
		GameObject folderObj = Instantiate (folderObject) as GameObject;

		folderObj.GetComponent<RectTransform> ().SetParent (this.GetComponent<RectTransform> (),false);

		folderObj.GetComponent<RectTransform> ().localPosition = new Vector3(x,y,0);

		folderObj.GetComponentInChildren<Text> ().text= text;


		folderObj.SetActive (true);
		return folderObj;
	}

    string GetExtDir()
    {
        string[] targetFolders = new string[]
        {
            "/storage",
            "/sdcard",
            "/storage/emulated/0",
            "/mnt/sdcard",
            "/storage/sdcard0",
            "/storage/sdcard1",
            "/sdcard0",
            "/sdcard1",
            
        };

        for(int i=0;i<targetFolders.Length;i++)
        {
            if(Directory.Exists(targetFolders[i]))
            {
                return targetFolders[i];
            }

        }
        return Application.dataPath;
            
    }
}
