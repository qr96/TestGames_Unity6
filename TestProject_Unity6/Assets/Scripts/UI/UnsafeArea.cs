using UnityEngine;

public class UnsafeArea : MonoBehaviour
{
    RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        Managers.UIManager.AddCanvasChangeCallback(MakeUnsafeArea);
        MakeUnsafeArea();
    }

    void MakeUnsafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x = rt.anchorMin.x;
        anchorMax.x = rt.anchorMax.x;

        anchorMin.y = 0f;
        anchorMax.y = Screen.height / anchorMax.y;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }
}
