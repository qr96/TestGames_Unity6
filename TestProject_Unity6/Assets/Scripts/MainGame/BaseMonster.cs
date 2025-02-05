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
    public TriggerEvent detectTrigger;

    public float knockBack;
    public float knockBackTime;
    public float moveSpeed;
    public float chaseDistance;
    public float targetPositionError;

    public GameObject targetPlayer;
    State nowState;
    DateTime knockBackEnd;
    public Vector3 targetPosition;

    public int Id { get; private set; }
    public int Code { get; private set; }

    private void Start()
    {
        detectTrigger.Set(OnEnterDetect, null);
    }

    private void Update()
    {
        if (nowState == State.Idle)
        {
            if (targetPlayer != null)
                nowState = State.Move;
        }
        else if(nowState == State.Move)
        {
            if (targetPlayer != null)
            {
                var targetDistance = (targetPlayer.transform.position - transform.position).sqrMagnitude;
                if (targetDistance < chaseDistance)
                {
                    targetPosition = targetPlayer.transform.position;
                }
                else
                {
                    targetPlayer = null;
                    nowState = State.Idle;
                }
            }
            else
            {
                nowState = State.Idle;
            }
        }
        else if (nowState == State.Damaged)
        {
            if (DateTime.Now > knockBackEnd)
                nowState = State.Idle;
        }
    }

    private void FixedUpdate()
    {
        if (nowState == State.Move)
        {
            var deltaPosition = targetPosition - transform.position;
            
            if (deltaPosition.sqrMagnitude > targetPositionError)
            {
                var resultVelocity = deltaPosition.normalized * moveSpeed;
                resultVelocity.y = rigid.linearVelocity.y;
                deltaPosition.y = 0f;

                rigid.linearVelocity = resultVelocity;
                rigid.rotation = Quaternion.LookRotation(deltaPosition);
            }
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

    void OnEnterDetect(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (targetPlayer == null)
                targetPlayer = col.gameObject;
        }
    }
}
