using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreResultPopup : UIPopup
{
    public SlotView slotView;
    public KButton yesButton;

    private void Start()
    {
        yesButton.onClick.AddListener(Hide);
    }

    public void SetItems(List<ItemData> items)
    {
        slotView.SetInventory(items, OnSetItem);
    }

    void OnSetItem(ItemData itemData, GameObject slot)
    {
        var itemSlot = slot.GetComponent<IconSlot>();
        itemSlot.gameObject.SetActive(true);
        itemSlot.SetSlot(Resources.Load<Sprite>($"Sprites/Items/{itemData.itemCode}"), itemData.count);
    }
}
