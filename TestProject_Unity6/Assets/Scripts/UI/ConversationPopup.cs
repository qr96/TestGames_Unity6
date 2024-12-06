using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationPopup : UIPopup
{
    public TMP_Text message;
    public Button nextButton;

    List<string> conversations = new List<string>();
    Action onEndConversation;

    int nowPage;

    private void Start()
    {
        nextButton.onClick.AddListener(() => NextPage());
    }

    public void SetPopup(List<string> conversations, Action onEndConversation)
    {
        this.conversations = conversations;
        this.onEndConversation = onEndConversation;
    }

    public override void OnShow()
    {
        if (conversations != null && conversations.Count > 0)
        {
            nowPage = 0;
            message.text = conversations[nowPage];
        }
        else
        {
            Hide();
            onEndConversation?.Invoke();
        }
    }

    void NextPage()
    {
        nowPage++;
        if (nowPage >= conversations.Count)
        {
            Hide();
            onEndConversation?.Invoke();
        }
        else
        {
            message.text = conversations[nowPage];
        }
    }
}
