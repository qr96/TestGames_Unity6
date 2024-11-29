using System;
using TMPro;
using UnityEngine;

public class ConfirmPopup : UIPopup
{
    public TMP_Text title;
    public TMP_Text content;
    public KButton yesButton;
    public KButton noButton;

    Action onYes;
    Action onNo;

    private void Start()
    {
        yesButton.onClick.AddListener(() => onYes?.Invoke());
        noButton.onClick.AddListener(() => onNo?.Invoke());
    }

    public override void OnCreate()
    {
        
    }

    public void SetPopup(string title, string content, Action onYes, Action onNo)
    {
        this.title.text = title;
        this.content.text = content;
        this.onYes = onYes;
        this.onNo = onNo;
    }
}
