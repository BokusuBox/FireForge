// 挂载于: 无（纯数据模型，非节点脚本）

using System.Collections.Generic;

public class ArchetypeSquadData
{
    public int Id { get; set; }
    public int SquadPoolId { get; set; }
    public int AdventurerId { get; set; }
    public SquadRole RoleLabel { get; set; }
    public int Count { get; set; }

    public static ArchetypeSquadData FromRow(ArchetypeSquadRow row)
    {
        return new ArchetypeSquadData
        {
            Id = row.Id,
            SquadPoolId = row.SquadPoolId,
            AdventurerId = row.AdventurerId,
            RoleLabel = row.RoleLabel,
            Count = row.Count,
        };
    }
}
