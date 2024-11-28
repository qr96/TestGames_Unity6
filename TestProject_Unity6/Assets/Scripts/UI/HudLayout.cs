using System;
using System.Collections.Generic;
using UnityEngine;

public class HudLayout : UILayout
{
    public GuageBar hpBarPrefab;

    List<int> targetIds = new List<int>();
    Dictionary<int, GuageBar> targetDic = new Dictionary<int, GuageBar>();
    Stack<GuageBar> hpBarPool = new Stack<GuageBar>();

    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        hpBarPrefab.SetActive(false);
    }

    private void LateUpdate()
    {
        foreach (var targetId in targetIds)
        {
            var targetPos = Managers.MonsterManager.GetMonsterPosition(targetId);
            var barPos = mainCamera.WorldToScreenPoint(targetPos + Vector3.up * 1.2f);

            targetDic[targetId].transform.position = barPos;
        }
    }

    public void AddTarget(int targetId)
    {
        if (!targetDic.ContainsKey(targetId))
        {
            if (hpBarPool.Count > 0)
                targetDic.Add(targetId, hpBarPool.Pop());
            else
                targetDic.Add(targetId, Instantiate(hpBarPrefab, hpBarPrefab.transform.parent));

            targetIds.Add(targetId);
            targetDic[targetId].SetActive(true);
        }
    }

    public void RemoveTarget(int targetId)
    {
        if (targetDic.ContainsKey(targetId))
        {
            var hpBar = targetDic[targetId];
            hpBar.SetActive(false);
            hpBarPool.Push(hpBar);

            targetDic.Remove(targetId);
            targetIds.Remove(targetId);
        }
    }

    public void SetHpBar(int targetId, long max, long now)
    {
        if (targetDic.ContainsKey(targetId))
            targetDic[targetId].SetGuage(max, now);
    }
}
