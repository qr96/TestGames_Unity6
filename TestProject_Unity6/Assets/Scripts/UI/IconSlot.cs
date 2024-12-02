using System;
using UnityEngine;
using UnityEngine.UI;

public class IconSlot : MonoBehaviour
{
    public Image cool;
    public Image icon;

    float coolTime;
    DateTime coolEndTime;
    Action onEnd;

    private void Update()
    {
        if (DateTime.Now < coolEndTime)
        {
            cool.fillAmount = (float)((coolEndTime - DateTime.Now).TotalSeconds / coolTime);
        }
        else
        {
            cool.fillAmount = 0;
            onEnd?.Invoke();
        }
    }

    public void StartCoolTime(float seconds, Action onEnd = null)
    {
        coolTime = seconds;
        coolEndTime = DateTime.Now.AddSeconds(seconds);
        this.onEnd = onEnd;
    }
}
