// 挂载于: GameRoot 子节点（Autoload 自动加载）

using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    private readonly List<ISaveable> _saveables = new();
    private const string SaveDir = "user://saves/";
    private const string SaveExtension = ".sav";

    public override void _Ready()
    {
        Instance = this;
        DirAccess.MakeDirRecursiveAbsolute(SaveDir);
    }

    public void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable)
    {
        _saveables.Remove(saveable);
    }

    public void SaveGame(int slot = 0)
    {
        var saveData = new Dictionary();
        saveData["version"] = ProjectSettings.GetSetting("application/config/version", "0.1.0").AsString();
        saveData["timestamp"] = System.DateTime.Now.ToString("o");

        foreach (var saveable in _saveables)
        {
            saveData[saveable.SaveKey] = saveable.Serialize();
        }

        var path = GetSavePath(slot);
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr($"[SaveManager] 存档写入失败: {FileAccess.GetOpenError()}");
            return;
        }

        var json = Json.Stringify(saveData, "\t");
        file.StoreString(json);
        file.Close();

        GD.Print($"[SaveManager] 存档已保存 → Slot {slot}");
        EventBus.Publish(GameEvents.SaveCompleted, slot);
    }

    public bool LoadGame(int slot = 0)
    {
        var path = GetSavePath(slot);
        if (!FileAccess.FileExists(path))
        {
            GD.PrintErr($"[SaveManager] 存档不存在: Slot {slot}");
            return false;
        }

        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"[SaveManager] 存档读取失败: {FileAccess.GetOpenError()}");
            return false;
        }

        var jsonStr = file.GetAsText();
        file.Close();

        var json = new Json();
        var err = json.Parse(jsonStr);
        if (err != Error.Ok)
        {
            GD.PrintErr($"[SaveManager] JSON解析失败: {json.GetErrorMessage()}");
            return false;
        }

        var saveData = json.Data.AsGodotDictionary();
        if (saveData == null)
        {
            GD.PrintErr("[SaveManager] 存档数据格式错误");
            return false;
        }

        foreach (var saveable in _saveables)
        {
            if (saveData.ContainsKey(saveable.SaveKey))
            {
                var data = saveData[saveable.SaveKey].AsGodotDictionary();
                if (data != null)
                    saveable.Deserialize(data);
            }
        }

        GD.Print($"[SaveManager] 存档已加载 ← Slot {slot}");
        EventBus.Publish(GameEvents.LoadCompleted, slot);
        return true;
    }

    public bool HasSave(int slot = 0)
    {
        return FileAccess.FileExists(GetSavePath(slot));
    }

    public void DeleteSave(int slot = 0)
    {
        var path = GetSavePath(slot);
        if (FileAccess.FileExists(path))
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(path));
    }

    public Dictionary GetSaveInfo(int slot = 0)
    {
        var path = GetSavePath(slot);
        if (!FileAccess.FileExists(path))
            return null;

        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null) return null;

        var json = new Json();
        json.Parse(file.GetAsText());
        file.Close();

        var data = json.Data.AsGodotDictionary();
        if (data == null) return null;

        return new Dictionary
        {
            { "version", data.GetValueOrDefault("version", "") },
            { "timestamp", data.GetValueOrDefault("timestamp", "") },
        };
    }

    private static string GetSavePath(int slot)
    {
        return $"{SaveDir}save_{slot}{SaveExtension}";
    }
}
