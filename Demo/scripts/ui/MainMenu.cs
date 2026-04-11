// 挂载于: Control（主菜单根节点）

using Godot;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/NewGameButton").Pressed += OnNewGamePressed;
        GetNode<Button>("VBoxContainer/QuitButton").Pressed += OnQuitPressed;
    }

    private void OnNewGamePressed()
    {
        GD.Print("[MainMenu] 新游戏 - 待实现");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
