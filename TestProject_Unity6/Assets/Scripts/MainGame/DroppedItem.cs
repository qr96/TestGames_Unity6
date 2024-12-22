using System;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public GameObject model;
    public Rigidbody rigid;

    public float rotateSpeed;

    int itemId;
    int itemCode;
    float rotateY;
    Action<int, int, DroppedItem> onGetItem;

    private void Update()
    {
        model.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotateY, 0f));
        rotateY += Time.deltaTime * rotateSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onGetItem?.Invoke(itemId, itemCode, this);
        }
    }

    public void SpawnItem(int itemId, int itemCode, Vector3 force, Action<int, int, DroppedItem> onGetItem)
    {
        this.itemId = itemId;
        this.itemCode = itemCode;
        gameObject.SetActive(true);
        rigid.AddForce(force, ForceMode.Impulse);
        this.onGetItem = onGetItem;
    }
}
