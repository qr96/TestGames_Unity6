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
        return 99;
    }

    public static long GetEquipmentEnhancePrice(int itemCode)
    {
        return 1000;
        //return (long)(Mathf.Pow(2, itemCode) * 500);
    }

    public static float GetEquipmentEnhancePossibilty(int itemCode, int enhanceLevel)
    {
        var possibility = 0f;

        if (enhanceLevel < 10)
            possibility = 0.95f;
        else if (enhanceLevel < 20)
            possibility = 0.9f;
        else if (enhanceLevel < 30)
            possibility = 0.85f;
        else if (enhanceLevel < 40)
            possibility = 0.8f;
        else if (enhanceLevel < 50)
            possibility = 0.75f;
        else if (enhanceLevel < 60)
            possibility = 0.7f;
        else if (enhanceLevel < 70)
            possibility = 0.65f;
        else if (enhanceLevel < 80)
            possibility = 0.6f;
        else if (enhanceLevel < 90)
            possibility = 0.55f;
        else
            possibility = 0.5f;

        return possibility;
    }

    public static void GetEquipmentStat(Equipment equipment)
    {

    }

    public static string GetEquipmentSpritePath(int code, Equipment.Part part)
    {
        var partPath = part switch
        {
            Equipment.Part.Weapon => "Weapon",
            Equipment.Part.Necklace => "Necklace",
            Equipment.Part.Gloves => "Gloves",
            Equipment.Part.Hat => "Hat",
            Equipment.Part.Armor => "Armor",
            Equipment.Part.Shoes => "Shoes",
            _ => ""
        };

        return $"Sprites/Equipments/{partPath}/{code}";
    }

    public static Sprite GetEquipmentSprite(int code, Equipment.Part part)
    {
        return Resources.Load<Sprite>(GetEquipmentSpritePath(code, part));
    }

    public static Sprite GetSpriteNone()
    {
        return Resources.Load<Sprite>("Sprites/None");
    }
}
