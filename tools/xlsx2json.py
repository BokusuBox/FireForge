#!/usr/bin/env python3
"""
xlsx2json.py - 《通货与铁砧》导表工具
将 data/ 目录下的 xlsx 文件转换为 JSON 和 C# 代码

用法: python tools/xlsx2json.py
      或直接运行 data/xlsx2json.exe

目录约定:
  data/             → xlsx 源文件（按系统分子文件夹，如 data/test/testXlsx.xlsx）
  demo/data/        → 导出的 JSON 文件
  demo/scripts/data/generated/ → 导出的 C# 代码（仅 __enum__.cs + TableRecord.cs + TableData.cs + TableManager.cs）

表结构约定:
  第1行: 字段名称
  第2行: 数据类型（int/float/double/string/bool/枚举名/(list#sep=X),T）
  第3行: 字段备注
  第4行起: 数据行（A列为 ## 的行不导出）
"""

import os
import json
import re
import sys
import keyword
import ctypes

_RED = '\033[91m'
_YELLOW = '\033[93m'
_GREEN = '\033[92m'
_RESET = '\033[0m'

if sys.platform == 'win32':
    kernel32 = ctypes.windll.kernel32
    kernel32.SetConsoleMode(kernel32.GetStdHandle(-11), 7)

def _c(text, color):
    return f"{color}{text}{_RESET}"

CS_KEYWORDS = set(keyword.kwlist) | {
    'abstract', 'as', 'base', 'bool', 'break', 'byte', 'case', 'catch', 'char',
    'checked', 'class', 'const', 'continue', 'decimal', 'default', 'delegate',
    'do', 'double', 'else', 'enum', 'event', 'explicit', 'extern', 'false',
    'finally', 'fixed', 'float', 'for', 'foreach', 'goto', 'if', 'implicit',
    'in', 'int', 'interface', 'internal', 'is', 'lock', 'long', 'namespace',
    'new', 'null', 'object', 'operator', 'out', 'override', 'params', 'private',
    'protected', 'public', 'readonly', 'ref', 'return', 'sbyte', 'sealed',
    'short', 'sizeof', 'stackalloc', 'static', 'string', 'struct', 'switch',
    'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong', 'unchecked',
    'unsafe', 'ushort', 'using', 'virtual', 'void', 'volatile', 'while',
    'var', 'dynamic', 'async', 'await', 'partial', 'yield', 'nameof',
    'value', 'get', 'set', 'add', 'remove', 'global', 'alias', 'ascending',
    'descending', 'from', 'group', 'into', 'join', 'let', 'on', 'orderby',
    'select', 'where', 'NotImplementedException',
}

VALID_IDENTIFIER_RE = re.compile(r'^[A-Za-z_][A-Za-z0-9_]*$')

from openpyxl import load_workbook


def validate_enum_name(name, source):
    errors = []
    if not VALID_IDENTIFIER_RE.match(name):
        errors.append(f"[错误] 枚举名 '{name}' 不是合法的C#标识符 ({source})")
    if name in CS_KEYWORDS:
        errors.append(f"[错误] 枚举名 '{name}' 是C#关键字，请修改 ({source})")
    return errors


def validate_enum_member(enum_name, member_name, member_value, source):
    errors = []
    if not member_name:
        errors.append(f"[错误] 枚举 {enum_name} 的值 {member_value} 缺少枚举成员名(B列) ({source})")
    elif not VALID_IDENTIFIER_RE.match(member_name):
        errors.append(f"[错误] 枚举 {enum_name} 的成员名 '{member_name}' 不是合法的C#标识符 ({source})")
    if member_name and member_name in CS_KEYWORDS:
        errors.append(f"[错误] 枚举 {enum_name} 的成员名 '{member_name}' 是C#关键字，请修改 ({source})")
    return errors


