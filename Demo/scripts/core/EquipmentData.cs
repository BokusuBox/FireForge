// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class EquipmentData
{
    public int Id { get; set; }
    public string EquipmentName { get; set; } = "";
    public EquipmentSlot Slot { get; set; }
    public EquipmentRarity Rarity { get; set; }
    public int ILvl { get; set; }
    public int MaxAP { get; set; }
    public int CurrentAP { get; set; }
    public int BaseAttack { get; set; }
    public int BaseArmor { get; set; }
    public int PrefixSlots { get; set; }
    public int SuffixSlots { get; set; }
    public List<int> SkillPool { get; set; } = new();
    public List<AffixData> Prefixes { get; set; } = new();
    public List<AffixData> Suffixes { get; set; } = new();
    public bool IsCorrupted { get; set; }

    public int UsedPrefixSlots => Prefixes.Count;
    public int UsedSuffixSlots => Suffixes.Count;
    public bool HasRemainingAP => CurrentAP > 0;
    public bool CanCraft => !IsCorrupted && (HasRemainingAP || CurrentAP == 0);
    public bool HasPrefixSpace => UsedPrefixSlots < PrefixSlots;
    public bool HasSuffixSpace => UsedSuffixSlots < SuffixSlots;

    public static EquipmentData FromRow(EquipmentRow row)
    {
        return new EquipmentData
        {
            Id = row.Id,
            EquipmentName = row.EquipmentName,
            Slot = row.Slot,
            Rarity = row.Rarity,
            ILvl = row.ILvl,
            MaxAP = row.MaxAP,
            CurrentAP = row.MaxAP,
            BaseAttack = row.BaseAttack,
            BaseArmor = row.BaseArmor,
            PrefixSlots = row.PrefixSlots,
            SuffixSlots = row.SuffixSlots,
            SkillPool = new List<int>(row.SkillPool),
        };
    }
}
