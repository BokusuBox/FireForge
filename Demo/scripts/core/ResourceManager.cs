// 挂载于: GameRoot 子节点（Autoload 自动加载）

using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class ResourceManager : Node, ISaveable
{
    public static ResourceManager Instance { get; private set; }

    public string SaveKey => "resource";

    private int _gold;
    private readonly Dictionary<CurrencyType, int> _currencies = new();

    public int Gold
    {
        get => _gold;
        set
        {
            var old = _gold;
            _gold = System.Math.Max(0, value);
            if (old != _gold)
                EventBus.Publish(GameEvents.GoldChanged, _gold);
        }
    }

    public override void _Ready()
    {
        Instance = this;
        InitCurrencies();
    }

    private void InitCurrencies()
    {
        foreach (CurrencyType type in System.Enum.GetValues(typeof(CurrencyType)))
            _currencies[type] = 0;
    }

    public int GetCurrency(CurrencyType type)
    {
        return _currencies.TryGetValue(type, out var val) ? val : 0;
    }

    public void SetCurrency(CurrencyType type, int amount)
    {
        var old = GetCurrency(type);
        _currencies[type] = System.Math.Max(0, amount);
        if (old != _currencies[type])
            EventBus.Publish(GameEvents.CurrencyChanged, type);
    }

    public bool AddCurrency(CurrencyType type, int amount)
    {
        if (amount < 0) return false;
        SetCurrency(type, GetCurrency(type) + amount);
        return true;
    }

    public bool SpendCurrency(CurrencyType type, int amount)
    {
        if (amount < 0) return false;
        if (GetCurrency(type) < amount) return false;
        SetCurrency(type, GetCurrency(type) - amount);
        return true;
    }

    public bool AddGold(int amount)
    {
        if (amount < 0) return false;
        Gold += amount;
        return true;
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0 || _gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public Dictionary Serialize()
    {
        var currencyDict = new Dictionary();
        foreach (var kv in _currencies)
            currencyDict[(int)kv.Key] = kv.Value;

        return new Dictionary
        {
            { "gold", _gold },
            { "currencies", currencyDict },
        };
    }

    public void Deserialize(Dictionary data)
    {
        _gold = data.GetValueOrDefault("gold", 0).AsInt32();

        var currencyDict = data.GetValueOrDefault("currencies", new Dictionary()).AsGodotDictionary();
        if (currencyDict != null)
        {
            foreach (var kv in currencyDict)
            {
                var type = (CurrencyType)kv.Key.AsInt32();
                _currencies[type] = kv.Value.AsInt32();
            }
        }

        EventBus.Publish(GameEvents.GoldChanged, _gold);
        EventBus.Publish(GameEvents.CurrencyChanged, CurrencyType.Hammer);
    }
}
