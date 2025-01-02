using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : UIPopup
{
    public TMP_Text title;
    public TMP_Text content;
    public KButton okButton;

    private void Start()
    {
        okButton.onClick.AddListener(Hide);
    }

    public void SetPopup(string title, string content)
    {
        this.title.text = title;
        this.content.text = content;
    }
}
