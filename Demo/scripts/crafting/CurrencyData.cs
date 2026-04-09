// 挂载于: 无（纯数据模型，非节点脚本）

public class CurrencyData
{
    public int Id { get; set; }
    public string CurrencyName { get; set; } = "";
    public CurrencyType CurrencyType { get; set; }
    public int ApCost { get; set; }
    public CurrencyEffect EffectType { get; set; }
    public bool IsCorruption { get; set; }
    public int MinWorkshopLevel { get; set; }
    public string Description { get; set; } = "";

    public bool CanUse(int workshopLevel, int currentAp)
    {
        if (workshopLevel < MinWorkshopLevel) return false;
        if (ApCost > 0 && currentAp < ApCost) return false;
        return true;
    }

    public bool IsOvercraft => ApCost == 0 && IsCorruption;

    public static CurrencyData FromRow(CurrencyRow row)
    {
        return new CurrencyData
        {
            Id = row.Id,
            CurrencyName = row.CurrencyName,
            CurrencyType = row.CurrencyType,
            ApCost = row.ApCost,
            EffectType = row.EffectType,
            IsCorruption = row.IsCorruption,
            MinWorkshopLevel = row.MinWorkshopLevel,
            Description = row.Description,
        };
    }
}
