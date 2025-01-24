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
        Managers.CameraMover.MoveToTarget();
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
        if (Managers.TableData.TryGetMapWarpInfo(mapId, out var warpInfos))
        {
            var portalInfos = warpInfos.portals;

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
                portal.Set((col) => OnPortalEnter(col, portalInfo.mapCode), null);
                portal.transform.position = portalInfo.position.ToVector3();
            }

            for (int i = portalInfos.Count; i < portalPool.Count; i++)
                portalPool[i].gameObject.SetActive(false);
        }
    }

    void OnPortalEnter(Collider col, int targetMapId)
    {
        if (col.CompareTag("Player"))
            MoveMap(targetMapId);
    }

    Vector3 GetWarpPoint(int targetMapId, int prevMapId)
    {
        if (Managers.TableData.TryGetMapWarpInfo(targetMapId, out var warpInfos))
        {
            var startPoints = warpInfos.startPoints;
            foreach (var startPoint in startPoints)
                if (startPoint.mapCode == prevMapId)
                    return startPoint.position.ToVector3();
        }

        return Vector3.zero;
    }
}
