using System.Collections.Generic;
using UnityEngine;

public class DroppedItemManager : MonoBehaviour
{
    public List<DroppedItem> items = new List<DroppedItem>();
    public int maxItem;
    public float spwanPower;

    Dictionary<int, Stack<DroppedItem>> pool = new Dictionary<int, Stack<DroppedItem>>();

    private void Awake()
    {
        foreach (var item in items)
            item.gameObject.SetActive(false);

        for (int i = 0; i < items.Count; i++)
            pool.Add(i, new Stack<DroppedItem>());
    }

    public void SpawnItem(int typeId, Vector3 position, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            DroppedItem item;

            if (pool[typeId].Count > 0)
                item = pool[typeId].Pop();
            else
                item = Instantiate(items[typeId]);

            var spwanDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
            item.transform.position = position;
            item.SpawnItem(typeId, spwanDirection * spwanPower, OnGetItem);
        }
    }

    void OnGetItem(int typeId, DroppedItem item)
    {
        item.gameObject.SetActive(false);
        pool[typeId].Push(item);
        Managers.effect.ShowEffect(3, item.transform.position);
        Managers.GameData.PickupItem(typeId);
    }
}
