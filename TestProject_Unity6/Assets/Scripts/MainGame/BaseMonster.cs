using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseMonster : MonoBehaviour
{
    enum State
    {
        Idle,
        Move,
        Damaged
    }

    public Rigidbody rigid;

    public float knockBack;

    public int Id { get; private set; }

    public void OnAttacked(Vector3 pushed)
    {
        rigid.rotation = Quaternion.LookRotation(-pushed);
        rigid.AddForce(pushed.normalized * knockBack, ForceMode.Impulse);
    }

    public void OnDead()
    {
        gameObject.SetActive(false);
    }

    public void OnSpawn(int id)
    {
        Id = id;
        gameObject.SetActive(true);
    }
}
