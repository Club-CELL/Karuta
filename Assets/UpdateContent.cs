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

    int maxSimulateousDownloads = 10;
    int currentNumberOfDownloads = 0;

    public ThemeLoaderMainMenu themeLoaderMainMenu;


    void Awake()
    {
        text = GetComponentInChildren<Text>();
        doneDecks = false;
        doneSongs = false;
        doneVisuals = false;
        text.text = "Connecting...";
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
    IEnumerator OnEnableCoroutine()
    {
        if(service == null)
        {
            Awake();
        }
        if(service != null)
        {
            text.text = "Connected !";
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
            text.text = "<color=red>Could not connect :((</color>";
        }
        
    }

    private void GetAllDecks()
    {

        text.text = "Starting to look for decks !";
        //Get Karuta id:
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Decks/")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Decks/"));
        }
        string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1M0WGFWXYq8A0shk2NrUVpYbZN-CsB8qs"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        text.text = "Sending request for decks !";
        IList<Google.Apis.Drive.v3.Data.File> files =null;
        try
        {

             files = listRequest.Execute().Files;
        }
        catch(System.Exception e)
        {
            text.text = "<color=red>Exception : " + e.ToString() + "</color>";
            return;//yield break;
        }


        Debug.Log(files.Count);
        text.text = files.Count + " decks files found !";
        numberOfFiles += files.Count;
        int i = 0;

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
                /*if (!string.IsNullOrEmpty(onlineFile.WebContentLink))
                {
                    UnityWebRequest request = new UnityWebRequest(onlineFile.WebContentLink);

                    request.downloadHandler = new DownloadHandlerBuffer();

                    yield return request.SendWebRequest();

                    if (request.isNetworkError)
                    {
                        Debug.LogError("Network error");
                        text.text = "<color=red>Network error :(</color>";
                    }
                    else if (request.isHttpError)
                    {
                        Debug.LogError("Http error");
                        text.text = "<color=red>Http error :(</color>";
                    }
                    else
                    {
                        File.WriteAllBytes(path, request.downloadHandler.data);
                        text.text = "Downloaded " + onlineFile.Name + " !";
                    }
                }
                else
                {
                    Debug.LogError("file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink");
                    text.text = "<color=red>file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink</color>";
                }*/
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
        string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1qbrCPZlH1BxABNf5BlvFO9gDzRCYNzBy"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;
        int i = 0;

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
               /*if (!string.IsNullOrEmpty(onlineFile.WebContentLink))
                {
                    UnityWebRequest request = new UnityWebRequest(onlineFile.WebContentLink);

                    request.downloadHandler = new DownloadHandlerBuffer();

                    yield return request.SendWebRequest();

                    if (request.isNetworkError)
                    {
                        Debug.LogError("Network error");
                        text.text = "<color=red>file:Network error :(</color>";
                    }
                    else if (request.isHttpError)
                    {
                        Debug.LogError("Http error");
                        text.text = "<color=red>file:Http error :(</color>";
                    }
                    else
                    {
                        File.WriteAllBytes(path, request.downloadHandler.data);
                        text.text = "Downloaded " + onlineFile.Name + " !";
                    }
                }
                else
                {
                    Debug.LogError("file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink");
                    text.text = "<color=red>file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink</color>";
                }*/
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
        string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1ELySPLkGydDzrc6SZml05xbDoey188TQ"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;
        int i = 0;

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
                /*if (!string.IsNullOrEmpty(onlineFile.WebContentLink))
                {
                    UnityWebRequest request = new UnityWebRequest(onlineFile.WebContentLink);

                    request.downloadHandler = new DownloadHandlerBuffer();

                    yield return request.SendWebRequest();

                    if (request.isNetworkError)
                    {
                        Debug.LogError("Network error");
                        text.text = "<color=red>file:Network error :(</color>";
                    }
                    else if (request.isHttpError)
                    {
                        Debug.LogError("Http error");
                        text.text = "<color=red>file:Http error :(</color>";
                    }
                    else
                    {
                        File.WriteAllBytes(path, request.downloadHandler.data);
                        text.text = "Downloaded " + onlineFile.Name + " !";
                    }
                }
                else
                {
                    Debug.LogError("file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink");
                    text.text = "<color=red>file: " + onlineFile.Name + " / " + onlineFile.Id + "had no webcontentlink" +"</color>";
                }*/
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
        string baseID = "";


        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink, size)";
        listRequest.Q = $"'{"1fMxu63shjuiUnfeddIbjOavvyiiziE1b"}' in parents";



        listRequest.PageSize = 1000;
        //request.ResponseData.NextPageToken

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;


        Debug.Log(files.Count);
        numberOfFiles += files.Count;
        int i = 0;

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
                /*if (!string.IsNullOrEmpty(onlineFile.WebContentLink))
                {
                    UnityWebRequest request = new UnityWebRequest(onlineFile.WebContentLink);

                    request.downloadHandler = new DownloadHandlerBuffer();

                    yield return request.SendWebRequest();

                    if (request.isNetworkError)
                    {
                        Debug.LogError("Network error");
                        text.text = "<color=red>file:Network error :(</color>";
                    }
                    else if (request.isHttpError)
                    {
                        Debug.LogError("Http error");
                        text.text = "<color=red>file:Http error :(</color>";
                    }
                    else
                    {
                        File.WriteAllBytes(path, request.downloadHandler.data);
                        text.text = "Downloaded " + onlineFile.Name + " !";
                    }
                }
                else
                {
                    Debug.LogError("file: " + onlineFile.Name + "/" + onlineFile.Id + "had no webcontentlink");
                    text.text = "<color=red>file: " + onlineFile.Name + " / " + onlineFile.Id + "had no webcontentlink" +"</color>";
                }*/
            }
            else
            {
                filesHandled++;
            }
        }
        doneVisuals = true;
        //CheckDone();
    }


    /*
        private IEnumerator GetAllSongs()
        {
            //Get Karuta id:
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Son/")))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Son/"));
            }
            string baseID = "";
            var request = new GoogleDriveFiles.ListRequest();
            request.Fields = new List<string> { "files(id, name)" };
            request.Q = $"'{"1eglmnT1k2xfYoU83LYpJ9EEx3Tchf9j7"}' in parents";
            request.PageSize = 1000;
            yield return request.Send();

            Debug.Log(request.ResponseData.Files.Count);

            int i = 0;

            foreach (UnityGoogleDrive.Data.File onlineFile in request.ResponseData.Files)
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath, "Son/" + onlineFile.Name)))
                {
                    var downloadRequest = GoogleDriveFiles.Download(onlineFile.Id);
                    yield return downloadRequest.Send();
                    UnityGoogleDrive.Data.File file = downloadRequest.ResponseData;
                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "Son/" + onlineFile.Name), file.Content);
                    text.text = "Downloaded " + onlineFile.Name + " !";
                }

            }

            doneSongs = true;
        }

        private IEnumerator GetAllVisuals()
        {
            //Get Karuta id:
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Visuels/")))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Visuels/"));
            }

            string baseID = "";
            var request = new GoogleDriveFiles.ListRequest();
            request.Fields = new List<string> { "files(id, name)" };
            request.Q = $"'{"19eXJw8waPW1i4GxgMH68SXKW3O5mQvGY"}' in parents";
            request.PageSize = 1000;
            yield return request.Send();

            Debug.Log(request.ResponseData.Files.Count);

            int i = 0;

            foreach (UnityGoogleDrive.Data.File onlineFile in request.ResponseData.Files)
            {
                if(!File.Exists(Path.Combine(Application.persistentDataPath, "Visuels/" + onlineFile.Name)))
                {
                    var downloadRequest = GoogleDriveFiles.Download(onlineFile.Id);
                    yield return downloadRequest.Send();
                    UnityGoogleDrive.Data.File file = downloadRequest.ResponseData;
                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "Visuels/" + onlineFile.Name), file.Content);

                    text.text = "Downloaded " + onlineFile.Name + " !";
                }

            }

            doneVisuals = true;
            CheckDone();
        }*/

    void CheckDone()
    {
        Debug.Log(filesHandled + "/" + numberOfFiles + " files handled");
        text.text += "\n\n<color=yellow>" + filesHandled + "/" + numberOfFiles + " files handled</color>";
        if (readyToFinish && filesHandled >= numberOfFiles)
        {
            text.text = "Finished !";
            CancelInvoke("Close");
            Invoke("Close", 0.5f);
            themeLoaderMainMenu.ReloadThemes();
        }
    }
    /* void CheckDone()
     {
         if(doneVisuals && doneDecks && doneSongs)
         {
             text.text = "Finished !";
             CancelInvoke("Close");
             Invoke("Close", 0.5f);
         }
     }*/

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
            text.text = "<color=red>Error occurred - Key file does not exist </color>";
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
        text.text = "Credentials: \n" + jsonCredentials;
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
            text.text = "<color=red>" +e.ToString()+"</color>";
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
        text.text = "Got the certificate";
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
            text.text = "Connected !";
            return service;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.InnerException);
            text.text = "<color=red>Exception:" + ex.ToString() + " </color>";
            return null;
        }


    }

    public IEnumerator DownloadFile(Google.Apis.Drive.v3.Data.File file, string path)
    {
        Debug.Log("Tring to download " + file.Name);
        
        if (!string.IsNullOrEmpty(file.WebContentLink))
        {
            UnityWebRequest request = new UnityWebRequest(file.WebContentLink);

            //request.downloadHandler = new DownloadHandlerBuffer();
            request.downloadHandler = new DownloadHandlerFile(path);

            while(currentNumberOfDownloads >= maxSimulateousDownloads)
            {
                yield return new WaitForSeconds(0.5f + 0.5f*Random.value);
            }
            currentNumberOfDownloads++;
            yield return request.SendWebRequest();
            currentNumberOfDownloads--;
            if (request.error != null)
            {
                Debug.LogError("<color=red>" + request.error + "</color>");
            }
                var fileStream = File.Open(path, FileMode.Open);
            //if (fileStream.Length == 0)
            if(file.Size != fileStream.Length)
            {
                fileStream.Close();
                request.Dispose();
                request = null;
                File.Delete(path);
                yield return new WaitForSeconds(Random.value * 10 + 10);
                StartCoroutine(DownloadFile(file, path));
                Debug.LogWarning("Problem with:" + file.Name);
                yield break;
            }
            text.text = "Downloaded " + file.Name + " !";
            if(request != null)
            {
                request.Dispose();
                request = null;
            }
            fileStream.Close();
            /*
            if (request.isNetworkError)
            {
                Debug.LogError("Network error");
            }
            else if (request.isHttpError)
            {
                Debug.LogError("Http error");
            }
            else
            {
                File.WriteAllBytes(path, request.downloadHandler.data);
                text.text = "Downloaded " + file.Name + " !";
            }*/
        }
        else
        {
            Debug.LogError("file: " + file.Name + "/" + file.Id + "had no webcontentlink");
        }
        filesHandled++;
        CheckDone();
    }


    /*
    public static DriveService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath)
    {

        // check the file exists
        if (!File.Exists(keyFilePath))
        {
            Console.WriteLine("An Error occurred - Key file does not exist");
            return null;
        }

        //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
        string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveAppsReadonly,   // view your drive apps
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts };  // modify your app scripts     

        var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
        try
        {
            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = scopes
                }.FromCertificate(certificate));

            // Create the service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Daimto Drive API Sample",
            });
            return service;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            return null;

        }
    }
}*/
//22mn 42s 58

}
