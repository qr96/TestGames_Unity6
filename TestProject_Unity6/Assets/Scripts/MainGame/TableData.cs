using UnityEngine;

public class TableData
{
    public static long GetMaxExp(int level)
    {
        return level * 5;
    }

    public static Stat GetStatPerLevel(int level)
    {
        var stat = new Stat();
        
        stat.maxHp = 10 + level * 10;
        stat.maxMp = 10 + level * 5;
        stat.attack = 10 + level;

        return stat;
    }

    public static float GetSkillBuffTime(int skillID, int skillLevel)
    {
        float[] buffTimes = new float[] { 10f, 9999999f };

        if (skillID < buffTimes.Length)
            return buffTimes[skillID];
        else
            return 0f;
    }

    public static Stat GetSkillBuffStat(int skillID, int skillLevel)
    {
        Stat[] stats = new Stat[] { new Stat() { speed = 2f }, new Stat() { speed = 4f } };

        if (skillID < stats.Length)
            return stats[skillID];
        else
            return default;
    }

    public static Stat GetSkillIncreaseStat(int skillCode, int nowLevel)
    {
        var stat = new Stat();

        return stat;
    }

    public static int GetSkillMaxLevel(int skillCode)
    {
        return 0;
    }

    public static long GetSellPrice(int itemCode, int count)
    {
        long[] sellPrice = new long[] { 0, 10, 50, 100 };
        return sellPrice[itemCode] * count;
    }

    public static long GetEquipmentBuyPrice(int itemCode)
    {
        return itemCode * 1000;
    }

    public static string GetEquipmentName(int itemCode)
    {
        return $"병사의 검 {itemCode}";
    }

    public static int GetEquipmentMaxEnhance(int itemCode)
    {
        return 20;
    }

    public static long GetEquipmentEnhancePrice(int itemCode)
    {
        return (long)(Mathf.Pow(2, itemCode) * 500);
    }

    public static float GetEquipmentEnhancePossibilty(int itemCode, int upgradeLevel)
    {
        var possibility = 1f - itemCode * 0.05f;
        if (possibility < 0.1f)
            possibility = 0.1f;

        return possibility;
    }
}
