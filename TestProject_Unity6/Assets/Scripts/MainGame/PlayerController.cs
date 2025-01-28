using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attack
    }

    readonly float cos45 = 0.707107f;
    readonly float sin45 = 0.707107f;

    public Rigidbody rigid;
    public Animator animator;
    //public FloatingJoystick joystick;
    public VirtualJoystick joystick;
    public TriggerEvent attackTrigger;

    public float speed;
    public float attackCoolTime;
    public float repulsivePower;

    InputAction inputAction;
    Vector3 moveDirection;
    DateTime attackEnd;
    State nowState;

    HashSet<BaseMonster> enemies = new HashSet<BaseMonster>();

    private void Start()
    {
        inputAction = InputSystem.actions.FindAction("Move");
        attackTrigger.Set(OnEnterAttackTrigger, OnExitAttackTrigger);

        nowState = State.Idle;
    }

    private void Update()
    {
        if (nowState == State.Idle)
        {
            var input = Vector2.zero;

            if (joystick != null)
                input = joystick.Direction;

            if (input == Vector2.zero && inputAction != null)
                input = inputAction.ReadValue<Vector2>();

            input.Normalize();

            moveDirection = new Vector3(input.x, 0f, input.y);

            if (enemies.Count > 0)
            {
                var inputAttackVector = moveDirection;
                var isAttack = false;
                var lastEnemyVector = Vector3.zero;

                // 타겟 공격
                foreach (var enemy in enemies)
                {
                    var enemyVector = (enemy.transform.position - transform.position);
                    isAttack = Vector3.Dot(enemyVector, inputAttackVector) > 0f;
                    enemy.OnAttacked(isAttack ? inputAttackVector : enemyVector);
                    Managers.GameData.Battle(enemy.Id, isAttack);
                    lastEnemyVector = enemyVector;
                }

                // 비활성화된 타겟 목록에서 제거 (반드시 공격 직후에 해줘야함)
                foreach (var enemy in enemies)
                    if (!enemy.isActiveAndEnabled) Managers.UIManager.GetLayout<HudLayout>().RemoveTarget(enemy.Id);
                enemies.RemoveWhere(enemy => !enemy.isActiveAndEnabled);

                if (isAttack)
                {
                    // 공격 애니메이션
                    animator.SetTrigger("Attack");
                    animator.SetBool("Moving", false);
                    attackEnd = DateTime.Now.AddSeconds(attackCoolTime);
                    
                    // 상태 변경
                    nowState = State.Attack;
                }

                OnPushed(isAttack ? -inputAttackVector : -lastEnemyVector);
            }
            else
            {
                // 애니메이션
                if (moveDirection == Vector3.zero)
                    animator.SetBool("Moving", false);
                else
                {
                    animator.SetBool("Moving", true);
                }
            }
        }
        else if (nowState == State.Attack)
        {
            if (DateTime.Now > attackEnd)
                nowState = State.Idle;
        }
    }

    private void FixedUpdate()
    {
        if(nowState == State.Idle)
        {
            var resultVelocity = rigid.linearVelocity;
            resultVelocity = moveDirection * speed;

            var moveDirectionLeft = new Vector3(moveDirection.x * cos45 - moveDirection.z * sin45, 0f, moveDirection.x * sin45 + moveDirection.z * cos45);
            var moveDirectionRight = new Vector3(moveDirection.x * cos45 + moveDirection.z * sin45, 0f, -moveDirection.x * sin45 + moveDirection.z * cos45);

            // 벽 붙어서 이동 보정
            if (Physics.Raycast(transform.position, moveDirectionLeft, out var hit, 0.52f, LayerMask.GetMask("Wall"))
                || Physics.Raycast(transform.position, moveDirectionLeft, out hit, 0.71f, LayerMask.GetMask("Wall"))
                || Physics.Raycast(transform.position, moveDirectionRight, out hit, 0.71f, LayerMask.GetMask("Wall")))
            {
                var wallNormal = hit.normal;
                var slideDirection = Vector3.ProjectOnPlane(moveDirection, wallNormal);
                resultVelocity = slideDirection.normalized * speed;
            }

            resultVelocity.y = rigid.linearVelocity.y;
            rigid.linearVelocity = resultVelocity;

            if (moveDirection != Vector3.zero)
                rigid.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    public void OnPushed(Vector3 pushed)
    {
        rigid.linearVelocity = new Vector3(0f, rigid.linearVelocity.y, 0f);
        rigid.AddForce(pushed * repulsivePower, ForceMode.Impulse);
    }
    
    void OnEnterAttackTrigger(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            var enemy = col.GetComponent<BaseMonster>();
            enemies.Add(enemy);
            Managers.UIManager.GetLayout<HudLayout>().AddTarget(enemy.Id);
        }
    }

    void OnExitAttackTrigger(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            var enemy = col.GetComponent<BaseMonster>();
            if (enemies.Contains(enemy))
                enemies.Remove(enemy);
        }
    }
}
