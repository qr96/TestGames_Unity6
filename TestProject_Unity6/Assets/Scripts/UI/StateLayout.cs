using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateLayout : UILayout
{
    public GuageBar userHpBar;
    public TMP_Text questPrefab;

    List<TMP_Text> questPool = new List<TMP_Text>();

    private void Start()
    {
        questPrefab.gameObject.SetActive(false);
    }

    public void SetUserHpBar(long maxHp, long nowHp)
    {
        userHpBar.SetGuage(maxHp, nowHp);
    }

    public void SetQuestList(List<Quest> quests)
    {
        for (int i = 0; i < quests.Count; i++)
        {
            var nowQuest = quests[i];

            if (i >= questPool.Count)
                questPool.Add(Instantiate(questPrefab, questPrefab.transform.parent));

            questPool[i].text = $"[몬스터] 사냥 {nowQuest.nowAmount}/{nowQuest.targetAmount}";
            questPool[i].gameObject.SetActive(true);
        }

        for (int i = quests.Count; i < questPool.Count; i++)
        {
            questPool[i].gameObject.SetActive(false);
        }
    }
}
