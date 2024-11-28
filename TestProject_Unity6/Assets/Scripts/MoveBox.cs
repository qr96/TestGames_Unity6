using System;
using UnityEngine;

namespace PushPush
{
    public class MoveBox : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Grab,
            Pushing,
            Moving
        }

        public float pushTime;
        public int moveUnit;
        public float moveSpeed;
        public float moveError;

        Func<Direction, bool> pushKeyDownEvent;
        Func<GameObject, Direction, bool> movePossibleEvent;
        Action<GameObject, Direction> moveEvent;

        GameObject pusher;

        public State nowState;
        public bool isGrab;
        public DateTime pushStart;
        public Vector3 desPos;

        private void Start()
        {
            nowState = State.Idle;
        }

        private void Update()
        {
            if (nowState == State.Idle)
            {
                if (isGrab)
                {
                    nowState = State.Grab;
                }
            }
            else if (nowState == State.Grab)
            {
                if (isGrab)
                {
                    var dir = GetPushDirection(pusher.transform.position, transform.position);

                    if (pushKeyDownEvent?.Invoke(dir) ?? false)
                    {
                        pushStart = DateTime.Now;
                        nowState = State.Pushing;
                    }
                }
                else
                {
                    nowState = State.Idle;
                }
            }
            else if (nowState == State.Pushing)
            {
                if (isGrab)
                {
                    var dir = GetPushDirection(pusher.transform.position, transform.position);
                    if (pushKeyDownEvent(dir))
                    {
                        if (DateTime.Now > pushStart.AddSeconds(pushTime))
                        {
                            if (movePossibleEvent(gameObject, dir))
                            {
                                moveEvent(gameObject, dir);
                                desPos = transform.position + GetVector3(dir) * moveUnit;
                                nowState = State.Moving;
                            }
                            else
                            {
                                nowState = State.Grab;
                            }
                        }
                    }
                    else
                    {
                        nowState = State.Grab;
                    }
                }
                else
                {
                    nowState = State.Idle;
                }
            }
            else if (nowState == State.Moving)
            {
                var remainDis = Vector3.SqrMagnitude(transform.position - desPos);
                if (remainDis > moveError)
                {
                    var newPos = Vector3.MoveTowards(transform.position, desPos, Time.deltaTime * moveSpeed);
                    transform.position = newPos;
                }
                else
                {
                    transform.position = desPos;

                    if (isGrab)
                    {
                        var dir = GetPushDirection(pusher.transform.position, transform.position);
                        if (pushKeyDownEvent(dir) && movePossibleEvent(gameObject, dir))
                        {
                            moveEvent(gameObject, dir);
                            desPos = transform.position + GetVector3(dir) * moveUnit;
                        }
                        else
                        {
                            nowState = State.Idle;
                        }
                    }
                    else
                    {
                        nowState = State.Idle;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                pusher = other.gameObject;
                isGrab = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                pusher = null;
                isGrab = false;
            }
        }

        public void SetMoveBox(Func<Direction, bool> pushKeyDownEvent,Func<GameObject, Direction, bool> movePossibleEvent, Action<GameObject, Direction> moveEvent)
        {
            this.pushKeyDownEvent = pushKeyDownEvent;
            this.movePossibleEvent = movePossibleEvent;
            this.moveEvent = moveEvent;
        }

        Direction GetPushDirection(Vector3 pusherPos, Vector3 pusheePos)
        {
            var vec = pusheePos - pusherPos;
            var dir = Direction.None;

            if (Mathf.Abs(vec.x) > Mathf.Abs(vec.z))
            {
                if (vec.x > 0)
                    dir = Direction.Right;
                else
                    dir = Direction.Left;
            }
            else
            {
                if (vec.z > 0)
                    dir = Direction.Up;
                else
                    dir = Direction.Down;
            }

            return dir;
        }

        Vector3 GetVector3(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Vector3.forward,
                Direction.Down => Vector3.back,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.zero
            };
        }
    }
}