def parse_enum_xlsx(xlsx_path):
    file_name = os.path.splitext(os.path.basename(xlsx_path))[0]
    all_errors = []
    enum_defs = {}

    try:
        wb = load_workbook(xlsx_path, read_only=True, data_only=True)
    except Exception as e:
        all_errors.append(f"[错误] 无法打开文件 {file_name}.xlsx: {e}")
        return enum_defs, all_errors

    for sheet_name in wb.sheetnames:
        ws = wb[sheet_name]
        if ws.max_row < 2 or ws.max_column < 1:
            continue

        current_enum = None
        current_members = []

        for row in range(1, ws.max_row + 1):
            a_val = ws.cell(row=row, column=1).value
            b_val = ws.cell(row=row, column=2).value
            c_val = ws.cell(row=row, column=3).value

            if a_val is None:
                continue

            a_str = str(a_val).strip()

            if not a_str:
                continue

            is_number = False
            try:
                int_val = int(float(a_str))
                is_number = True
            except (ValueError, TypeError):
                pass

            if not is_number:
                if current_enum is not None:
                    enum_defs[current_enum] = current_members
                current_enum = a_str
                current_members = []
                source = f"{file_name}.xlsx / {sheet_name} / 第{row}行"
                all_errors.extend(validate_enum_name(current_enum, source))
            else:
                if current_enum is None:
                    source = f"{file_name}.xlsx / {sheet_name} / 第{row}行"
                    all_errors.append(f"[错误] 第{row}行A列值为数值 '{a_str}'，但之前没有枚举名定义 ({source})")
                    continue

                member_name = str(b_val).strip() if b_val is not None else ''
                comment = str(c_val).strip() if c_val is not None else ''
                source = f"{file_name}.xlsx / {sheet_name} / {current_enum} / 第{row}行"
                all_errors.extend(validate_enum_member(current_enum, member_name, a_str, source))
                current_members.append({
                    'name': member_name,
                    'value': int_val,
                    'comment': comment,
                })

        if current_enum is not None:
            enum_defs[current_enum] = current_members

    wb.close()

    seen_names = {}
    for enum_name, members in enum_defs.items():
        name_set = set()
        value_set = set()
        for m in members:
            if m['name'] in name_set:
                all_errors.append(f"[错误] 枚举 {enum_name} 中存在重复的成员名 '{m['name']}'")
            name_set.add(m['name'])
            if m['value'] in value_set:
                all_errors.append(f"[错误] 枚举 {enum_name} 中存在重复的值 {m['value']} (成员 '{m['name']}')")
            value_set.add(m['value'])
        if enum_name in seen_names:
            all_errors.append(f"[错误] 枚举名 '{enum_name}' 重复定义 (在 {seen_names[enum_name]} 和当前Sheet中)")
        seen_names[enum_name] = enum_name

    return enum_defs, all_errors

def _resolve_root_dir():
    if getattr(sys, 'frozen', False):
        return os.path.dirname(os.path.dirname(os.path.abspath(sys.executable)))
    return os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

ROOT_DIR = _resolve_root_dir()
DATA_DIR = os.path.join(ROOT_DIR, 'data')
JSON_OUTPUT_DIR = os.path.join(ROOT_DIR, 'demo', 'data')
CS_OUTPUT_DIR = os.path.join(ROOT_DIR, 'demo', 'scripts', 'data', 'generated')

BASIC_TYPES = {'int', 'float', 'double', 'string', 'bool'}
LIST_PATTERN = re.compile(r'^\(list#sep=(.)\),(\w+)$')


def parse_type(type_str):
    type_str = type_str.strip()
    match = LIST_PATTERN.match(type_str)
    if match:
        return {
            'kind': 'list',
            'separator': match.group(1),
            'element_type': match.group(2)
        }
    if type_str in BASIC_TYPES:
        return {
            'kind': 'basic',
            'type': type_str
        }
    return {
        'kind': 'enum',
        'type': type_str
    }


