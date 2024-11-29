using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDataManager : MonoBehaviour
{
    public int maxMonster = 5;
    public float respawnTime = 30f;

    Stat playerStat;
    List<Stat> monsters = new List<Stat>();

    List<Quest> progressQuests = new List<Quest>();
    List<Quest> completeQuests = new List<Quest>();
    Dictionary<int, DateTime> itemCoolEnds = new Dictionary<int, DateTime>();

    DateTime nextSpawnTime;

    private void Awake()
    {
        playerStat = new Stat() { maxHp = 20, attack = 10 };
        playerStat.ReSpawn();

        for (int i = 0; i < maxMonster; i++)
            monsters.Add(new Stat() { maxHp = 100, attack = 2 });
    }

    private void Update()
    {
        if (DateTime.Now > nextSpawnTime)
        {
            nextSpawnTime = DateTime.Now.AddSeconds(respawnTime);

            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                if (monster.nowHp <= 0)
                {
                    var newPos = new Vector3(Random.Range(15f, 50f), 1f, Random.Range(15f, 50f));
                    var newRotate = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    monster.ReSpawn();
                    Managers.MonsterManager.SpawnMonster(i, newPos, newRotate);
                }
            }
        }
    }

    public void Battle(int monsterId)
    {
        var monster = monsters[monsterId];

        if (monster.nowHp <= 0)
        {
            Debug.LogError($"Attack dead monster. id={monsterId}");
        }
        else
        {
            var monsterPosition = Managers.MonsterManager.GetMonsterPosition(monsterId);
            monster.ModifyNowHp(-playerStat.attack);

            if (monster.nowHp <= 0)
            {
                foreach (var quest in progressQuests)
                    quest.nowAmount++;

                Managers.MonsterManager.RemoveMonster(monsterId);
                Managers.effect.ShowEffect(1, monsterPosition);
                Managers.DropItem.SpawnItem(0, monsterPosition, 5);
                Managers.UIManager.GetLayout<StateLayout>().SetQuestList(progressQuests);
            }
            else
            {
                playerStat.ModifyNowHp(-monsters[monsterId].attack);
                Managers.UIManager.GetLayout<HudLayout>().SetHpBar(monsterId, monster.maxHp, monster.nowHp);
            }

            Managers.effect.ShowEffect(0, monsterPosition);
            Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerStat.maxHp, playerStat.nowHp);

            if (Random.Range(0f, 1f) < 0.2f)
                Managers.DropItem.SpawnItem(1, monsterPosition, 1);
        }
    }

    public void UseItem(int itemId)
    {
        if (!itemCoolEnds.ContainsKey(itemId))
            itemCoolEnds.Add(itemId, DateTime.MinValue);

        if (DateTime.Now > itemCoolEnds[itemId])
        {
            var itemCool = 5f;
            itemCoolEnds[itemId] = DateTime.Now.AddSeconds(itemCool);
            playerStat.ModifyNowHp(10);

            Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerStat.maxHp, playerStat.nowHp);
            Managers.UIManager.GetLayout<VirtualButtonLayout>().StartItemCoolTime(itemId, itemCool);
        }
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
        if (typeId == 1)
        {
            playerStat.ModifyNowHp(5);
            
            Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerStat.maxHp, playerStat.nowHp);
        }
    }
}

public class Stat
{
    public long maxHp;
    public long nowHp;
    public long attack;

    public void ReSpawn()
    {
        nowHp = maxHp;
    }

    public void ModifyNowHp(long hp)
    {
        nowHp += hp;
        if (nowHp > maxHp) nowHp = maxHp;
        else if (nowHp < 0) nowHp = 0;
    }
}

public class Quest
{
    public int id;
    public int targetAmount;
    public int nowAmount;
    public bool isComplete;
}
