using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VirtualJoystick : MonoBehaviour
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    public float joystickRange = 100f;

    RectTransform myRectTransform;

    public Vector2 Direction { get; private set; }
    int activeTouchId = -1;

    private void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        ResetJoystick();
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        foreach (var touch in Touchscreen.current.touches)
        {
            var phase = touch.phase.ReadValue();
            var position = touch.position.ReadValue();
            
            if (phase == UnityEngine.InputSystem.TouchPhase.Began && activeTouchId == -1)
            {
                if (IsTouchWithinJoystick(position) && !IsPointerOverUI(position))
                {
                    activeTouchId = touch.touchId.ReadValue();
                    BeginJoystickDrag(position);
                }
            }
            else if (touch.touchId.ReadValue() == activeTouchId)
            {
                if (phase == UnityEngine.InputSystem.TouchPhase.Moved || phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    UpdateJoystickPosition(position);
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    ResetJoystick();
                    activeTouchId = -1;
                }
            }
        }
    }

    bool IsTouchWithinJoystick(Vector2 touchPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(myRectTransform, touchPosition);
    }

    void BeginJoystickDrag(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myRectTransform, touchPosition, null, out var anchoredPosition);
        joystickBackground.anchoredPosition = anchoredPosition;
        joystickHandle.anchoredPosition = anchoredPosition;
        joystickBackground.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);
    }

    void UpdateJoystickPosition(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myRectTransform, touchPosition, null, out var anchoredPosition);
        var direction = anchoredPosition - joystickBackground.anchoredPosition;
        Direction = Vector2.ClampMagnitude(direction, joystickRange);
        joystickHandle.anchoredPosition = joystickBackground.anchoredPosition + Direction;
        Direction /= joystickRange;
    }

    void ResetJoystick()
    {
        Direction = Vector2.zero;
        joystickBackground.anchoredPosition = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
    }

    private bool IsPointerOverUI(Vector2 touchPosition)
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = touchPosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
}
