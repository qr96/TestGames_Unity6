using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public RectTransform enemyPointPrefab;
    public GameObject target;

    public float distanceFactor;

    List<RectTransform> enemyPoints = new List<RectTransform>();

    void Update()
    {
        foreach (var point in enemyPoints)
            point.gameObject.SetActive(false);

        var enemyList = Managers.MonsterManager.GetSpawnedMonsterList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (i >= enemyPoints.Count)
            {
                var newPoint = Instantiate(enemyPointPrefab, enemyPointPrefab.parent);
                enemyPoints.Add(newPoint);
            }

            var pos = (enemyList[i].transform.position - target.transform.position) * distanceFactor;
            enemyPoints[i].anchoredPosition = new Vector2(pos.x, pos.z);
            enemyPoints[i].gameObject.SetActive(true);
        }
    }
}
