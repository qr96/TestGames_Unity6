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

        SetPopup(equipment.code, equipment.part, equipment.upgradeLevel, maxEnhance, needMoney, success, playerMoney);
    }

    void SetPopup(int code, Equipment.Part part, int nowUpgrade, int maxUpgrade, long needMoney, float success, long playerMoney)
    {
        var equipIcon = TableData.GetEquipmentSprite(code, part);
        equipmentSlot.SetSlot(equipIcon, nowUpgrade);
        SetInfo(code, part, nowUpgrade, maxUpgrade, needMoney, success, playerMoney);
    }

    void SetInfo(int code, Equipment.Part part, int nowUpgrade, int maxUpgrade, long needMoney, float success, long playerMoney)
    {
        var info = "";
        var enhanceStat = Managers.TableData.GetEquipmentEnhanceIncrease(part, nowUpgrade);
        var nowPrice = Managers.TableData.GetEquipmentPrice(part, code, nowUpgrade);
        var nextPrice = Managers.TableData.GetEquipmentPrice(part, code, nowUpgrade + 1);

        if (nowUpgrade == maxUpgrade)
            info = "강화 완료";
        else
        {
            info += "[강화 정보]\n";
            info += $"강화 레벨 : +{nowUpgrade} > +{nowUpgrade + 1}\n";
            info += $"소모 비용 : {needMoney.ToFormat()}\n";
            info += $"성공 확률 : {success * 100}%\n";
            info += $"현재 가격 : {nowPrice.ToFormat()}\n";
            info += $"다음 가격 : {nextPrice.ToFormat()}\n";
            info += "\n";
            info += "[강화 능력치]\n";
            info += enhanceStat.attack > 0 ? $"공격력 : +{enhanceStat.attack}\n" : "";
            info += enhanceStat.hp > 0 ? $"체력 : +{enhanceStat.hp}\n" : "";
            info += enhanceStat.mp > 0 ? $"기력 : +{enhanceStat.mp}\n" : "";
            info += enhanceStat.speed > 0 ? $"이동속도 : +{enhanceStat.speed * 10}\n" : "";
            info += enhanceStat.mastery > 0 ? $"무기숙련 : +{enhanceStat.mastery * 100}\n" : "";
            info += "\n";
        }

        enhanceInfo.text = info;
        money.text = playerMoney.ToFormat();
    }
}
