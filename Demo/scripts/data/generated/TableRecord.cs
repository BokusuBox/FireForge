// 自动生成的代码 - 请勿手动修改

using Godot;
using System;
using System.Collections.Generic;

public class TableRecord
{
    private readonly Dictionary<string, object> _fields = new();

    public string TableName { get; }

    public TableRecord(string tableName)
    {
        TableName = tableName;
    }

    internal void SetField(string name, object value)
    {
        _fields[name] = value;
    }

    public bool HasField(string name) => _fields.ContainsKey(name);

    public object GetRaw(string name)
    {
        if (_fields.TryGetValue(name, out var value))
            return value;
        GD.PrintErr($"[TableRecord] 字段不存在: {TableName}.{name}");
        return null;
    }

    public int GetInt(string name) => Convert.ToInt32(GetRaw(name));
    public float GetFloat(string name) => Convert.ToSingle(GetRaw(name));
    public double GetDouble(string name) => Convert.ToDouble(GetRaw(name));
    public string GetString(string name) => Convert.ToString(GetRaw(name)) ?? "";
    public bool GetBool(string name) => Convert.ToBoolean(GetRaw(name));

    public T GetEnum<T>(string name) where T : struct, Enum
    {
        var raw = GetRaw(name);
        if (raw is T typed)
            return typed;
        return Enum.Parse<T>(raw?.ToString() ?? "");
    }

    public List<int> GetIntList(string name) => GetList<int>(name);
    public List<float> GetFloatList(string name) => GetList<float>(name);
    public List<string> GetStringList(string name) => GetList<string>(name);

    public List<T> GetList<T>(string name)
    {
        var raw = GetRaw(name);
        if (raw is List<T> list)
            return list;
        return new List<T>();
    }

    public T GetBean<T>(string name) where T : new()
    {
        var raw = GetRaw(name);
        if (raw is T typed)
            return typed;
        if (raw is Godot.Collections.Dictionary dict)
            return BeanConverter.FromDict<T>(dict);
        return new T();
    }

    public List<T> GetBeanList<T>(string name) where T : new()
    {
        var raw = GetRaw(name);
        if (raw is List<T> list)
            return list;
        if (raw is Godot.Collections.Array arr)
        {
            var result = new List<T>();
            foreach (var item in arr)
            {
                if (item is Godot.Collections.Dictionary dict)
                    result.Add(BeanConverter.FromDict<T>(dict));
            }
            return result;
        }
        return new List<T>();
    }

    public override string ToString()
    {
        var parts = new List<string>();
        foreach (var kv in _fields)
            parts.Add($"{kv.Key}={kv.Value}");
        return $"[{TableName}] {{ {string.Join(", ", parts)} }}";
    }
}
