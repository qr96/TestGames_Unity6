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
        yesButton.onClick.AddListener(() =>
        {
            Hide();
            onYes?.Invoke();
        });
        noButton.onClick.AddListener(() =>
        {
            Hide();
            onNo?.Invoke();
        });
    }

    public void SetPopup(string title, string content, Action onYes, Action onNo)
    {
        this.title.text = title;
        this.content.text = content;
        this.onYes = onYes;
        this.onNo = onNo;
    }
}
