using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public PlayerController player;
    public BaseMonster monsterPrefab;

    Dictionary<int, Stack<BaseMonster>> monsterPoolDic = new Dictionary<int, Stack<BaseMonster>>();
    List<BaseMonster> spawnedMonsters = new List<BaseMonster>();
    Dictionary<int, BaseMonster> monsterDic = new Dictionary<int, BaseMonster>();

    public List<BaseMonster> GetSpawnedMonsterList()
    {
        return spawnedMonsters;
    }

    public void SpawnMonster(int id, int code, Vector3 position, Quaternion rotation)
    {
        if (!monsterDic.ContainsKey(id))
        {
            if (!monsterPoolDic.ContainsKey(code))
                monsterPoolDic.Add(code, new Stack<BaseMonster>());

            var monsterPool = monsterPoolDic[code];
            var monster = monsterPool.Count > 0 ? monsterPool.Pop() : Instantiate(Resources.Load<BaseMonster>($"Prefabs/Monsters/{code}"));
            monster.transform.position = position;
            monster.transform.rotation = rotation;
            monster.OnSpawn(id, code);

            spawnedMonsters.Add(monster);
            monsterDic.Add(id, monster);
        }
        else
        {
            Debug.LogError($"Monster's id is already using. id={id}");
        }
    }

    public void KillMonster(int id)
    {
        if (monsterDic.ContainsKey(id))
        {
            var monster = monsterDic[id];
            monster.OnDead();

            monsterPoolDic[monster.Code].Push(monster);
            spawnedMonsters.Remove(monster);
            monsterDic.Remove(id);
        }
        else
        {
            Debug.LogError("Monster's id is not exist");
        }
    }

    public void RemoveAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            monster.OnDead();
            monsterPoolDic[monster.Code].Push(monster);
            monsterDic.Remove(monster.Id);
        }

        spawnedMonsters.Clear();
    }

    public bool TryGetMonsterPosition(int monsterId, out Vector3 position)
    {
        if (monsterDic.ContainsKey(monsterId))
        {
            position = monsterDic[monsterId].transform.position;
            return true;
        }
        else
        {
            Debug.LogError($"Id not exist. id = {monsterId}");
            position = Vector3.zero;
            return false;
        }
    }
}
