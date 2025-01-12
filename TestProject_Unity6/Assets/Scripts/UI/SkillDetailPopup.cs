using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDetailPopup : UIPopup
{
    public SkillSlot skillSlot;
    public KButton equipButton;
    public TMP_Text equipButtonText;
    public Button closeButton;

    public TMP_Text skillName;
    public TMP_Text skillLevel;
    public TMP_Text skillDescription;

    bool isEquipped;
    int skillCode;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        equipButton.onClick.AddListener(OnEquipButton);
    }

    public void SetPopup(int skillCode, bool isEquipped, int skillLevel)
    {
        this.isEquipped = isEquipped;
        this.skillCode = skillCode;

        var attackCount = Managers.TableData.GetSkillAttackCount(skillCode);
        var skillDamage = Managers.TableData.GetSkillDamage(skillCode, skillLevel, 100);
        var proChance = Managers.TableData.GetSkillProcChance(skillCode);
        var skillName = Managers.TableData.GetSkillName(skillCode);
        var maxLevel = Managers.TableData.GetSkillMaxLevel(skillCode);

        this.skillName.text = skillName;
        this.skillLevel.text = $"Lv.{skillLevel} (max:{maxLevel})";

        equipButtonText.text = isEquipped ? "장착 해제" : "장착";
        skillSlot.SetSlot(TableData.GetSkillSprite(skillCode), skillLevel, 0);
        skillDescription.text = $"{skillDamage}%의 데미지로 {attackCount}회 공격.\n";
        skillDescription.text += $"공격 시 {proChance}% 확률로 발동.";
    }

    void OnEquipButton()
    {
        if (isEquipped)
            Managers.GameData.UnEquipSkill(skillCode);
        else
            Managers.GameData.EquipSkill(skillCode);

        Hide();
    }
}
