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
    public int targetMapCode;
    public MapUnitInfo.UnitType unitType;
    public Vector3 position;

    public List<MapUnitInfo> unitInfoList = new List<MapUnitInfo>();
    public List<GameObject> unitObjectList = new List<GameObject>();
    
    public MapWarpInfo nowMapInfo = new MapWarpInfo();
    public List<GameObject> portalObjectList = new List<GameObject>();
    public List<GameObject> startPointObjectList = new List<GameObject>();

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

    public void SpawnPortal()
    {
        SpawnPortal(targetMapCode, position);
    }

    public void SpawnStartPoint()
    {
        SpawnStartPoint(targetMapCode, position);
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

        for (int i = 0; i < nowMapInfo.portals.Count; i++)
        {
            var portalInfo = nowMapInfo.portals[i];
            var portalObject = portalObjectList[i];

            portalInfo.position = portalObject.transform.position.ToArray();
        }

        for (int i = 0; i < nowMapInfo.startPoints.Count; i++)
        {
            var startPointInfo = nowMapInfo.startPoints[i];
            var startPointObject = startPointObjectList[i];

            startPointInfo.position = startPointObject.transform.position.ToArray();
        }

        SaveData(MapUnitDirectoryPath, $"{mapCode}.json", unitInfoList);
        SaveData(MapInfoDirectoryPath, $"{mapCode}.json", nowMapInfo);

        Debug.Log("SaveData");
    }

    public void LoadData()
    {
        ClearAll();

        if (TryLoadData<List<MapUnitInfo>>($"{MapUnitDirectoryPath}/{mapCode}.json", out var mapUnitInfos))
        {
            unitInfoList = mapUnitInfos;

            foreach (var unitInfo in unitInfoList)
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{unitInfo.code}");
                var instantiated = Instantiate(prefab, unitInfo.position.ToVector3(), unitInfo.rotation.ToEuler());

                instantiated.gameObject.SetActive(true);
                unitObjectList.Add(instantiated);
            }
        }

        if (TryLoadData<MapWarpInfo>($"{MapInfoDirectoryPath}/{mapCode}.json", out var mapInfo))
        {
            foreach (var portalInfo in mapInfo.portals)
                SpawnPortal(portalInfo.mapCode, portalInfo.position.ToVector3());

            foreach (var startPoint in mapInfo.startPoints)
                SpawnStartPoint(startPoint.mapCode, startPoint.position.ToVector3());
        }
    }

    void SpawnPortal(int mapCode, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/Etc/Portal");
        var instantiated = Instantiate(prefab);
        instantiated.transform.position = position;
        instantiated.gameObject.SetActive(true);
        portalObjectList.Add(instantiated);
        nowMapInfo.portals.Add(new WarpInfo() { mapCode = mapCode, position = position.ToArray() });
    }

    void SpawnStartPoint(int mapCode, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/Etc/StartPoint");
        var instantiated = Instantiate(prefab);
        instantiated.transform.position = position;
        instantiated.gameObject.SetActive(true);
        startPointObjectList.Add(instantiated);
        nowMapInfo.startPoints.Add(new WarpInfo() { mapCode = mapCode, position = position.ToArray() });
    }

    void ClearAll()
    {
        foreach (var unitObject in unitObjectList)
            Destroy(unitObject);

        foreach (var portal in portalObjectList)
            Destroy(portal);

        foreach (var startPoint in startPointObjectList)
            Destroy(startPoint);

        unitObjectList.Clear();
        portalObjectList.Clear();
        startPointObjectList.Clear();

        nowMapInfo.startPoints.Clear();
        nowMapInfo.portals.Clear();
    }

    void SaveData<T>(string directoryPath, string fileName, T targetObject)
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

    bool TryLoadData<T>(string filePath, out T loadObject)
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


