using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : UIPopup
{
    public TMP_Text title;
    public TMP_Text content;
    public Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void SetPopup(string title, string content)
    {
        this.title.text = title;
        this.content.text = content;
    }
}
