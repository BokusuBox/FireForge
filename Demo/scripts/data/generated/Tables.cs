// 自动生成的代码 - 请勿手动修改
// 强类型表包装：由导表工具根据数据表自动生成

using Godot;
using System;
using System.Collections.Generic;

public class TestXlsxRow
{
    private readonly TableRecord _raw;
    public TestXlsxRow(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string WeaponName => _raw.GetString("weapon_name");
    public WeaponType WeaponType => _raw.GetEnum<WeaponType>("weapon_type");
    public List<int> SkillPool => _raw.GetIntList("skill_pool");
}

public class TestXlsx2Row
{
    private readonly TableRecord _raw;
    public TestXlsx2Row(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string WeaponName => _raw.GetString("weapon_name");
    public List<WeaponType> WeaponType => _raw.GetStringList("weapon_type").ConvertAll(s => Enum.Parse<WeaponType>(s));
    public int SkillPool => _raw.GetInt("skill_pool");
}

public class TestXlsx3Row
{
    private readonly TableRecord _raw;
    public TestXlsx3Row(TableRecord raw) { _raw = raw; }

    public int Id => _raw.GetInt("id");
    public string WeaponName => _raw.GetString("weapon_name");
    public List<WeaponType> WeaponType => _raw.GetStringList("weapon_type").ConvertAll(s => Enum.Parse<WeaponType>(s));
    public int SkillPool => _raw.GetInt("skill_pool");
    public ItemCost ItemCost => _raw.GetBean<ItemCost>("item_cost");
    public List<ItemAward> ItemAward => _raw.GetBeanList<ItemAward>("item_award");
    public List<ItemTest> ItemTest => _raw.GetBeanList<ItemTest>("item_test");
}

public class TestXlsxTable
{
    private readonly TableData _raw;
    private List<TestXlsxRow> _rows;

    public TestXlsxTable(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<TestXlsxRow> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new TestXlsxRow(r));
        return _rows;
    }

    public TestXlsxRow FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new TestXlsxRow(record) : null;
    }

    public TestXlsxRow Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new TestXlsxRow(record) : null;
    }

    public List<TestXlsxRow> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new TestXlsxRow(r));
    }
}

public class TestXlsx2Table
{
    private readonly TableData _raw;
    private List<TestXlsx2Row> _rows;

    public TestXlsx2Table(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<TestXlsx2Row> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new TestXlsx2Row(r));
        return _rows;
    }

    public TestXlsx2Row FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new TestXlsx2Row(record) : null;
    }

    public TestXlsx2Row Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new TestXlsx2Row(record) : null;
    }

    public List<TestXlsx2Row> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new TestXlsx2Row(r));
    }
}

public class TestXlsx3Table
{
    private readonly TableData _raw;
    private List<TestXlsx3Row> _rows;

    public TestXlsx3Table(TableData raw) { _raw = raw; }
    public int Count => _raw.Count;

    public List<TestXlsx3Row> GetAll()
    {
        if (_rows == null)
            _rows = _raw.GetAll().ConvertAll(r => new TestXlsx3Row(r));
        return _rows;
    }

    public TestXlsx3Row FindById(int id)
    {
        var record = _raw.Find("id", id);
        return record != null ? new TestXlsx3Row(record) : null;
    }

    public TestXlsx3Row Find(string fieldName, object value)
    {
        var record = _raw.Find(fieldName, value);
        return record != null ? new TestXlsx3Row(record) : null;
    }

    public List<TestXlsx3Row> FindAll(string fieldName, object value)
    {
        return _raw.FindAll(fieldName, value).ConvertAll(r => new TestXlsx3Row(r));
    }
}

public static class Tables
{
    private static TestXlsxTable _testXlsx;
    public static TestXlsxTable TestXlsx
    {
        get
        {
            if (_testXlsx == null)
                _testXlsx = new TestXlsxTable(TableManager.Instance.GetTable("testXlsx"));
            return _testXlsx;
        }
    }

    private static TestXlsx2Table _testXlsx2;
    public static TestXlsx2Table TestXlsx2
    {
        get
        {
            if (_testXlsx2 == null)
                _testXlsx2 = new TestXlsx2Table(TableManager.Instance.GetTable("testXlsx2"));
            return _testXlsx2;
        }
    }

    private static TestXlsx3Table _testXlsx3;
    public static TestXlsx3Table TestXlsx3
    {
        get
        {
            if (_testXlsx3 == null)
                _testXlsx3 = new TestXlsx3Table(TableManager.Instance.GetTable("testXlsx3"));
            return _testXlsx3;
        }
    }

}
