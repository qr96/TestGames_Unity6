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
        ResetEquipped();

        weaponSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Weapon));
        necklaceSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Necklace));
        glovesSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Gloves));
        hatSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Hat));
        armorSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Armor));
        shoesSlot.GetComponent<KButton>().onClick.AddListener(() => OnClickUnEquip(Equipment.Part.Shoes));
    }

    public void SetPopup(List<Equipment> equipments, List<Equipment> equipped)
    {
        SetEquipments(equipments);
        SetEquipped(equipped);
    }

    void SetEquipped(List<Equipment> equipments)
    {
        ResetEquipped();

        foreach (var equip in equipments)
        {
            if (equip.part == Equipment.Part.Weapon)
            {
                weaponSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                weaponSlot.GetComponent<KButton>().enabled = true;
            }
            else if (equip.part == Equipment.Part.Necklace)
            {
                necklaceSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                necklaceSlot.GetComponent<KButton>().enabled = false;
            }
            else if (equip.part == Equipment.Part.Gloves)
            {
                glovesSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                glovesSlot.GetComponent<KButton>().enabled = false;
            }
            else if (equip.part == Equipment.Part.Hat)
            {
                hatSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                hatSlot.GetComponent<KButton>().enabled = false;
            }
            else if (equip.part == Equipment.Part.Armor)
            {
                armorSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                armorSlot.GetComponent<KButton>().enabled = false;
            }
            else if (equip.part == Equipment.Part.Shoes)
            {
                shoesSlot.SetSlot(TableData.GetEquipmentSprite(equip.code, equip.part), equip.upgradeLevel);
                shoesSlot.GetComponent<KButton>().enabled = false;
            }
        }
    }

    void ResetEquipped()
    {
        weaponSlot.SetSlot(TableData.GetSpriteNone());
        necklaceSlot.SetSlot(TableData.GetSpriteNone());
        glovesSlot.SetSlot(TableData.GetSpriteNone());
        hatSlot.SetSlot(TableData.GetSpriteNone());
        armorSlot.SetSlot(TableData.GetSpriteNone());
        shoesSlot.SetSlot(TableData.GetSpriteNone());

        weaponSlot.GetComponent<KButton>().enabled = false;
        necklaceSlot.GetComponent<KButton>().enabled = false;
        glovesSlot.GetComponent<KButton>().enabled = false;
        hatSlot.GetComponent<KButton>().enabled = false;
        armorSlot.GetComponent<KButton>().enabled = false;
        shoesSlot.GetComponent<KButton>().enabled = false;
    }

    void SetEquipments(List<Equipment> equipments)
    {
        for (int i = slotPool.Count; i < equipments.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemData = equipments[i];
            var itemSlot = slotPool[i];
            var button = itemSlot.GetComponent<KButton>();

            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(TableData.GetEquipmentSprite(itemData.code, itemData.part), itemData.upgradeLevel);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickEquip(itemData.id));
        }

        for (int i = equipments.Count; i < slotPool.Count; i++)
            slotPool[i].gameObject.SetActive(false);
    }

    void OnClickEquip(int equipmentId)
    {
        Managers.UIManager.ShowPopup<ConfirmPopup>().SetPopup("안내", "장비를 착용하시겠습니까?", () =>
        {
            Managers.GameData.Equip(equipmentId);
        }, null);
    }

    void OnClickUnEquip(Equipment.Part part)
    {
        Managers.UIManager.ShowPopup<ConfirmPopup>().SetPopup("안내", "장비를 해제하시겠습니까?", () =>
        {
            Managers.GameData.UnEquip(part);
        }, null);
    }
}
