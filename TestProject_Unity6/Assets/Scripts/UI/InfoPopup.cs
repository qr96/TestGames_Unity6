using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : UIPopup
{
    enum Tab
    {
        Equipment,
        Bag,
    }

    public TMP_Text titleText;
    public InfoPopupBagTab bagTab;
    public InfoPopupEquipmentTab equipTab;
    
    public KButton closeButton;
    public KButton bagButton;
    public KButton equipButton;

    private void Start()
    {
        closeButton.onClick.AddListener(() => Hide());
        bagButton.onClick.AddListener(() => ChangeTab(Tab.Bag));
        equipButton.onClick.AddListener(() => ChangeTab(Tab.Equipment));

        ChangeTab(Tab.Bag);
    }

    public void SetBagTab(List<ItemData> items)
    {
        bagTab.SetTab(items);
    }

    public void SetEquipTab(List<Equipment> equipments)
    {
        equipTab.SetPopup(equipments);
    }

    public void SetMoney(long money)
    {
        bagTab.SetMoney(money);
    }

    public void SetBagWeight(long max, long now)
    {
        bagTab.SetWeightGuage(max, now);
    }

    void ChangeTab(Tab tab)
    {
        switch (tab)
        {
            case Tab.Bag:
                titleText.text = "인벤토리";
                break;
            case Tab.Equipment:
                titleText.text = "장비";
                break;
        }

        bagTab.gameObject.SetActive(tab == Tab.Bag);
        equipTab.gameObject.SetActive(tab == Tab.Equipment);
    }
}
