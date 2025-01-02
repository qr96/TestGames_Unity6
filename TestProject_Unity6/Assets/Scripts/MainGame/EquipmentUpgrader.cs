using UnityEngine;

public class EquipmentUpgrader : MonoBehaviour
{
    public void Enhance(Equipment equipment)
    {
        var maxEnhance = TableData.GetEquipmentMaxEnhance(equipment.code);
        var success = TableData.GetEquipmentEnhancePossibilty(equipment.code, equipment.upgradeLevel);
        var needMoney = TableData.GetEquipmentEnhancePrice(equipment.code);
        var random = Random.Range(0f, 1f);

        Managers.GameData.ModifyPlayerMoney(-needMoney);

        if (equipment.upgradeLevel < maxEnhance)
        {
            if (random < success)
            {
                UpgradeEquipment(equipment);
                //Managers.UIManager.ShowPopup<MessagePopup>().SetPopup("강화 성공", "강화 레벨이 올랐습니다.");
                Managers.UIManager.GetPopup<EnhancePopup>().SetPopup(equipment.id, Managers.GameData.GetPlayerMoney());
                Managers.UIManager.GetPopup<EnhancePopup>().ShowEffect(true);
                Managers.UIManager.GetPopup<EnhanceSelectPopup>().SetPopup(Managers.GameData.GetPlayerEquipments(), equipment.id);
            }
            else
            {
                Managers.GameData.RemoveEquipment(equipment.id);
                //Managers.UIManager.ShowPopup<MessagePopup>().SetPopup("강화 실패", "장비가 파괴되었습니다.");
                Managers.UIManager.GetPopup<EnhancePopup>().SetDestroyPopup(Managers.GameData.GetPlayerMoney());
                Managers.UIManager.GetPopup<EnhancePopup>().ShowEffect(false);
                Managers.UIManager.GetPopup<EnhanceSelectPopup>().SetPopup(Managers.GameData.GetPlayerEquipments());
            }
        }
    }

    void ResetEquipment(Equipment equipment)
    {
        equipment.upgradeLevel = 0;
    }

    void UpgradeEquipment(Equipment equipment)
    {
        equipment.upgradeLevel++;
    }
}
