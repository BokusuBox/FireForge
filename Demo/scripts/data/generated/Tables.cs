// 自动生成的代码 - 请勿手动修改
// 强类型表包装：由导表工具根据数据表自动生成

using Godot;
using System;
using System.Collections.Generic;

public class AdventurerRow
{
    private readonly TableRecord _raw;
    public AdventurerRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string AdventurerName => _raw.GetString("adventurer_name");
    public AdventurerRole Role => _raw.GetEnum<AdventurerRole>("role");
    public int Level => _raw.GetInt("level");
    public int Hp => _raw.GetInt("hp");
    public int BaseAttack => _raw.GetInt("base_attack");
    public int BaseArmor => _raw.GetInt("base_armor");
    public float AttackSpeed => _raw.GetFloat("attack_speed");
    public float MoveSpeed => _raw.GetFloat("move_speed");
    public float CritRate => _raw.GetFloat("crit_rate");
    public float CritDmgMultiplier => _raw.GetFloat("crit_dmg_multiplier");
    public float Cdr => _raw.GetFloat("cdr");
    public List<int> PassiveTrait => _raw.GetIntList("passive_trait");
}

public class ArchetypeRow
{
    private readonly TableRecord _raw;
    public ArchetypeRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string ArchetypeName => _raw.GetString("archetype_name");
    public ArchetypeDimension Dimension => _raw.GetEnum<ArchetypeDimension>("dimension");
    public int MinDifficulty => _raw.GetInt("min_difficulty");
    public int MaxDifficulty => _raw.GetInt("max_difficulty");
    public List<MaterialTag> BaseMaterialTags => _raw.GetStringList("base_material_tags").ConvertAll(s => Enum.Parse<MaterialTag>(s));
    public string CorrectAffixGroups => _raw.GetString("correct_affix_groups");
    public int SquadPoolId => _raw.GetInt("squad_pool_id");
    public SceneType SceneAnchor => _raw.GetEnum<SceneType>("scene_anchor");
    public float EnemyIlvlScale => _raw.GetFloat("enemy_ilvl_scale");
    public int DefaultTier => _raw.GetInt("default_tier");
    public int BaseDamage => _raw.GetInt("base_damage");
    public string Description => _raw.GetString("description");
}

public class ArchetypeSquadRow
{
    private readonly TableRecord _raw;
    public ArchetypeSquadRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public int SquadPoolId => _raw.GetInt("squad_pool_id");
    public int AdventurerId => _raw.GetInt("adventurer_id");
    public SquadRole RoleLabel => _raw.GetEnum<SquadRole>("role_label");
    public int Count => _raw.GetInt("count");
}

public class CurrencyRow
{
    private readonly TableRecord _raw;
    public CurrencyRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string CurrencyName => _raw.GetString("currency_name");
    public CurrencyType CurrencyType => _raw.GetEnum<CurrencyType>("currency_type");
    public int ApCost => _raw.GetInt("ap_cost");
    public CurrencyEffect EffectType => _raw.GetEnum<CurrencyEffect>("effect_type");
    public bool IsCorruption => _raw.GetBool("is_corruption");
    public int MinWorkshopLevel => _raw.GetInt("min_workshop_level");
    public string Description => _raw.GetString("description");
}

public class AffixRow
{
    private readonly TableRecord _raw;
    public AffixRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public int GroupId => _raw.GetInt("group_id");
    public string GroupName => _raw.GetString("group_name");
    public AffixSlotType SlotType => _raw.GetEnum<AffixSlotType>("slot_type");
    public int Tier => _raw.GetInt("tier");
    public int RequiredILvl => _raw.GetInt("required_i_lvl");
    public int Weight => _raw.GetInt("weight");
    public Dictionary<StatType, float> StatModifiers => _raw.GetDict<StatType, float>("stat_modifiers");
    public string Tag => _raw.GetString("tag");
}

public class EquipmentRow
{
    private readonly TableRecord _raw;
    public EquipmentRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string EquipmentName => _raw.GetString("equipment_name");
    public EquipmentSlot Slot => _raw.GetEnum<EquipmentSlot>("slot");
    public EquipmentRarity Rarity => _raw.GetEnum<EquipmentRarity>("rarity");
    public int ILvl => _raw.GetInt("i_lvl");
    public int MaxAp => _raw.GetInt("max_ap");
    public int BaseAttack => _raw.GetInt("base_attack");
    public int BaseArmor => _raw.GetInt("base_armor");
    public int PrefixSlots => _raw.GetInt("prefix_slots");
    public int SuffixSlots => _raw.GetInt("suffix_slots");
    public List<int> SkillPool => _raw.GetIntList("skill_pool");
}

