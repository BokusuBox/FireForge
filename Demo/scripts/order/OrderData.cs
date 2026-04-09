// 挂载于: 无（纯数据模型，非节点脚本）

using Godot;

public enum OrderStatus
{
    Pending,
    InProgress,
    Submitted,
    Completed,
    Failed,
    Cancelled
}

public class OrderData
{
    public int Id { get; set; }
    public string OrderName { get; set; } = "";
    public OrderType OrderType { get; set; }
    public OrderDifficulty Difficulty { get; set; }
    public int MinReputation { get; set; }
    public int ArchetypeId { get; set; }
    public OrderVariant Variant { get; set; }
    public int AdventurerId { get; set; }
    public int EquipmentCount { get; set; }
    public int RewardGold { get; set; }
    public int RewardReputation { get; set; }
    public string Description { get; set; } = "";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public int ActualScore { get; set; }
    public int TargetScore { get; set; }

    public bool IsAvailable(int currentReputation) => Status == OrderStatus.Pending && currentReputation >= MinReputation;

    public bool IsCompleted => Status == OrderStatus.Completed;

    public bool MeetsTarget => ActualScore >= TargetScore;

    public float OverkillPercent => TargetScore > 0 ? (float)ActualScore / TargetScore * 100f : 0f;

    public int CalculateFinalReward()
    {
        if (!MeetsTarget) return 0;

        float pct = OverkillPercent;
        if (pct >= 200f)
        {
            float bonus = (float)System.Math.Log10(pct / 200f) * RewardGold * 0.5f;
            return (int)(RewardGold * 2f + bonus);
        }
        if (pct >= 150f) return (int)(RewardGold * 1.5f);
        return RewardGold;
    }

    public static OrderData FromRow(OrderRow row)
    {
        return new OrderData
        {
            Id = row.Id,
            OrderName = row.OrderName,
            OrderType = row.OrderType,
            Difficulty = row.Difficulty,
            MinReputation = row.MinReputation,
            ArchetypeId = row.ArchetypeId,
            Variant = row.Variant,
            AdventurerId = row.AdventurerId,
            EquipmentCount = row.EquipmentCount,
            RewardGold = row.RewardGold,
            RewardReputation = row.RewardReputation,
            Description = row.Description,
        };
    }
}
