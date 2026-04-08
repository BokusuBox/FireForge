// 自动生成的代码 - 请勿手动修改

using Godot;
using System.Collections.Generic;

public partial class TableManager : Node
{
    public static TableManager Instance { get; private set; }

    private readonly Dictionary<string, TableData> _tables = new();

    public TableData testXlsx { get; private set; }
    public TableData testXlsx2 { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        testXlsx = LoadTable("testXlsx");
        testXlsx2 = LoadTable("testXlsx2");

        GD.Print($"[TableManager] 所有表加载完成, 共 {_tables.Count} 张表");
    }

    private TableData LoadTable(string tableName)
    {
        var table = new TableData(tableName);
        table.Load();
        _tables[tableName] = table;
        return table;
    }

    public TableData GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
            return table;
        GD.PrintErr($"[TableManager] 表不存在: {tableName}");
        return null;
    }

}
