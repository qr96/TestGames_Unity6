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
        var prefabs = Resources.LoadAll<DroppedItem>("Prefabs/DroppedItems");
        foreach (var prefab in prefabs)
            items.Add(prefab);

        for (int i = 0; i < items.Count; i++)
            pool.Add(i, new Stack<DroppedItem>());
    }

    public void SpawnItem(int itemId, int itemCode, Vector3 position)
    {
        DroppedItem item;

        if (pool[itemCode].Count > 0)
            item = pool[itemCode].Pop();
        else
            item = Instantiate(items[itemCode]);

        var spwanDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
        item.transform.position = position;
        item.SpawnItem(itemId, itemCode, spwanDirection * spwanPower, OnGetItem);
    }

    void OnGetItem(int itemId, int itemCode, DroppedItem item)
    {
        item.gameObject.SetActive(false);
        pool[itemCode].Push(item);
        Managers.effect.ShowEffect(3, item.transform.position);
        Managers.GameData.PickupItem(itemId);
    }
}