public class OrderRow
{
    private readonly TableRecord _raw;
    public OrderRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string OrderName => _raw.GetString("order_name");
    public OrderType OrderType => _raw.GetEnum<OrderType>("order_type");
    public OrderDifficulty Difficulty => _raw.GetEnum<OrderDifficulty>("difficulty");
    public int MinReputation => _raw.GetInt("min_reputation");
    public int ArchetypeId => _raw.GetInt("archetype_id");
    public OrderVariant Variant => _raw.GetEnum<OrderVariant>("variant");
    public int AdventurerId => _raw.GetInt("adventurer_id");
    public int EquipmentCount => _raw.GetInt("equipment_count");
    public int RewardGold => _raw.GetInt("reward_gold");
    public int RewardReputation => _raw.GetInt("reward_reputation");
    public string Description => _raw.GetString("description");
}

public class ReputationRow
{
    private readonly TableRecord _raw;
    public ReputationRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public int Level => _raw.GetInt("level");
    public string LevelName => _raw.GetString("level_name");
    public int Threshold => _raw.GetInt("threshold");
    public List<OrderType> OrderUnlock => _raw.GetStringList("order_unlock").ConvertAll(s => Enum.Parse<OrderType>(s));
    public string Description => _raw.GetString("description");
}

public class SkillRow
{
    private readonly TableRecord _raw;
    public SkillRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string SkillName => _raw.GetString("skill_name");
    public SkillType SkillType => _raw.GetEnum<SkillType>("skill_type");
    public SkillTrigger Trigger => _raw.GetEnum<SkillTrigger>("trigger");
    public string ThresholdLevels => _raw.GetString("threshold_levels");
    public float BaseCooldown => _raw.GetFloat("base_cooldown");
    public string Description => _raw.GetString("description");
}

public class TraitRow
{
    private readonly TableRecord _raw;
    public TraitRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public TraitType TraitType => _raw.GetEnum<TraitType>("trait_type");
    public string TraitName => _raw.GetString("trait_name");
    public TraitTriggerType TriggerType => _raw.GetEnum<TraitTriggerType>("trigger_type");
    public string TriggerCondition => _raw.GetString("trigger_condition");
    public TraitEffectType EffectType => _raw.GetEnum<TraitEffectType>("effect_type");
    public StatType StatType => _raw.GetEnum<StatType>("stat_type");
    public float Value => _raw.GetFloat("value");
    public float Duration => _raw.GetFloat("duration");
    public int Priority => _raw.GetInt("priority");
    public string Description => _raw.GetString("description");
}

public class AdventurerTable
{
    private readonly TableData _raw;
    private List<AdventurerRow> _rows;

    public AdventurerTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<AdventurerRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new AdventurerRow(r));
        return _rows;
    }

    public AdventurerRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new AdventurerRow(record) : null;
    }

    public AdventurerRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new AdventurerRow(record) : null;
    }

    public List<AdventurerRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new AdventurerRow(r));
    }
}

public class ArchetypeTable
{
    private readonly TableData _raw;
    private List<ArchetypeRow> _rows;

    public ArchetypeTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<ArchetypeRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new ArchetypeRow(r));
        return _rows;
    }

    public ArchetypeRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new ArchetypeRow(record) : null;
    }

    public ArchetypeRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new ArchetypeRow(record) : null;
    }

    public List<ArchetypeRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new ArchetypeRow(r));
    }
}

public class ArchetypeSquadTable
{
    private readonly TableData _raw;
    private List<ArchetypeSquadRow> _rows;

    public ArchetypeSquadTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<ArchetypeSquadRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new ArchetypeSquadRow(r));
        return _rows;
    }

    public ArchetypeSquadRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new ArchetypeSquadRow(record) : null;
    }

    public ArchetypeSquadRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new ArchetypeSquadRow(record) : null;
    }

    public List<ArchetypeSquadRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new ArchetypeSquadRow(r));
    }
}

public class CurrencyTable
{
    private readonly TableData _raw;
    private List<CurrencyRow> _rows;

    public CurrencyTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<CurrencyRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new CurrencyRow(r));
        return _rows;
    }

    public CurrencyRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new CurrencyRow(record) : null;
    }

    public CurrencyRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new CurrencyRow(record) : null;
    }

    public List<CurrencyRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new CurrencyRow(r));
    }
}

public class AffixTable
{
    private readonly TableData _raw;
    private List<AffixRow> _rows;

