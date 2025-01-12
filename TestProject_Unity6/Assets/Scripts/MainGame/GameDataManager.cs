using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDataManager : MonoBehaviour
{
    public MapData mapData;
    public EquipmentUpgrader equipmentUpgrader;

    playerInfo playerInfo;

    List<Quest> progressQuests = new List<Quest>();
    List<Quest> completeQuests = new List<Quest>();

    private void Start()
    {
        playerInfo = new playerInfo(1);
        ModifyPlayerExp(0);
        ModifyPlayerMoney(1000000);
        SkillLevelUp(1);
        SkillLevelUp(4);
        SkillLevelUp(6);
        EquipSkill(1);
        EquipSkill(4);
        EquipSkill(6);
    }

    public Stat GetPlayerStat()
    {
        return playerInfo.maxStat;
    }

    public void ModifyPlayerMoney(long money)
    {
        playerInfo.money += money;
        Managers.UIManager.GetPopup<InfoPopup>().SetMoney(playerInfo.money);
        Managers.UIManager.GetPopup<EquipmentShopPopup>().SetMoney(playerInfo.money);
    }

    public void ModifyPlayerExp(long exp)
    {
        playerInfo.ModifyExp(exp, () => Managers.effect.ShowEffect(4, Managers.MonsterManager.player.transform.position, Managers.MonsterManager.player.transform));
        Managers.UIManager.GetLayout<StateLayout>().SetUserExpBar(TableData.GetMaxExp(playerInfo.level), playerInfo.nowExp);
        Managers.UIManager.GetLayout<StateLayout>().SetLevel(playerInfo.level);
    }

    public void AddMiscItemToBag(int itemCode, int count)
    {
        playerInfo.miscBag.AddItem(itemCode, count);
        Managers.UIManager.GetPopup<InfoPopup>().SetBagTab(playerInfo.miscBag.ToList());
    }

    public void Battle(int monsterId)
    {
        mapData.Battle(monsterId);
    }

    public void EnterMap(int mapId)
    {
        Managers.UIManager.GetLayout<HudLayout>().ClearLayout();
        mapData.EnterMap(mapId);
    }

    // Shop
    public void ShowMiscShop()
    {
        Managers.UIManager.ShowPopup<MiscShopPopup>().SetPopup(playerInfo.miscBag.ToList(), GetPlayerMoney());
    }

    public void SellAllItems()
    {
        var bag = playerInfo.miscBag.ToList();
        var price = 0L;

        foreach (var itemData in bag)
            price += Managers.TableData.GetMiscItemPrice(itemData.itemCode, itemData.count);

        playerInfo.miscBag.Clear();
        ModifyPlayerMoney(price);

        Managers.UIManager.GetPopup<InfoPopup>().SetBagTab(playerInfo.miscBag.ToList());
        Managers.UIManager.GetPopup<MiscShopPopup>().SetPopup(playerInfo.miscBag.ToList(), GetPlayerMoney());
    }

    public void StartQuest(int questId)
    {
        if (completeQuests.Any((quest) => quest.id == questId))
        {
            Debug.LogError($"Quest is completed already. id = {questId}");
        }
        else if (progressQuests.Any((quest) => quest.id == questId))
        {
            Debug.LogError($"Quest is progressed already. id = {questId}");
        }
        else
        {
            progressQuests.Add(new Quest() { id = questId, targetAmount = 10 });
            Managers.UIManager.GetLayout<StateLayout>().SetQuestList(progressQuests);
        }
    }

    public void CompleteQuest(int questId)
    {
        var questIndex = progressQuests.FindIndex((quest) => quest.id == questId);
        if (questIndex > -1)
        {
            var quest = progressQuests[questIndex];
            if (quest.nowAmount >= quest.targetAmount)
            {
                progressQuests.RemoveAt(questIndex);
                completeQuests.Add(quest);
                Managers.UIManager.GetLayout<StateLayout>().SetQuestList(progressQuests);
            }
        }
        else
        {
            Debug.LogError($"Quest is not progressing. id = {questId}");
        }
    }

    public void PickupItem(int itemId)
    {
        mapData.PickupItem(itemId);
    }

    public List<Equipment> GetPlayerEquipments()
    {
        return playerInfo.equipmentBag.ToList();
    }

    public Equipment GetPlayerEquipment(int id)
    {
        return playerInfo.equipmentBag.GetById(id);
    }

    public void EnhanceEquipment(int id)
    {
        var equipment = GetPlayerEquipment(id);
        equipmentUpgrader.Enhance(equipment);

        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList(), playerInfo.maxStat);
    }

    public void AddEquipment(int equipmentCode, int upgradeLevel, Equipment.Part part)
    {
        playerInfo.equipmentBag.Add(new Equipment(equipmentCode, upgradeLevel, part));

        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList(), playerInfo.maxStat);
        Managers.UIManager.ShowPopup<MessagePopup>().SetPopup("안내", $"{Managers.TableData.GetEquipmentName(part, equipmentCode)} (+{upgradeLevel})이(가) 구매 완료되었습니다.");
    }

    public void RemoveEquipment(int equipmentId)
    {
        playerInfo.equipmentBag.Remove(equipmentId);
    }

    public void Equip(int equipmentId)
    {
        var equipment = playerInfo.equipmentBag.GetById(equipmentId);

        if (playerInfo.equipped.HasPart(equipment.part))
            UnEquip(equipment.part);

        if (playerInfo.equipped.Equip(equipment))
            playerInfo.equipmentBag.Remove(equipmentId);

        playerInfo.UpdateStat();
        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList(), playerInfo.maxStat);
        mapData.UpdatePlayerStat();
    }

    public void UnEquip(Equipment.Part part)
    {
        var equipment = playerInfo.equipped.UnEquip(part);

        if (equipment != null)
            playerInfo.equipmentBag.Add(equipment);

        playerInfo.UpdateStat();
        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList(), playerInfo.maxStat);
        mapData.UpdatePlayerStat();
    }

    public long GetPlayerMoney()
    {
        return playerInfo.money;
    }

    public List<SkillData> GetEquippedSkillDatas()
    {
        return playerInfo.skillData.GetEquippedSkills();
    }

    public void SkillLevelUp(int skillCode)
    {
        var nowLevel = playerInfo.skillData.GetSkillLevel(skillCode);
        if (nowLevel < Managers.TableData.GetSkillMaxLevel(skillCode))
            playerInfo.skillData.SkillLevelUp(skillCode);
    }

    public void EquipSkill(int skillCode)
    {
        playerInfo.skillData.EquipSkill(skillCode);
        Managers.UIManager.GetPopup<InfoPopup>().SetSkillTab(playerInfo.skillData.GetSkills(), GetEquippedSkillDatas());
    }

    public void UnEquipSkill(int skillCode)
    {
        playerInfo.skillData.UnEquipSkill(skillCode);
        Managers.UIManager.GetPopup<InfoPopup>().SetSkillTab(playerInfo.skillData.GetSkills(), GetEquippedSkillDatas());
    }
}

