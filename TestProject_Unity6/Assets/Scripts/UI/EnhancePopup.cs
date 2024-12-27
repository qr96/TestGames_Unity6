using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancePopup : UIPopup
{
    public EquipmentSlot equipmentSlot;
    public TMP_Text enhanceInfo;
    public KButton enhanceButton;
    public Button closeButton;

    private void Start()
    {
        //enhanceButton.onClick.AddListener();
        closeButton.onClick.AddListener(Hide);
    }

    public void SetPopup(Sprite equipIcon, int nowUpgrade, int maxUpgrade, long needMoney, float success, float destroy)
    {
        equipmentSlot.SetSlot(equipIcon, nowUpgrade);
        SetInfo(nowUpgrade, maxUpgrade, needMoney, success, destroy);
    }

    void SetInfo(int nowLevel, int maxLevel, long needMoney, float success, float destroy)
    {
        var info = "";
        
        if (nowLevel == maxLevel)
            info = "강화 완료";
        else
        {
            info += $"강화 레벨 : +{nowLevel} > +{nowLevel + 1}\n";
            info += $"소모 비용 : {needMoney}\n";
            info += $"성공 확률 : {success * 100}%\n";
            info += $"파괴 확률 : {destroy * 100}%";
        }

        enhanceInfo.text = info;
    }
}
