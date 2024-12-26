using TMPro;
using UnityEngine;

public class EnhancePopup : UIPopup
{
    public EquipmentSlot equipmentSlot;
    public TMP_Text enhanceInfo;
    public KButton enhanceButton;
    public KButton closeButton;

    int now = 0;
    int max = 20;

    private void Start()
    {
        enhanceButton.onClick.AddListener(Enhance);
        closeButton.onClick.AddListener(Hide);

        SetInfo(now, max, now * 10000, 0.8f, 0f);
    }

    void Enhance()
    {
        var rand = Random.Range(0f, 1f);
        var destroy = 0.01f;
        var success = 0.8f;

        if (rand < destroy)
        {
            now = 0;
        }
        else if (rand < destroy + success)
        {
            now++;
        }

        SetInfo(now, max, now * 10000, 0.1f, destroy);
        equipmentSlot.SetUpgrade(now);
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