def convert_basic_value(raw_value, basic_type):
    if raw_value is None:
        return None
    try:
        if basic_type == 'int':
            return int(float(str(raw_value)))
        elif basic_type == 'float':
            return float(str(raw_value))
        elif basic_type == 'double':
            return float(str(raw_value))
        elif basic_type == 'string':
            return str(raw_value)
        elif basic_type == 'bool':
            return str(raw_value).lower() in ('true', '1', 'yes')
    except (ValueError, TypeError) as e:
        raise ValueError(f"无法将 '{raw_value}' 转换为 {basic_type}: {e}")
    return raw_value


def convert_value(raw_value, type_info):
    if raw_value is None:
        return None
    if type_info['kind'] == 'list':
        raw_str = str(raw_value)
        if not raw_str:
            return []
        parts = raw_str.split(type_info['separator'])
        elem_type = type_info['element_type']
        if elem_type in BASIC_TYPES:
            return [convert_basic_value(p.strip(), elem_type) for p in parts]
        return [p.strip() for p in parts]
    if type_info['kind'] == 'basic':
        return convert_basic_value(raw_value, type_info['type'])
    if type_info['kind'] == 'enum':
        return str(raw_value).strip()
    return raw_value


def process_sheet(ws, sheet_name):
    if ws.max_row < 4 or ws.max_column < 2:
        return None

    errors = []

    field_names = []
    for col in range(2, ws.max_column + 1):
        val = ws.cell(row=1, column=col).value
        if val is None:
            break
        field_names.append(str(val).strip())

    if not field_names:
        return None

    field_types = []
    for col in range(2, 2 + len(field_names)):
        val = ws.cell(row=2, column=col).value
        if val is None:
            field_types.append({'kind': 'basic', 'type': 'string'})
        else:
            type_str = str(val).strip()
            type_info = parse_type(type_str)
            if type_info['kind'] == 'list':
                elem = type_info['element_type']
                if elem not in BASIC_TYPES and not VALID_IDENTIFIER_RE.match(elem):
                    errors.append(f"[错误] 表 {sheet_name} 第2行第{col}列: list元素类型 '{elem}' 不是合法标识符")
            field_types.append(type_info)

    field_comments = []
    for col in range(2, 2 + len(field_names)):
        val = ws.cell(row=3, column=col).value
        field_comments.append(str(val).strip() if val else '')

    data_rows = []
    enum_values = {}

    for row in range(4, ws.max_row + 1):
        for col_idx, type_info in enumerate(field_types):
            if type_info['kind'] == 'enum':
                cell_val = ws.cell(row=row, column=col_idx + 2).value
                if cell_val is not None:
                    enum_name = type_info['type']
                    if enum_name not in enum_values:
                        enum_values[enum_name] = set()
                    enum_values[enum_name].add(str(cell_val).strip())
            elif type_info['kind'] == 'list' and type_info['element_type'] not in BASIC_TYPES:
                cell_val = ws.cell(row=row, column=col_idx + 2).value
                if cell_val is not None:
                    enum_name = type_info['element_type']
                    raw_str = str(cell_val).strip()
                    if raw_str:
                        if enum_name not in enum_values:
                            enum_values[enum_name] = set()
                        parts = raw_str.split(type_info['separator'])
                        for p in parts:
                            v = p.strip()
                            if v:
                                enum_values[enum_name].add(v)

        marker = ws.cell(row=row, column=1).value
        if marker is not None and str(marker).strip() == '##':
            continue

        has_data = False
        for col in range(2, 2 + len(field_names)):
            if ws.cell(row=row, column=col).value is not None:
                has_data = True
                break
        if not has_data:
            continue

        record = {}
        for col_idx, field_name in enumerate(field_names):
            raw_val = ws.cell(row=row, column=col_idx + 2).value
            type_info = field_types[col_idx]
            try:
                record[field_name] = convert_value(raw_val, type_info)
            except ValueError as e:
                col_letter = chr(ord('A') + col_idx + 1)
                errors.append(f"[错误] 表 {sheet_name} 第{row}行 {col_letter}列 ({field_name}): {e}")
                record[field_name] = None
        data_rows.append(record)

    fields = []
    for i, name in enumerate(field_names):
        fields.append({
            'name': name,
            'type': str(ws.cell(row=2, column=i + 2).value or 'string').strip(),
            'comment': field_comments[i]
        })

    return {
        'tableName': sheet_name,
        'fields': fields,
        'data': data_rows,
        'enum_values': enum_values,
        'errors': errors
    }


