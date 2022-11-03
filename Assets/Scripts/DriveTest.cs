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

    public static DriveService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath)
    {
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

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error ?? "null"}");
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