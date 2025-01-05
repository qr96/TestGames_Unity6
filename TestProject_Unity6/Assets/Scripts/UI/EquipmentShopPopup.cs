using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentShopPopup : UIPopup
{
    public Button closeButton;
    public EquipmentShopItem itemPrefab;
    public TMP_Text moneyText;

    List<EquipmentShopItem> itemPool = new List<EquipmentShopItem>();

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        itemPrefab.gameObject.SetActive(false);
        SetPopup(new List<Equipment>() {
            new Equipment(1, 0, Equipment.Part.Weapon),
            new Equipment(1, 20, Equipment.Part.Weapon),
            new Equipment(1, 40, Equipment.Part.Weapon),
            new Equipment(1, 60, Equipment.Part.Weapon),
            new Equipment(1, 0, Equipment.Part.Armor),
            new Equipment(1, 20, Equipment.Part.Armor),
            new Equipment(1, 40, Equipment.Part.Armor),
            new Equipment(1, 60, Equipment.Part.Armor),
            new Equipment(1, 0, Equipment.Part.Necklace),
            new Equipment(1, 20, Equipment.Part.Necklace),
            new Equipment(1, 40, Equipment.Part.Necklace),
            new Equipment(1, 60, Equipment.Part.Necklace),
            new Equipment(1, 0, Equipment.Part.Gloves),
            new Equipment(1, 20, Equipment.Part.Gloves),
            new Equipment(1, 40, Equipment.Part.Gloves),
            new Equipment(1, 60, Equipment.Part.Gloves),
        });
    }

    public void SetMoney(long money)
    {
        moneyText.text = money.ToFormat();
    }

    void SetPopup(List<Equipment> equipments)
    {
        for (int i = itemPool.Count; i < equipments.Count; i++)
            itemPool.Add(Instantiate(itemPrefab, itemPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var equipment = equipments[i];
            var itemSlot = itemPool[i];

            itemSlot.SetItem(equipment.code, equipment.upgradeLevel, equipment.part, () => OnBuyButton(equipment.code, equipment.upgradeLevel, equipment.part));
            itemSlot.gameObject.SetActive(true);
        }

        for (int i = equipments.Count; i < itemPool.Count; i++)
            itemPool[i].gameObject.SetActive(false);
    }

    void OnBuyButton(int equipmentCode, int upgradeLevel, Equipment.Part part)
    {
        var equipmentName = Managers.TableData.GetEquipmentName(part, equipmentCode);
        var equipmentPrice = Managers.TableData.GetEquipmentBuyPrice(part, equipmentCode, upgradeLevel);
        var message = $"정말로 {equipmentName} (+{upgradeLevel})을(를) 구매하시겠습니까?\n구매 가격 : {equipmentPrice.ToFormat()}";
        Managers.UIManager.ShowPopup<ConfirmPopup>().SetPopup("안내", message, () =>
        {
            Managers.GameData.ModifyPlayerMoney(-equipmentPrice);
            Managers.GameData.AddEquipment(equipmentCode, upgradeLevel, part);
        }, null);
    }
}
