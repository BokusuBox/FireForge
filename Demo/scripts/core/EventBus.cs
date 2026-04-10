// 挂载于: 无（纯静态工具类，非节点脚本）

using Godot;
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<string, List<Delegate>> _handlers = new();

    public static void Subscribe<T>(string eventName, Action<T> handler)
    {
        if (!_handlers.ContainsKey(eventName))
            _handlers[eventName] = new List<Delegate>();
        _handlers[eventName].Add(handler);
    }

    public static void Subscribe(string eventName, Action handler)
    {
        if (!_handlers.ContainsKey(eventName))
            _handlers[eventName] = new List<Delegate>();
        _handlers[eventName].Add(handler);
    }

    public static void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (_handlers.TryGetValue(eventName, out var list))
            list.Remove(handler);
    }

    public static void Unsubscribe(string eventName, Action handler)
    {
        if (_handlers.TryGetValue(eventName, out var list))
            list.Remove(handler);
    }

    public static void Publish<T>(string eventName, T data)
    {
        if (!_handlers.TryGetValue(eventName, out var list))
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            try
            {
                (list[i] as Action<T>)?.Invoke(data);
            }
            catch (Exception e)
            {
                GD.PrintErr($"[EventBus] 事件 '{eventName}' 处理异常: {e.Message}");
            }
        }
    }

    public static void Publish(string eventName)
    {
        if (!_handlers.TryGetValue(eventName, out var list))
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            try
            {
                (list[i] as Action)?.Invoke();
            }
            catch (Exception e)
            {
                GD.PrintErr($"[EventBus] 事件 '{eventName}' 处理异常: {e.Message}");
            }
        }
    }

    public static void Clear()
    {
        _handlers.Clear();
    }

    public static void Clear(string eventName)
    {
        if (_handlers.ContainsKey(eventName))
            _handlers.Remove(eventName);
    }
}

public static class GameEvents
{
    public const string GoldChanged = "GoldChanged";
    public const string CurrencyChanged = "CurrencyChanged";
    public const string ReputationChanged = "ReputationChanged";
    public const string OrderAccepted = "OrderAccepted";
    public const string OrderCompleted = "OrderCompleted";
    public const string CombatStarted = "CombatStarted";
    public const string CombatEnded = "CombatEnded";
    public const string EquipmentCrafted = "EquipmentCrafted";
    public const string SaveCompleted = "SaveCompleted";
    public const string LoadCompleted = "LoadCompleted";
    public const string SceneChanged = "SceneChanged";
}
