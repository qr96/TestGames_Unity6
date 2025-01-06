using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TableData : MonoBehaviour
{
    readonly string EquipmentJsonPath = "Jsons/Equipments";
    readonly string MiscItemJsonPath = "Jsons/MiscItem";

    Dictionary<Equipment.Part, Dictionary<int, EquipmentData>> equipmentData = new Dictionary<Equipment.Part, Dictionary<int, EquipmentData>>();
    Dictionary<int, MiscItemData> miscItems = new Dictionary<int, MiscItemData>();

    class EquipmentData
    {
        public int code;
        public string name;
    }

    class MiscItemData
    {
        public int code;
        public string name;
        public long price;
    }

    private void Start()
    {
        var equipmentParts = Enum.GetValues(typeof(Equipment.Part));
        foreach (var partObject in equipmentParts)
        {
            var part = (Equipment.Part)partObject;
            var fileName = partObject.ToString();

            if (part == Equipment.Part.None)
                continue;

            ParseJson<List<EquipmentData>>($"{EquipmentJsonPath}/{fileName}", (dataList) =>
            {
                if (!equipmentData.ContainsKey(part))
                    equipmentData.Add(part, new Dictionary<int, EquipmentData>());

                foreach (var data in dataList)
                    equipmentData[part].Add(data.code, data);
            });
        }

        ParseJson<List<MiscItemData>>(MiscItemJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                miscItems.Add(data.code, data);
        });
    }

    void ParseJson<T>(string jsonPath, Action<T> onDesrialize)
    {
        var json = Resources.Load<TextAsset>(jsonPath);
        if (json != null)
        {
            try
            {
                var dataList = JsonConvert.DeserializeObject<T>(json.text);
                onDesrialize(dataList);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.LogError($"Json file not exist. jsonPath = {jsonPath}");
        }
    }

    public static bool IsTown(int mapId)
    {
        if (mapId == 0) return true;
        return false;
    }

    public static long GetMaxExp(int level)
    {
        return level * 5;
    }

    public static Stat GetStatPerLevel(int level)
    {
        var stat = new Stat();
        stat.hp = 10 + level * 2;
        stat.mp = 10 + level * 1;
        stat.attack = 10 + level;
        stat.speed = 6f;
        stat.mastery = 0.5f;

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

    public static long GetSkillDamage(int skillId, int skillLevel, long attack)
    {
        if (skillId == 0)
            return attack;
        else if (skillId == 1)
            return attack * 2;
        else if (skillId == 2)
            return attack * 3;

        return 0;
    }

    public long GetMiscItemPrice(int itemCode, int count)
    {
        if (miscItems.ContainsKey(itemCode))
            return miscItems[itemCode].price * count;

        return 1000;
    }

    public string GetMiscItemName(int itemCode)
    {
        if (miscItems.ContainsKey(itemCode))
            return miscItems[itemCode].name;
        return "Error";
    }

    public long GetEquipmentPrice(Equipment.Part part, int itemCode, int upgradeLevel)
    {
        var expectPrice = 1000L;
        var upgradePrice = 1000L;
        var originPrice = 2000L;

        expectPrice = originPrice / 100;

        for (int i = 1; i <= upgradeLevel; i++)
            expectPrice = (long)((expectPrice + upgradePrice) / (double)GetEquipmentEnhancePossibilty(itemCode, i));

        expectPrice *= 100;

        return expectPrice;
    }

    public string GetEquipmentName(Equipment.Part part, int itemCode)
    {
        if (equipmentData.ContainsKey(part))
        {
            if (equipmentData[part].ContainsKey(itemCode) && equipmentData[part][itemCode] != null)
                return equipmentData[part][itemCode].name;
        }
        
        return "Error";
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
            possibility = 0.9f;
        else if (enhanceLevel < 40)
            possibility = 0.85f;
        else if (enhanceLevel < 50)
            possibility = 0.8f;
        else if (enhanceLevel < 60)
            possibility = 0.8f;
        else if (enhanceLevel < 70)
            possibility = 0.75f;
        else if (enhanceLevel < 80)
            possibility = 0.7f;
        else if (enhanceLevel < 90)
            possibility = 0.7f;
        else
            possibility = 0.6f;

        return possibility;
    }

    public static Stat GetEquipmentStat(Equipment equipment)
    {
        return new Stat() { attack = equipment.upgradeLevel, hp = equipment.upgradeLevel * 5 };
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
