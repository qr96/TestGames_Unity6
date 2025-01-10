using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TableData : MonoBehaviour
{
    readonly string EquipmentJsonPath = "Jsons/Equipments";
    readonly string MiscItemJsonPath = "Jsons/MiscItem";
    readonly string SkillDataJsonPath = "Jsons/SKills";

    Dictionary<Equipment.Part, Dictionary<int, EquipmentData>> equipmentDataDic = new Dictionary<Equipment.Part, Dictionary<int, EquipmentData>>();
    Dictionary<int, SkillData> skillDataDic = new Dictionary<int, SkillData>();
    Dictionary<int, MiscItemData> miscItems = new Dictionary<int, MiscItemData>();

    class EquipmentData
    {
        public int code;
        public string name;
        public long price;
        public Stat stat;
    }

    class SkillData
    {
        public int code;
        public string name;
        public int procChance;
        public int attackCount;
        public int maxLevel;
        public int effectCode;
        public int[] damage;
    }

    class MiscItemData
    {
        public int code;
        public string name;
        public long price;
    }

    private void Awake()
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
                if (!equipmentDataDic.ContainsKey(part))
                    equipmentDataDic.Add(part, new Dictionary<int, EquipmentData>());

                foreach (var data in dataList)
                    equipmentDataDic[part].Add(data.code, data);
            });
        }

        ParseJson<List<MiscItemData>>(MiscItemJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                miscItems.Add(data.code, data);
        });

        ParseJson<List<SkillData>>(SkillDataJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                skillDataDic.Add(data.code, data);
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

    public int GetSkillMaxLevel(int skillCode)
    {
        if (TryGetSkillData(skillCode, out var skillData))
            return skillData.maxLevel;

        return 0;
    }

    public int GetSkillProcChange(int skillCode)
    {
        if (TryGetSkillData(skillCode, out var skillData))
            return skillData.procChance;

        return 0;
    }

    public long GetSkillDamage(int skillCode, int skillLevel, long attack)
    {
        if (TryGetSkillData(skillCode, out var skillData))
        {
            if (skillLevel <= skillData.maxLevel)
            {
                if (skillLevel - 1 < skillData.damage.Length)
                    return attack * skillData.damage[skillLevel - 1] / 100;
                else
                    Debug.LogError($"GetSkillDamage(). skillLevel is larger than skillData. skillCode={skillCode}, skillLevel={skillLevel}, damage.Length={skillData.damage.Length}");
            }
            else
            {
                Debug.LogError($"GetSkillDamage(). skillLevel is larger than maxLevel. skillCode={skillCode}, skillLevel={skillLevel}, maxLevel={skillData.maxLevel}");
            }
        }

        return 0;
    }

    public int GetSkillEffectCode(int skillCode)
    {
        if (TryGetSkillData(skillCode, out var skillData))
            return skillData.effectCode;

        return 0;
    }

    public static Sprite GetSkillImage(int skillCode)
    {
        return Resources.Load<Sprite>($"Sprites/Skills/{skillCode}");
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
        var originPrice = GetEquipmentOriginPrice(part, itemCode);

        expectPrice = originPrice / 100;

        for (int i = 1; i <= upgradeLevel; i++)
            expectPrice = (long)((expectPrice + upgradePrice) / (double)GetEquipmentEnhancePossibilty(itemCode, i));

        expectPrice *= 100;

        return expectPrice;
    }

    public string GetEquipmentName(Equipment.Part part, int itemCode)
    {
        if (TryGetEquipmentData(part, itemCode, out var equipmentData))
            return equipmentData.name;

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

    public Stat GetEquipmentStat(Equipment equipment)
    {
        var stat = new Stat();

        if (TryGetEquipmentData(equipment.part, equipment.code, out var equipmentData))
        {
            stat.Add(equipmentData.stat);
            stat.Add(new Stat() { attack = equipment.upgradeLevel, hp = equipment.upgradeLevel * 5 });
        }

        return stat;
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

    long GetEquipmentOriginPrice(Equipment.Part part, int itemCode)
    {
        if (TryGetEquipmentData(part, itemCode, out var equipmentData))
            return equipmentData.price;

        return 0;
    }

    bool TryGetEquipmentData(Equipment.Part part, int itemCode, out EquipmentData equipmentData)
    {
        if (equipmentDataDic.ContainsKey(part))
        {
            if (equipmentDataDic[part].ContainsKey(itemCode))
            {
                equipmentData = equipmentDataDic[part][itemCode];
                return true;
            }
        }

        equipmentData = default;
        return false;
    }

    bool TryGetSkillData(int skillCode, out SkillData skillData)
    {
        if (skillDataDic.ContainsKey(skillCode))
        {
            skillData = skillDataDic[skillCode];
            return true;
        }

        skillData = default;
        return false;
    }
}
