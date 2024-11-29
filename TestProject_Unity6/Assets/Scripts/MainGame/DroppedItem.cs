using System;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public GameObject model;
    public Rigidbody rigid;

    public float rotateSpeed;

    int id;
    float rotateY;
    Action<int, DroppedItem> onGetItem;

    private void Update()
    {
        model.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotateY, 0f));
        rotateY += Time.deltaTime * rotateSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onGetItem?.Invoke(id, this);
        }
    }

    public void SpawnItem(int id, Vector3 force, Action<int, DroppedItem> onGetItem)
    {
        this.id = id;
        gameObject.SetActive(true);
        rigid.AddForce(force, ForceMode.Impulse);
        this.onGetItem = onGetItem;
    }
}
