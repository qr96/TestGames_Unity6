using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PushPush
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public class MapMaker : MonoBehaviour
    {
        public List<GameObject> prefabs;
        public GameObject parent;
        
        public FloatingJoystick joystick;

        public int[,] map0 = new int[,]
        {
        {0,1,1,1,1,1,0,0},
        {0,0,0,0,1,1,1,0},
        {0,1,0,2,0,0,1,0},
        {1,1,1,0,1,0,1,1},
        {1,3,1,0,1,0,0,1},
        {1,3,2,0,0,1,0,1},
        {1,3,0,0,0,2,0,1},
        {1,1,1,1,1,1,1,1}
        };
        public int[,] map1 = new int[,]
        {
        {0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,1,2,0,2,1,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0},
        {0,0,1,0,0,0,2,0,0,1,0,0,0,0,0,0,0,0,0},
        {1,1,1,0,1,2,1,1,2,1,0,0,0,1,1,1,1,1,1},
        {1,0,0,0,1,0,1,1,0,1,1,1,1,1,0,0,3,3,1},
        {1,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,1},
        {1,1,1,1,1,0,1,1,1,0,1,0,1,1,0,0,3,3,1},
        {0,0,0,0,1,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
        {0,0,0,0,1,1,1,1,1,0,1,0,0,0,0,0,0,0,0}
        };

        int[,] map;
        int[,] nowMap;

        InputAction inputAction;
        Dictionary<GameObject, Vector2Int> boxPos = new Dictionary<GameObject, Vector2Int>();

        int goalCount = 0;
        int achieveCount = 0;

        private void Start()
        {
            map = (int[,])map1.Clone();
            nowMap = (int[,])map.Clone();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    var goType = map[i, j];
                    if (goType != 0)
                    {
                        var go = Instantiate(prefabs[goType], parent.transform);
                        go.transform.localPosition = new Vector3(j * 2, 0, i * -2);
                        go.SetActive(true);
                        
                        if (goType == 2)
                        {
                            var moveBox = go.GetComponent<MoveBox>();
                            if (moveBox != null)
                                moveBox.SetMoveBox(PushKeyDown, MovePossible, MoveBox);

                            boxPos.Add(go, new Vector2Int(i, j));
                        }
                        else if (goType == 3)
                        {
                            goalCount++;
                        }
                    }
                }
            }

            inputAction = InputSystem.actions.FindAction("Move");
        }

        bool PushKeyDown(Direction dir)
        {
            var input = joystick.Direction;
            input = input.normalized;
            if (input == Vector2.zero)
                input = inputAction.ReadValue<Vector2>();

            return dir switch
            {
                Direction.Up => input.y > 0.95f,
                Direction.Down => input.y < -0.95f,
                Direction.Left => input.x < -0.95f,
                Direction.Right => input.x > 0.95f,
                _ => false
            };
        }

        bool MovePossible(GameObject go, Direction dir)
        {
            var newPos = boxPos[go] + GetVector(dir);
            var tileType = nowMap[newPos.x, newPos.y];
            return tileType == 0 || tileType == 3;
        }

        void MoveBox(GameObject go, Direction dir)
        {
            if (MovePossible(go, dir))
            {
                var prevPos = boxPos[go];
                var newPos = boxPos[go] + GetVector(dir);

                if (nowMap[newPos.x, newPos.y] == 3)
                    achieveCount++;
                if (map[prevPos.x, prevPos.y] == 3)
                    achieveCount--;

                nowMap[newPos.x, newPos.y] = 2;
                nowMap[prevPos.x, prevPos.y] = map[prevPos.x, prevPos.y] == 2 ? 0 : map[prevPos.x, prevPos.y];

                boxPos[go] = newPos;

                if (achieveCount >= goalCount)
                    Debug.Log("Finish!!");
            }
        }

        Vector2Int GetVector(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Vector2Int.left,
                Direction.Down => Vector2Int.right,
                Direction.Left => Vector2Int.down,
                Direction.Right => Vector2Int.up,
                _ => Vector2Int.zero
            };
        }
    }
}

