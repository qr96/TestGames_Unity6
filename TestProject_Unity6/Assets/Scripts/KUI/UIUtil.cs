using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtil
{
    public static void ApplySafeAreaAnchor(ref RectTransform rt)
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x = rt.anchorMin.x;
        anchorMax.x = rt.anchorMax.x;

        anchorMin.y = 0f;
        anchorMax.y /= Screen.height;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }

    public static void ApplyPreserveRatio(RectTransform rt, float maxRatio)
    {
        var nowRatio = rt.rect.width / rt.rect.height;

        if (nowRatio > maxRatio)
        {
            var margin = (nowRatio - maxRatio) / nowRatio / 2f;

            rt.anchorMin = new Vector2(0f + margin, 0f);
            rt.anchorMax = new Vector2(1f - margin, 1f);
        }
        else
        {
            rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
            rt.anchorMax = new Vector2(1f, rt.anchorMax.y);
        }
    }
}
