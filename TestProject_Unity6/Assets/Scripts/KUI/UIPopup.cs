using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPopup : MonoBehaviour
{
    protected bool playerMoveLock = true;

    public abstract void OnCreate();
    public virtual void OnShow()
    {
        // Called after gameObject activated;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
