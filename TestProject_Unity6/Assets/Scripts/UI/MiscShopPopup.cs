using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiscShopPopup : UIPopup
{
    public IconSlot slotPrefab;
    public KButton sellButton;
    public KButton closeButton;
    public TMP_Text moneyText;
    public TMP_Text sellPriceText;

    List<IconSlot> slotPool = new List<IconSlot>();

    private void Start()
    {
        sellButton.onClick.AddListener(() => Managers.GameData.SellAllItems());
        closeButton.onClick.AddListener(() => Hide());
    }

    public void SetPopup(List<ItemData> items, long myMoney)
    {
        var sellPrice = 0L;

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

            sellPrice += Managers.TableData.GetMiscItemPrice(itemData.itemCode, itemData.count);
        }

        moneyText.text = myMoney.ToFormat();
        sellPriceText.text = $"판매 가격 : {sellPrice.ToFormat()}";
    }
}
