// 挂载于: 无（纯数据模型，非节点脚本）

using System;
using System.Collections.Generic;

public class TraitData
{
    public int Id { get; set; }
    public TraitType TraitType { get; set; }
    public string TraitName { get; set; } = "";
    public TraitTriggerType TriggerType { get; set; }
    public string TriggerCondition { get; set; } = "";
    public TraitEffectType EffectType { get; set; }
    public StatType? StatType { get; set; }
    public float Value { get; set; }
    public float Duration { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; } = "";

    public static TraitData FromRow(TraitRow row)
    {
        return new TraitData
        {
            Id = row.Id,
            TraitType = row.TraitType,
            TraitName = row.TraitName,
            TriggerType = row.TriggerType,
            TriggerCondition = row.TriggerCondition,
            EffectType = row.EffectType,
            StatType = row.StatType,
            Value = row.Value,
            Duration = row.Duration,
            Priority = row.Priority,
            Description = row.Description,
        };
    }

    public bool IsPermanent => Duration <= 0;

    public bool HasCondition => !string.IsNullOrEmpty(TriggerCondition);

    public bool IsStatModifier => EffectType == TraitEffectType.StatMultiplier ||
                                   EffectType == TraitEffectType.StatAdditive;

    public float GetMultiplier()
    {
        return EffectType switch
        {
            TraitEffectType.StatMultiplier => Value,
            TraitEffectType.StatAdditive => Value,
            _ => 1f,
        };
    }

    public string GetMultiplierCategory()
    {
        return EffectType switch
        {
            TraitEffectType.StatMultiplier => "more",
            TraitEffectType.StatAdditive => "increased",
            _ => "none",
        };
    }
}
