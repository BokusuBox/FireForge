// 挂载于: 无（纯数据模型，非节点脚本）

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;

public static class TraitConditionParser
{
    public static bool Evaluate(string condition, TraitContext context)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return true;

        condition = condition.Trim();

        if (condition.Contains("|"))
        {
            var orParts = condition.Split('|');
            foreach (var part in orParts)
            {
                if (EvaluateAnd(part.Trim(), context))
                    return true;
            }
            return false;
        }

        return EvaluateAnd(condition, context);
    }

    private static bool EvaluateAnd(string condition, TraitContext context)
    {
        if (condition.Contains("&"))
        {
            var andParts = condition.Split('&');
            foreach (var part in andParts)
            {
                if (!EvaluateSingle(part.Trim(), context))
                    return false;
            }
            return true;
        }

        return EvaluateSingle(condition, context);
    }

    private static bool EvaluateSingle(string condition, TraitContext context)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return true;

        var match = Regex.Match(condition, @"^(\w+)\s*([<>=!]+)\s*(.+)$");
        if (!match.Success)
        {
            GD.PrintErr($"[TraitConditionParser] Invalid condition format: {condition}");
            return false;
        }

        var variable = match.Groups[1].Value.ToLower();
        var op = match.Groups[2].Value;
        var valueStr = match.Groups[3].Value.Trim();

        var contextValue = context.GetValue(variable);

        if (!float.TryParse(valueStr, out var compareValue))
        {
            var otherValue = context.GetValue(valueStr);
            if (otherValue != 0f || context.CustomValues.ContainsKey(valueStr.ToLower()))
            {
                compareValue = otherValue;
            }
            else
            {
                GD.PrintErr($"[TraitConditionParser] Cannot parse value: {valueStr}");
                return false;
            }
        }

        return op switch
        {
            "<" => contextValue < compareValue,
            "<=" => contextValue <= compareValue,
            ">" => contextValue > compareValue,
            ">=" => contextValue >= compareValue,
            "==" or "=" => Math.Abs(contextValue - compareValue) < 0.001f,
            "!=" => Math.Abs(contextValue - compareValue) >= 0.001f,
            _ => false,
        };
    }

    public static List<string> ParseVariables(string condition)
    {
        var variables = new List<string>();
        if (string.IsNullOrWhiteSpace(condition))
            return variables;

        var parts = condition.Split(new[] { '&', '|' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var match = Regex.Match(part.Trim(), @"^(\w+)\s*[<>=!]+\s*.+$");
            if (match.Success)
            {
                variables.Add(match.Groups[1].Value.ToLower());
            }
        }

        return variables;
    }
}
