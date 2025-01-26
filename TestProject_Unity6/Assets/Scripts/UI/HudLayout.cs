using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HudLayout : UILayout
{
    const float DAMAGE_TEXT_SPACE = 36f;

    public GuageBar hpBarPrefab;
    public TMP_Text nameTagPrefab;
    public TMP_Text damagePrefab;

    Dictionary<int, GuageBar> hpBarTargetDic = new Dictionary<int, GuageBar>();
    Stack<GuageBar> hpBarPool = new Stack<GuageBar>();

    Dictionary<Transform, TMP_Text> nameTargetDic = new Dictionary<Transform, TMP_Text>();
    Stack<TMP_Text> nameTagPool = new Stack<TMP_Text>();

    Stack<TMP_Text> damagePool = new Stack<TMP_Text>();

    Camera mainCamera;
    RectTransform rectTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        hpBarPrefab.SetActive(false);
        nameTagPrefab.gameObject.SetActive(false);
        damagePrefab.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        foreach (var hpBarTarget in hpBarTargetDic)
        {
            var targetId = hpBarTarget.Key;
            var hpBar = hpBarTarget.Value;

            if (Managers.MonsterManager.TryGetMonsterPosition(targetId, out var targetPos))
            {
                var barPos = mainCamera.WorldToScreenPoint(targetPos + Vector3.up * 1.2f);
                hpBar.transform.position = barPos;
            }
        }

        foreach (var target in nameTargetDic)
        {
            var targetPos = target.Key.position;
            var nameTag = target.Value;

            nameTag.transform.position = mainCamera.WorldToScreenPoint(targetPos + Vector3.up * 1.4f);
        }
    }

    public void ClearLayout()
    {
        var hpBarTargetIds = hpBarTargetDic.Keys.ToArray();
        var nameTagTargets = nameTargetDic.Keys.ToArray();

        foreach (var hpBarTargetId in hpBarTargetIds)
            RemoveTarget(hpBarTargetId);

        foreach (var nameTagTarget in nameTagTargets)
            RemoveNameTarget(nameTagTarget);
    }

    public void AddTarget(int targetId)
    {
        if (!hpBarTargetDic.ContainsKey(targetId))
        {
            if (hpBarPool.Count > 0)
                hpBarTargetDic.Add(targetId, hpBarPool.Pop());
            else
                hpBarTargetDic.Add(targetId, Instantiate(hpBarPrefab, hpBarPrefab.transform.parent));

            hpBarTargetDic[targetId].SetActive(true);
        }
    }

    public void RemoveTarget(int targetId)
    {
        if (hpBarTargetDic.ContainsKey(targetId))
        {
            var hpBar = hpBarTargetDic[targetId];
            hpBar.SetActive(false);
            hpBarPool.Push(hpBar);

            hpBarTargetDic.Remove(targetId);
        }
    }

    public void SetHpBar(int targetId, long max, long now)
    {
        if (hpBarTargetDic.ContainsKey(targetId))
            hpBarTargetDic[targetId].SetGuage(max, now);
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
            if (nameTag != null)
            {
                nameTag.gameObject.SetActive(false);
                nameTagPool.Push(nameTag);
                nameTargetDic.Remove(target);
            }
        }
    }

    public void ShowDamage(long[] damages, Vector3 position)
    {
        var rectPos = WorldToAnchored(position, rectTransform);
        ShowDamage(damages, rectPos);
    }

    void ShowDamage(long damage, Vector2 startPosition)
    {
        var damageIns = damagePool.Count > 0 ? damagePool.Pop() : Instantiate(damagePrefab, transform);

        damageIns.gameObject.SetActive(true);
        damageIns.text = damage.ToString();
        damageIns.alpha = 1f;
        damageIns.transform.SetAsLastSibling();
        damageIns.rectTransform.anchoredPosition = startPosition;
        damageIns.rectTransform.DOAnchorPosY(startPosition.y + 100f, 1f);
        damageIns.DOFade(0f, 1f)
            .SetEase(Ease.InCirc)
            .OnComplete(() => damagePool.Push(damageIns));
    }

    void ShowDamage(long[] damages, Vector2 position)
    {
        for (int i = damages.Length - 1; i >= 0; i--)
            ShowDamage(damages[i], position + Vector2.up * DAMAGE_TEXT_SPACE * i);
    }

    Vector2 WorldToAnchored(Vector3 worldPos, RectTransform parentRect)
    {
        var screenPoint = mainCamera.WorldToScreenPoint(worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out var anchoredPos))
            return anchoredPos;

        return Vector2.zero;
    }
}
