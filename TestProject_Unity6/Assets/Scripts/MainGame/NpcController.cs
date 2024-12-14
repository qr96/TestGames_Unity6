using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public List<string> conversations = new List<string>();

    private void Start()
    {
        Managers.UIManager.GetLayout<HudLayout>().AddNameTarget(transform, "촌장");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Managers.UIManager.GetPopup<ConversationPopup>().SetPopup(conversations, () => StartQuest(0));
            Managers.UIManager.ShowPopup<ConversationPopup>();
        }
    }

    void StartQuest(int id)
    {
        Managers.UIManager.GetPopup<ConfirmPopup>().SetPopup("퀘스트", "이런 사정이 있네. 도와주겠나?",
                () => Managers.GameData.StartQuest(id), null);
        Managers.UIManager.ShowPopup<ConfirmPopup>();
    }
}