def write_json(table_data, output_dir):
    os.makedirs(output_dir, exist_ok=True)
    output = {
        'tableName': table_data['tableName'],
        'fields': table_data['fields'],
        'data': table_data['data']
    }
    json_path = os.path.join(output_dir, f"{table_data['tableName']}.json")
    with open(json_path, 'w', encoding='utf-8') as f:
        json.dump(output, f, ensure_ascii=False, indent=2)
    print(f"  [JSON] {json_path}")


def generate_enum_cs(enum_defs, auto_enum_values):
    lines = []
    lines.append('// 自动生成的代码 - 请勿手动修改')
    lines.append('// 枚举定义：由导表工具根据 __enum__.xlsx 和数据表自动生成')
    lines.append('')

    all_enum_names = set(enum_defs.keys()) | set(auto_enum_values.keys())

    for enum_name in sorted(all_enum_names):
        if enum_name in enum_defs:
            members = enum_defs[enum_name]
            lines.append(f'public enum {enum_name}')
            lines.append('{')
            for i, m in enumerate(members):
                comma = ',' if i < len(members) - 1 else ''
                comment = f'  // {m["comment"]}' if m['comment'] else ''
                lines.append(f'    {m["name"]} = {m["value"]}{comma}{comment}')
            lines.append('}')
            lines.append('')
        else:
            values = sorted(auto_enum_values[enum_name])
            lines.append(f'public enum {enum_name}')
            lines.append('{')
            for i, val in enumerate(values):
                comma = ',' if i < len(values) - 1 else ''
                lines.append(f'    {val}{comma}')
            lines.append('}')
            lines.append('')

    return '\n'.join(lines)


def generate_table_record_cs():
    return '''// 自动生成的代码 - 请勿手动修改

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

    public override string ToString()
    {
        var parts = new List<string>();
        foreach (var kv in _fields)
            parts.Add($"{kv.Key}={kv.Value}");
        return $"[{TableName}] {{ {string.Join(", ", parts)} }}";
    }
}
'''


def generate_table_data_cs():
    return '''// 自动生成的代码 - 请勿手动修改

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
            var arr = variant.AsGodotArray();
            var list = CreateTypedList(typeInfo.ElementType);
            foreach (var item in arr)
            {
                list.Add(ConvertSingleVariant(item, typeInfo.ElementType));
            }
            return list;
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

    private (string Kind, string ElementType) ParseFieldType(string fieldType)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            fieldType, @"^\\(list#sep=.\\),(\\w+)$");
        if (match.Success)
            return ("list", match.Groups[1].Value);

        if (fieldType == "int" || fieldType == "float" || fieldType == "double"
            || fieldType == "string" || fieldType == "bool")
            return (fieldType, "");

        return (fieldType, "");
    }

    private void BuildIndexes()
    {
        _indexes.Clear();
        if (_records.Count == 0) return;

        foreach (var fieldName in _fieldTypes.Keys)
        {
            var typeInfo = ParseFieldType(_fieldTypes[fieldName]);
            if (typeInfo.Kind == "list") continue;

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
'''


def generate_table_manager_cs():
    return '''// 自动生成的代码 - 请勿手动修改

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
        var errorList = string.Join("\\n", _loadErrors);
        OS.Alert($"以下数据表加载失败，请重新导表后重启游戏:\\n\\n{errorList}", "数据加载错误");
    }

    public TableData GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
            return table;
        GD.PrintErr($"[TableManager] 表不存在: {tableName}");
        return null;
    }

}
'''


