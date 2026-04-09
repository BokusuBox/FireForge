// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class ArchetypeData
{
    public int Id { get; set; }
    public string ArchetypeName { get; set; } = "";
    public ArchetypeDimension Dimension { get; set; }
    public int MinDifficulty { get; set; }
    public int MaxDifficulty { get; set; }
    // [暂代] 后续建 MaterialTag 枚举或底材标签表
    public List<string> BaseMaterialTags { get; set; } = new();
    // [暂代] 锻造引擎开发时验证与affix表group_id的关联查询
    public List<int> CorrectAffixGroups { get; set; } = new();
    public int SquadPoolId { get; set; }
    // [暂代] 后续建 dungeon.xlsx 场景配置表后改为 int 引用
    public string SceneAnchor { get; set; } = "";
    public float EnemyIlvlScale { get; set; }
    public int DefaultTier { get; set; }
    public int BaseDamage { get; set; }
    public string Description { get; set; } = "";

    public bool IsDifficultyInRange(int difficulty) => difficulty >= MinDifficulty && difficulty <= MaxDifficulty;

    public int CalculateTargetScore(int difficulty)
    {
        float diffRatio = MaxDifficulty > MinDifficulty
            ? (float)(difficulty - MinDifficulty) / (MaxDifficulty - MinDifficulty)
            : 0f;
        float tierMultiplier = 1f + (5 - DefaultTier) * 0.3f;
        float ilvlMultiplier = 1f + (diffRatio * (EnemyIlvlScale - 1f));
        return (int)(BaseDamage * tierMultiplier * ilvlMultiplier);
    }

    public static ArchetypeData FromRow(ArchetypeRow row)
    {
        var data = new ArchetypeData
        {
            Id = row.Id,
            ArchetypeName = row.ArchetypeName,
            Dimension = row.Dimension,
            MinDifficulty = row.MinDifficulty,
            MaxDifficulty = row.MaxDifficulty,
            SquadPoolId = row.SquadPoolId,
            SceneAnchor = row.SceneAnchor,
            EnemyIlvlScale = row.EnemyIlvlScale,
            DefaultTier = row.DefaultTier,
            BaseDamage = row.BaseDamage,
            Description = row.Description,
        };

        var tags = row.BaseMaterialTags;
        if (!string.IsNullOrEmpty(tags))
        {
            foreach (var tag in tags.Split('|'))
            {
                var t = tag.Trim();
                if (!string.IsNullOrEmpty(t))
                    data.BaseMaterialTags.Add(t);
            }
        }

        var groups = row.CorrectAffixGroups;
        if (!string.IsNullOrEmpty(groups))
        {
            foreach (var g in groups.Split('|'))
            {
                var t = g.Trim();
                if (!string.IsNullOrEmpty(t) && int.TryParse(t, out var gid))
                    data.CorrectAffixGroups.Add(gid);
            }
        }

        return data;
    }
}