    public AffixTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<AffixRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new AffixRow(r));
        return _rows;
    }

    public AffixRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new AffixRow(record) : null;
    }

    public AffixRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new AffixRow(record) : null;
    }

    public List<AffixRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new AffixRow(r));
    }
}

public class EquipmentTable
{
    private readonly TableData _raw;
    private List<EquipmentRow> _rows;

    public EquipmentTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<EquipmentRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new EquipmentRow(r));
        return _rows;
    }

    public EquipmentRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new EquipmentRow(record) : null;
    }

    public EquipmentRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new EquipmentRow(record) : null;
    }

    public List<EquipmentRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new EquipmentRow(r));
    }
}

public class OrderTable
{
    private readonly TableData _raw;
    private List<OrderRow> _rows;

    public OrderTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<OrderRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new OrderRow(r));
        return _rows;
    }

    public OrderRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new OrderRow(record) : null;
    }

    public OrderRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new OrderRow(record) : null;
    }

    public List<OrderRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new OrderRow(r));
    }
}

public class ReputationTable
{
    private readonly TableData _raw;
    private List<ReputationRow> _rows;

    public ReputationTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<ReputationRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new ReputationRow(r));
        return _rows;
    }

    public ReputationRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new ReputationRow(record) : null;
    }

    public ReputationRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new ReputationRow(record) : null;
    }

    public List<ReputationRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new ReputationRow(r));
    }
}

public class SkillTable
{
    private readonly TableData _raw;
    private List<SkillRow> _rows;

    public SkillTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<SkillRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new SkillRow(r));
        return _rows;
    }

    public SkillRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new SkillRow(record) : null;
    }

    public SkillRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new SkillRow(record) : null;
    }

    public List<SkillRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new SkillRow(r));
    }
}

public class TraitTable
{
    private readonly TableData _raw;
    private List<TraitRow> _rows;

    public TraitTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<TraitRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new TraitRow(r));
        return _rows;
    }

    public TraitRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new TraitRow(record) : null;
    }

    public TraitRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new TraitRow(record) : null;
    }

    public List<TraitRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new TraitRow(r));
    }
}

public static class Tables
{
    private static AdventurerTable _adventurer;
    public static AdventurerTable Adventurer
    {
        get
        {
            if (_adventurer == null)
                _adventurer = new AdventurerTable(TableManager.Instance.GetTable("adventurer"));
            return _adventurer;
        }
    }

    private static ArchetypeTable _archetype;
    public static ArchetypeTable Archetype
    {
        get
        {
            if (_archetype == null)
                _archetype = new ArchetypeTable(TableManager.Instance.GetTable("archetype"));
            return _archetype;
        }
    }

    private static ArchetypeSquadTable _archetype_squad;
    public static ArchetypeSquadTable ArchetypeSquad
    {
        get
        {
            if (_archetype_squad == null)
                _archetype_squad = new ArchetypeSquadTable(TableManager.Instance.GetTable("archetype_squad"));
            return _archetype_squad;
        }
    }

    private static CurrencyTable _currency;
    public static CurrencyTable Currency
    {
        get
        {
            if (_currency == null)
                _currency = new CurrencyTable(TableManager.Instance.GetTable("currency"));
            return _currency;
        }
    }

    private static AffixTable _affix;
    public static AffixTable Affix
    {
        get
        {
            if (_affix == null)
                _affix = new AffixTable(TableManager.Instance.GetTable("affix"));
            return _affix;
        }
    }

    private static EquipmentTable _equipment;
    public static EquipmentTable Equipment
    {
        get
        {
            if (_equipment == null)
                _equipment = new EquipmentTable(TableManager.Instance.GetTable("equipment"));
            return _equipment;
        }
    }

    private static OrderTable _order;
    public static OrderTable Order
    {
        get
        {
            if (_order == null)
                _order = new OrderTable(TableManager.Instance.GetTable("order"));
            return _order;
        }
    }

    private static ReputationTable _reputation;
    public static ReputationTable Reputation
    {
        get
        {
            if (_reputation == null)
                _reputation = new ReputationTable(TableManager.Instance.GetTable("reputation"));
            return _reputation;
        }
    }

    private static SkillTable _skill;
    public static SkillTable Skill
    {
        get
        {
            if (_skill == null)
                _skill = new SkillTable(TableManager.Instance.GetTable("skill"));
            return _skill;
        }
    }

    private static TraitTable _trait;
    public static TraitTable Trait
    {
        get
        {
            if (_trait == null)
                _trait = new TraitTable(TableManager.Instance.GetTable("trait"));
            return _trait;
        }
    }

}
