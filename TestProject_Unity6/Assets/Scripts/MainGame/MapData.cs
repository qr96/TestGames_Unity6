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

    DateTime nextSpawnTime;

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

    public void EnterMap(int mapId)
    {
        Managers.MonsterManager.RemoveAllMonsters();

        if (mapId == 1)
        {
            for (int i = 0; i < maxMonster; i++)
                monsters.Add(new Stat() { maxHp = 50, attack = 2 });

            nextSpawnTime = DateTime.Now;
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
                }
                else
                {
                    Managers.GameData.ModifyPlayerMp(-monsters[monsterId].attack);
                }
            }
        }
    }

    void SpawnHpPotion(Vector3 monsterPosition)
    {
        if (Random.Range(0f, 1f) < 0.2f)
            Managers.DropItem.SpawnItem(1, monsterPosition, 1);
    }
}
