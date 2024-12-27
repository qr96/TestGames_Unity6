using System.Collections.Generic;
using UnityEngine;

public class KToggleGroup : MonoBehaviour
{
    public bool allowSwitchOff = false;

    List<KToggle> toggles = new List<KToggle>();

    public void AddToggle(KToggle toggle)
    {
        if (toggles.Count > 0)
            toggle.SetToggle(false);
        else
            toggle.SetToggle(true);

        toggles.Add(toggle);
    }

    public void RemoveToggle(KToggle toggle)
    {
        toggles.Remove(toggle);
    }

    public void OnToggleOn(KToggle selected)
    {
        foreach (var toggle in toggles)
        {
            if (!toggle.Equals(selected))
                toggle.SetToggle(false);
        }
    }

    public void OnToggleOff(KToggle selected)
    {
        if (allowSwitchOff)
            return;

        var hasOn = false;

        foreach (var toggle in toggles)
        {
            if (toggle.IsOn())
                hasOn = true;
        }

        if (!hasOn)
            selected.SetToggle(true);
    }
}
