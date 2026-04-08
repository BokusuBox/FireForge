// 自动生成的代码 - 请勿手动修改

using Godot;
using System.Collections.Generic;

public partial class TableManager : Node
{
    public static TableManager Instance { get; private set; }

    private readonly Dictionary<string, TableData> _tables = new();
    private readonly List<string> _loadErrors = new();

    public TableData testXlsx { get; private set; }
    public TableData testXlsx2 { get; private set; }
    public TableData testXlsx3 { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        testXlsx = LoadTable("testXlsx");
        testXlsx2 = LoadTable("testXlsx2");
        testXlsx3 = LoadTable("testXlsx3");

        if (_loadErrors.Count > 0)
        {
            GD.PrintErr($"[TableManager] {_loadErrors.Count} 张表加载失败!");
            foreach (var err in _loadErrors)
                GD.PrintErr($"  {err}");
            ShowLoadErrorDialog();
        }
        else
        {
            GD.Print($"[TableManager] 所有表加载完成, 共 {_tables.Count} 张表");
        }
    }

    private TableData LoadTable(string tableName)
    {
        var table = new TableData(tableName);
        table.Load();
        _tables[tableName] = table;
        if (!table.IsLoaded)
            _loadErrors.Add($"表 [{tableName}] 加载失败，请检查 res://data/{tableName}.json 是否存在且格式正确");
        return table;
    }

    private void ShowLoadErrorDialog()
    {
        var errorList = string.Join("\n", _loadErrors);
        OS.Alert($"以下数据表加载失败，请重新导表后重启游戏:\n\n{errorList}", "数据加载错误");
    }

    public TableData GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
            return table;
        GD.PrintErr($"[TableManager] 表不存在: {tableName}");
        return null;
    }

}
