using System.Collections.Generic;
using UnityEngine;

public class InfoPopupSkillTab : MonoBehaviour
{
    public SkillSlot havePrefab;
    public SkillSlot equippedPrefab;

    List<SkillSlot> havePool = new List<SkillSlot>();
    List<SkillSlot> equippedPool = new List<SkillSlot>();

    private void Start()
    {
        havePrefab.gameObject.SetActive(false);
        equippedPrefab.gameObject.SetActive(false);
    }

    public void SetTab(List<SkillData> have, List<SkillData> equipped)
    {
        SetHave(have);
        SetEquipped(equipped);
    }

    void SetHave(List<SkillData> skills)
    {
        foreach (var slot in havePool)
            slot.gameObject.SetActive(false);

        for (int i = havePool.Count; i < skills.Count; i++)
            havePool.Add(Instantiate(havePrefab, havePrefab.transform.parent));

        for (int i = 0; i < skills.Count; i++)
        {
            var slot = havePool[i];
            var skillData = skills[i];

            slot.SetSlot(TableData.GetSkillImage(skillData.code), skillData.level, Managers.TableData.GetSkillMaxLevel(skillData.code));
            slot.gameObject.SetActive(true);
        }

        for (int i = skills.Count; i < havePool.Count; i++)
            havePool[i].gameObject.SetActive(false);
    }

    void SetEquipped(List<SkillData> skills)
    {
        foreach (var slot in equippedPool)
            slot.gameObject.SetActive(false);

        for (int i = equippedPool.Count; i < skills.Count; i++)
            equippedPool.Add(Instantiate(equippedPrefab, equippedPrefab.transform.parent));

        for (int i = 0; i < skills.Count; i++)
        {
            var slot = equippedPool[i];
            var skillData = skills[i];

            slot.SetSlot(TableData.GetSkillImage(skillData.code), skillData.level, Managers.TableData.GetSkillMaxLevel(skillData.code));
            slot.gameObject.SetActive(true);
        }
    }
}
