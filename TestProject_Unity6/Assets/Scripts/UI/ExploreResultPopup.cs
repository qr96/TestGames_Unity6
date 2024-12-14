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

    public void SetItems(List<int> items)
    {
        slotView.SetInventory(items, OnSetItem);
    }

    void OnSetItem(int item, GameObject itemSlot)
    {
        var iconSlot = itemSlot.GetComponent<IconSlot>();
        iconSlot.gameObject.SetActive(true);
        iconSlot.SetSprite(Resources.Load<Sprite>($"Sprites/Items/{item}"));
    }
}
