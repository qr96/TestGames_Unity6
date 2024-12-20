using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDataManager : MonoBehaviour
{
    public MapData mapData;

    playerInfo playerInfo;

    List<Quest> progressQuests = new List<Quest>();
    List<Quest> completeQuests = new List<Quest>();
    Dictionary<int, DateTime> itemCoolEnds = new Dictionary<int, DateTime>();
    Dictionary<int, DateTime> buffEndTimes = new Dictionary<int, DateTime>();
    Dictionary<int, bool> buffActives = new Dictionary<int, bool>();

    private void Awake()
    {
        playerInfo = new playerInfo() { level = 1, nowExp = 0, stat = TableData.GetStatPerLevel(1) };
    }

    private void Start()
    {
        MakePlayerHpFull();
        ModifyPlayerExp(0);
        StartCoroutine(ChargeMpCo(1f));
    }

    private void Update()
    {
        foreach (var buff in buffEndTimes)
        {
            var skillId = buff.Key;

            if (buffActives[skillId] == true)
            {
                if (buffEndTimes[skillId] < DateTime.Now)
                {
                    buffActives[skillId] = false;
                    Managers.MonsterManager.player.speed -= TableData.GetSkillBuffStat(skillId, 1).speed;
                }
            }
        }
    }

    public void ModifyPlayerHp(long hp)
    {
        playerInfo.stat.ModifyNowHp(hp);
        Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerInfo.stat.maxHp, playerInfo.stat.nowHp);
    }

    public void ModifyPlayerMp(long mp)
    {
        playerInfo.stat.ModifyNowMp(mp);
        Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerInfo.stat.maxMp, playerInfo.stat.nowMp);
    }

    public void ModifyPlayerExp(long exp)
    {
        playerInfo.ModifyExp(exp, () => Managers.effect.ShowEffect(4, Managers.MonsterManager.player.transform.position, Managers.MonsterManager.player.transform));
        Managers.UIManager.GetLayout<StateLayout>().SetUserExpBar(TableData.GetMaxExp(playerInfo.level), playerInfo.nowExp);
        Managers.UIManager.GetLayout<StateLayout>().SetLevel(playerInfo.level);
    }

    public void MakePlayerHpFull()
    {
        var needHp = playerInfo.stat.maxHp - playerInfo.stat.nowHp;
        ModifyPlayerHp(needHp);
    }

    public long GetPlayerDamage()
    {
        return GetDamage(playerInfo.stat);
    }

    public void Battle(int monsterId)
    {
        mapData.Battle(monsterId);
    }

    public void EnterMap(int mapId)
    {
        Managers.UIManager.GetLayout<HudLayout>().ClearLayout();
        mapData.EnterMap(mapId);
    }

    public void UseItem(int itemId)
    {
        var itemCool = 5f;

        if (!itemCoolEnds.ContainsKey(itemId))
            itemCoolEnds.Add(itemId, DateTime.MinValue);

        if (DateTime.Now > itemCoolEnds[itemId])
        {
            itemCoolEnds[itemId] = DateTime.Now.AddSeconds(itemCool);
            ModifyPlayerHp(10);
            Managers.UIManager.GetLayout<VirtualButtonLayout>().StartItemCoolTime(itemId, itemCool);
        }
    }

    public void UseBuffSkill(int skillId)
    {
        var buffTime = TableData.GetSkillBuffTime(skillId, 1);
        var buffStat = TableData.GetSkillBuffStat(skillId, 1);

        if (!buffEndTimes.ContainsKey(skillId))
            buffEndTimes.Add(skillId, DateTime.MinValue);

        if (!buffActives.ContainsKey(skillId))
            buffActives.Add(skillId, false);

        buffEndTimes[skillId] = DateTime.Now.AddSeconds(buffTime);
        buffActives[skillId] = true;

        Managers.MonsterManager.player.speed = playerInfo.stat.speed + buffStat.speed;
        Managers.UIManager.GetLayout<StateLayout>().SetSkillDutaion(skillId, buffTime);
    }

    public void StartQuest(int questId)
    {
        if (completeQuests.Any((quest) => quest.id == questId))
        {
            Debug.LogError($"Quest is completed already. id = {questId}");
        }
        else if (progressQuests.Any((quest) => quest.id == questId))
        {
            Debug.LogError($"Quest is progressed already. id = {questId}");
        }
        else
        {
            progressQuests.Add(new Quest() { id = questId, targetAmount = 10 });
            Managers.UIManager.GetLayout<StateLayout>().SetQuestList(progressQuests);
        }
    }

    public void CompleteQuest(int questId)
    {
        var questIndex = progressQuests.FindIndex((quest) => quest.id == questId);
        if (questIndex > -1)
        {
            var quest = progressQuests[questIndex];
            if (quest.nowAmount >= quest.targetAmount)
            {
                progressQuests.RemoveAt(questIndex);
                completeQuests.Add(quest);
                Managers.UIManager.GetLayout<StateLayout>().SetQuestList(progressQuests);
            }
        }
        else
        {
            Debug.LogError($"Quest is not progressing. id = {questId}");
        }
    }

    public void PickupItem(int typeId)
    {
        mapData.PickupItem(typeId);

        //if (typeId == 1)
        //{
        //    playerStat.ModifyNowHp(5);
            
        //    Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerStat.maxHp, playerStat.nowHp);
        //}
    }

    public long GetDamage(Stat stat)
    {
        var rand = Random.Range(stat.mastery, 1f);
        return (long)(rand * stat.attack);
    }

    IEnumerator ChargeMpCo(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            ModifyPlayerMp(1);
        }
    }
}

public class Stat
{
    public long maxHp;
    public long nowHp;
    public long maxMp;
    public long nowMp;
    public long attack;
    public float speed;
    public float mastery;

    public void ReSpawn()
    {
        nowHp = maxHp;
        nowMp = maxMp;
    }

    public void ModifyNowHp(long hp)
    {
        nowHp += hp;
        if (nowHp > maxHp) nowHp = maxHp;
        else if (nowHp < 0) nowHp = 0;
    }

    public void ModifyNowMp(long mp)
    {
        nowMp += mp;
        if (nowMp > maxMp) nowMp = maxMp;
        else if (nowMp < 0) nowMp = 0;
    }
}

public class playerInfo
{
    public int level;
    public long nowExp;
    public Stat stat;

    public void ModifyExp(long exp, Action onLevelUp)
    {
        nowExp += exp;

        if (nowExp < 0)
            nowExp = 0;

        while (nowExp >= TableData.GetMaxExp(level))
            LevelUp(onLevelUp);
    }

    void LevelUp(Action onLevelUp)
    {
        nowExp -= TableData.GetMaxExp(level);
        level++;
        stat = TableData.GetStatPerLevel(level);
        onLevelUp?.Invoke();
    }
}

public class Quest
{
    public int id;
    public int targetAmount;
    public int nowAmount;
    public bool isComplete;
}
