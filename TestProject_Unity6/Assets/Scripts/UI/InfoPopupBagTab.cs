using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPopupBagTab : MonoBehaviour
{
    public IconSlot slotPrefab;
    public GuageBar bagWeight;
    public TMP_Text moneyText;

    List<IconSlot> slotPool = new List<IconSlot>();

    private void Start()
    {
        slotPrefab.gameObject.SetActive(false);
    }

    public void SetTab(List<ItemData> items)
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

    public void SetMoney(long money)
    {
        moneyText.text = money.ToFormat();
    }

    public void SetWeightGuage(long max, long now)
    {
        bagWeight.SetGuage(max, now);
    }
}
