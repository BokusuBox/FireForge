// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class SkillData
{
    public int Id { get; set; }
    public string SkillName { get; set; } = "";
    public SkillType SkillType { get; set; }
    public SkillTrigger Trigger { get; set; }
    // [暂代] 后续建 skill_threshold.xlsx 拆分每个坎的具体效果数值
    public List<int> ThresholdLevels { get; set; } = new();
    public float BaseCooldown { get; set; }
    // [暂代] 纯文本无法解析，后续拆分为结构化效果字段
    public string Description { get; set; } = "";

    public int MaxThreshold => ThresholdLevels.Count > 0 ? ThresholdLevels[^1] : 0;

    public int GetThresholdRank(int totalLevel)
    {
        int rank = 0;
        foreach (var threshold in ThresholdLevels)
        {
            if (totalLevel >= threshold)
                rank++;
            else
                break;
        }
        return rank;
    }

    public bool IsThresholdMet(int totalLevel, int targetRank) => GetThresholdRank(totalLevel) >= targetRank;

    public static SkillData FromRow(SkillRow row)
    {
        var data = new SkillData
        {
            Id = row.Id,
            SkillName = row.SkillName,
            SkillType = row.SkillType,
            Trigger = row.Trigger,
            BaseCooldown = row.BaseCooldown,
            Description = row.Description,
        };

        var raw = row.ThresholdLevels;
        if (!string.IsNullOrEmpty(raw))
        {
            foreach (var part in raw.Split('|'))
            {
                var t = part.Trim();
                if (!string.IsNullOrEmpty(t) && int.TryParse(t, out var level))
                    data.ThresholdLevels.Add(level);
            }
        }

        return data;
    }
}
