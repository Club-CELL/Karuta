using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Google.Apis.Drive.v3;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using UnityEngine.Networking;

public class UpdateContent : MonoBehaviour
{
    bool doneDecks = false;
    bool doneSongs = false;
    bool doneVisuals = false;
    bool doneThemes = false;
    public bool DoneDecks
    {
        get => doneDecks;
    }
    public bool DoneSongs
    {
        get => doneSongs;
    }
    public bool DoneVisuals
    {
        get => doneVisuals;
    }
    public bool DoneThemes
    {
        get => doneThemes;
    }

    string progressText = "Connecting...";
    Text text;
    DriveService service;

    string resourcesServiceFile = "Credentials/credentials";
    
    string mainFolderId = "";

    string decksFolderId = "";
    string visualsFolderId = "";
    string soundsFolderId = "";
    string themesFolderId = "";

    int numberOfFiles = 0;
    int filesHandled = 0;
    bool readyToFinish = false;

    public int maxSimulateousDownloads = 20;
    int currentNumberOfDownloads = 0;

    string MainFolder
    {
        get
        {
            return pack != null ? Path.Combine(Application.persistentDataPath, "Packs", pack.driveFolderId) : Application.persistentDataPath;
        }
    }

    public ThemeLoaderMainMenu themeLoaderMainMenu;

    bool finished = false;

