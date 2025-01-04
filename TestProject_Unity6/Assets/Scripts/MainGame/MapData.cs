using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapData : MonoBehaviour
{
    public int maxMonster = 5;
    public float respawnTime = 30f;

    List<Monster> monsters = new List<Monster>();
    Dictionary<int, ItemData> spawnedItem = new Dictionary<int, ItemData>();
    Bag acquiredBag = new Bag(999);

    DateTime nextSpawnTime;
    Coroutine reduceHpCo;
    int nowMapId = -1;
    int spawnedItemId;

    private void Update()
    {
        if (DateTime.Now > nextSpawnTime)
        {
            nextSpawnTime = DateTime.Now.AddSeconds(respawnTime);

            foreach (var monster in monsters)
            {
                if (monster.IsDead())
                {
                    var newPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
                    var newRotate = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    monster.ReSpawn();
                    Managers.MonsterManager.SpawnMonster(monster.id, newPos, newRotate);
                }
            }
        }
    }

    public void EnterMap(int mapId)
    {
        monsters.Clear();
        Managers.MonsterManager.RemoveAllMonsters();

        if (reduceHpCo != null)
            StopCoroutine(reduceHpCo);

        if (mapId == 0 && nowMapId == 1)
        {
            Managers.GameData.MakePlayerHpFull();

            if (nowMapId == 1)
            {
                foreach (var itemData in acquiredBag.ToList())
                    Managers.GameData.AddMiscItemToBag(itemData.itemCode, itemData.count);
                Managers.UIManager.ShowPopup<ExploreResultPopup>().SetItems(acquiredBag.ToList());
                acquiredBag.Clear();
            }
        }
        else if (mapId == 1)
        {
            for (int i = 0; i < maxMonster; i++)
                monsters.Add(new Monster(i, 50, 2));

            nextSpawnTime = DateTime.Now;    
            reduceHpCo = StartCoroutine(ReduceHpCo(1f));
        }

        nowMapId = mapId;
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
                var playerAttackDamage = Managers.GameData.GetPlayerDamage();
                var playerAttackSkills = Managers.GameData.UseAttackSkill();

                Managers.effect.ShowEffect(6, monsterPosition);
                Managers.UIManager.GetLayout<HudLayout>().ShowDamage(playerAttackDamage, monsterPosition + Vector3.up * 1f);
                
                foreach (var attackSkill in playerAttackSkills)
                {
                    playerAttackDamage += attackSkill.Item2;

                    if (attackSkill.Item1 == 0)
                        Managers.effect.ShowEffect(7, monsterPosition);
                    else if (attackSkill.Item1 == 1)
                        Managers.effect.ShowEffect(8, monsterPosition);
                    
                    Managers.UIManager.GetLayout<HudLayout>().ShowDamage(attackSkill.Item2, monsterPosition + Vector3.up * 1f);
                }

                monster.TakeDamage(playerAttackDamage);
                Managers.UIManager.GetLayout<HudLayout>().SetHpBar(monsterId, monster.stat.maxHp, monster.stat.nowHp);

                if (monster.IsDead())
                {
                    Managers.GameData.ModifyPlayerExp(10);
                    Managers.GameData.UseBuffSkill(0);

                    Managers.MonsterManager.KillMonster(monsterId);
                    Managers.effect.ShowEffect(1, monsterPosition);
                    SpawnItem(2, 5, monsterPosition);
                    SpawnItems(1, 100, monsterPosition, 3);
                }
                else
                {
                    Managers.GameData.DamagePlayer(monsters[monsterId].GetDamge());
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

    void SpawnItem(int itemCode, int count, Vector3 position)
    {
        var itemId = spawnedItemId++;
        spawnedItem.Add(itemId, new ItemData(itemCode, count));
        Managers.DropItem.SpawnItem(itemId, itemCode, position);
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
            Managers.GameData.ModifyPlayerHp(-1);
        }
    }
}
