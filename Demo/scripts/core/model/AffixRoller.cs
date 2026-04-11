// 挂载于: 无（纯算法工具类，非节点脚本。由 CraftingManager 调用）

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AffixRoller
{
    private readonly AffixRegistry _registry;
    private Random _rng = new();

    public AffixRoller(AffixRegistry registry)
    {
        _registry = registry;
    }

    public AffixRow RollSingle(int iLvl, AffixSlotType slotType, HashSet<int> excludeGroupIds = null)
    {
        var candidates = _registry.GetCandidates(iLvl, slotType, excludeGroupIds);
        if (candidates.Count == 0) return null;

        return WeightedRoll(candidates);
    }

    public List<AffixRow> RollThreeChoices(int iLvl, AffixSlotType slotType, HashSet<int> excludeGroupIds = null)
    {
        var result = new List<AffixRow>();
        var usedGroupIds = new HashSet<int>();

        if (excludeGroupIds != null)
            foreach (var gid in excludeGroupIds)
                usedGroupIds.Add(gid);

        for (int i = 0; i < 3; i++)
        {
            var candidates = _registry.GetCandidates(iLvl, slotType, usedGroupIds);
            if (candidates.Count == 0) break;

            var picked = WeightedRoll(candidates);
            if (picked == null) break;

            result.Add(picked);
            usedGroupIds.Add(picked.GroupId);
        }

        return result;
    }

    public AffixRow RollSpecificGroup(int groupId, int iLvl)
    {
        var groupAffixes = _registry.GetByGroup(groupId);
        var candidates = groupAffixes.Where(a => a.RequiredILvl <= iLvl).ToList();
        if (candidates.Count == 0) return null;

        return WeightedRoll(candidates);
    }

    public AffixRow RollByTag(string tag, int iLvl, AffixSlotType slotType, HashSet<int> excludeGroupIds = null)
    {
        var candidates = _registry.GetCandidates(iLvl, slotType, excludeGroupIds);
        var tagged = candidates.Where(a => !string.IsNullOrEmpty(a.Tag) && a.Tag.Contains(tag)).ToList();
        if (tagged.Count == 0) return null;

        return WeightedRoll(tagged);
    }

    private AffixRow WeightedRoll(List<AffixRow> candidates)
    {
        if (candidates.Count == 0) return null;

        var totalWeight = candidates.Sum(a => a.Weight);
        var roll = _rng.Next(totalWeight);
        var cumulative = 0;

        foreach (var affix in candidates)
        {
            cumulative += affix.Weight;
            if (roll < cumulative)
                return affix;
        }

        return candidates[^1];
    }

    public void SetSeed(int seed)
    {
        _rng = new Random(seed);
    }
}
