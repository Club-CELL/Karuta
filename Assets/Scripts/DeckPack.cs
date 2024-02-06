using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Pack", menuName = "Deck pack")]
public class DeckPack : ScriptableObject
{
    public string title;
    public string driveFolderId;
    public Texture banner;

    private void OnValidate()
    {
        string[] parts = driveFolderId.Split('/');
        for(int i=0;i<parts.Length;i++)
        {
            if(parts[i].Equals("folders") && parts.Length>i+1)
            {
                driveFolderId = parts[i + 1];
                return;
            }
        }
        //driveFolderId = driveFolderId.Replace("https://drive.google.com/drive/folders/", "");
    }
}

[System.Serializable]
public class SerializedDeckPack
{
    public string title;
    public string driveFolderId;
    public string banner = "banner.png";
}
