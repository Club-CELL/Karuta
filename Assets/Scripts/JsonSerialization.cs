using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSerialization
{

    /// <summary>
    /// Returns the object of type T serialized in the Resource folder at the path
    /// filePath
    /// </summary>
    /// <typeparam name="T">Type of the resource to read.</typeparam>
    /// <param name="filePath">Subpath in the Resource folder (without extensions)</param>
    /// <returns>Deserialized object of type T</returns>
    public static T ReadFromJsonResource<T>(string filePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        T obj = JsonUtility.FromJson<T>(textAsset.text);
        return obj;
    }

    
    /// <summary>
    /// Serializes the object obj in the Resource folder at the path filePath
    /// </summary>
    /// <typeparam name="T">Type of the resource to write.</typeparam>
    /// <param name="filePath">Subpath in the Resource folder (without extensions)</param>
    /// <param name="obj">Object to serialize</param>
    /// <param name="append">Whether you want to append to an existing file</param>
    public static void WriteToJsonResource<T>(string filePath, T obj, bool append = false)
    {
        TextWriter writer = null;

        if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
        }
        try
        {
            string json = JsonUtility.ToJson(obj, true);
            writer = new StreamWriter(Path.Combine(Path.Combine(Application.dataPath, "Resources"), filePath + ".json"), append);
            writer.Write(json);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }


    /// <summary>
    /// Returns the object of type T serialized in the given filepath
    /// </summary>
    /// <typeparam name="T">Type of the serialized object.</typeparam>
    /// <param name="filepath">Path of the file.</param>
    /// <returns>Deserialized object of type T</returns>
    public static T ReadFromJson<T>(string filepath)
    {
        StreamReader streamReader = new StreamReader(filepath);
        string s = streamReader.ReadToEnd();
        T obj = JsonUtility.FromJson<T>(s);
        streamReader.Close();
        return obj;
    }

    /// <summary>
    /// Serializes the object obj in the at the path filePath
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize.</typeparam>
    /// <param name="filePath">Path of the file to serialize in</param>
    /// <param name="obj">Object to serialize</param>
    /// <param name="append">Whether you want to append to an existing file</param>
    public static void WriteToJson<T>(string filePath, T obj, bool append = false)
    {
        List<string> folders = new List<string>();
        folders.Add(Directory.GetParent(filePath).FullName);
        while (!Directory.Exists(folders[folders.Count - 1]))
        {
            folders.Add(Directory.GetParent(folders[folders.Count - 1]).FullName);
        }
        for(int i=folders.Count - 1; i>=0; i--)
        {
            Directory.CreateDirectory(folders[i]);
        }

        StreamWriter streamWriter = null;
        try
        {
            string json = JsonUtility.ToJson(obj, true);
            streamWriter = new StreamWriter(filePath, append);
            streamWriter.Write(json);
        }
        finally
        {
            if (streamWriter != null)
                streamWriter.Close();
        }
    }



    struct JsonDateTime//To allow dateTime to be serialized in the form of utc long
    {
        public long m_utc;
        public static implicit operator DateTime(JsonDateTime jsonDateTime)
        {
            return DateTime.FromFileTimeUtc(jsonDateTime.m_utc);
        }
        public static implicit operator JsonDateTime(DateTime dateTime)
        {
            JsonDateTime jdt = new JsonDateTime();
            jdt.m_utc = dateTime.ToFileTimeUtc();
            return jdt;
        }
    }


}
