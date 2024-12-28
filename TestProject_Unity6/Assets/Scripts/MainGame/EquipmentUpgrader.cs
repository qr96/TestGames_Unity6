using UnityEngine;

public class EquipmentUpgrader : MonoBehaviour
{
    const int MAX_UPGRADE = 20;

    public void Enhance(Equipment equipment)
    {
        var success = 0.5f;
        var destroy = 0.01f;
        var random = Random.Range(0f, 1f);

        if (random < success)
            UpgradeEquipment(equipment);
        else if (random < success + destroy)
            DestroyEquipment(equipment);
    }

    void DestroyEquipment(Equipment equipment)
    {
        equipment.upgradeLevel = 0;
    }

    void UpgradeEquipment(Equipment equipment)
    {
        if (equipment.upgradeLevel < MAX_UPGRADE)
            equipment.upgradeLevel++;
    }
}
