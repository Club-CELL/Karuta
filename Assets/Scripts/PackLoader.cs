using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PackLoader : MonoBehaviour
{


    public static Dictionary<string, DeckPack> packPerId = new Dictionary<string, DeckPack>();

    public delegate void OnLoadedBanner(DeckPack pack);
    public static OnLoadedBanner onLoadedBanner;


    public static PackLoader instance;

    void Start()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        CheckPacks();
        DontDestroyOnLoad(gameObject);
    }




    void CheckPacks()
    {
        var resourcePacks = Resources.LoadAll<DeckPack>("Packs");
        foreach (var pack in resourcePacks)
        {
            //packs.Add(pack);
            packPerId.Add(pack.driveFolderId, pack);
            Debug.Log($"[PacksMenu] Added Pack from Resources: {pack.title}");
        }



        string packsFolder = Path.Combine(PathManager.MainPath, "Packs");

        if (!Directory.Exists(packsFolder))
        {
            return;
        }
        var folders = Directory.GetDirectories(packsFolder);

        foreach (var folder in folders)
        {
            var file = Path.Combine(folder, "pack.json");
            if (File.Exists(file))
            {
                try
                {
                    var serializedPack = JsonSerialization.ReadFromJson<SerializedDeckPack>(file);

                    if (!string.IsNullOrEmpty(serializedPack.title))
                    {
                        AddSerializedPack(serializedPack);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[PacksMenu] - Could not read pack: {file}: {e}");
                }
            }
        }

    }


    public void AddSerializedPack(SerializedDeckPack serializedPack)
    {

        if(packPerId.ContainsKey(serializedPack.driveFolderId))
        {
            packPerId[serializedPack.driveFolderId].title = serializedPack.title;
            packPerId[serializedPack.driveFolderId].driveFolderId = serializedPack.driveFolderId;

        }
        else
        {
            DeckPack pack = (DeckPack)ScriptableObject.CreateInstance("DeckPack");
            pack.title = serializedPack.title;
            pack.driveFolderId = serializedPack.driveFolderId;
            packPerId[serializedPack.driveFolderId] = pack;
        }

        if(!string.IsNullOrEmpty(serializedPack.banner))
        {
            LoadBanner(serializedPack);
        }
    }


    public void LoadBanner(SerializedDeckPack serializedPack)
    {
        string path = Path.Combine(PathManager.MainPath, "Packs", serializedPack.driveFolderId, serializedPack.banner);
        StartCoroutine(LoadBannerTexture(path, serializedPack.driveFolderId));
    }
    IEnumerator LoadBannerTexture(string path, string id)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        Debug.Log("<color=yellow>file://" + path + "</color>");
        yield return www.SendWebRequest();

        Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");


        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        packPerId[id].banner = texture;
        onLoadedBanner?.Invoke(packPerId[id]);

    }
}
