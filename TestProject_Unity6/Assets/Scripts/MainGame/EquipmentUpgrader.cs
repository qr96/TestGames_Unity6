using UnityEngine;

public class EquipmentUpgrader : MonoBehaviour
{
    public void Enhance(Equipment equipment)
    {
        var maxEnhance = TableData.GetEquipmentMaxEnhance(equipment.code);
        var success = TableData.GetEquipmentEnhancePossibilty(equipment.code, equipment.upgradeLevel);
        var destroy = 0.01f;
        var random = Random.Range(0f, 1f);

        if (equipment.upgradeLevel < maxEnhance)
        {
            if (random < success)
                UpgradeEquipment(equipment);
            else if (random < success + destroy)
                DestroyEquipment(equipment);
        }
    }

    void DestroyEquipment(Equipment equipment)
    {
        equipment.upgradeLevel = 0;
    }

    void UpgradeEquipment(Equipment equipment)
    {
        equipment.upgradeLevel++;
    }
}
