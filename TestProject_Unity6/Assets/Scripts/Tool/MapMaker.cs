using System.Collections.Generic;
using UnityEngine;
using static TableData;

public class MapMaker : MonoBehaviour
{
    readonly string MapInfoDirectoryPath = "MapMaker";

    public int mapCode;
    public int unitCode;
    public MapUnitInfo.UnitType unitType;
    public Vector3 position;

    public List<MapUnitInfo> unitInfoList = new List<MapUnitInfo>();
    public List<GameObject> unitObjectList = new List<GameObject>();

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

        Managers.File.SaveData(MapInfoDirectoryPath, $"{mapCode}.json", unitInfoList);
        Debug.Log("SaveData");
    }

    public void LoadData()
    {
        if (Managers.File.TryLoadData<List<MapUnitInfo>>($"{MapInfoDirectoryPath}/{mapCode}.json", out var loaded))
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
}


