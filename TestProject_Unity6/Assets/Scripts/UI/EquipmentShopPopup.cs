using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentShopPopup : UIPopup
{
    public Button closeButton;
    public EquipmentShopItem itemPrefab;

    List<EquipmentShopItem> itemPool = new List<EquipmentShopItem>();

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        itemPrefab.gameObject.SetActive(false);
        SetPopup(new List<Tuple<int, int>>() {
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(2, 0),
            new Tuple<int, int>(3, 0),
            new Tuple<int, int>(4, 0),
            new Tuple<int, int>(4, 5),
            new Tuple<int, int>(5, 0),
            new Tuple<int, int>(5, 5),
        });
    }

    void SetPopup(List<Tuple<int, int>> equipments)
    {
        for (int i = itemPool.Count; i < equipments.Count; i++)
            itemPool.Add(Instantiate(itemPrefab, itemPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemCode = equipments[i].Item1;
            var upgrade = equipments[i].Item2;
            var itemSlot = itemPool[i];

            itemSlot.SetItem(itemCode, upgrade, () => OnBuyButton(itemCode, upgrade));
            itemSlot.gameObject.SetActive(true);
        }

        for (int i = equipments.Count; i < itemPool.Count; i++)
            itemPool[i].gameObject.SetActive(false);
    }

    void OnBuyButton(int equipmentCode, int upgradeLevel)
    {
        var message = $"정말로 {TableData.GetEquipmentName(equipmentCode)}을(를) 구매하시겠습니까?\n구매 가격 : {TableData.GetEquipmentBuyPrice(equipmentCode)}";
        Managers.UIManager.ShowPopup<ConfirmPopup>().SetPopup("안내", message, () => Managers.GameData.AddEquipment(equipmentCode, upgradeLevel), null);
    }
}
