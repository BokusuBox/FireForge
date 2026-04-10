// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class ArchetypeData
{
    public int Id { get; set; }
    public string ArchetypeName { get; set; } = "";
    public ArchetypeDimension Dimension { get; set; }
    public int MinDifficulty { get; set; }
    public int MaxDifficulty { get; set; }
    public List<MaterialTag> BaseMaterialTags { get; set; } = new();
    // [暂代] 锻造引擎开发时验证与affix表group_id的关联查询
    public List<int> CorrectAffixGroups { get; set; } = new();
    public int SquadPoolId { get; set; }
    public SceneType SceneAnchor { get; set; }
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
            BaseMaterialTags = new List<MaterialTag>(row.BaseMaterialTags),
            SquadPoolId = row.SquadPoolId,
            SceneAnchor = row.SceneAnchor,
            EnemyIlvlScale = row.EnemyIlvlScale,
            DefaultTier = row.DefaultTier,
            BaseDamage = row.BaseDamage,
            Description = row.Description,
        };

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
