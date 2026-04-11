// 挂载于: 无（运行时实例模型，非节点脚本）

using System.Collections.Generic;

public class Adventurer
{
    public int Id { get; set; }
    public string AdventurerName { get; set; } = "";
    public AdventurerRole Role { get; set; }
    public int Level { get; set; }
    public int Hp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseArmor { get; set; }
    public float AttackSpeed { get; set; }
    public float MoveSpeed { get; set; }
    public float CritRate { get; set; }
    public float CritDmgMultiplier { get; set; }
    public float Cdr { get; set; }
    public List<int> PassiveTraitIds { get; set; } = new();
    public Dictionary<EquipmentSlot, Equipment> EquippedItems { get; set; } = new();

    public float GetAggregatedStat(StatType type)
    {
        float total = 0f;
        foreach (var eq in EquippedItems.Values)
        {
            foreach (var affix in eq.Prefixes)
                if (affix.StatModifiers.TryGetValue(type, out var v))
                    total += v;
            foreach (var affix in eq.Suffixes)
                if (affix.StatModifiers.TryGetValue(type, out var v))
                    total += v;
        }
        return total;
    }

    public int TotalAttack
    {
        get
        {
            int bonus = 0;
            foreach (var eq in EquippedItems.Values)
                bonus += eq.BaseAttack;
            return BaseAttack + bonus + (int)GetAggregatedStat(StatType.Attack);
        }
    }

    public int TotalArmor
    {
        get
        {
            int bonus = 0;
            foreach (var eq in EquippedItems.Values)
                bonus += eq.BaseArmor;
            return BaseArmor + bonus + (int)GetAggregatedStat(StatType.Armor);
        }
    }

    public bool Equip(Equipment equipment)
    {
        if (EquippedItems.ContainsKey(equipment.Slot))
            return false;
        EquippedItems[equipment.Slot] = equipment;
        return true;
    }

    public Equipment Unequip(EquipmentSlot slot)
    {
        if (!EquippedItems.TryGetValue(slot, out var equipment))
            return null;
        EquippedItems.Remove(slot);
        return equipment;
    }

    public static Adventurer FromRow(AdventurerRow row)
    {
        return new Adventurer
        {
            Id = row.Id,
            AdventurerName = row.AdventurerName,
            Role = row.Role,
            Level = row.Level,
            Hp = row.Hp,
            BaseAttack = row.BaseAttack,
            BaseArmor = row.BaseArmor,
            AttackSpeed = row.AttackSpeed,
            MoveSpeed = row.MoveSpeed,
            CritRate = row.CritRate,
            CritDmgMultiplier = row.CritDmgMultiplier,
            Cdr = row.Cdr,
            PassiveTraitIds = new List<int>(row.PassiveTrait),
        };
    }
}
