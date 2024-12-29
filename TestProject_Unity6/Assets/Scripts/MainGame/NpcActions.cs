using UnityEngine;

public class NpcActions : MonoBehaviour
{
    public void MiscShop()
    {
        Managers.GameData.ShowMiscShop();
    }

    public void Anvil()
    {
        Managers.UIManager.ShowPopup<EnhanceSelectPopup>().SetPopup(Managers.GameData.GetPlayerEquipments());
    }

    public void EquipmentShop()
    {
        Managers.UIManager.ShowPopup<EquipmentShopPopup>();
    }
}
