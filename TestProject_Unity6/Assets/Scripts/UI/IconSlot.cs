using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconSlot : MonoBehaviour
{
    public Image cool;
    public Image icon;
    public TMP_Text count;

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

    public void SetSlot(Sprite sprite, int count = 0)
    {
        icon.sprite = sprite;
        this.count.text = count.ToString();
        this.count.gameObject.SetActive(count > 0);
    }
}
