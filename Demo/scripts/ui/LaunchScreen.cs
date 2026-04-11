// 挂载于: Control（启动设置界面根节点）

using Godot;

public partial class LaunchScreen : Control
{
	private OptionButton _resolutionOption;
	private OptionButton _windowModeOption;
	private Button _startButton;

	public override void _Ready()
	{
		_resolutionOption = GetNode<OptionButton>("%ResolutionOption");
		_windowModeOption = GetNode<OptionButton>("%WindowModeOption");
		_startButton = GetNode<Button>("%StartButton");

		PopulateResolutionOptions();
		PopulateWindowModeOptions();
		RestoreCurrentSettings();

		_startButton.Pressed += OnStartPressed;
	}

	private void PopulateResolutionOptions()
	{
		_resolutionOption.Clear();
		for (int i = 0; i < GameSettings.SupportedResolutions.Length; i++)
		{
			_resolutionOption.AddItem(GameSettings.SupportedResolutions[i].label, i);
		}
	}

	private void PopulateWindowModeOptions()
	{
		_windowModeOption.Clear();
		_windowModeOption.AddItem("窗口", (int)GameSettings.WindowModeType.Windowed);
		_windowModeOption.AddItem("全屏", (int)GameSettings.WindowModeType.Fullscreen);
		_windowModeOption.AddItem("无边框全屏", (int)GameSettings.WindowModeType.BorderlessFullscreen);
	}

	private void RestoreCurrentSettings()
	{
		var settings = GameSettings.Instance;

		for (int i = 0; i < GameSettings.SupportedResolutions.Length; i++)
		{
			if (GameSettings.SupportedResolutions[i].size == settings.Resolution)
			{
				_resolutionOption.Selected = i;
				break;
			}
		}

		_windowModeOption.Select((int)settings.CurrentWindowMode);
	}

	private void OnStartPressed()
	{
		int resIdx = _resolutionOption.GetSelectedId();
		var resolution = GameSettings.SupportedResolutions[resIdx].size;
		var windowMode = (GameSettings.WindowModeType)_windowModeOption.GetSelectedId();

		GameSettings.Instance.ApplySettings(resolution, windowMode);
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