public struct Stat
{
    public long hp;
    public long mp;
    public long attack;
    public float speed;
    public float mastery;

    public void Add(Stat stat)
    {
        hp += stat.hp;
        mp += stat.mp;
        attack += stat.attack;
        speed += stat.speed;
        mastery += stat.mastery;
    }
}

public class playerInfo
{
    public int level;
    public long nowExp;
    public long money;
    Stat pureStat;
    public Stat maxStat;
    public Bag miscBag = new Bag(999);
    public EquipmentBag equipmentBag = new();
    public EquippedEquipments equipped = new();
    public SkillDataSet skillData = new SkillDataSet();

    public playerInfo(int level)
    {
        this.level = level;
        pureStat = new Stat();
        SetLevelStat(TableData.GetStatPerLevel(level));
        UpdateStat();
    }

    public void ModifyExp(long exp, Action onLevelUp)
    {
        nowExp += exp;

        if (nowExp < 0)
            nowExp = 0;

        while (nowExp >= TableData.GetMaxExp(level))
            LevelUp(onLevelUp);
    }

    public void UpdateStat()
    {
        maxStat = new Stat();
        maxStat.Add(pureStat);
        maxStat.Add(equipped.GetStat());
    }

    void SetLevelStat(Stat stat)
    {
        pureStat = stat;
        UpdateStat();
    }

