// 自动生成的代码 - 请勿手动修改

using Godot;
using System;
using System.Collections.Generic;

public partial class TableManager : Node
{
    public static TableManager Instance { get; private set; }

    private readonly Dictionary<string, TableData> _tables = new();
    private readonly List<string> _loadErrors = new();
    private readonly Dictionary<Type, (string tableName, Func<TableData, object> factory)> _typeRegistry = new();
    private readonly Dictionary<Type, object> _typedTables = new();

    public override void _Ready()
    {
        Instance = this;
        RegisterTypes();
        ScanTableFiles();

        GD.Print($"[TableManager] 扫描完成, 发现 {_tables.Count} 张表（延迟加载模式）");
    }

    private void ScanTableFiles()
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
                _tables[tableName] = new TableData(tableName);
            }
            fileName = dir.GetNext();
        }
        dir.ListDirEnd();
    }

    private void RegisterTypes()
    {
        RegisterTable<AdventurerTable>("adventurer", td => new AdventurerTable(td));
        RegisterTable<ArchetypeTable>("archetype", td => new ArchetypeTable(td));
        RegisterTable<ArchetypeSquadTable>("archetype_squad", td => new ArchetypeSquadTable(td));
        RegisterTable<CurrencyTable>("currency", td => new CurrencyTable(td));
        RegisterTable<AffixTable>("affix", td => new AffixTable(td));
        RegisterTable<EquipmentTable>("equipment", td => new EquipmentTable(td));
        RegisterTable<OrderTable>("order", td => new OrderTable(td));
        RegisterTable<ReputationTable>("reputation", td => new ReputationTable(td));
        RegisterTable<SkillTable>("skill", td => new SkillTable(td));
        RegisterTable<TraitTable>("trait", td => new TraitTable(td));
    }

    private void RegisterTable<T>(string tableName, Func<TableData, T> factory) where T : class
    {
        _typeRegistry[typeof(T)] = (tableName, td => factory(td));
    }

    public void PreloadTables(params string[] tableNames)
    {
        _loadErrors.Clear();
        foreach (var tableName in tableNames)
        {
            if (!_tables.TryGetValue(tableName, out var table))
            {
                _loadErrors.Add($"表 [{tableName}] 不存在");
                continue;
            }
            if (!table.IsLoaded)
            {
                table.Load();
                if (!table.IsLoaded)
                    _loadErrors.Add($"表 [{tableName}] 加载失败，请检查 res://data/{tableName}.json 是否存在且格式正确");
                else
                    GD.Print($"[TableManager] 预加载表 {tableName}: {table.Count} 条记录");
            }
        }

        if (_loadErrors.Count > 0)
        {
            GD.PrintErr($"[TableManager] {_loadErrors.Count} 张表预加载失败!");
            foreach (var err in _loadErrors)
                GD.PrintErr($"  {err}");
        }
    }

    public void PreloadAllTables()
    {
        _loadErrors.Clear();
        foreach (var kv in _tables)
        {
            if (!kv.Value.IsLoaded)
            {
                kv.Value.Load();
                if (!kv.Value.IsLoaded)
                    _loadErrors.Add($"表 [{kv.Key}] 加载失败，请检查 res://data/{kv.Key}.json 是否存在且格式正确");
            }
        }

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

    public TableData GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            if (!table.IsLoaded)
            {
                table.Load();
                if (!table.IsLoaded)
                {
                    GD.PrintErr($"[TableManager] 表 [{tableName}] 加载失败");
                    return null;
                }
                GD.Print($"[TableManager] 延迟加载表 {tableName}: {table.Count} 条记录");
            }
            return table;
        }
        GD.PrintErr($"[TableManager] 表不存在: {tableName}");
        return null;
    }

    public T GetTable<T>() where T : class
    {
        if (_typedTables.TryGetValue(typeof(T), out var cached))
            return (T)cached;

        if (!_typeRegistry.TryGetValue(typeof(T), out var entry))
        {
            GD.PrintErr($"[TableManager] 未注册的表类型: {typeof(T).Name}");
            return null;
        }

        var tableData = GetTable(entry.tableName);
        if (tableData == null) return null;

        var typed = (T)entry.factory(tableData);
        _typedTables[typeof(T)] = typed;
        return typed;
    }

    public bool IsTableLoaded(string tableName)
    {
        return _tables.TryGetValue(tableName, out var table) && table.IsLoaded;
    }

    public int LoadedTableCount
    {
        get
        {
            int count = 0;
            foreach (var kv in _tables)
                if (kv.Value.IsLoaded) count++;
            return count;
        }
    }

    private void ShowLoadErrorDialog()
    {
        var errorList = string.Join("\n", _loadErrors);
        OS.Alert($"以下数据表加载失败，请重新导表后重启游戏:\n\n{errorList}", "数据加载错误");
    }

}
