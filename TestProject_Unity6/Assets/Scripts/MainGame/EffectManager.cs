using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public List<GameObject> effects = new List<GameObject>();
    public int maxEffect = 5;

    Dictionary<int, Queue<GameObject>> effectPool = new Dictionary<int, Queue<GameObject>>();

    private void Awake()
    {
        for (int i = 0; i < effects.Count; i++)
            effectPool.Add(i, new Queue<GameObject>());
    }

    private void Start()
    {
        foreach (var effect in effects)
            effect.SetActive(false);
    }

    public void ShowEffect(int id, Vector3 position, Transform parent = null)
    {
        GameObject newEffect;

        if (id == 0)
            return;

        if (effectPool[id].Count < maxEffect)
            newEffect = Instantiate(effects[id]);
        else
            newEffect = effectPool[id].Dequeue();

        newEffect.transform.position = position;
        newEffect.transform.SetParent(parent);
        newEffect.SetActive(false);
        newEffect.SetActive(true);

        effectPool[id].Enqueue(newEffect);
    }
}
