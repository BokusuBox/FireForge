// 挂载于: 无（泛型工具类，非节点脚本。使用时由调用方 Node 持有实例）

using Godot;
using System.Collections.Generic;

public class ObjectPool<T> where T : Node, new()
{
    private readonly Stack<T> _pool = new();
    private readonly PackedScene _scene;
    private readonly int _maxSize;
    private readonly Node _owner;
    private int _activeCount;

    public int PoolSize => _pool.Count;
    public int ActiveCount => _activeCount;
    public int MaxSize => _maxSize;

    public ObjectPool(Node owner, PackedScene scene, int prewarm = 0, int maxSize = 100)
    {
        _owner = owner;
        _scene = scene;
        _maxSize = maxSize;
        _activeCount = 0;

        for (int i = 0; i < prewarm; i++)
        {
            var instance = CreateInstance();
            instance.SetProcess(false);
            instance.SetPhysicsProcess(false);
            SetVisible(instance, false);
            _pool.Push(instance);
        }
    }

    public ObjectPool(Node owner, string scenePath, int prewarm = 0, int maxSize = 100)
        : this(owner, GD.Load<PackedScene>(scenePath), prewarm, maxSize)
    {
    }

    public T Get()
    {
        _activeCount++;

        if (_pool.Count > 0)
        {
            var instance = _pool.Pop();
            instance.SetProcess(true);
            instance.SetPhysicsProcess(true);
            SetVisible(instance, true);
            return instance;
        }

        return CreateInstance();
    }

    public void Return(T instance)
    {
        if (_pool.Count >= _maxSize)
        {
            instance.QueueFree();
            _activeCount--;
            return;
        }

        instance.SetProcess(false);
        instance.SetPhysicsProcess(false);
        SetVisible(instance, false);

        if (instance.GetParent() != null)
            instance.GetParent().RemoveChild(instance);

        _pool.Push(instance);
        _activeCount--;
    }

    public void ReturnAll()
    {
        var children = new List<T>();
        foreach (var child in _owner.GetChildren())
        {
            if (child is T typed)
                children.Add(typed);
        }

        foreach (var child in children)
            Return(child);
    }

    public void Clear()
    {
        while (_pool.Count > 0)
            _pool.Pop().QueueFree();
        _activeCount = 0;
    }

    private T CreateInstance()
    {
        if (_scene != null)
            return _scene.Instantiate<T>();

        return new T();
    }

    private static void SetVisible(Node node, bool visible)
    {
        if (node is CanvasItem ci)
            ci.Visible = visible;
        else if (node is Node3D n3d)
            n3d.Visible = visible;
    }
}
