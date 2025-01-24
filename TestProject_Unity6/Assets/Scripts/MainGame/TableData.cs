using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TableData : MonoBehaviour
{
    readonly string EquipmentJsonPath = "Jsons/Equipments";
    readonly string MiscItemJsonPath = "Jsons/MiscItem";
    readonly string SkillDataJsonPath = "Jsons/SKills";
    readonly string MonsterJsonPath = "Jsons/Monsters";
    readonly string MapUnitJsonPath = "Jsons/MapUnits";
    readonly string MapWarpJsonPath = "Jsons/MapInfos";
    readonly int MapUnitInfoCount = 2;

    Dictionary<Equipment.Part, Dictionary<int, EquipmentInfo>> equipmentInfoDic = new Dictionary<Equipment.Part, Dictionary<int, EquipmentInfo>>();
    Dictionary<int, SkillInfo> skillInfoDic = new Dictionary<int, SkillInfo>();
    Dictionary<int, MiscItemInfo> miscItemInfoDic = new Dictionary<int, MiscItemInfo>();
    Dictionary<int, MonsterInfo> monsterInfoDic = new Dictionary<int, MonsterInfo>();
    Dictionary<int, List<MapUnitInfo>> mapUnitInfoDic = new Dictionary<int, List<MapUnitInfo>>();
    Dictionary<int, MapWarpInfo> mapWarpInfoDic = new Dictionary<int, MapWarpInfo>(); 

    class EquipmentInfo
    {
        public int code;
        public string name;
        public long price;
        public Stat stat;
    }

    class SkillInfo
    {
        public int code;
        public string name;
        public int procChance;
        public int attackCount;
        public int maxLevel;
        public int effectCode;
        public int[] damage;
    }

    class MiscItemInfo
    {
        public int code;
        public string name;
        public long price;
    }

    class MonsterInfo
    {
        public int code;
        public string name;
        public Stat stat;
    }

    public class MapUnitInfo
    {
        public int code;
        public UnitType unitType;
        public float[] position;
        public float[] rotation;

        public enum UnitType
        {
            None,
            Monster
        }
    }

    public class MapWarpInfo
    {
        public List<WarpInfo> startPoints = new List<WarpInfo>();
        public List<WarpInfo> portals = new List<WarpInfo>();
    }

    public class WarpInfo
    {
        public int mapCode;
        public float[] position;
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

            ParseJson<List<EquipmentInfo>>($"{EquipmentJsonPath}/{fileName}", (dataList) =>
            {
                if (!equipmentInfoDic.ContainsKey(part))
                    equipmentInfoDic.Add(part, new Dictionary<int, EquipmentInfo>());

                foreach (var data in dataList)
                    equipmentInfoDic[part].Add(data.code, data);
            });
        }

        ParseJson<List<MiscItemInfo>>(MiscItemJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                miscItemInfoDic.Add(data.code, data);
        });

        ParseJson<List<SkillInfo>>(SkillDataJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                skillInfoDic.Add(data.code, data);
        });

        ParseJson<List<MonsterInfo>>(MonsterJsonPath, (dataList) =>
        {
            foreach (var data in dataList)
                monsterInfoDic.Add(data.code, data);
        });

        for (int i = 0; i < MapUnitInfoCount; i++)
            ParseJson<List<MapUnitInfo>>($"{MapUnitJsonPath}/{i}", (dataList) => mapUnitInfoDic.Add(i, dataList));

        for (int i = 0; i < MapUnitInfoCount; i++)
            ParseJson<MapWarpInfo>($"{MapWarpJsonPath}/{i}", (data) => mapWarpInfoDic.Add(i, data));
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
        stat.hp = 30;
        stat.mp = 10;
        stat.attack = 20;
        stat.speed = 6f;
        stat.mastery = 0.5f;

        return stat;
    }

    public int GetSkillMaxLevel(int skillCode)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
            return skillData.maxLevel;

        return 0;
    }

    public int GetSkillProcChance(int skillCode)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
            return skillData.procChance;

        return 0;
    }

    public int GetSkillAttackCount(int skillCode)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
            return skillData.attackCount;

        return 0;
    }

    public long GetSkillDamage(int skillCode, int skillLevel, long attack)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
        {
            if (skillLevel > 0 && skillLevel <= skillData.maxLevel)
            {
                if (skillLevel - 1 < skillData.damage.Length)
                    return attack * skillData.damage[skillLevel - 1] / 100;
                else
                    Debug.LogError($"GetSkillDamage(). Error occured. skillCode={skillCode}, skillLevel={skillLevel}, damage.Length={skillData.damage.Length}");
            }
            else
            {
                Debug.LogError($"GetSkillDamage(). Error occured. skillCode={skillCode}, skillLevel={skillLevel}, maxLevel={skillData.maxLevel}");
            }
        }

        return 0;
    }

    public int GetSkillEffectCode(int skillCode)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
            return skillData.effectCode;

        return 0;
    }

    public static Sprite GetSkillSprite(int skillCode)
    {
        return Resources.Load<Sprite>($"Sprites/Skills/{skillCode}");
    }

    public string GetSkillName(int skillCode)
    {
        if (TryGetSkillInfo(skillCode, out var skillData))
            return skillData.name;

        return "에러";
    }

    public long GetMiscItemPrice(int itemCode, int count)
    {
        if (miscItemInfoDic.ContainsKey(itemCode))
            return miscItemInfoDic[itemCode].price * count;

        return 1000;
    }

    public string GetMiscItemName(int itemCode)
    {
        if (miscItemInfoDic.ContainsKey(itemCode))
            return miscItemInfoDic[itemCode].name;
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
        if (TryGetEquipmentInfo(part, itemCode, out var equipmentData))
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

    public Stat GetEquipmentPureStat(Equipment.Part part, int equipmentCode)
    {
        if (TryGetEquipmentInfo(part, equipmentCode, out var equipmentData))
            return equipmentData.stat;
        else
            return default;
    }

    public Stat GetEquipmentEnhanceIncrease(Equipment.Part part, int nowUpgrade)
    {
        return part switch
        {
            Equipment.Part.Weapon => new Stat() { attack = 4 },
            Equipment.Part.Necklace => new Stat() { attack = 2 },
            Equipment.Part.Gloves => new Stat() { mp = 10 },
            Equipment.Part.Hat => new Stat() { hp = 5 },
            Equipment.Part.Armor => new Stat() { hp = 10 },
            Equipment.Part.Shoes => new Stat() { mp = 5 },
            _ => new Stat()
        };
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

    public Stat GetMonsterStat(int code)
    {
        if (TryGetMonsterInfo(code, out var info))
            return info.stat;

        return default;
    }

    public bool TryGetMapUnitsInfo(int mapCode, out List<MapUnitInfo> mapUnits)
    {
        if (mapUnitInfoDic.ContainsKey(mapCode))
        {
            mapUnits = mapUnitInfoDic[mapCode];
            return true;
        }

        mapUnits = default;
        return false;
    }

    public bool TryGetMapWarpInfo(int mapCode, out MapWarpInfo mapWarpInfo)
    {
        if (mapWarpInfoDic.ContainsKey(mapCode))
        {
            mapWarpInfo = mapWarpInfoDic[mapCode];
            return true;
        }

        mapWarpInfo = default;
        return false;
    }

    long GetEquipmentOriginPrice(Equipment.Part part, int itemCode)
    {
        if (TryGetEquipmentInfo(part, itemCode, out var equipmentData))
            return equipmentData.price;

        return 0;
    }

    bool TryGetEquipmentInfo(Equipment.Part part, int itemCode, out EquipmentInfo equipmentData)
    {
        if (equipmentInfoDic.ContainsKey(part))
        {
            if (equipmentInfoDic[part].ContainsKey(itemCode))
            {
                equipmentData = equipmentInfoDic[part][itemCode];
                return true;
            }
        }

        equipmentData = default;
        return false;
    }

    bool TryGetSkillInfo(int skillCode, out SkillInfo skillData)
    {
        if (skillInfoDic.ContainsKey(skillCode))
        {
            skillData = skillInfoDic[skillCode];
            return true;
        }

        skillData = default;
        return false;
    }

    bool TryGetMonsterInfo(int code, out MonsterInfo monsterInfo)
    {
        if (monsterInfoDic.ContainsKey(code))
        {
            monsterInfo = monsterInfoDic[code];
            return true;
        }

        monsterInfo = default;
        return false;
    }
}