    void LevelUp(Action onLevelUp)
    {
        nowExp -= TableData.GetMaxExp(level);
        level++;
        //SetLevelStat(TableData.GetStatPerLevel(level));
        onLevelUp?.Invoke();
    }
}

public class Quest
{
    public int id;
    public int targetAmount;
    public int nowAmount;
    public bool isComplete;
}

public class ItemData
{
    public int itemCode;
    public int count;

    public ItemData(int itemCode, int count)
    {
        this.itemCode = itemCode;
        this.count = count;
    }
}

public class Bag
{
    public List<ItemData> bag = new List<ItemData>();
    public int maxItemPerSlot;

    public Bag(int maxItemPerSlot = 1)
    {
        this.maxItemPerSlot = maxItemPerSlot;
    }

    public void AddItem(int itemCode, int count)
    {
        if (itemCode == 0 || count <= 0)
            return;

        var remain = count;

        foreach (var itemData in bag)
        {
            if (remain > 0 && itemData.itemCode == itemCode)
            {
                itemData.count += count;
                remain = 0;

                if (itemData.count > maxItemPerSlot)
                {
                    remain = itemData.count - maxItemPerSlot;
                    itemData.count = maxItemPerSlot;
                }
            }
        }

        while (remain > 0)
        {
            if (remain > maxItemPerSlot)
            {
                bag.Add(new ItemData(itemCode, maxItemPerSlot));
                remain -= maxItemPerSlot;
            }
            else
            {
                bag.Add(new ItemData(itemCode, remain));
                remain = 0;
            }
        }
    }

    public void Clear()
    {
        bag.Clear();
    }

    public List<ItemData> ToList()
    {
        return bag;
    }
}

public class SkillData
{
    public int code { get; private set; }
    public int level { get; private set; }

    public SkillData(int code)
    {
        this.code = code;
        level = 0;
    }

    public void LevelUp()
    {
        level++;
    }
}

public class SkillDataSet
{
    Dictionary<int, SkillData> skillDataDic = new Dictionary<int, SkillData>();
    List<int> equipSkills = new List<int>();
    int maxSkill = 3;

    public void SkillLevelUp(int skillCode)
    {
        if (!skillDataDic.ContainsKey(skillCode))
            skillDataDic.Add(skillCode, new SkillData(skillCode));

        skillDataDic[skillCode].LevelUp();
    }

    public int GetSkillLevel(int skillCode)
    {
        if (skillDataDic.ContainsKey(skillCode))
            return skillDataDic[skillCode].level;
        else
            return 0;
    }

    public void EquipSkill(int skillCode)
    {
        if (equipSkills.Contains(skillCode))
            return;

        if (equipSkills.Count >= maxSkill)
            return;

        if (skillDataDic.ContainsKey(skillCode) && skillDataDic[skillCode].level > 0)
            equipSkills.Add(skillCode);
    }

    public void UnEquipSkill(int skillCode)
    {
        if (equipSkills.Contains(skillCode))
            equipSkills.Remove(skillCode);
    }

    public List<SkillData> GetSkills()
    {
        return skillDataDic.Values.ToList();
    }

