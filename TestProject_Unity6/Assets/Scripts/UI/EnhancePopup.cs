using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancePopup : UIPopup
{
    public EquipmentSlot equipmentSlot;
    public TMP_Text enhanceInfo;
    public TMP_Text money;
    public KButton enhanceButton;
    public Button closeButton;

    public RectTransform successEffect;
    public RectTransform failureEffect;

    int nowEquipmentId;

    private void Start()
    {
        enhanceButton.onClick.AddListener(() => Managers.GameData.EnhanceEquipment(nowEquipmentId));
        closeButton.onClick.AddListener(Hide);
    }

    public void SetPopup(int nowEquipmentId, long playerMoney)
    {
        this.nowEquipmentId = nowEquipmentId;
        SetPopup(Managers.GameData.GetPlayerEquipment(nowEquipmentId), playerMoney);
        enhanceButton.enabled = true;
    }

    public void SetDestroyPopup(long playerMoney)
    {
        equipmentSlot.SetSlot(Resources.Load<Sprite>($"Sprites/Etc/1"), 0);
        enhanceInfo.text = "장비 파괴됨";
        money.text = playerMoney.ToFormat();
        enhanceButton.enabled = false;
    }

    public void ShowEffect(bool success)
    {
        if (success)
        {
            var successEffectSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                enhanceButton.enabled = false;
                successEffect.localScale = Vector3.one * 0.5f;
                successEffect.gameObject.SetActive(true);
            })
            .Append(successEffect.DOScale(Vector3.one * 0.75f, 0.1f).OnComplete(() => enhanceButton.enabled = true))
            .Append(successEffect.DOScale(Vector3.one, 0.1f))
            .OnComplete(() => successEffect.gameObject.SetActive(false));
        }
        else
        {
            var failureEffectSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                enhanceButton.enabled = false;
                failureEffect.localScale = Vector3.one * 0.5f;
                failureEffect.gameObject.SetActive(true);
            })
            .Append(failureEffect.DOScale(Vector3.one * 0.75f, 0.1f))
            .Append(failureEffect.DOScale(Vector3.one, 0.1f))
            .OnComplete(() => failureEffect.gameObject.SetActive(false));
        }
    }

    void SetPopup(Equipment equipment, long playerMoney)
    {
        var maxEnhance = TableData.GetEquipmentMaxEnhance(equipment.code);
        var needMoney = TableData.GetEquipmentEnhancePrice(equipment.code);
        var success = TableData.GetEquipmentEnhancePossibilty(equipment.code, equipment.upgradeLevel);

        SetPopup(TableData.GetEquipmentSprite(equipment.code, equipment.part), equipment.upgradeLevel, maxEnhance, needMoney, success, playerMoney);
    }

    void SetPopup(Sprite equipIcon, int nowUpgrade, int maxUpgrade, long needMoney, float success, long playerMoney)
    {
        equipmentSlot.SetSlot(equipIcon, nowUpgrade);
        SetInfo(nowUpgrade, maxUpgrade, needMoney, success, playerMoney);
    }

    void SetInfo(int nowLevel, int maxLevel, long needMoney, float success, long playerMoney)
    {
        var info = "";
        
        if (nowLevel == maxLevel)
            info = "강화 완료";
        else
        {
            info += $"강화 레벨 : +{nowLevel} > +{nowLevel + 1}\n";
            info += $"소모 비용 : {needMoney}\n";
            info += $"성공 확률 : {success * 100}%";
        }

        enhanceInfo.text = info;
        money.text = playerMoney.ToFormat();
    }
}