def scan_xlsx_files(data_dir):
    xlsx_files = []
    for root, dirs, files in os.walk(data_dir):
        for f in files:
            if f.endswith('.xlsx') and not f.startswith('~'):
                xlsx_files.append(os.path.join(root, f))
    return xlsx_files


def main():
    print("=" * 60)
    print("《通货与铁砧》导表工具 v1.0")
    print("=" * 60)

    if not os.path.exists(DATA_DIR):
        print(_c(f"[错误] 数据目录不存在: {DATA_DIR}", _RED))
        sys.exit(1)

    xlsx_files = scan_xlsx_files(DATA_DIR)
    if not xlsx_files:
        print(_c("[警告] 未找到任何 xlsx 文件", _YELLOW))
        sys.exit(0)

    enum_files = []
    data_files = []
    for f in xlsx_files:
        name = os.path.splitext(os.path.basename(f))[0]
        if name.startswith('__') and name.endswith('__'):
            enum_files.append(f)
        else:
            data_files.append(f)

    print(f"\n扫描到 {len(data_files)} 个数据表, {len(enum_files)} 个枚举表:")
    for f in data_files:
        print(f"  - {os.path.relpath(f, ROOT_DIR)}")
    for f in enum_files:
        print(f"  - {os.path.relpath(f, ROOT_DIR)} (枚举表)")

    all_errors = []
    enum_defs = {}

    for xlsx_path in enum_files:
        file_name = os.path.splitext(os.path.basename(xlsx_path))[0]
        print(f"\n解析枚举表: {file_name}.xlsx")
        defs, errors = parse_enum_xlsx(xlsx_path)
        all_errors.extend(errors)
        for enum_name, members in defs.items():
            if enum_name in enum_defs:
                all_errors.append(f"[错误] 枚举 '{enum_name}' 在多个枚举表中重复定义")
            else:
                enum_defs[enum_name] = members
                member_names = [m['name'] for m in members]
                print(f"  枚举 {enum_name}: {', '.join(member_names)}")

    all_tables = []
    all_enum_values = {}

    for xlsx_path in data_files:
        file_name = os.path.splitext(os.path.basename(xlsx_path))[0]
        print(f"\n处理: {file_name}.xlsx")

        try:
            wb = load_workbook(xlsx_path, read_only=True, data_only=True)
        except Exception as e:
            print(_c(f"  [错误] 无法打开文件: {e}", _RED))
            continue

        data_sheets = []
        for sheet_name in wb.sheetnames:
            ws = wb[sheet_name]
            if ws.max_row < 4 or ws.max_column < 2:
                continue
            first_cell = ws.cell(row=1, column=1).value
            if first_cell and str(first_cell).strip() == '字段名称':
                data_sheets.append((sheet_name, ws))

        if len(data_sheets) == 1:
            table_name = file_name
            sheet_name, ws = data_sheets[0]
            result = process_sheet(ws, table_name)
            if result:
                all_tables.append(result)
                all_errors.extend(result.get('errors', []))
                for enum_name, values in result['enum_values'].items():
                    if enum_name not in all_enum_values:
                        all_enum_values[enum_name] = set()
                    all_enum_values[enum_name].update(values)
                print(f"  Sheet '{sheet_name}' → 表 '{table_name}' ({len(result['data'])} 条记录)")
        elif len(data_sheets) > 1:
            for sheet_name, ws in data_sheets:
                table_name = f"{file_name}_{sheet_name}"
                result = process_sheet(ws, table_name)
                if result:
                    all_tables.append(result)
                    all_errors.extend(result.get('errors', []))
                    for enum_name, values in result['enum_values'].items():
                        if enum_name not in all_enum_values:
                            all_enum_values[enum_name] = set()
                        all_enum_values[enum_name].update(values)
                    print(f"  Sheet '{sheet_name}' → 表 '{table_name}' ({len(result['data'])} 条记录)")

        wb.close()

    for enum_name in enum_defs:
        if enum_name in all_enum_values:
            del all_enum_values[enum_name]

    for enum_name in all_enum_values:
        if enum_name in enum_defs:
            defined_members = {m['name'] for m in enum_defs[enum_name]}
            for val in all_enum_values[enum_name]:
                if val not in defined_members:
                    all_errors.append(f"[错误] 数据表中使用了枚举 {enum_name} 的成员 '{val}'，但该成员未在 __enum__.xlsx 中定义 (已有: {', '.join(sorted(defined_members))})")

    if all_errors:
        print(f"\n{'=' * 60}")
        print(_c(f"发现 {len(all_errors)} 个错误:", _RED))
        for err in all_errors:
            print(_c(f"  {err}", _RED))
        print(_c(f"\n请修复以上错误后重新导出!", _RED))
        print(f"{'=' * 60}")
        sys.exit(1)

    if not all_tables and not enum_defs:
        print(_c("\n[警告] 未找到有效的数据表或枚举定义", _YELLOW))
        sys.exit(0)

    for xlsx_path in enum_files:
        stale_name = os.path.splitext(os.path.basename(xlsx_path))[0]
        stale_json = os.path.join(JSON_OUTPUT_DIR, f"{stale_name}.json")
        if os.path.exists(stale_json):
            os.remove(stale_json)
            print(f"  [清理] 删除残留文件: {os.path.relpath(stale_json, ROOT_DIR)}")

    print(f"\n{'=' * 60}")
    print("导出 JSON 文件:")
    for table in all_tables:
        write_json(table, JSON_OUTPUT_DIR)

    print(f"\n{'=' * 60}")
    print("生成 C# 代码:")

    os.makedirs(CS_OUTPUT_DIR, exist_ok=True)

    if enum_defs or all_enum_values:
        enum_code = generate_enum_cs(enum_defs, all_enum_values)
        enum_path = os.path.join(CS_OUTPUT_DIR, '__enum__.cs')
        with open(enum_path, 'w', encoding='utf-8') as f:
            f.write(enum_code)
        print(f"  [C#] {enum_path}")
        for enum_name, members in sorted(enum_defs.items()):
            member_names = [m['name'] for m in members]
            print(f"        枚举 {enum_name}: {', '.join(member_names)} (显式定义)")
        for enum_name, values in sorted(all_enum_values.items()):
            print(f"        枚举 {enum_name}: {', '.join(sorted(values))} (自动推断)")

    record_code = generate_table_record_cs()
    record_path = os.path.join(CS_OUTPUT_DIR, 'TableRecord.cs')
    with open(record_path, 'w', encoding='utf-8') as f:
        f.write(record_code)
    print(f"  [C#] {record_path}")

    table_code = generate_table_data_cs()
    table_path = os.path.join(CS_OUTPUT_DIR, 'TableData.cs')
    with open(table_path, 'w', encoding='utf-8') as f:
        f.write(table_code)
    print(f"  [C#] {table_path}")

    manager_code = generate_table_manager_cs()
    manager_path = os.path.join(CS_OUTPUT_DIR, 'TableManager.cs')
    with open(manager_path, 'w', encoding='utf-8') as f:
        f.write(manager_code)
    print(f"  [C#] {manager_path}")

    print(f"\n{'=' * 60}")
    print(_c(f"导出完成! 共处理 {len(all_tables)} 张表, {len(enum_defs)} 个显式枚举, {len(all_enum_values)} 个自动推断枚举", _GREEN))
    print(f"JSON 输出: {os.path.relpath(JSON_OUTPUT_DIR, ROOT_DIR)}/")
    print(f"C# 输出:   {os.path.relpath(CS_OUTPUT_DIR, ROOT_DIR)}/")
    print(f"  固定文件: __enum__.cs + TableRecord.cs + TableData.cs + TableManager.cs")
    print(f"{'=' * 60}")


if __name__ == '__main__':
    try:
        main()
    except SystemExit:
        pass
    finally:
        if getattr(sys, 'frozen', False):
            input("\n按任意键关闭...")
