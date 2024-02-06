using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PacksMenu : MonoBehaviour
{
    List<DeckPack> packs = new List<DeckPack>();
    public GameObject packMenuPrefab;
    public Transform packMenusParent;
    public UpdateContent updateContent;
    List<PackControl> packControls = new List<PackControl>();

    public Transform newPackPanel;
    public GameObject newPackMenu;

    private void Start()
    {
        CheckPacks();
    }

    void CheckPacks()
    {
        foreach (var pack in PackLoader.packPerId.Values)
        {
            AddPackControl(pack);
        }
    
    
        //string packsFolder = Path.Combine(Application.persistentDataPath, "Packs");
        //
        //if(!Directory.Exists(packsFolder))
        //{
        //    return;
        //}
        //var folders = Directory.GetDirectories(packsFolder);
        //
        //foreach(var folder in folders)
        //{
        //    var file = Path.Combine(folder, "pack.json");
        //    if (File.Exists(file))
        //    {
        //        try
        //        {
        //            var serializedPack = JsonSerialization.ReadFromJson<SerializedDeckPack>(file);
        //
        //            if (!string.IsNullOrEmpty(serializedPack.title))
        //            {
        //                AddSerializedPack(serializedPack);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogError($"[PacksMenu] - Could not read pack: {file}: {e}");
        //        }
        //    }
        //}
    
    }

    PackControl AddPackControl(DeckPack pack)
    {
        GameObject packMenu = Instantiate(packMenuPrefab, packMenusParent);
        PackControl packControl = packMenu.GetComponent<PackControl>();
        packControls.Add(packControl);
        packControl.pack = pack;
        packControl.updateContent = updateContent;
        packControl.Setup();

        newPackPanel.SetAsLastSibling();
        return packControl;
    }

    public void AddSerializedPack(SerializedDeckPack serializedPack)
    {
        PackLoader.instance.AddSerializedPack(serializedPack);
        if(PackLoader.packPerId.ContainsKey(serializedPack.driveFolderId))
        {
            DeckPack pack = PackLoader.packPerId[serializedPack.driveFolderId];
            PackControl packControl = null;
            foreach (var control in packControls)
            {
                if (control.pack.driveFolderId == serializedPack.driveFolderId)
                {
                    packControl = control;
                    packControl.Setup();
                    break;
                }
            }
            if (packControl == null)
            {
                packControl = AddPackControl(pack);
            }
        }
        else
        {
            Debug.LogError($"No deck pack after AddSerializedPack id: {serializedPack.driveFolderId}");
        }
        //StartCoroutine(LoadBannerTexture(Path.Combine(Application.persistentDataPath,"Packs", serializedPack.driveFolderId, serializedPack.banner), packControl));
    }
    //IEnumerator LoadBannerTexture(string path, PackControl packControl)
    //{
    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
    //    Debug.Log("<color=yellow>file://" + path + "</color>");
    //    yield return www.SendWebRequest();
    //
    //    Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");
    //
    //    Texture2D texture = DownloadHandlerTexture.GetContent(www);
    //    packControl.LoadBannerTexture(texture);
    //    www.Dispose();
    //
    //}



    public void Back()
    {
        if(newPackMenu.gameObject.activeSelf)
        {
            newPackMenu.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
