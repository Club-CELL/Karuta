using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Google.Apis.Drive.v3;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using UnityEngine.Networking;
public class DriveTest: MonoBehaviour
{

    string serviceEmail = "quickstartservicetest@quickstart-1586701334354.iam.gserviceaccount.com";
    string serviceFile = "quickstart-1586701334354-8a6f0582acc2.p12";
    void Start()
    {
        DriveService ds = AuthenticateServiceAccount(serviceEmail, serviceFile);
        ListStuff(ds);
    }
    /*
    private GoogleDriveFiles.ListRequest request;
    private string filePath = string.Empty;
    string a;

    string decksFolderId = "1DQiiXM6Z6BvNhOUXNXgTvUXm4pCK-mMC";
    private string result {
        get
        {
            return a;
        }
        set
        {
            a = value;
            Debug.Log(result);
        }
    }

    
    void Start()
    {
        //StartCoroutine(GetFileByPathRoutine("Karuta/Decks/Deck Inferture.txt"));

        StartCoroutine(GetAllDecks());

    }

    private IEnumerator GetAllDecks()
    {
        //Get Karuta id:

        string baseID = "";
        request = new GoogleDriveFiles.ListRequest();
        request.Fields = new List<string> { "files(id, name)" };
        request.Q = $"'{"1DQiiXM6Z6BvNhOUXNXgTvUXm4pCK-mMC"}' in parents";

        yield return request.Send();

        Debug.Log(request.ResponseData.Files.Count);

        int i = 0;

        foreach(UnityGoogleDrive.Data.File onlineFile in request.ResponseData.Files)
        {
            var request = GoogleDriveFiles.Download(onlineFile.Id);
            yield return request.Send();
            UnityGoogleDrive.Data.File file = request.ResponseData;

            if(!Directory.Exists(Path.Combine(Application.dataPath, "Decks/")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Decks/"));
            }
            
            File.WriteAllBytes(Path.Combine(Application.dataPath, "Decks/" +onlineFile.Name), file.Content);
        }
    }



    private IEnumerator GetFileByPathRoutine(string filePath)
    {
        // A folder in Google Drive is actually a file with the MIME type 'application/vnd.google-apps.folder'. 
        // Hierarchy relationship is implemented via File's 'Parents' property. To get the actual file using it's path 
        // we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. 
        // Thus, we need to traverse the entire hierarchy chain using List requests. 
        // More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

        string[] filePathSplit = filePath.Split('/');


        var fileName = filePath.Contains("/") ? filePathSplit[filePathSplit.Length-1] : filePath;
        var parentNames = filePathSplit;//filePath.Contains("/") ? filePath.GetBeforeLast("/").Split('/') : null;

        // Resolving folder IDs one by one to find ID of the file's parent folder.
        var parendId = "root"; // 'root' is alias ID for the root folder in Google Drive.
        if (parentNames != null && parentNames.Length>1)
        {
            for (int i = 0; i < parentNames.Length-1; i++)
            {
                request = new GoogleDriveFiles.ListRequest();
                request.Fields = new List<string> { "files(id)" };
                request.Q = $"'{parendId}' in parents and name = '{parentNames[i]}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";

                yield return request.Send();
                if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
                {
                    result = $"Failed to retrieve '{parentNames[i]}' part of '{filePath}' file path.";
                    yield break;
                }

                if (request.ResponseData.Files.Count > 1)
                    Debug.LogWarning($"Multiple '{parentNames[i]}' folders been found.");

                parendId = request.ResponseData.Files[0].Id;
            }
        }
        Debug.Log("a");
        // Searching the file.
        request = new GoogleDriveFiles.ListRequest();
        request.Fields = new List<string> { "files(id, size, modifiedTime)" };
        request.Q = $"'{parendId}' in parents and name = '{fileName}'";
        Debug.Log(result);
        yield return request.Send();

        if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
        {
            result = $"Failed to retrieve '{filePath}' file.";
            Debug.Log(result);
            yield break;
        }

        if (request.ResponseData.Files.Count > 1)
            Debug.LogWarning($"Multiple '{filePath}' files been found.");

        var file = request.ResponseData.Files[0];

        result = string.Format("ID: {0} Size: {1:0.00}MB Modified: {2:dd.MM.yyyy HH:MM:ss}",
            file.Id, file.Size * .000001f, file.CreatedTime);

        Debug.Log(result);
    }*/


    public static DriveService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath)
    {

        // check the file exists
        if (!File.Exists(keyFilePath))
        {
            Debug.Log("An Error occurred - Key file does not exist");
            return null;
        }

        //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
        string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
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
        catch (IOException ex)
        {
            Debug.Log(ex.InnerException);
            return null;

        }
    }
    void ListStuff(DriveService service)
    {
        // Define parameters of request.
        FilesResource.ListRequest listRequest = new FilesResource.ListRequest(service);//service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name, webContentLink)";
        listRequest.Q = $"'{"1ELySPLkGydDzrc6SZml05xbDoey188TQ"}' in parents";


        // List files.
        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        Debug.Log("Files:");
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                Debug.Log(file.Name + "/" + file.Id + "/" + file.WebContentLink);
                if(file.Name.StartsWith("9"))
                {
                    StartCoroutine(DownloadFile(file, Path.Combine(Application.dataPath, file.Name)));
                }
            }
        }
        else
        {
            Debug.Log("No files found.");
        }


        /*
        //var file2 = service.Files.Get("1ISPBQc4JnFKPuELlWJYaKnx6C7PGDhBp").Execute();
        var file2 = service.Files.Get("1twGeCVAPDBCnolvJa6Hp2JIgeAmTQwoL").Execute();

        Debug.Log("MIME type: " + file2.WebContentLink);
        Debug.Log("Title: " + file2.Name);
        Debug.Log("MIME type: " + file2.MimeType);
        StartCoroutine(DownloadFile(file2, Path.Combine(Application.dataPath, file2.Name)));*/

    }

    public static IEnumerator DownloadFile(Google.Apis.Drive.v3.Data.File file, string path)
    {
        Debug.Log("Tring to download " + file.Name);
        if (!string.IsNullOrEmpty(file.WebContentLink))
        {
            UnityWebRequest request = new UnityWebRequest(file.WebContentLink);

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if(request.isNetworkError)
            {
                Debug.LogError("Network error");
            }
            else if(request.isHttpError)
            {
                Debug.LogError("Http error");
            }
            else
            {
                File.WriteAllBytes(path, request.downloadHandler.data);
            }
        }
        else
        {
            Debug.LogError("file: " + file.Name + "/" + file.Id + "had no webcontentlink");
        }
    }





}