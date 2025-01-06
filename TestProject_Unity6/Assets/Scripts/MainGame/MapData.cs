using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapData : MonoBehaviour
{
    List<BattleUnit> monsters = new List<BattleUnit>();
    Dictionary<int, ItemData> spawnedItem = new Dictionary<int, ItemData>();
    Bag acquiredBag = new Bag(999);
    BattleUnit playerUnit;

    Coroutine reduceHpCo;
    int nowMapId = -1;
    int spawnedItemId;

    private void Start()
    {
        playerUnit = new BattleUnit(1, Managers.GameData.GetPlayerStat());
        
        StartCoroutine(ChargeMpCo(1));
    }

    public void EnterMap(int mapId)
    {
        RemoveAllMonsters();

        if (reduceHpCo != null)
            StopCoroutine(reduceHpCo);

        if (TableData.IsTown(mapId))
        {
            if (!TableData.IsTown(nowMapId))
            {
                foreach (var itemData in acquiredBag.ToList())
                    Managers.GameData.AddMiscItemToBag(itemData.itemCode, itemData.count);
                Managers.UIManager.ShowPopup<ExploreResultPopup>().SetItems(acquiredBag.ToList());
                acquiredBag.Clear();
            }
        }
        else
        {
            SpawnMonsters();
            reduceHpCo = StartCoroutine(ReduceHpCo(1f));
        }

        nowMapId = mapId;
        UpdatePlayerStat();
    }

    public void SpawnMonsters()
    {
        for (int i = 0; i < 5; i++)
            monsters.Add(new BattleUnit(i, new Stat() { hp = 50, attack = 2 }));

        foreach (var monster in monsters)
        {
            if (monster.IsDead())
            {
                var newPos = new Vector3(Random.Range(0f, 20f), 1f, Random.Range(-10f, 10f));
                var newRotate = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                monster.Respawn();
                Managers.MonsterManager.SpawnMonster(monster.id, newPos, newRotate);
            }
        }
    }

    public void RemoveAllMonsters()
    {
        monsters.Clear();
        Managers.MonsterManager.RemoveAllMonsters();
    }

    public void Battle(int monsterId)
    {
        var monster = monsters[monsterId];

        if (monster.IsDead())
        {
            Debug.LogError($"Attack dead monster. id={monsterId}");
        }
        else
        {
            if (Managers.MonsterManager.TryGetMonsterPosition(monsterId, out var monsterPosition))
            {
                var playerAttackSkills = GetPlayerAttackInfos();

                foreach (var attackSkill in playerAttackSkills)
                {
                    var playerAttackDamage = attackSkill.Item2;
                    monster.TakeDamage(playerAttackDamage);

                    if (attackSkill.Item1 == 0)
                        Managers.effect.ShowEffect(6, monsterPosition);
                    else if (attackSkill.Item1 == 1)
                        Managers.effect.ShowEffect(7, monsterPosition);
                    else if (attackSkill.Item1 == 2)
                        Managers.effect.ShowEffect(8, monsterPosition);

                    Managers.UIManager.GetLayout<HudLayout>().ShowDamage(attackSkill.Item2, monsterPosition + Vector3.up * 1f);
                }

                Managers.UIManager.GetLayout<HudLayout>().SetHpBar(monsterId, monster.MaxStat.hp, monster.NowStat.hp);

                if (monster.IsDead())
                {
                    Managers.GameData.ModifyPlayerExp(10);

                    Managers.MonsterManager.KillMonster(monsterId);
                    Managers.effect.ShowEffect(1, monsterPosition);
                    SpawnItem(3, 5, monsterPosition);
                    SpawnItems(1, 100, monsterPosition, 3);
                }
                else
                {
                    playerUnit.TakeDamage(monster.GetAttack());
                    Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerUnit.MaxStat.hp, playerUnit.NowStat.hp);
                    Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerUnit.MaxStat.mp, playerUnit.NowStat.mp);
                }
            }
        }
    }

    public void PickupItem(int itemId)
    {
        if (spawnedItem.ContainsKey(itemId))
        {
            var itemData = spawnedItem[itemId];
            acquiredBag.AddItem(itemData.itemCode, itemData.count);
            spawnedItem.Remove(itemId);
        }
        else
        {
            Debug.Log($"PickupItem() Failed to acquire item. itemId={itemId}");
        }
    }

    public void UpdatePlayerStat()
    {
        if (TableData.IsTown(nowMapId))
        {
            playerUnit.SetStat(Managers.GameData.GetPlayerStat());
            playerUnit.Respawn();
            Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerUnit.MaxStat.hp, playerUnit.NowStat.hp);
            Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerUnit.MaxStat.mp, playerUnit.NowStat.mp);
            Managers.MonsterManager.player.speed = playerUnit.NowStat.speed;
        }
    }

    void SpawnItem(int itemCode, int count, Vector3 position)
    {
        var itemId = spawnedItemId++;
        spawnedItem.Add(itemId, new ItemData(itemCode, count));
        Managers.DropItem.SpawnItem(itemId, itemCode == 1 ? 1 : 4, position);
    }

    void SpawnItems(int itemCode, int count, Vector3 position, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnItem(itemCode, count, position);
    }

    IEnumerator ReduceHpCo(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            playerUnit.ReduceHp(1);
            Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerUnit.MaxStat.hp, playerUnit.NowStat.hp);
        }
    }

    IEnumerator ChargeMpCo(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            playerUnit.HealMp(1);
            Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerUnit.MaxStat.mp, playerUnit.NowStat.mp);
        }
    }

    List<Tuple<int, long>> GetPlayerAttackInfos()
    {
        var skillPossibility = new int[] { 1000, 200, 100 };
        var usingSkills = new List<Tuple<int, long>>();

        for (int i = 0; i < skillPossibility.Length; i++)
        {
            if (Random.Range(0, 1000) < skillPossibility[i])
                usingSkills.Add(new Tuple<int, long>(i, TableData.GetSkillDamage(i, 1, playerUnit.GetAttack())));
        }

        return usingSkills;
    }
}
