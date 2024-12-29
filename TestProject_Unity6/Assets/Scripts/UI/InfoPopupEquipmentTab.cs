using System.Collections.Generic;
using UnityEngine;

public class InfoPopupEquipmentTab : UIPopup
{
    public EquipmentSlot slotPrefab;
    
    List<EquipmentSlot> slotPool = new List<EquipmentSlot>();

    private void Start()
    {
        slotPrefab.gameObject.SetActive(false);
    }

    public void SetPopup(List<Equipment> equipments)
    {
        for (int i = slotPool.Count; i < equipments.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemData = equipments[i];
            var itemSlot = slotPool[i];
            
            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(Resources.Load<Sprite>($"Sprites/Equipments/{itemData.code}"), itemData.upgradeLevel);
        }

        for (int i = equipments.Count; i < slotPool.Count; i++)
            slotPool[i].gameObject.SetActive(false);
    }
}
