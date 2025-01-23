using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TableData;

public class MapMaker : MonoBehaviour
{
    readonly string MapInfoDirectoryPath = "Resources/Jsons/MapInfos";
    readonly string MapUnitDirectoryPath = "Resources/Jsons/MapUnits";

    public int mapCode;
    public int unitCode;
    public MapUnitInfo.UnitType unitType;
    public Vector3 position;

    public List<MapUnitInfo> unitInfoList = new List<MapUnitInfo>();
    public List<GameObject> unitObjectList = new List<GameObject>();
    public MapInfo nowMapInfo = new MapInfo();

    public class MapInfo
    {
        public List<WarpInfo> startPoints = new List<WarpInfo>();
        public List<WarpInfo> portals = new List<WarpInfo>();
    }

    public class WarpInfo
    {
        public int mapCode;
        public float[] position;
    }

    public void Spawn()
    {
        if (unitType == MapUnitInfo.UnitType.Monster)
        {
            var prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{unitCode}");
            var instantiated = Instantiate(prefab);
            instantiated.gameObject.SetActive(true);

            unitObjectList.Add(instantiated);
            unitInfoList.Add(new MapUnitInfo() { code = unitCode, unitType = unitType });
        }
    }

    public void SaveData()
    {
        for (int i = 0; i < unitInfoList.Count; i++)
        {
            var unitInfo = unitInfoList[i];
            var unitObject = unitObjectList[i];

            var position = unitObject.transform.position;
            var eulerAngles = unitObject.transform.eulerAngles;

            unitInfo.position = new float[] { position.x, position.y, position.z };
            unitInfo.rotation = new float[] { eulerAngles.x, eulerAngles.y, eulerAngles.z };
        }

        SaveData(MapUnitDirectoryPath, $"{mapCode}.json", unitInfoList);
        
        Debug.Log("SaveData");
    }

    public void LoadData()
    {
        if (TryLoadData<List<MapUnitInfo>>($"{MapUnitDirectoryPath}/{mapCode}.json", out var loaded))
        {
            Debug.Log("LoadData");
            unitInfoList = loaded;

            foreach (var unitObject in unitObjectList)
                Destroy(unitObject);
            unitObjectList.Clear();

            foreach (var unitInfo in unitInfoList)
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{unitInfo.code}");
                var instantiated = Instantiate(prefab, unitInfo.position.ToVector3(), unitInfo.rotation.ToEuler());

                instantiated.gameObject.SetActive(true);
                unitObjectList.Add(instantiated);
            }
        }
    }

    public void SaveData<T>(string directoryPath, string fileName, T targetObject)
    {
        var objectType = targetObject.GetType();
        var fullPath = $"{Application.dataPath}/{directoryPath}/{fileName}";
        var objectJson = JsonConvert.SerializeObject(targetObject);

        try
        {
            Directory.CreateDirectory($"{Application.dataPath}/{directoryPath}");
            File.WriteAllText(fullPath, objectJson);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public bool TryLoadData<T>(string filePath, out T loadObject)
    {
        var fullPath = $"{Application.dataPath}/{filePath}";
        if (File.Exists(fullPath))
        {
            try
            {
                var fileData = File.ReadAllText(fullPath);
                loadObject = JsonConvert.DeserializeObject<T>(fileData);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        loadObject = default;
        return false;
    }
}


