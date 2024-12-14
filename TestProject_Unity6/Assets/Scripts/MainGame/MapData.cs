using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapData : MonoBehaviour
{
    public int maxMonster = 5;
    public float respawnTime = 30f;

    List<Stat> monsters = new List<Stat>();
    List<int> acquiredItems = new List<int>();

    DateTime nextSpawnTime;
    Coroutine reduceHpCo;
    long acquiredMoney;
    int nowMapId = -1;

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
                    var newPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
                    var newRotate = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    monster.ReSpawn();
                    Managers.MonsterManager.SpawnMonster(i, newPos, newRotate);
                }
            }
        }
    }

    public void EnterMap(int mapId)
    {
        Debug.Log($"{mapId}, {nowMapId}");

        monsters.Clear();
        Managers.MonsterManager.RemoveAllMonsters();

        if (reduceHpCo != null)
            StopCoroutine(reduceHpCo);

        if (mapId == 0 && nowMapId == 1)
        {
            Managers.UIManager.GetPopup<ExploreResultPopup>().SetItems(acquiredItems);
            Managers.UIManager.ShowPopup<ExploreResultPopup>();

            acquiredItems.Clear();
            acquiredMoney = 0;
        }
        else if (mapId == 1)
        {
            for (int i = 0; i < maxMonster; i++)
                monsters.Add(new Stat() { maxHp = 50, attack = 2 });

            nextSpawnTime = DateTime.Now;    
            reduceHpCo = StartCoroutine(ReduceHpCo(1f));
        }

        nowMapId = mapId;
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
            if (Managers.MonsterManager.TryGetMonsterPosition(monsterId, out var monsterPosition))
            {
                var playerAttackDamage = Managers.GameData.GetPlayerDamage();

                monster.ModifyNowHp(-playerAttackDamage);
                SpawnHpPotion(monsterPosition);

                Managers.effect.ShowEffect(0, monsterPosition);
                Managers.UIManager.GetLayout<HudLayout>().ShowDamage(playerAttackDamage, monsterPosition + Vector3.up * 1f);
                Managers.UIManager.GetLayout<HudLayout>().SetHpBar(monsterId, monster.maxHp, monster.nowHp);

                if (monster.nowHp <= 0)
                {
                    Managers.GameData.ModifyPlayerExp(10);
                    Managers.GameData.UseBuffSkill(0);

                    Managers.MonsterManager.KillMonster(monsterId);
                    Managers.effect.ShowEffect(1, monsterPosition);
                    Managers.DropItem.SpawnItem(0, monsterPosition, 5);
                    Managers.DropItem.SpawnItem(2, monsterPosition, 1);
                }
                else
                {
                    Managers.GameData.ModifyPlayerMp(-monsters[monsterId].attack);
                }
            }
        }
    }

    public void PickupItem(int typeId)
    {
        if (typeId == 0)
            acquiredMoney += 10;
        else if (typeId != 1)
            acquiredItems.Add(typeId);
    }

    void SpawnHpPotion(Vector3 monsterPosition)
    {
        if (Random.Range(0f, 1f) < 0.2f)
            Managers.DropItem.SpawnItem(1, monsterPosition, 1);
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
