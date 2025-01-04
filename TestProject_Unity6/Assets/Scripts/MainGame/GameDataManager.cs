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
    Dictionary<int, DateTime> itemCoolEnds = new Dictionary<int, DateTime>();
    Dictionary<int, DateTime> buffEndTimes = new Dictionary<int, DateTime>();
    Dictionary<int, bool> buffActives = new Dictionary<int, bool>();

    private void Awake()
    {
        playerInfo = new playerInfo(1);
    }

    private void Start()
    {
        MakePlayerHpFull();
        ModifyPlayerMp(0);
        ModifyPlayerExp(0);
        ModifyPlayerMoney(1000000);
        StartCoroutine(ChargeMpCo(1f));
    }

    private void Update()
    {
        foreach (var buff in buffEndTimes)
        {
            var skillId = buff.Key;

            if (buffActives[skillId] == true)
            {
                if (buffEndTimes[skillId] < DateTime.Now)
                {
                    buffActives[skillId] = false;
                    Managers.MonsterManager.player.speed -= TableData.GetSkillBuffStat(skillId, 1).speed;
                }
            }
        }
    }

    public void ModifyPlayerHp(long hp)
    {
        playerInfo.nowStat.hp += hp;
        if (playerInfo.nowStat.hp < 0) playerInfo.nowStat.hp = 0;
        else if (playerInfo.nowStat.hp > playerInfo.maxStat.hp) playerInfo.nowStat.hp = playerInfo.maxStat.hp;

        Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerInfo.maxStat.hp, playerInfo.nowStat.hp);
    }

    public void ModifyPlayerMp(long mp)
    {
        playerInfo.nowStat.mp += mp;
        if (playerInfo.nowStat.mp < 0) playerInfo.nowStat.mp = 0;
        else if (playerInfo.nowStat.mp > playerInfo.maxStat.mp) playerInfo.nowStat.mp = playerInfo.maxStat.mp;

        Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerInfo.maxStat.mp, playerInfo.nowStat.mp);
    }

    public void ModifyPlayerMoney(long money)
    {
        playerInfo.money += money;
        Managers.UIManager.GetPopup<InfoPopup>().SetMoney(playerInfo.money);
        Managers.UIManager.GetPopup<EquipmentShopPopup>().SetMoney(playerInfo.money);
    }

    public void DamagePlayer(long damage)
    {
        if (playerInfo.nowStat.mp < damage)
        {
            var remainDamage = damage - playerInfo.nowStat.mp;
            ModifyPlayerMp(-playerInfo.nowStat.mp);
            ModifyPlayerHp(-remainDamage);
        }
        else
        {
            ModifyPlayerMp(-damage);
        }
    }

    public void ModifyPlayerExp(long exp)
    {
        playerInfo.ModifyExp(exp, () => Managers.effect.ShowEffect(4, Managers.MonsterManager.player.transform.position, Managers.MonsterManager.player.transform));
        Managers.UIManager.GetLayout<StateLayout>().SetUserExpBar(TableData.GetMaxExp(playerInfo.level), playerInfo.nowExp);
        Managers.UIManager.GetLayout<StateLayout>().SetLevel(playerInfo.level);
    }

    public void MakePlayerHpFull()
    {
        var needHp = playerInfo.maxStat.hp - playerInfo.nowStat.hp;
        ModifyPlayerHp(needHp);
    }

    public void AddMiscItemToBag(int itemCode, int count)
    {
        playerInfo.miscBag.AddItem(itemCode, count);
        Managers.UIManager.GetPopup<InfoPopup>().SetBagTab(playerInfo.miscBag.ToList());
    }

    public long GetPlayerDamage()
    {
        return GetDamage(playerInfo.nowStat);
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

    public void UseItem(int itemId)
    {
        var itemCool = 5f;

        if (!itemCoolEnds.ContainsKey(itemId))
            itemCoolEnds.Add(itemId, DateTime.MinValue);

        if (DateTime.Now > itemCoolEnds[itemId])
        {
            itemCoolEnds[itemId] = DateTime.Now.AddSeconds(itemCool);
            ModifyPlayerHp(10);
            Managers.UIManager.GetLayout<VirtualButtonLayout>().StartItemCoolTime(itemId, itemCool);
        }
    }

    // Shop
    public void ShowMiscShop()
    {
        Managers.UIManager.ShowPopup<MiscShopPopup>().SetPopup(playerInfo.miscBag.ToList());
    }

    public void SellAllItems()
    {
        var bag = playerInfo.miscBag.ToList();
        var price = 0L;

        foreach (var itemData in bag)
            price += TableData.GetSellPrice(itemData.itemCode, itemData.count);

        playerInfo.miscBag.Clear();
        ModifyPlayerMoney(price);

        Managers.UIManager.GetPopup<InfoPopup>().SetBagTab(playerInfo.miscBag.ToList());
        Managers.UIManager.GetPopup<MiscShopPopup>().SetPopup(playerInfo.miscBag.ToList());
    }

    public void UseBuffSkill(int skillId)
    {
        var buffTime = TableData.GetSkillBuffTime(skillId, 1);
        var buffStat = TableData.GetSkillBuffStat(skillId, 1);

        if (!buffEndTimes.ContainsKey(skillId))
            buffEndTimes.Add(skillId, DateTime.MinValue);

        if (!buffActives.ContainsKey(skillId))
            buffActives.Add(skillId, false);

        buffEndTimes[skillId] = DateTime.Now.AddSeconds(buffTime);
        buffActives[skillId] = true;

        Managers.MonsterManager.player.speed = playerInfo.nowStat.speed + buffStat.speed;
        Managers.UIManager.GetLayout<StateLayout>().SetSkillDutaion(skillId, buffTime);
    }

    // skillId, skillDamage
    public List<Tuple<int, long>> UseAttackSkill()
    {
        var skillPossibility = new float[] { 0.2f, 0.1f };
        var usingSkills = new List<Tuple<int, long>>();

        for (int i = 0; i < skillPossibility.Length; i++)
        {
            if (Random.Range(0f, 1f) < skillPossibility[i])
                usingSkills.Add(new Tuple<int, long>(i, GetSkillDamage(i)));
        }

        return usingSkills;
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

    public long GetDamage(Stat stat)
    {
        var rand = Random.Range(stat.mastery, 1f);
        return (long)(rand * stat.attack);
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

        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList());
    }

    public void AddEquipment(int equipmentCode, int upgradeLevel, Equipment.Part part)
    {
        playerInfo.equipmentBag.Add(new Equipment(equipmentCode, upgradeLevel, part));

        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList());
        Managers.UIManager.ShowPopup<MessagePopup>().SetPopup("안내", $"{TableData.GetEquipmentName(equipmentCode)}이(가) 구매 완료되었습니다.");
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
        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList());
    }

    public void UnEquip(Equipment.Part part)
    {
        var equipment = playerInfo.equipped.UnEquip(part);

        if (equipment != null)
            playerInfo.equipmentBag.Add(equipment);

        playerInfo.UpdateStat();
        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList(), playerInfo.equipped.ToList());
    }

    public long GetPlayerMoney()
    {
        return playerInfo.money;
    }

    long GetSkillDamage(int skillId)
    {
        if (skillId == 0)
            return GetPlayerDamage() * 2;
        else if (skillId == 1)
            return GetPlayerDamage() * 3;

        return 0;
    }

    IEnumerator ChargeMpCo(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            ModifyPlayerMp(1);
        }
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
    public Stat nowStat;
    public Bag miscBag = new Bag(999);
    public EquipmentBag equipmentBag = new();
    public EquippedEquipments equipped = new();

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

        nowStat = maxStat;
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
        SetLevelStat(TableData.GetStatPerLevel(level));
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
    Dictionary<int, int> skillLevels = new Dictionary<int, int>();
    Stat stat;

    public void SkillLevelUp(int skillCode)
    {
        if (!skillLevels.ContainsKey(skillCode))
            skillLevels.Add(skillCode, 0);

        stat.Add(TableData.GetSkillIncreaseStat(skillCode, skillLevels[skillCode]));

        if (skillLevels[skillCode] < TableData.GetSkillMaxLevel(skillCode))
            skillLevels[skillCode]++;
    }

    public int GetSkillLevel(int skillCode)
    {
        if (skillLevels.ContainsKey(skillCode))
            return skillLevels[skillCode];
        else
            return default;
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
            stat.Add(TableData.GetEquipmentStat(equipment));

        return stat;
    }
}

public class Monster
{
    public int id;
    public Stat maxStat;
    public Stat nowStat;

    public Monster(int id, long maxHp, long attack)
    {
        this.id = id;
        maxStat.hp = maxHp;
        maxStat.attack = attack;
    }

    public void ReSpawn()
    {
        nowStat = maxStat;
    }

    public bool IsDead()
    {
        return nowStat.hp <= 0;
    }

    public void TakeDamage(long damage)
    {
        nowStat.hp -= damage;
        if (nowStat.hp < 0)
            nowStat.hp = 0;
    }

    public long GetDamge()
    {
        return nowStat.attack;
    }
}
