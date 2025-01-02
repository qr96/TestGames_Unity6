using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using static UnityEngine.UI.Toggle;

public class KToggle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public GameObject onGraphic;
    public KToggleGroup toggleGroup;

    ToggleEvent m_OnValueChanged = new ToggleEvent();

    Vector3 normalScale;
    public bool isOn;

    private void Awake()
    {
        normalScale = transform.localScale;
        isOn = false;
        onGraphic.SetActive(false);
    }

    private void OnEnable()
    {
        if (toggleGroup != null)
            toggleGroup.AddToggle(this);
    }

    private void OnDisable()
    {
        if (toggleGroup != null)
            toggleGroup.RemoveToggle(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = normalScale * 0.9f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = normalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetToggle(!isOn);
    }

    public ToggleEvent onValueChanged
    {
        get { return m_OnValueChanged; }
        set { m_OnValueChanged = value; }
    }
    
    public void SetToggle(bool isOn)
    {
        this.isOn = isOn;
        onGraphic.SetActive(isOn);
        onValueChanged?.Invoke(isOn);

        if (toggleGroup != null)
        {
            if (isOn)
                toggleGroup.OnToggleOn(this);
            else
                toggleGroup.OnToggleOff(this);
        }
    }

    public bool IsOn()
    {
        return isOn;
    }
}
