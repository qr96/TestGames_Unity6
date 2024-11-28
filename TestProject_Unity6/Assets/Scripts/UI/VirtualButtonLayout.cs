using UnityEngine;

public class VirtualButtonLayout : UILayout
{
    public KButton item0Button;

    public IconSlot item0Slot;

    private void Start()
    {
        item0Button.onClick.AddListener(() =>
        {
            Managers.GameData.UseItem(0);
        });
    }

    public void StartItemCoolTime(int id, float coolTime)
    {
        item0Slot.StartCoolTime(coolTime);
    }
}
