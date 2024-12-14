using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<GameObject> maps = new List<GameObject>();
    public TriggerEvent portalPrefab;

    List<TriggerEvent> portalPool = new List<TriggerEvent>();

    int nowMapId;

    private void Start()
    {
        portalPrefab.gameObject.SetActive(false);

        MoveMap(0);
    }

    public void MoveMap(int targetMapId)
    {
        Managers.GameData.EnterMap(targetMapId);
        DestroyAllMap();
        SpawnPortal(targetMapId);
        Managers.MonsterManager.player.transform.position = GetWarpPoint(targetMapId, nowMapId);
        maps[targetMapId].SetActive(true);
        nowMapId = targetMapId;
    }

    void DestroyAllMap()
    {
        foreach (var map in maps)
            map.SetActive(false);
    }

    void SpawnPortal(int mapId)
    {
        var portalInfos = GetPortalInfos(mapId);
        
        while (portalPool.Count < portalInfos.Count)
        {
            var portal = Instantiate(portalPrefab);
            portalPool.Add(portal);
        }
        
        for (int i = 0; i < portalInfos.Count; i++)
        {
            var portal = portalPool[i];
            var portalInfo = portalInfos[i];

            portal.gameObject.SetActive(true);
            portal.Set((col) => MoveMap(portalInfo.Item1), null);
            portal.transform.position = portalInfo.Item2;
        }

        for (int i = portalInfos.Count; i < portalPool.Count; i++)
            portalPool[i].gameObject.SetActive(false);
    }

    Vector3 GetWarpPoint(int targetMapId, int prevMapId)
    {
        if (targetMapId == 1 && prevMapId == 0)
            return new Vector3(30f, 1f, 1f);
        else if (targetMapId == 0 && prevMapId == 1)
            return new Vector3(-24f, 1f, 5f);

        return Vector3.zero;
    }

    List<Tuple<int, Vector3>> GetPortalInfos(int mapId)
    {
        if (mapId == 0)
            return new List<Tuple<int, Vector3>>() { new Tuple<int, Vector3>(1, new Vector3(-24f, 0f, 5f)) };
        else if (mapId == 1)
            return new List<Tuple<int, Vector3>>() { new Tuple<int, Vector3>(0, new Vector3(30f, 0f, 1f)) };

        return default;
    }
}
