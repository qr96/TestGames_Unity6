using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public BaseMonster monsterPrefab;

    Stack<BaseMonster> monsterPool = new Stack<BaseMonster>();
    List<BaseMonster> spawnedMonsters = new List<BaseMonster>();
    Dictionary<int, BaseMonster> monsterDic = new Dictionary<int, BaseMonster>();

    public List<BaseMonster> GetSpawnedMonsterList()
    {
        return spawnedMonsters;
    }

    public void SpawnMonster(int id, Vector3 position, Quaternion rotation)
    {
        if (!monsterDic.ContainsKey(id))
        {
            var monster = monsterPool.Count > 0 ? monsterPool.Pop() : Instantiate(monsterPrefab);
            monster.transform.position = position;
            monster.transform.rotation = rotation;
            monster.OnSpawn(id);

            spawnedMonsters.Add(monster);
            monsterDic.Add(id, monster);
        }
        else
        {
            Debug.LogError("Monster's id is already using");
        }
    }

    public void RemoveMonster(int id)
    {
        if (monsterDic.ContainsKey(id))
        {
            var monster = monsterDic[id];
            monster.OnDead();

            monsterPool.Push(monster);
            spawnedMonsters.Remove(monster);
            monsterDic.Remove(id);
        }
        else
        {
            Debug.LogError("Monster's id is not exist");
        }
    }

    public Vector3 GetMonsterPosition(int id)
    {
        if (monsterDic.ContainsKey(id))
            return monsterDic[id].transform.position;
        else
        {
            Debug.LogError($"Id not exist. id = {id}");
            return Vector3.zero;
        }
    }
}
