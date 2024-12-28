using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentShopItem : MonoBehaviour
{
    public EquipmentSlot slot;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public KButton buyButton;

    public void SetItem(int itemCode, int upgradeLevel, UnityAction onBuy)
    {
        var itemSprite = Resources.Load<Sprite>($"Sprites/Equipments/{itemCode}");
        var itemName = TableData.GetEquipmentName(itemCode);
        var itemPrice = TableData.GetEquipmentBuyPrice(itemCode);

        SetItem(itemSprite, upgradeLevel, itemName, itemPrice, onBuy);
    }

    void SetItem(Sprite icon, int upgradeLevel, string itemName, long price, UnityAction buy)
    {
        slot.SetSlot(icon, upgradeLevel);
        itemNameText.text = itemName;
        priceText.text = price.ToString();
        buyButton.onClick.RemoveAllListeners();
        if (buy != null)
            buyButton.onClick.AddListener(buy);
    }
}
