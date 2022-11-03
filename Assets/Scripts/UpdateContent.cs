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

    string progressText = "Connecting...";
    Text text;
    DriveService service;

    string serviceEmail = "quickstartservicetest@quickstart-1586701334354.iam.gserviceaccount.com";
    //string serviceFile = "quickstart-1586701334354-8a6f0582acc2.p12";
    string serviceFile = "quickstart-1586701334354-266efb641b89.json";
    string resourcesServiceFile = "Credentials/quickstart-1586701334354-266efb641b89";

    int numberOfFiles = 0;
    int filesHandled = 0;
    bool readyToFinish = false;

    //Dictionary<Google.Apis.Drive.v3.Data.File, Coroutine> coroutines = new Dictionary<Google.Apis.Drive.v3.Data.File, Coroutine>();

    int maxSimulateousDownloads = 1;
    int currentNumberOfDownloads = 0;

    public ThemeLoaderMainMenu themeLoaderMainMenu;

    bool finished = false;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        doneDecks = false;
        doneSongs = false;
        doneVisuals = false;
        progressText = "Connecting...";
        service = AuthenticateServiceAccount(serviceEmail, serviceFile);
    }
    void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());

        ThreadPool.SetMinThreads(100, 4);

        System.Net.ServicePointManager.DefaultConnectionLimit = 100;

        numberOfFiles = 0;
        filesHandled = 0;

    }
    private void Update()
    {
        text.text = progressText;
        if(finished)
        {
            finished = false;

            progressText = "Finished !";
            themeLoaderMainMenu.ReloadThemes();
            Invoke("Close", 0.5f);
        }
    }
    IEnumerator OnEnableCoroutine()
    {
        if(service == null)
        {
            Awake();
        }
        if(service != null)
        {
            progressText = "Connected !";
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            yield return new WaitForSeconds(0.5f);
            GetAllDecks();
            yield return new WaitForSeconds(0.5f);
            GetAllSongs();
            GetAllVisuals();
            GetAllThemes();
            readyToFinish = true;
            CheckDone();
            Debug.Log(filesHandled + "/" + numberOfFiles + " files done");
        }
        else
        {
            progressText = "<color=red>Could not connect :((</color>";
        }
        
    }

    private void GetAllDecks()
    {

        progressText = "Starting to look for decks !";
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Decks/")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Decks/"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1M0WGFWXYq8A0shk2NrUVpYbZN-CsB8qs"}' in parents";



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
            string path = Path.Combine(Application.persistentDataPath, "Decks/" + onlineFile.Name);
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
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Son/")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Son/"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1qbrCPZlH1BxABNf5BlvFO9gDzRCYNzBy"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;
        //int i = 0;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(Application.persistentDataPath, "Son/" + onlineFile.Name);
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
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Visuels/")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Visuels/"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1ELySPLkGydDzrc6SZml05xbDoey188TQ"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(Application.persistentDataPath, "Visuels/" + onlineFile.Name);
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
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Themes/")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Themes/"));
        }
        //string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1fMxu63shjuiUnfeddIbjOavvyiiziE1b"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;

        foreach (var onlineFile in files)
        {
            string path = Path.Combine(Application.persistentDataPath, "Themes/" + onlineFile.Name);
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
        doneVisuals = true;
        //CheckDone();
    }


    void CheckDone()
    {
        Debug.Log(filesHandled + "/" + numberOfFiles + " files handled");
        progressText += "\n\n<color=yellow>" + filesHandled + "/" + numberOfFiles + " files handled</color>";
        if (readyToFinish && filesHandled >= numberOfFiles)
        {
            finished = true;
        }
    }


    void Close()
    {
        gameObject.SetActive(false);
    }

    public DriveService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath)
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
            yield return new WaitForSeconds(0.5f + 0.5f * Random.value);
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
        request.DownloadAsync(stream);

    }

//22mn 42s 58

}
