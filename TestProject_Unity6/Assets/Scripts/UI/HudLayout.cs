using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudLayout : UILayout
{
    public GuageBar hpBarPrefab;
    public TMP_Text nameTagPrefab;
    public TMP_Text damagePrefab;

    List<int> targetIds = new List<int>();
    Dictionary<int, GuageBar> targetDic = new Dictionary<int, GuageBar>();
    Stack<GuageBar> hpBarPool = new Stack<GuageBar>();

    Dictionary<Transform, TMP_Text> nameTargetDic = new Dictionary<Transform, TMP_Text>();
    Stack<TMP_Text> nameTagPool = new Stack<TMP_Text>();

    Stack<TMP_Text> damagePool = new Stack<TMP_Text>();

    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        hpBarPrefab.SetActive(false);
        nameTagPrefab.gameObject.SetActive(false);
        damagePrefab.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        foreach (var targetId in targetIds)
        {
            var targetPos = Managers.MonsterManager.GetMonsterPosition(targetId);
            var barPos = mainCamera.WorldToScreenPoint(targetPos + Vector3.up * 1.2f);

            targetDic[targetId].transform.position = barPos;
        }

        foreach (var target in nameTargetDic)
        {
            var targetPos = target.Key.position;
            var nameTag = target.Value;

            nameTag.transform.position = mainCamera.WorldToScreenPoint(targetPos + Vector3.up * 1.4f);
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

    public void AddNameTarget(Transform target, string name)
    {
        if (!nameTargetDic.ContainsKey(target))
        {
            if (nameTagPool.Count > 0)
                nameTargetDic.Add(target, nameTagPool.Pop());
            else
                nameTargetDic.Add(target, Instantiate(nameTagPrefab, nameTagPrefab.transform.parent));

            nameTargetDic[target].gameObject.SetActive(true);
            nameTargetDic[target].text = name;
        }
    }

    public void RemoveNameTarget(Transform target)
    {
        if (nameTargetDic.ContainsKey(target))
        {
            var nameTag = nameTargetDic[target];
            nameTag.gameObject.SetActive(false);
            nameTagPool.Push(nameTag);
            nameTargetDic.Remove(target);
        }
    }

    public void ShowDamage(long damage, Vector3 position)
    {
        TMP_Text damageIns;
        var startPos = mainCamera.WorldToScreenPoint(position);
        var endPos = mainCamera.WorldToScreenPoint(position + Vector3.up);

        if (damagePool.Count > 0)
            damageIns = damagePool.Pop();
        else
            damageIns = Instantiate(damagePrefab, transform);

        damageIns.gameObject.SetActive(true);
        damageIns.text = damage.ToString();
        damageIns.color = Color.white;
        damageIns.transform.position = startPos;
        damageIns.transform.SetAsLastSibling();
        damageIns.transform.DOMoveY(endPos.y, 1f);
        damageIns.DOFade(0f, 1f)
            .SetEase(Ease.InCirc)
            .OnComplete(() => damagePool.Push(damageIns));
    }

    public void ShowDamage(List<long> damages, Vector3 position)
    {
        for (int i = 0; i < damages.Count; i++)
            ShowDamage(damages[i], position);
    }
}
