using System.Collections.Generic;
using UnityEngine;

public class VirtualButtonLayout : UILayout
{
    public List<KButton> itemButtons = new List<KButton>();
    public List<IconSlot> iconSlots = new List<IconSlot>();

    private void Start()
    {

    }

    public void StartItemCoolTime(int id, float coolTime)
    {
        iconSlots[id].StartCoolTime(coolTime);
    }
}
