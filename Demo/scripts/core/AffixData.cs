// 挂载于: 无（纯数据模型，非节点脚本）

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
    public Dictionary<string, int> StatModifiers { get; set; } = new();
    public string Tag { get; set; } = "";

    public bool IsInGroup(int groupId) => GroupId == groupId;

    public bool MeetsILvl(int iLvl) => RequiredILvl <= iLvl;

    public bool IsHighestTier => Tier == 1;

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
                if (parts.Length == 2 && int.TryParse(parts[1], out var val))
                    data.StatModifiers[parts[0].Trim()] = val;
            }
        }

        return data;
    }
}
