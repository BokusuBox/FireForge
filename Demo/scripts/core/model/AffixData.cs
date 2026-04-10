// 挂载于: 无（纯数据模型，非节点脚本）

using System;
using System.Collections.Generic;

public class AffixData
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = "";
    public AffixSlotType SlotType { get; set; }
    public int Tier { get; set; }
    public int RequiredILvl { get; set; }
    public int Weight { get; set; }
    public Dictionary<StatType, float> StatModifiers { get; set; } = new();
    public string Tag { get; set; } = "";

    public bool IsInGroup(int groupId) => GroupId == groupId;

    public bool MeetsILvl(int iLvl) => RequiredILvl <= iLvl;

    public bool IsHighestTier => Tier == 1;

    public float GetStat(StatType type) => StatModifiers.TryGetValue(type, out var v) ? v : 0f;

    public static AffixData FromRow(AffixRow row)
    {
        var data = new AffixData
        {
            Id = row.Id,
            GroupId = row.GroupId,
            GroupName = row.GroupName,
            SlotType = row.SlotType,
            Tier = row.Tier,
            RequiredILvl = row.RequiredILvl,
            Weight = row.Weight,
            Tag = row.Tag,
        };

        var raw = row.StatModifiers;
        if (!string.IsNullOrEmpty(raw))
        {
            foreach (var pair in raw.Split('|'))
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    if (Enum.TryParse<StatType>(key, out var statType) &&
                        float.TryParse(parts[1].Trim(), out var val))
                    {
                        data.StatModifiers[statType] = val;
                    }
                }
            }
        }

        return data;
    }
}
