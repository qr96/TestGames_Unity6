using System;
using UnityEngine;
using UnityEngine.UI;

public class IconSlot : MonoBehaviour
{
    public Image cool;
    public Image icon;

    float coolTime;
    DateTime coolEndTime;

    private void Update()
    {
        if (DateTime.Now < coolEndTime)
        {
            cool.fillAmount = (float)((coolEndTime - DateTime.Now).TotalSeconds / coolTime);
        }
        else
        {
            cool.fillAmount = 0;
        }
    }

    public void StartCoolTime(float seconds)
    {
        coolTime = seconds;
        coolEndTime = DateTime.Now.AddSeconds(seconds);
    }
}
