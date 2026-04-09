// 自动生成的代码 - 请勿手动修改

using Godot;
using System;
using System.Collections.Generic;

public class TableData
{
    private readonly List<TableRecord> _records = new();
    private readonly Dictionary<string, Dictionary<object, List<TableRecord>>> _indexes = new();
    private readonly Dictionary<string, string> _fieldTypes = new();

    public string TableName { get; }
    public int Count => _records.Count;
    public bool IsLoaded { get; private set; }

    public TableData(string tableName)
    {
        TableName = tableName;
    }

    public void Load()
    {
        var jsonText = FileAccess.GetFileAsString($"res://data/{TableName}.json");
        if (string.IsNullOrEmpty(jsonText))
        {
            GD.PrintErr($"[TableData] 加载表失败: {TableName}");
            IsLoaded = false;
            return;
        }
        var parsed = Json.ParseString(jsonText);
        if (parsed == null || parsed.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr($"[TableData] JSON格式无效: {TableName}");
            IsLoaded = false;
            return;
        }
        var root = new Godot.Collections.Dictionary(parsed);

        _fieldTypes.Clear();
        var fieldsArray = root["fields"].AsGodotArray();
        foreach (var entry in fieldsArray)
        {
            var fieldDict = new Godot.Collections.Dictionary(entry);
            var fieldName = fieldDict["name"].AsString();
            var fieldType = fieldDict["type"].AsString();
            _fieldTypes[fieldName] = fieldType;
        }

        var dataArray = root["data"].AsGodotArray();
        foreach (var entry in dataArray)
        {
            var dict = new Godot.Collections.Dictionary(entry);
            var record = new TableRecord(TableName);

            foreach (var keyObj in dict.Keys)
            {
                var fieldName = keyObj.ToString();
                var variant = dict[keyObj];
                var fieldType = _fieldTypes.GetValueOrDefault(fieldName, "string");
                object value = ConvertVariant(variant, fieldType);
                record.SetField(fieldName, value);
            }

            _records.Add(record);
        }

        BuildIndexes();
        IsLoaded = true;
        GD.Print($"[TableData] 加载表 {TableName}: {_records.Count} 条记录");
    }

    private object ConvertVariant(Variant variant, string fieldType)
    {
        var typeInfo = ParseFieldType(fieldType);

        if (typeInfo.Kind == "list")
        {
            if (typeInfo.IsBeanList)
            {
                var arr = variant.AsGodotArray();
                var result = new List<Godot.Collections.Dictionary>();
                foreach (var item in arr)
                    result.Add(item.AsGodotDictionary());
                return result;
            }
            var arr2 = variant.AsGodotArray();
            var list = CreateTypedList(typeInfo.ElementType);
            foreach (var item in arr2)
            {
                list.Add(ConvertSingleVariant(item, typeInfo.ElementType));
            }
            return list;
        }

        if (typeInfo.Kind == "bean")
        {
            return variant.AsGodotDictionary();
        }

        return ConvertSingleVariant(variant, typeInfo.Kind);
    }

    private object ConvertSingleVariant(Variant variant, string type)
    {
        return type switch
        {
            "int" => variant.AsInt32(),
            "float" => variant.AsSingle(),
            "double" => variant.AsDouble(),
            "bool" => variant.AsBool(),
            "string" => variant.AsString(),
            _ => variant.AsString()
        };
    }

    private dynamic CreateTypedList(string elementType)
    {
        return elementType switch
        {
            "int" => new List<int>(),
            "float" => new List<float>(),
            "double" => new List<double>(),
            "bool" => new List<bool>(),
            "string" => new List<string>(),
            _ => new List<string>()
        };
    }

    private (string Kind, string ElementType, bool IsBeanList) ParseFieldType(string fieldType)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            fieldType, @"^\(list#sep=.\),(.+)$");
        if (match.Success)
        {
            var elem = match.Groups[1].Value;
            var isBean = elem.Contains(".") || !new[] { "int", "float", "double", "string", "bool" }.Contains(elem);
            return ("list", elem, isBean);
        }

        if (fieldType == "int" || fieldType == "float" || fieldType == "double"
            || fieldType == "string" || fieldType == "bool")
            return (fieldType, "", false);

        if (fieldType.Contains("."))
            return ("bean", fieldType, false);

        return (fieldType, "", false);
    }

    private void BuildIndexes()
    {
        _indexes.Clear();
        if (_records.Count == 0) return;

        foreach (var fieldName in _fieldTypes.Keys)
        {
            var typeInfo = ParseFieldType(_fieldTypes[fieldName]);
            if (typeInfo.Kind == "list" || typeInfo.Kind == "bean") continue;

            var index = new Dictionary<object, List<TableRecord>>();
            foreach (var record in _records)
            {
                var key = record.GetRaw(fieldName);
                if (key == null) continue;

                var boxKey = key is Enum ? key.ToString() : key;
                if (!index.ContainsKey(boxKey))
                    index[boxKey] = new List<TableRecord>();
                index[boxKey].Add(record);
            }
            _indexes[fieldName] = index;
        }
    }

    public TableRecord Find(string fieldName, object value)
    {
        var boxValue = value is Enum ? value.ToString() : value;
        if (_indexes.TryGetValue(fieldName, out var index))
        {
            if (index.TryGetValue(boxValue, out var list) && list.Count > 0)
                return list[0];
        }
        return null;
    }

    public List<TableRecord> FindAll(string fieldName, object value)
    {
        var boxValue = value is Enum ? value.ToString() : value;
        if (_indexes.TryGetValue(fieldName, out var index))
        {
            if (index.TryGetValue(boxValue, out var list))
                return new List<TableRecord>(list);
        }
        return new List<TableRecord>();
    }

    public TableRecord GetAt(int index)
    {
        if (index >= 0 && index < _records.Count)
            return _records[index];
        return null;
    }

    public List<TableRecord> GetAll() => new(_records);

    public string GetFieldType(string fieldName)
    {
        return _fieldTypes.GetValueOrDefault(fieldName, "");
    }
}
