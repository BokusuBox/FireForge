// 挂载于: Autoload（project.godot 中注册为全局自动加载，唯一入口）

using Godot;

public partial class GameRoot : Node
{
    public static GameRoot Instance { get; private set; }

    public SaveManager SaveManager { get; private set; }
    public ResourceManager ResourceManager { get; private set; }
    public ReputationManager ReputationManager { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        InitializeManagers();
        RegisterSaveables();
        GD.Print("[GameRoot] 初始化完成");
    }

    private void InitializeManagers()
    {
        SaveManager = CreateChild<SaveManager>("SaveManager");
        ResourceManager = CreateChild<ResourceManager>("ResourceManager");
        ReputationManager = CreateChild<ReputationManager>("ReputationManager");
    }

    private void RegisterSaveables()
    {
        SaveManager.Register(ResourceManager);
        SaveManager.Register(ReputationManager);
    }

    private T CreateChild<T>(string name) where T : Node, new()
    {
        var node = new T();
        node.Name = name;
        AddChild(node);
        return node;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            SaveManager.SaveGame();
            GetTree().Quit();
        }
    }
}
