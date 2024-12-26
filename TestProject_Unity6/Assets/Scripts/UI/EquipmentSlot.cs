using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text upgrade;

    public void SetSlot(Sprite sprite, int upgrade = 0)
    {
        icon.sprite = sprite;
        this.upgrade.text = upgrade.ToString();
        this.upgrade.gameObject.SetActive(upgrade > 0);
    }

    public void SetUpgrade(int upgrade = 0)
    {
        this.upgrade.text = $"+{upgrade}";
        this.upgrade.gameObject.SetActive(upgrade > 0);
    }
}