    public List<SkillData> GetEquippedSkills()
    {
        var skills = new List<SkillData>();

        foreach (var skillCode in equipSkills)
        {
            if (skillDataDic.ContainsKey(skillCode))
                skills.Add(skillDataDic[skillCode]);
        }
        
        return skills;
    }
}

public class Equipment
{
    public int id;
    public int code;
    public int upgradeLevel;
    public Stat stat;
    public Part part;

    public Equipment(int code, int upgradeLevel, Part part)
    {
        this.code = code;
        this.upgradeLevel = upgradeLevel;
        this.part = part;
    }

    public enum Part
    {
        None = 0,
        Weapon = 1,
        Necklace = 2,
        Gloves = 3,
        Hat = 4,
        Armor = 5,
        Shoes = 6
    }
}

public class EquipmentBag
{
    List<Equipment> bag = new List<Equipment>();
    int nextId;

    public void Add(Equipment item)
    {
        item.id = nextId++;
        bag.Add(item);
    }

    public void Remove(int id)
    {
        for (int i = 0; i < bag.Count; i++)
        {
            if (bag[i].id == id)
            {
                bag.RemoveAt(i);
                break;
            }
        }
    }

    public Equipment GetById(int id)
    {
        foreach (var equipment in bag)
            if (equipment.id == id)
                return equipment;

        Debug.LogError($"Equipment GetById() Can't find id = {id}");
        return default;
    }

    public List<Equipment> ToList()
    {
        return bag;
    }
}

public class EquippedEquipments
{
    List<Equipment> equipped = new List<Equipment>();

    public bool Equip(Equipment equipment)
    {
        if (HasPart(equipment.part))
            return false;
        else
        {
            equipped.Add(equipment);
            return true;
        }
    }

    public Equipment UnEquip(Equipment.Part part)
    {
        if (HasPart(part))
        {
            var equipment = GetPart(part);
            equipped.Remove(equipment);
            return equipment;
        }
        return null;
    }

    public bool HasPart(Equipment.Part part)
    {
        foreach (var equipment in equipped)
        {
            if (equipment.part == part)
                return true;
        }

        return false;
    }

    public Equipment GetPart(Equipment.Part part)
    {
        if (HasPart(part))
        {
            foreach (var equipment in equipped)
            {
                if (equipment.part == part)
                    return equipment;
            }
        }

        return null;
    }

    public List<Equipment> ToList()
    {
        return equipped;
    }

    public Stat GetStat()
    {
        var stat = new Stat();

        foreach (var equipment in equipped)
            stat.Add(Managers.TableData.GetEquipmentStat(equipment));

        return stat;
    }
}

public class BattleUnit
{
    public int id { get; private set; }
    public Stat MaxStat => maxStat;
    public Stat NowStat => nowStat;

    Stat maxStat;
    Stat nowStat;

    public BattleUnit(int id, Stat stat)
    {
        this.id = id;
        maxStat = stat;
    }

    public void Respawn()
    {
        nowStat = maxStat;
    }

    public void SetStat(Stat stat)
    {
        maxStat = stat;
    }

    public void ReduceHp(long hp)
    {
        nowStat.hp -= hp;
        if (nowStat.hp < 0)
            nowStat.hp = 0;
    }

    public void TakeDamage(long damage)
    {
        nowStat.mp -= damage;

        if(nowStat.mp < 0)
        {
            nowStat.hp += nowStat.mp;
            nowStat.mp = 0;

            if (nowStat.hp < 0)
                nowStat.hp = 0;
        }
    }

    public void HealHp(long hp)
    {
        nowStat.hp += hp;
        if (nowStat.hp > maxStat.hp)
            nowStat.hp = maxStat.hp;
    }

    public void HealMp(long mp)
    {
        nowStat.mp += mp;
        if (nowStat.mp > maxStat.mp)
            nowStat.mp = maxStat.mp;
    }

    public long GetAttack()
    {
        return nowStat.attack;
    }

    public bool IsDead()
    {
        return nowStat.hp <= 0;
    }
}
