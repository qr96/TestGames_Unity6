using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPopup : UIPopup
{
    enum Tab
    {
        Bag,
    }

    public TMP_Text titleText;
    public InfoPopupBagTab bagTab;
    public KButton exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(() => Hide());

        ChangeTab(Tab.Bag);
    }

    public void SetBagTab(List<ItemData> items)
    {
        bagTab.SetTab(items);
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
        }

        bagTab.gameObject.SetActive(tab == Tab.Bag);
    }
}
