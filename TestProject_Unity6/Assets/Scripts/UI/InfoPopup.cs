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
        Skill
    }

    public TMP_Text titleText;
    public InfoPopupBagTab bagTab;
    public InfoPopupEquipmentTab equipTab;
    public InfoPopupSkillTab skillTab;

    public KButton closeButton;
    public KButton bagButton;
    public KButton equipButton;
    public KButton skillButton;

    private void Start()
    {
        closeButton.onClick.AddListener(() => Hide());
        bagButton.onClick.AddListener(() => ChangeTab(Tab.Bag));
        equipButton.onClick.AddListener(() => ChangeTab(Tab.Equipment));
        skillButton.onClick.AddListener(() => ChangeTab(Tab.Skill));

        ChangeTab(Tab.Equipment);
    }

    public void SetBagTab(List<ItemData> items)
    {
        bagTab.SetTab(items);
    }

    public void SetEquipTab(List<Equipment> equipments, List<Equipment> equipped, Stat stat)
    {
        equipTab.SetPopup(equipments, equipped, stat);
    }

    public void SetSkillTab(List<SkillData> skills, List<SkillData> equipped)
    {
        skillTab.SetTab(skills, equipped);
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
            case Tab.Skill:
                titleText.text = "무공";
                break;
        }

        bagTab.gameObject.SetActive(tab == Tab.Bag);
        equipTab.gameObject.SetActive(tab == Tab.Equipment);
        skillTab.gameObject.SetActive(tab == Tab.Skill);
    }
}
