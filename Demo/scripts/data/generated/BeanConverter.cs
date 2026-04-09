// 自动生成的代码 - 请勿手动修改
// Bean转换器：将Godot字典转换为Bean对象

using Godot;
using System;
using System.Collections.Generic;

public static class BeanConverter
{
    public static DrawAward ToDrawAward(Godot.Collections.Dictionary dict)
    {
        var bean = new DrawAward();
        if (dict.ContainsKey("award_id_list"))
        {
            var arr = dict["award_id_list"].AsGodotArray();
            var list = new List<int>();
            foreach (var item in arr) list.Add(item.AsInt32());
            bean.award_id_list = list;
        }
        if (dict.ContainsKey("award_result_list"))
        {
            var arr = dict["award_result_list"].AsGodotArray();
            var list = new List<int>();
            foreach (var item in arr) list.Add(item.AsInt32());
            bean.award_result_list = list;
        }
        return bean;
    }

    public static ItemAward ToItemAward(Godot.Collections.Dictionary dict)
    {
        var bean = new ItemAward();
        if (dict.ContainsKey("id")) bean.id = dict["id"].AsInt32();
        if (dict.ContainsKey("num")) bean.num = dict["num"].AsInt32();
        return bean;
    }

    public static ItemCondition ToItemCondition(Godot.Collections.Dictionary dict)
    {
        var bean = new ItemCondition();
        if (dict.ContainsKey("id")) bean.id = dict["id"].AsInt32();
        if (dict.ContainsKey("num")) bean.num = dict["num"].AsInt32();
        return bean;
    }

    public static ItemCost ToItemCost(Godot.Collections.Dictionary dict)
    {
        var bean = new ItemCost();
        if (dict.ContainsKey("id")) bean.id = dict["id"].AsInt32();
        if (dict.ContainsKey("num")) bean.num = dict["num"].AsInt32();
        return bean;
    }

    public static ItemRandomNum ToItemRandomNum(Godot.Collections.Dictionary dict)
    {
        var bean = new ItemRandomNum();
        if (dict.ContainsKey("item_id")) bean.item_id = dict["item_id"].AsInt32();
        if (dict.ContainsKey("min_num")) bean.min_num = dict["min_num"].AsInt32();
        if (dict.ContainsKey("max_num")) bean.max_num = dict["max_num"].AsInt32();
        return bean;
    }

    public static ItemTest ToItemTest(Godot.Collections.Dictionary dict)
    {
        var bean = new ItemTest();
        if (dict.ContainsKey("id")) bean.id = dict["id"].AsInt32();
        if (dict.ContainsKey("award"))
        {
            var arr = dict["award"].AsGodotArray();
            var list = new List<ItemAward>();
            foreach (var item in arr)
                list.Add(ToItemAward(item.AsGodotDictionary()));
            bean.award = list;
        }
        return bean;
    }

    public static T FromDict<T>(Godot.Collections.Dictionary dict) where T : new()
    {
        var typeName = typeof(T).Name;
        return typeName switch
        {
            "DrawAward" => (T)(object)ToDrawAward(dict),
            "ItemAward" => (T)(object)ToItemAward(dict),
            "ItemCondition" => (T)(object)ToItemCondition(dict),
            "ItemCost" => (T)(object)ToItemCost(dict),
            "ItemRandomNum" => (T)(object)ToItemRandomNum(dict),
            "ItemTest" => (T)(object)ToItemTest(dict),
            _ => new T()
        };
    }
}
