using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcController : MonoBehaviour
{
    public string npcName;
    public UnityEvent onEnter;

    public List<string> conversations = new List<string>();

    private void OnEnable()
    {
        Managers.UIManager.GetLayout<HudLayout>().AddNameTarget(transform, npcName);
    }

    private void OnDisable()
    {
        Managers.UIManager.GetLayout<HudLayout>().RemoveNameTarget(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnter?.Invoke();
            //Managers.UIManager.GetPopup<ConversationPopup>().SetPopup(conversations, () => StartQuest(0));
            //Managers.UIManager.ShowPopup<ConversationPopup>();
        }
    }

    void StartQuest(int id)
    {
        Managers.UIManager.GetPopup<ConfirmPopup>().SetPopup("퀘스트", "이런 사정이 있네. 도와주겠나?",
                () => Managers.GameData.StartQuest(id), null);
        Managers.UIManager.ShowPopup<ConfirmPopup>();
    }
}
