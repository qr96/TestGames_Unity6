using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<UILayout> uiLayouts = new List<UILayout>();
    public List<UIPopup> uiPopups = new List<UIPopup>();

    public RectTransform canvas;
    public RectTransform root;

    public RectTransform layoutParent;
    public RectTransform popupParent;

    private void Awake()
    {
        UIUtil.ApplySafeAreaAnchor(ref root);
        UIUtil.ApplyPreserveRatio(root, 0.75f);

        var canvasChangeCallback = canvas.GetComponent<CanvasDimensionsChangeCallback>();
        canvasChangeCallback.SetDimensionsChangeCallback(() =>
        {
            UIUtil.ApplySafeAreaAnchor(ref root);
        });

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

    void RegisterAllLayoutsAndPopups()
    {
        var layouts = layoutParent.GetComponentsInChildren<UILayout>(true);
        var popups = popupParent.GetComponentsInChildren<UIPopup>(true);

        foreach (var layout in layouts)
            uiLayouts.Add(layout);

        foreach (var popup in popups)
            uiPopups.Add(popup);
    }
}
