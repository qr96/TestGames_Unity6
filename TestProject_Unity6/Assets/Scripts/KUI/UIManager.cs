using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<UILayout> uiLayouts = new List<UILayout>();
    public List<UIPopup> uiPopups = new List<UIPopup>();

    public RectTransform canvas;
    public RectTransform safeArea;

    public RectTransform layoutParent;
    public RectTransform popupParent;

    public bool safeTop;
    public bool safeBottom;
    public bool safeLeft;
    public bool safeRight;

    public float minRatio;
    public float maxRatio;

    CanvasDimensionsChangeCallback canvasChangeCallback;
    CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        canvasChangeCallback = canvas.GetComponent<CanvasDimensionsChangeCallback>();
        canvasChangeCallback.SetDimensionsChangeCallback(() => UpdateSafeArea());

        UpdateSafeArea();
        RegisterAllLayoutsAndPopups();
    }

    private void Start()
    {
        foreach (var popup in uiPopups)
            popup.Hide();
    }

    public T GetLayout<T>() where T : UILayout
    {
        foreach (var layout in uiLayouts)
        {
            if (layout is T)
                return (T)layout;
        }

        Debug.LogError($"[{GetType().Name}] GetLayout(). Can't find layout {typeof(T)}");
        return null;
    }

    public T GetPopup<T>() where T : UIPopup
    {
        foreach (var popup in uiPopups)
        {
            if (popup is T) 
                return (T)popup;
        }

        Debug.LogError($"[{GetType().Name}] GetPopup(). Can't find popup {typeof(T)}");
        return null;
    }

    public T ShowPopup<T>() where T : UIPopup
    {
        foreach (var popup in uiPopups)
        {
            if (popup is T)
            {
                popup.Show();
                popup.transform.SetAsLastSibling();
                return (T)popup;
            }
        }

        Debug.LogError($"[{GetType().Name}] GetPopup(). Can't find popup {typeof(T)}");
        return null;
    }

    public void HidePopup<T>() where T : UIPopup
    {
        foreach (var popup in uiPopups)
        {
            if (popup is T)
            {
                // Prevent duplicate input callback removal
                if (popup != null && popup.isActiveAndEnabled)
                    popup.Hide();
                return;
            }
        }

        Debug.LogError($"[{GetType().Name}] GetPopup(). Can't find popup {typeof(T)}");
    }

    public void AddCanvasChangeCallback(Action onChange)
    {
        canvasChangeCallback.SetDimensionsChangeCallback(onChange);
    }

    void RegisterAllLayoutsAndPopups()
    {
        var layouts = layoutParent.GetComponentsInChildren<UILayout>(true);
        var popups = popupParent.GetComponentsInChildren<UIPopup>(true);

        foreach (var layout in layouts)
            uiLayouts.Add(layout);

        foreach (var popup in popups)
            uiPopups.Add(popup);
    }

    void UpdateSafeArea()
    {
        UIUtil.ApplySafeAreaAnchor(ref safeArea, safeTop, safeBottom, safeLeft, safeRight);
        UIUtil.ApplyPreserveRatio(safeArea.rect, canvasScaler, minRatio, maxRatio);
    }
}
