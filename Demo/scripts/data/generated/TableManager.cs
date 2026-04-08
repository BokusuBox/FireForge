// 自动生成的代码 - 请勿手动修改

using Godot;
using System.Collections.Generic;

public partial class TableManager : Node
{
    public static TableManager Instance { get; private set; }

    private readonly Dictionary<string, TableData> _tables = new();
    private readonly List<string> _loadErrors = new();

    public override void _Ready()
    {
        Instance = this;
        LoadAllTables();

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

    private void LoadAllTables()
    {
        var dir = DirAccess.Open("res://data/");
        if (dir == null)
        {
            GD.PrintErr("[TableManager] 无法打开 res://data/ 目录");
            OS.Alert("无法打开数据目录 res://data/，请检查游戏安装是否完整", "数据加载错误");
            return;
        }

        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (fileName != string.Empty)
        {
            if (fileName.EndsWith(".json"))
            {
                var tableName = fileName.Substring(0, fileName.Length - 5);
                var table = new TableData(tableName);
                table.Load();
                _tables[tableName] = table;
                if (!table.IsLoaded)
                    _loadErrors.Add($"表 [{tableName}] 加载失败，请检查 res://data/{fileName} 是否存在且格式正确");
            }
            fileName = dir.GetNext();
        }
        dir.ListDirEnd();
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
