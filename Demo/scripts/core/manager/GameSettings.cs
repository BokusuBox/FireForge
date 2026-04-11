// 挂载于: GameRoot 子节点（Autoload 自动加载）

using Godot;
using Godot.Collections;

public partial class GameSettings : Node
{
    public static GameSettings Instance { get; private set; }

    private const string SettingsPath = "user://settings.json";
    private bool _enforcing;

    public enum WindowModeType
    {
        Windowed,
        Fullscreen,
        BorderlessFullscreen
    }

    public static readonly (Vector2I size, string label)[] SupportedResolutions =
    {
        (new Vector2I(1280, 720), "1280 × 720 (HD)"),
        (new Vector2I(1600, 900), "1600 × 900 (HD+)"),
        (new Vector2I(1920, 1080), "1920 × 1080 (Full HD)"),
        (new Vector2I(2560, 1440), "2560 × 1440 (QHD)"),
        (new Vector2I(3840, 2160), "3840 × 2160 (4K UHD)"),
    };

    public Vector2I Resolution { get; private set; } = new(1920, 1080);
    public WindowModeType CurrentWindowMode { get; private set; } = WindowModeType.Windowed;

    public override void _Ready()
    {
        Instance = this;
        LoadSettings();
        GetWindow().SizeChanged += OnWindowSizeChanged;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.F11)
        {
            ToggleFullscreen();
        }
    }

    public void ApplySettings(Vector2I resolution, WindowModeType windowMode)
    {
        Resolution = resolution;
        CurrentWindowMode = windowMode;
        ApplyToEngine();
        SaveSettings();
        EventBus.Publish(GameEvents.SettingsChanged);
    }

    public void ToggleFullscreen()
    {
        var window = GetWindow();
        if (window.Mode == Window.ModeEnum.Windowed)
        {
            CurrentWindowMode = WindowModeType.Fullscreen;
        }
        else
        {
            CurrentWindowMode = WindowModeType.Windowed;
        }
        ApplyToEngine();
        SaveSettings();
    }

    private void ApplyToEngine()
    {
        var window = GetWindow();

        switch (CurrentWindowMode)
        {
            case WindowModeType.Windowed:
                window.Mode = Window.ModeEnum.Windowed;
                window.Size = Resolution;
                break;
            case WindowModeType.Fullscreen:
                window.Mode = Window.ModeEnum.ExclusiveFullscreen;
                break;
            case WindowModeType.BorderlessFullscreen:
                window.Mode = Window.ModeEnum.Fullscreen;
                break;
        }
    }

    private void OnWindowSizeChanged()
    {
        if (_enforcing) return;
        _enforcing = true;
        EnforceAspectRatio();
        _enforcing = false;
    }

    private void EnforceAspectRatio()
    {
        var window = GetWindow();
        if (window.Mode != Window.ModeEnum.Windowed) return;

        var size = window.Size;
        int targetHeight = size.X * 9 / 16;
        if (size.Y != targetHeight)
        {
            window.Size = new Vector2I(size.X, targetHeight);
        }
    }

    private void SaveSettings()
    {
        var data = new Dictionary
        {
            { "resolution_x", Resolution.X },
            { "resolution_y", Resolution.Y },
            { "window_mode", (int)CurrentWindowMode },
        };

        var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr($"[GameSettings] 设置保存失败: {FileAccess.GetOpenError()}");
            return;
        }
        file.StoreString(Json.Stringify(data, "\t"));
        file.Close();
    }

    private void LoadSettings()
    {
        if (!FileAccess.FileExists(SettingsPath))
        {
            Resolution = DetectDefaultResolution();
            ApplyToEngine();
            return;
        }

        var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Read);
        if (file == null) return;

        var json = new Json();
        var err = json.Parse(file.GetAsText());
        file.Close();

        if (err != Error.Ok) return;

        var data = json.Data.AsGodotDictionary();
        if (data == null) return;

        Resolution = new Vector2I(
            GetDictInt(data, "resolution_x", 1920),
            GetDictInt(data, "resolution_y", 1080)
        );
        CurrentWindowMode = (WindowModeType)GetDictInt(data, "window_mode", 0);
        ApplyToEngine();
    }

    private Vector2I DetectDefaultResolution()
    {
        var screenSize = DisplayServer.ScreenGetSize();
        for (int i = SupportedResolutions.Length - 1; i >= 0; i--)
        {
            if (SupportedResolutions[i].size.X <= screenSize.X &&
                SupportedResolutions[i].size.Y <= screenSize.Y)
            {
                return SupportedResolutions[i].size;
            }
        }
        return new Vector2I(1280, 720);
    }

    private static int GetDictInt(Godot.Collections.Dictionary dict, string key, int defaultVal)
    {
        if (dict.ContainsKey(key))
            return ((Variant)dict[key]).AsInt32();
        return defaultVal;
    }
}