    public DeckPack pack;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        doneDecks = false;
        doneSongs = false;
        doneVisuals = false;
        progressText = "Connecting...";
        if(service==null)
        {
            service = AuthenticateServiceAccount();
        }
    }
    private void OnEnable()
    {
        progressText = "";
        text.text = "";
    }
    private void OnDisable()
    {
        progressText = "";
        text.text = "";
    }
    public void UpdatePackContent()
    {
        Debug.Log($"<b>Started download at: {Time.time} </b>");
        StartCoroutine(UpdatePackContentCoroutine());

        ThreadPool.SetMinThreads(100, 4);

        System.Net.ServicePointManager.DefaultConnectionLimit = 100;

        numberOfFiles = 0;
        filesHandled = 0;
        doneDecks = false;
        doneSongs = false;
        doneVisuals = false;
        doneThemes = false;
        readyToFinish = false;
    }
    private void Update()
    {
        text.text = progressText;
        if(finished)
        {
            Debug.Log($"<b>Finished download at: {Time.time} </b>");
            finished = false;

            progressText = "Finished !";
            themeLoaderMainMenu.ReloadThemes();
            Invoke("Close", 0.5f);
        }
    }
    IEnumerator UpdatePackContentCoroutine()
    {
        if(service == null)
        {
            Awake();
        }
        if(service != null)
        {
            progressText = "Connected !";
            Screen.sleepTimeout = SleepTimeout.NeverSleep;


            GetSubFoldersIDs();
            yield return new WaitForSeconds(0.25f);
            if(!string.IsNullOrEmpty(decksFolderId))
            {
                GetAllDecks();
            }
            else
            {
                Debug.LogError("[UpdateContent] - Not found deck subfolder");
                progressText = "<color=red> Did not find 'Decks' folder</color>";
            }
            yield return new WaitForSeconds(0.25f);


            if (!string.IsNullOrEmpty(soundsFolderId))
            {
                GetAllSongs();
            }
            else
            {
                Debug.LogError("[UpdateContent] - Not found sounds subfolder");
                progressText = "<color=red> Did not find 'Sounds' folder</color>";
            }

            if (!string.IsNullOrEmpty(visualsFolderId))
            {
                GetAllVisuals();
            }
            else
            {
                Debug.LogError("[UpdateContent] - Not found visuals subfolder");
                progressText = "<color=yellow> Did not find 'Visuals' folder</color>";
            }

            if (!string.IsNullOrEmpty(themesFolderId))
            {
                GetAllThemes();
            }
            else
            {
                Debug.LogError("[UpdateContent] - Not found themes subfolder");
            }

            readyToFinish = true;
            CheckDone();
            Debug.Log(filesHandled + "/" + numberOfFiles + " files done");
        }
        else
        {
            progressText = "<color=red>Could not connect :((</color>";
            StartCoroutine(DeactivateInSeconds(3));
        }
        
    }

    IEnumerator DeactivateInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }


    public void GetPack(string folderId, System.Action<SerializedDeckPack> handlePack, System.Action<string> log)
    {
        StartCoroutine(GetPackCoroutine(folderId, handlePack, log));
    }

    IEnumerator GetPackCoroutine(string folderId, System.Action<SerializedDeckPack> handlePack, System.Action<string> log)
    {

        progressText = "";
        if (service == null)
        {
            Awake();
        }
        DeckPack pack = null;


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{folderId}' in parents and mimeType='application/vnd.google-apps.folder'";




        IList<Google.Apis.Drive.v3.Data.File> files = null;
        try
        {
            files = listRequest.Execute().Files;
        }
        catch(System.Exception e)
        {
            log($"<color=red>Could not check the pack :'(\n\nError:<color=red>{e.Message}</color>");
            yield break;
        }

        bool foundDecks = false;
        bool foundSounds = false;
        bool foundVisuals = false;

        foreach (var onlineFile in files)
        {
            if (onlineFile.Name.ToUpper() == "DECKS")
            {
                foundDecks = true;
            }
            if (onlineFile.Name.ToUpper() == "VISUALS")
            {
                foundVisuals = true;
            }
            if (onlineFile.Name.ToUpper() == "SOUNDS")
            {
                foundSounds = true;
            }
        }
        string s = "";
        s += foundDecks ? "<color=green>Found 'Decks' folder !</color>\n" : "<color=red>Did not find any 'Decks' folder !</color>\n";
        s += foundSounds ? "<color=green>Found 'Sounds' folder !</color>\n" : "<color=red>Did not find any 'Sounds' folder !</color>>\n";
        s += foundVisuals ? "<color=green>Found 'Visuals' folder !</color>\n" : "<color=yellow>Did not find any 'Visuals' folder !</color>\n";
        
        //log(s);
        progressText = s;
        yield return null;


        listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{folderId}' in parents and mimeType!='application/vnd.google-apps.folder'";//(mimeType='application/vnd.google-apps.file' or mimeType='application/vnd.google-apps.document')";
        files = listRequest.Execute().Files;

        string bannerImage = "banner.png";
        bool foundPackInfo = false;
        string packDirectory = Path.Combine(Application.persistentDataPath, "Packs", folderId);
        string infoFilePath = Path.Combine(packDirectory, "pack.json");
        foreach (var onlineFile in files)
        {
            if (onlineFile.Name.ToUpper() == "PACK.JSON")
            {
                var request = service.Files.Get(onlineFile.Id);

                if(!Directory.Exists(packDirectory))
                {
                    Directory.CreateDirectory(packDirectory);
                }
                var stream = new FileStream(infoFilePath, FileMode.Create);
                request.Download(stream);
                foundPackInfo = true;
                stream.Close();
                break;
            }
            else if (onlineFile.Name.ToUpper() == "PACK")
            {
                var request = service.Files.Export(onlineFile.Id, "text/plain");
                if (!Directory.Exists(packDirectory))
                {
                    Directory.CreateDirectory(packDirectory);
                }
                var stream = new FileStream(infoFilePath, FileMode.Create);
                request.Download(stream);
                foundPackInfo = true;
                stream.Close();
                break;
            }
        }
        s += (foundPackInfo ? "<color=green>Found pack info</color>\n" : "<color=red>Did not find pack info</color>\n");

        progressText = s;
        text.text = s;



        if (!foundPackInfo || !foundDecks || !foundSounds)
        {
            s += "<color=red>Did not get pack.</color>";
            log(s);
            handlePack(null);
            yield break;
        }

        //log(s);

        yield return null;

        SerializedDeckPack serializedPack = null;
        try
        {
            serializedPack = JsonSerialization.ReadFromJson<SerializedDeckPack>(infoFilePath);
        }
        catch (System.Exception e)
        {
            s = $"<color=red>Could not read the info from the info file (pack json) {e.Message}</color>";
            log(s);
            Debug.LogError(s);
            handlePack(null);
            yield break;
        }

        serializedPack.driveFolderId = folderId;

        JsonSerialization.WriteToJson<SerializedDeckPack>(infoFilePath, serializedPack);

        if(serializedPack==null)
        {
            s = "<color=red>Could not read the info from the info file (pack json)</color>";
            log(s);
            handlePack(null);
            yield break;
        }

        if (!string.IsNullOrEmpty(serializedPack.banner))
        {
            bannerImage = serializedPack.banner;
        }

        //bool foundBanner = false;
        foreach (var onlineFile in files)
        {
            Debug.Log($"In folder: {onlineFile.Name} [{onlineFile.MimeType}]");
            if (onlineFile.Name.ToUpper() == bannerImage.ToUpper())
            {
                var request = service.Files.Get(onlineFile.Id);

                if (!Directory.Exists(packDirectory))
                {
                    Directory.CreateDirectory(packDirectory);
                }
                var stream = new FileStream(Path.Combine(Application.persistentDataPath,"Packs", folderId, onlineFile.Name), FileMode.Create);
                request.Download(stream);
                stream.Close();
                break;
            }
        }

        s = "<color=green>Found pack !</color>\n";
        log(s);
        serializedPack.driveFolderId = folderId;
        handlePack(serializedPack);


    }



    void GetSubFoldersIDs()
    {
        if(pack == null)
        {
            Debug.LogError("Pack is null");
            progressText = "<color=red>Error with pack :((</color>";
            StartCoroutine(DeactivateInSeconds(3));
            return;
        }
        //DeckPack animintPack = Resources.Load<DeckPack>("Packs/Anim'INT");
        mainFolderId = pack.driveFolderId;

        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{mainFolderId}' in parents and mimeType='application/vnd.google-apps.folder'";
        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        decksFolderId = "";
        visualsFolderId = "";
        soundsFolderId = "";
        foreach (var onlineFile in files)
        {
            if(onlineFile.Name.ToUpper() == "DECKS")
            {
                decksFolderId = onlineFile.Id;
            }
            if (onlineFile.Name.ToUpper() == "VISUALS")
            {
                visualsFolderId = onlineFile.Id;
            }
            if (onlineFile.Name.ToUpper() == "SOUNDS")
            {
                soundsFolderId = onlineFile.Id;
            }
            if (onlineFile.Name.ToUpper() == "THEMES")
            {
                themesFolderId = onlineFile.Id;
            }
        }


    }

    private void GetAllDecks()
    {

        progressText = "Starting to look for decks !";
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(MainFolder, "Decks")))
        {
            Directory.CreateDirectory(Path.Combine(MainFolder, "Decks"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{decksFolderId}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        progressText = "Sending request for decks !";
        IList<Google.Apis.Drive.v3.Data.File> files =null;
        try
        {

             files = listRequest.Execute().Files;
        }
        catch(System.Exception e)
        {
            progressText = "<color=red>Exception : " + e.ToString() + "</color>";
            return;//yield break;
        }


        Debug.Log(files.Count);
        progressText = files.Count + " decks files found !";
        numberOfFiles += files.Count;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(MainFolder, "Decks", onlineFile.Name);
            if (File.Exists(path))
            {
                FileStream fileStream = File.Open(path, FileMode.Open);
                if (true || fileStream.Length != onlineFile.Size)//Decks are light to download
                {
                    fileStream.Close();
                    File.Delete(path);
                }
                else
                {
                    fileStream.Close();
                }
            }
            if (!File.Exists(path))
            {
                StartCoroutine(DownloadFile(onlineFile, path));
            }
            else
            {
                filesHandled++;
            }
        }
        doneDecks = true;
        //CheckDone();
    }




    private void GetAllSongs()
    {
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(MainFolder, "Sounds")))
        {
            Directory.CreateDirectory(Path.Combine(MainFolder, "Sounds"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{soundsFolderId}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
        

        Debug.Log(files.Count);
        numberOfFiles += files.Count;
        //int i = 0;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(MainFolder, "Sounds", onlineFile.Name);
            if (File.Exists(path))
            {
                FileStream fileStream = File.Open(path, FileMode.Open);
                if (fileStream.Length != onlineFile.Size)
                {
                    fileStream.Close();
                    File.Delete(path);
                }
                else
                {
                    fileStream.Close();
                }
            }
            if (!File.Exists(path))
            {
                StartCoroutine( DownloadFile(onlineFile, path));
            }
            else
            {
                filesHandled++;
            }
        }
        doneSongs = true;
        //CheckDone();
    }


    private void GetAllVisuals()
    {
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(MainFolder, "Visuals")))
        {
            Directory.CreateDirectory(Path.Combine(MainFolder, "Visuals"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{visualsFolderId}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(MainFolder, "Visuals", onlineFile.Name);
            if (File.Exists(path))
            {
                FileStream fileStream = File.Open(path, FileMode.Open);
                if (fileStream.Length != onlineFile.Size)
                {
                    fileStream.Close();
                    File.Delete(path);
                }
                else
                {
                    fileStream.Close();
                }
            }
            if (!File.Exists(path))
            {
                StartCoroutine( DownloadFile(onlineFile, path));
            }
            else
            {
                filesHandled++;
            }
        }
        doneVisuals = true;
        //CheckDone();
    }


    private void GetAllThemes()
    {
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(MainFolder, "Themes")))
        {
            Directory.CreateDirectory(Path.Combine(MainFolder, "Themes"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{themesFolderId}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(MainFolder, "Themes", onlineFile.Name);
            if (File.Exists(path))
            {
                FileStream fileStream = File.Open(path, FileMode.Open);
                if (fileStream.Length != onlineFile.Size || path.EndsWith(".json"))//.json are light
                {
                    fileStream.Close();
                    File.Delete(path);
                }
                else
                {
                    fileStream.Close();
                }
            }
            if (!File.Exists(path))
            {
                StartCoroutine(DownloadFile(onlineFile, path));
            }
            else
            {
                filesHandled++;
            }
        }
        doneThemes = true;
        //CheckDone();
    }


    void CheckDone()
    {
        Debug.Log(filesHandled + "/" + numberOfFiles + " files handled");

        bool doneRequests = doneDecks && doneThemes && doneVisuals && doneSongs;
        progressText += $"\n\n<color=yellow>{filesHandled}/{(doneRequests ? $"{numberOfFiles}" : $"???")}</color>";
        if (doneRequests && readyToFinish && filesHandled >= numberOfFiles)
        {
            finished = true;
            Debug.Log($"<b>Finished update with {filesHandled} files handled</b>");
        }
    }


    void Close()
    {
        gameObject.SetActive(false);
    }

    public DriveService AuthenticateServiceAccount()
    {
        /*
        // check the file exists
        if (!File.Exists(keyFilePath))
        {
            Debug.Log("An Error occurred - Key file does not exist");
            progressText = "<color=red>Error occurred - Key file does not exist </color>";
            return null;
        }*/

        //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
        string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts };  // modify your app scripts     

        //var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);

        string jsonCredentials = Resources.Load<TextAsset>(resourcesServiceFile).text;
        progressText = "Credentials: \n" + jsonCredentials;
        GoogleCredential credential = null;
        try
        {
            credential = GoogleCredential.FromJson(jsonCredentials).CreateScoped(new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts });  // modify your app scripts
        }
        catch(System.Exception e)
        {
            progressText = "<color=red>" +e.ToString()+"</color>";
            return null;
        }
        
        /*
        string newKeyFilePath = Path.Combine(Application.persistentDataPath, keyFilePath);
        File.WriteAllText(newKeyFilePath, jsonCredentials);
        
        GoogleCredential credential = GoogleCredential.FromStream(new FileStream(newKeyFilePath, FileMode.Open))
    .CreateScoped(new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts });  // modify your app scripts  
*/
        progressText = "Got the certificate";
        try
        {
            /*ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = scopes
                }.FromCertificate(certificate));*/

            // Create the service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Daimto Drive API Sample",
            });
            progressText = "Connected !";
            return service;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.InnerException);
            progressText = "<color=red>Exception:" + ex.ToString() + " </color>";
            return null;
        }


    }

    private void SaveStream(System.IO.MemoryStream stream, string path, long length, Google.Apis.Drive.v3.Data.File file)
    {
        using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
        {
            stream.WriteTo(fileStream);

            if (length != fileStream.Length)
            {
                //Debug.Log("-----------------------------------Problem with:" + file.Name);
                Debug.LogWarning("Problem with:" + file.Name + " " + length + "!=" + fileStream.Length);
                //fileStream.Close();
                File.Delete(path);
                StartCoroutine(DownloadFile(file, path, Random.value * 10 + 10));
            }
            else
            {
                Debug.Log("Finished with:" + file.Name);
                progressText = "Finished saving " + file.Name + " !";
                filesHandled++;
                CheckDone();
            }
            currentNumberOfDownloads--;
        }
    }
    private IEnumerator DownloadFile(Google.Apis.Drive.v3.Data.File file, string path, float timeToWait=0)
    {


        Debug.Log("Waiting to download file " + file.Id);
        yield return new WaitForSeconds(timeToWait);


        while (currentNumberOfDownloads >= maxSimulateousDownloads)
        {
            yield return null;// new WaitForSeconds(0.5f + 0.5f * Random.value);
        }
        Debug.Log("About to download file " + file.Id);

        var request = service.Files.Get(file.Id);
        var stream = new System.IO.MemoryStream();

        currentNumberOfDownloads++;
        // Add a handler which will be notified on progress changes.
        // It will notify on each chunk download and when the
        // download is completed or failed.
        request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
        {
            switch (progress.Status)
            {
                case Google.Apis.Download.DownloadStatus.Downloading:
                    {

                        int percent = Mathf.RoundToInt((float)(progress.BytesDownloaded) / (float)(file.Size) * 100);
                        progressText = "Downloading " + file.Name + " (" + percent  +"%) ";
                        Debug.Log("Downloading file " + file.Id);
                        break;
                    }
                case Google.Apis.Download.DownloadStatus.Completed:
                    {
                        Debug.Log("Downloaded file " + file.Id + " will save it at " + path);
                        progressText = "Downloaded " + file.Name + "\n !";
                        //currentNumberOfDownloads--;
                        SaveStream(stream,path, file.Size ?? -1, file);
                        //CheckDone();
                        break;
                    }
                case Google.Apis.Download.DownloadStatus.Failed:
                    {
                        currentNumberOfDownloads--;
                        Debug.LogError("Failed downloading file " + file.Id + "\n" + progress.Exception);
                        CheckDone();
                        progressText = "<color=red> failed to download " + file.Name + " !</color> : \n" + progress.Exception + "\n";
                        StartCoroutine(DownloadFile(file, path, 5 + 0.5f * Random.value));
                        break;
                    }

                   
            }
        };
        var task = request.DownloadAsync(stream);
        
        while(!task.IsCompleted)
        {
            yield return null;
        }
        stream.Close();
    }

//22mn 42s 58

}
