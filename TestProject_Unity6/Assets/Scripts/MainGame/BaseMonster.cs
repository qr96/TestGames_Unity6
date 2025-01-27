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
    public float knockBackTime;
    public float moveSpeed;

    GameObject targetPlayer;
    State nowState;
    DateTime knockBackEnd;

    public int Id { get; private set; }
    public int Code { get; private set; }

    private void Update()
    {
        if (nowState == State.Damaged)
        {
            if (DateTime.Now > knockBackEnd)
                nowState = State.Idle;
        }
    }

    private void FixedUpdate()
    {
        if (targetPlayer != null && nowState != State.Damaged)
        {
            var direction = targetPlayer.transform.position - transform.position;
            direction.y = 0;

            var resultVelocity = direction.normalized * moveSpeed;
            resultVelocity.y = rigid.linearVelocity.y;

            rigid.linearVelocity = resultVelocity;
            rigid.rotation = Quaternion.LookRotation(direction);

            nowState = State.Move;
        }
    }

    public void OnAttacked(Vector3 pushed)
    {
        rigid.linearVelocity = new Vector3(0f, rigid.linearVelocity.y, 0f);
        rigid.rotation = Quaternion.LookRotation(-pushed);
        rigid.AddForce(pushed.normalized * knockBack, ForceMode.Impulse);
        targetPlayer = Managers.MonsterManager.player.gameObject;

        nowState = State.Damaged;
        knockBackEnd = DateTime.Now.AddSeconds(knockBackTime);
    }

    public void OnDead()
    {
        gameObject.SetActive(false);
        targetPlayer = null;
    }

    public void OnSpawn(int id, int code)
    {
        Id = id;
        Code = code;
        gameObject.SetActive(true);
        nowState = State.Idle;
    }
}
