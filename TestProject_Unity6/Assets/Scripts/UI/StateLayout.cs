using UnityEngine;

public class StateLayout : UILayout
{
    public GuageBar userHpBar;

    public void SetUserHpBar(long maxHp, long nowHp)
    {
        userHpBar.SetGuage(maxHp, nowHp);
    }
}
