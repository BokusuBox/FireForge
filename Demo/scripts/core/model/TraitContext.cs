// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class TraitContext
{
    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public float CurrentHp { get; set; }
    public float MaxHp { get; set; }
    public float HpPercent => MaxHp > 0 ? (CurrentHp / MaxHp) * 100f : 0f;
    public float CombatTime { get; set; }
    public int KillCount { get; set; }
    public int HitCount { get; set; }
    public float LastDamageTaken { get; set; }
    public float LastHealAmount { get; set; }
    public bool IsInCombat { get; set; }
    public Dictionary<string, float> CustomValues { get; set; } = new();

    public float GetValue(string key)
    {
        return key.ToLower() switch
        {
            "hp" => CurrentHp,
            "maxhp" => MaxHp,
            "hppct" or "hp_percent" => HpPercent,
            "combat_time" => CombatTime,
            "kills" => KillCount,
            "hits" => HitCount,
            "last_damage" => LastDamageTaken,
            "last_heal" => LastHealAmount,
            _ => CustomValues.TryGetValue(key.ToLower(), out var val) ? val : 0f,
        };
    }

    public void SetValue(string key, float value)
    {
        CustomValues[key.ToLower()] = value;
    }

    public static TraitContext ForAdventurer(int adventurerId, float hp, float maxHp)
    {
        return new TraitContext
        {
            SourceId = adventurerId,
            CurrentHp = hp,
            MaxHp = maxHp,
        };
    }
}
