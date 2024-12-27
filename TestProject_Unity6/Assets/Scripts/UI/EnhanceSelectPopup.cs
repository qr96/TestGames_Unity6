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

    public void SetPopup(List<Equipment> equipments)
    {
        foreach (var slot in slotPool)
            slot.gameObject.SetActive(false);

        for (int i = slotPool.Count; i < equipments.Count; i++)
            slotPool.Add(Instantiate(slotPrefab, slotPrefab.transform.parent));

        for (int i = 0; i < equipments.Count; i++)
        {
            var itemData = equipments[i];
            var itemSlot = slotPool[i];
            var toggle = itemSlot.GetComponent<KToggle>();

            itemSlot.gameObject.SetActive(true);
            itemSlot.SetSlot(Resources.Load<Sprite>($"Sprites/Equipments/{itemData.code}"), itemData.upgradeLevel);

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => OnSelected(isOn, itemData.id));
        }
    }

    void OnClickEnhance(int equipmentId)
    {
        var equipment = Managers.GameData.GetPlayerEquipment(equipmentId);
        Managers.UIManager.ShowPopup<EnhancePopup>().SetPopup(
            Resources.Load<Sprite>($"Sprites/Equipments/{equipment.code}"), equipment.upgradeLevel, 20, 0, 0.5f, 0.01f);
    }

    void OnSelected(bool isOn, int equipmentId)
    {
        if (isOn)
            selectedId = equipmentId;
    }
}
