// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class AdventurerData
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
    public Dictionary<EquipmentSlot, EquipmentData> EquippedItems { get; set; } = new();

    public float GetAggregatedStat(StatType type)
    {
        float total = 0f;
        foreach (var eq in EquippedItems.Values)
        {
            foreach (var affix in eq.Prefixes)
                total += affix.GetStat(type);
            foreach (var affix in eq.Suffixes)
                total += affix.GetStat(type);
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

    public bool Equip(EquipmentData equipment)
    {
        if (EquippedItems.ContainsKey(equipment.Slot))
            return false;
        EquippedItems[equipment.Slot] = equipment;
        return true;
    }

    public EquipmentData Unequip(EquipmentSlot slot)
    {
        if (!EquippedItems.TryGetValue(slot, out var equipment))
            return null;
        EquippedItems.Remove(slot);
        return equipment;
    }

    public static AdventurerData FromRow(AdventurerRow row)
    {
        var data = new AdventurerData
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

        return data;
    }
}
