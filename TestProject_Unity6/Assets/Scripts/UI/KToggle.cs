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

    ToggleEvent m_OnValueChanged = new ToggleEvent();

    Vector3 normalScale;
    bool isOn;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    private void Start()
    {
        SetToggle(false);
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
    }
}
