using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MiscShopPopup : UIPopup
{
    public IconSlot slotPrefab;
    public KButton sellButton;
    public KButton closeButton;

    List<IconSlot> slotPool = new List<IconSlot>();

    private void Start()
    {
        sellButton.onClick.AddListener(() => Managers.GameData.SellAllItems());
        closeButton.onClick.AddListener(() => Hide());
    }

    public void SetPopup(List<ItemData> items)
    {
        foreach (var slot in slotPool)
            slot.gameObject.SetActive(false);

        for (int i = slotPool.Count; i < items.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < items.Count; i++)
        {
            var itemData = items[i];
            var itemSlot = slotPool[i];

            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(Resources.Load<Sprite>($"Sprites/Items/{itemData.itemCode}"), itemData.count);
        }
    }
}
