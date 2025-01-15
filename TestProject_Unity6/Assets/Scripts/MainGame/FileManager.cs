using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public void SaveData<T>(string directoryPath, string fileName, T targetObject)
    {
        try
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/{directoryPath}");

            var fullPath = $"{Application.persistentDataPath}/{directoryPath}/{fileName}";
            var objectJson = JsonConvert.SerializeObject(targetObject);

            File.WriteAllText(fullPath, objectJson);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public bool TryLoadData<T>(string filePath, out T loadObject)
    {
        var fullPath = $"{Application.persistentDataPath}/{filePath}";
        if (File.Exists(fullPath))
        {
            try
            {
                var fileData = File.ReadAllText(fullPath);
                loadObject = JsonConvert.DeserializeObject<T>(fileData);
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        loadObject = default;
        return false;
    }
}
