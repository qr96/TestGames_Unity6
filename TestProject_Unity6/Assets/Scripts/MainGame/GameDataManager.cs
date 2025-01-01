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
        playerInfo.equipmentBag.Add(new Equipment() { code = 1, upgradeLevel = 4 });
        playerInfo.equipmentBag.Add(new Equipment() { code = 2, upgradeLevel = 2 });
        playerInfo.equipmentBag.Add(new Equipment() { code = 4, upgradeLevel = 8 });
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
        playerInfo.stat.ModifyNowHp(hp);
        Managers.UIManager.GetLayout<StateLayout>().SetUserHpBar(playerInfo.stat.maxHp, playerInfo.stat.nowHp);
    }

    public void ModifyPlayerMp(long mp)
    {
        playerInfo.stat.ModifyNowMp(mp);
        Managers.UIManager.GetLayout<StateLayout>().SetUserMpBar(playerInfo.stat.maxMp, playerInfo.stat.nowMp);
    }

    public void ModifyPlayerMoney(long money)
    {
        playerInfo.money += money;
        Managers.UIManager.GetPopup<InfoPopup>().SetMoney(playerInfo.money);
        Managers.UIManager.GetPopup<EquipmentShopPopup>().SetMoney(playerInfo.money);
    }

    public void DamagePlayer(long damage)
    {
        if (playerInfo.stat.nowMp < damage)
        {
            var remainDamage = damage - playerInfo.stat.nowMp;
            ModifyPlayerMp(-playerInfo.stat.nowMp);
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
        var needHp = playerInfo.stat.maxHp - playerInfo.stat.nowHp;
        ModifyPlayerHp(needHp);
    }

    public void AddMiscItemToBag(int itemCode, int count)
    {
        playerInfo.miscBag.AddItem(itemCode, count);
        Managers.UIManager.GetPopup<InfoPopup>().SetBagTab(playerInfo.miscBag.ToList());
    }

    public long GetPlayerDamage()
    {
        return GetDamage(playerInfo.stat);
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

        Managers.MonsterManager.player.speed = playerInfo.stat.speed + buffStat.speed;
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

        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList());
    }

    public void AddEquipment(int equipmentCode, int upgradeLevel)
    {
        playerInfo.equipmentBag.Add(new Equipment() { code = equipmentCode, upgradeLevel = upgradeLevel });
        
        Managers.UIManager.GetPopup<InfoPopup>().SetEquipTab(playerInfo.equipmentBag.ToList());
        Managers.UIManager.ShowPopup<MessagePopup>().SetPopup("안내", $"{TableData.GetEquipmentName(equipmentCode)}이(가) 구매 완료되었습니다.");
    }

    public void RemoveEquipment(int equipmentId)
    {
        playerInfo.equipmentBag.Remove(equipmentId);
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

public class Stat
{
    public long maxHp;
    public long nowHp;
    public long maxMp;
    public long nowMp;
    public long attack;
    public float speed;
    public float mastery;

    public void ReSpawn()
    {
        nowHp = maxHp;
        nowMp = maxMp;
    }

    public void ModifyNowHp(long hp)
    {
        nowHp += hp;
        if (nowHp > maxHp) nowHp = maxHp;
        else if (nowHp < 0) nowHp = 0;
    }

    public void ModifyNowMp(long mp)
    {
        nowMp += mp;
        if (nowMp > maxMp) nowMp = maxMp;
        else if (nowMp < 0) nowMp = 0;
    }

    public void Add(Stat stat)
    {
        maxHp += stat.maxHp;
        nowHp += stat.nowHp;
        maxMp += stat.maxMp;
        nowMp += stat.nowMp;
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
    public Stat stat;
    public Stat skillStat;
    public Bag miscBag = new Bag(999);
    public EquipmentBag equipmentBag = new EquipmentBag();

    public playerInfo(int level)
    {
        this.level = level;
        stat = new Stat();
        stat.speed = 6f;
        stat.mastery = 0.5f;
        SetLevelStat(TableData.GetStatPerLevel(level));
    }

    public void ModifyExp(long exp, Action onLevelUp)
    {
        nowExp += exp;

        if (nowExp < 0)
            nowExp = 0;

        while (nowExp >= TableData.GetMaxExp(level))
            LevelUp(onLevelUp);
    }

    void SetLevelStat(Stat stat)
    {
        this.stat.maxHp = stat.maxHp;
        this.stat.maxMp = stat.maxMp;
        this.stat.attack = stat.attack;
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
