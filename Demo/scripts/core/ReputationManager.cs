// 挂载于: GameRoot 子节点（Autoload 自动加载）

using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class ReputationManager : Node, ISaveable
{
    public static ReputationManager Instance { get; private set; }

    public string SaveKey => "reputation";

    private int _reputation;
    private int _level;
    private List<ReputationRow> _levelConfigs = new();

    public int MaxLevel => _levelConfigs.Count;

    public int Reputation
    {
        get => _reputation;
        private set
        {
            _reputation = System.Math.Max(0, value);
            UpdateLevel();
            EventBus.Publish(GameEvents.ReputationChanged, _reputation);
        }
    }

    public int Level => _level;
    public string LevelName => _level < _levelConfigs.Count ? _levelConfigs[_level].LevelName : "";

    public int CurrentThreshold => _level < _levelConfigs.Count ? _levelConfigs[_level].Threshold : 0;
    public int NextThreshold => _level + 1 < _levelConfigs.Count ? _levelConfigs[_level + 1].Threshold : -1;

    public float ProgressToNext
    {
        get
        {
            if (_level + 1 >= _levelConfigs.Count) return 1f;
            var current = _levelConfigs[_level].Threshold;
            var next = _levelConfigs[_level + 1].Threshold;
            var range = next - current;
            return range > 0 ? (float)(_reputation - current) / range : 1f;
        }
    }

    public override void _Ready()
    {
        Instance = this;
        _level = 0;
        _reputation = 0;
        LoadConfig();
    }

    private void LoadConfig()
    {
        var table = TableManager.Instance.GetTable("reputation");
        if (table == null)
        {
            GD.PrintErr("[ReputationManager] reputation 表加载失败");
            return;
        }

        _levelConfigs = table.GetAll()
            .OrderBy(r => r.GetInt("level"))
            .Select(r => new ReputationRow(r))
            .ToList();

        GD.Print($"[ReputationManager] 加载 {_levelConfigs.Count} 个声望等级配置");
    }

    public void AddReputation(int amount)
    {
        if (amount <= 0) return;
        var oldLevel = _level;
        Reputation = _reputation + amount;

        if (_level > oldLevel)
            GD.Print($"[ReputationManager] 声望升级! Lv.{oldLevel}({GetLevelName(oldLevel)}) → Lv.{_level}({LevelName})");
    }

    public bool CanAcceptOrder(int minReputation)
    {
        return _reputation >= minReputation;
    }

    public bool IsOrderTypeUnlocked(OrderType orderType)
    {
        if (_level >= _levelConfigs.Count) return true;
        var unlocked = _levelConfigs[_level].OrderUnlock;
        return unlocked != null && unlocked.Contains(orderType);
    }

    public string GetLevelName(int level)
    {
        return level < _levelConfigs.Count ? _levelConfigs[level].LevelName : "";
    }

    private void UpdateLevel()
    {
        _level = 0;
        for (int i = _levelConfigs.Count - 1; i >= 0; i--)
        {
            if (_reputation >= _levelConfigs[i].Threshold)
            {
                _level = i;
                break;
            }
        }
    }

    public Dictionary Serialize()
    {
        return new Dictionary
        {
            { "reputation", _reputation },
            { "level", _level },
        };
    }

    public void Deserialize(Dictionary data)
    {
        _reputation = data.GetValueOrDefault("reputation", 0).AsInt32();
        UpdateLevel();
        EventBus.Publish(GameEvents.ReputationChanged, _reputation);
    }
}
