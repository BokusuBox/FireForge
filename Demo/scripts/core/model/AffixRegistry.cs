// 挂载于: 无（纯数据索引工具类，非节点脚本。由 CraftingManager 或 AffixRoller 持有/调用）

using Godot;
using System.Collections.Generic;
using System.Linq;

public class AffixRegistry
{
    private readonly List<AffixRow> _allAffixes = new();
    private readonly Dictionary<int, List<AffixRow>> _byGroup = new();
    private readonly Dictionary<AffixSlotType, List<AffixRow>> _bySlot = new();
    private readonly Dictionary<int, AffixRow> _byId = new();

    public void Load()
    {
        var table = Tables.Affix;
        if (table == null)
        {
            GD.PrintErr("[AffixRegistry] affix 表加载失败");
            return;
        }

        _allAffixes.Clear();
        _byGroup.Clear();
        _bySlot.Clear();
        _byId.Clear();

        foreach (var row in table.GetAll())
        {
            _allAffixes.Add(row);

            if (!_byGroup.ContainsKey(row.GroupId))
                _byGroup[row.GroupId] = new List<AffixRow>();
            _byGroup[row.GroupId].Add(row);

            if (!_bySlot.ContainsKey(row.SlotType))
                _bySlot[row.SlotType] = new List<AffixRow>();
            _bySlot[row.SlotType].Add(row);

            _byId[row.Id] = row;
        }

        foreach (var list in _byGroup.Values)
            list.Sort((a, b) => a.Tier.CompareTo(b.Tier));

        GD.Print($"[AffixRegistry] 加载 {_allAffixes.Count} 条词缀，{_byGroup.Count} 个词缀组");
    }

    public AffixRow GetById(int id) => _byId.GetValueOrDefault(id);

    public List<AffixRow> GetByGroup(int groupId) => _byGroup.GetValueOrDefault(groupId, new List<AffixRow>());

    public List<AffixRow> GetBySlot(AffixSlotType slotType) => _bySlot.GetValueOrDefault(slotType, new List<AffixRow>());

    public List<AffixRow> GetCandidates(int iLvl, AffixSlotType slotType, HashSet<int> excludeGroupIds)
    {
        var candidates = new List<AffixRow>();

        if (!_bySlot.TryGetValue(slotType, out var slotAffixes))
            return candidates;

        foreach (var affix in slotAffixes)
        {
            if (affix.RequiredILvl > iLvl) continue;
            if (excludeGroupIds != null && excludeGroupIds.Contains(affix.GroupId)) continue;
            candidates.Add(affix);
        }

        return candidates;
    }

    public List<int> GetAllGroupIds() => _byGroup.Keys.ToList();

    public int GroupCount => _byGroup.Count;

    public int TotalAffixCount => _allAffixes.Count;
}
