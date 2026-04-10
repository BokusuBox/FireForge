// 挂载于: 无（纯接口定义，非节点脚本）

using Godot.Collections;

public interface ISaveable
{
    string SaveKey { get; }

    Dictionary Serialize();

    void Deserialize(Dictionary data);
}
