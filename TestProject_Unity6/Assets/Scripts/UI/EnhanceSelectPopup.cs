using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceSelectPopup : UIPopup
{
    public EquipmentSlot slotPrefab;
    public KButton selectButton;
    public Button closeButton;

    List<EquipmentSlot> slotPool = new List<EquipmentSlot>();
    int selectedId = 0;

    private void Start()
    {
        slotPrefab.gameObject.SetActive(false);
        selectButton.onClick.AddListener(() => OnClickEnhance(selectedId));
        closeButton.onClick.AddListener(Hide);
    }

    public void SetPopup(List<Equipment> equipments, int selectedId = -1)
    {
        for (int i = slotPool.Count; i < equipments.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemData = equipments[i];
            var itemSlot = slotPool[i];
            var toggle = itemSlot.GetComponent<KToggle>();

            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(TableData.GetEquipmentSprite(itemData.code, itemData.part), itemData.upgradeLevel);

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => OnSelected(isOn, itemData.id));

            if (selectedId < 0)
            {
                if (i == 0)
                    toggle.SetToggle(true);
            }
            else
            {
                if (itemData.id == selectedId)
                    toggle.SetToggle(true);
            }
        }

        for (int i = equipments.Count; i < slotPool.Count; i++)
            slotPool[i].gameObject.SetActive(false);
    }

    void OnClickEnhance(int equipmentId)
    {
        Managers.UIManager.ShowPopup<EnhancePopup>().SetPopup(equipmentId, Managers.GameData.GetPlayerMoney());
    }

    void OnSelected(bool isOn, int equipmentId)
    {
        if (isOn)
            selectedId = equipmentId;
    }
}
