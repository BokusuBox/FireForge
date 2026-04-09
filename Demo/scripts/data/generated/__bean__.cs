// 自动生成的代码 - 请勿手动修改
// 结构体定义：由导表工具根据 __bean__.xlsx 自动生成

using Godot;
using System;
using System.Collections.Generic;

public class DrawAward
{
    public List<int> award_id_list;
    public List<int> award_result_list;

    public override string ToString()
    {
        return $"DrawAward({award_id_list}, {award_result_list})";
    }
}

public class ItemAward
{
    public int id;
    public int num;

    public override string ToString()
    {
        return $"ItemAward({id}, {num})";
    }
}

public class ItemCondition
{
    public int id;
    public int num;

    public override string ToString()
    {
        return $"ItemCondition({id}, {num})";
    }
}

public class ItemCost
{
    public int id;
    public int num;

    public override string ToString()
    {
        return $"ItemCost({id}, {num})";
    }
}

public class ItemRandomNum
{
    public int item_id;
    public int min_num;
    public int max_num;

    public override string ToString()
    {
        return $"ItemRandomNum({item_id}, {min_num}, {max_num})";
    }
}

public class ItemTest
{
    public int id;
    public List<ItemAward> award;

    public override string ToString()
    {
        return $"ItemTest({id}, {award})";
    }
}
