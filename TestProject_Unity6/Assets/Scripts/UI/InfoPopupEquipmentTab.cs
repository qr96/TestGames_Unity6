using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InfoPopupEquipmentTab : UIPopup
{
    public EquipmentSlot slotPrefab;

    public EquipmentSlot weaponSlot;
    public EquipmentSlot necklaceSlot;
    public EquipmentSlot glovesSlot;
    public EquipmentSlot hatSlot;
    public EquipmentSlot armorSlot;
    public EquipmentSlot shoesSlot;

    List<EquipmentSlot> slotPool = new List<EquipmentSlot>();

    private void Start()
    {
        slotPrefab.gameObject.SetActive(false);
    }

    public void SetPopup(List<Equipment> equipments)
    {
        SetEquipments(equipments);
    }

    void SetEquipped(Equipment weapon, Equipment necklace, Equipment gloves, Equipment hat, Equipment armor, Equipment shoes)
    {
        weaponSlot.SetSlot(TableData.GetEquipmentSprite(weapon.code, weapon.part), weapon.upgradeLevel);
    }

    void SetEquipments(List<Equipment> equipments)
    {
        for (int i = slotPool.Count; i < equipments.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemData = equipments[i];
            var itemSlot = slotPool[i];

            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(TableData.GetEquipmentSprite(itemData.code, itemData.part), itemData.upgradeLevel);
        }

        for (int i = equipments.Count; i < slotPool.Count; i++)
            slotPool[i].gameObject.SetActive(false);
    }
}
