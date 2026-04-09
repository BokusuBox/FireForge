// 自动生成的代码 - 请勿手动修改
// 枚举定义：由导表工具根据 __enum__.xlsx 和数据表自动生成

public enum AdventurerRole
{
    Warrior = 1,  // 战士
    Ranger = 2,  // 游侠
    Mage = 3,  // 法师
    Rogue = 4  // 刺客
}

public enum AffixSlotType
{
    Prefix = 1,  // 前缀
    Suffix = 2  // 后缀
}

public enum ArchetypeDimension
{
    PureDamage = 1,  // 纯输出
    TankSurvival = 2,  // 肉盾生存
    SupportHeal = 3,  // 辅助奶妈
    CritBurst = 4,  // 暴击爆发
    DotSpread = 5,  // 持续蔓延
    HybridComposite = 6  // 复合构筑
}

public enum CurrencyEffect
{
    RerollOne = 1,  // 重置单条词条(三选一)
    ForceTag = 2,  // 强制生成指定标签词条
    AddSlot = 3,  // 追加新词条槽位
    RerollValue = 4,  // 重Roll词条数值
    RerollAll = 5,  // 重置全部词条
    Corrupt = 6  // 不可逆腐化变异
}

public enum CurrencyType
{
    Hammer = 1,  // 学徒改造锤
    Essence = 2,  // 定向元素精华
    Chisel = 3,  // 崇高打孔锥
    Sandpaper = 4,  // 神圣微调砂纸
    Drill = 5,  // 混沌重铸钻
    VoidCore = 6  // 虚空魔核
}

public enum EquipmentRarity
{
    Normal = 1,  // 普通(白)
    Magic = 2,  // 魔法(蓝)
    Rare = 3,  // 稀有(黄)
    Unique = 4  // 传奇(橙)
}

public enum EquipmentSlot
{
    Weapon = 1,  // 武器
    Helmet = 2,  // 头盔
    BodyArmor = 3,  // 胸甲
    Gloves = 4,  // 手套
    Boots = 5,  // 靴子
    Ring = 6,  // 戒指
    Amulet = 7  // 护身符
}

public enum Gender
{
    NoneGender = 1,  // 无
    Male = 2,  // 男性
    Female = 3  // 女性
}

public enum OrderDifficulty
{
    Easy = 1,  // 简单
    Normal = 2,  // 普通
    Hard = 3,  // 困难
    Extreme = 4  // 极限
}

public enum OrderType
{
    WalkIn = 1,  // 散客订单
    Regional = 2,  // 区域倾向订单
    Guild = 3  // 公会订单
}

public enum OrderVariant
{
    SingleMissing = 1,  // 残缺拼图-单件
    FullSet = 2,  // 完全体拼图-多件套
    Commission = 3,  // 代工订单-锁底材
    SquadDrill = 4  // 实战编队演练
}

public enum SkillTrigger
{
    OnCooldown = 1,  // 冷却完毕自动释放
    OnLowHp = 2,  // 低血量触发
    OnCrit = 3,  // 暴击时触发
    OnKill = 4,  // 击杀时触发
    OnHit = 5,  // 受击时触发
    Always = 6  // 常驻生效
}

public enum SkillType
{
    Active = 1,  // 主动技能
    Passive = 2  // 质变被动
}

public enum WeaponType
{
    Dagger = 1,  // 小刀
    Sword = 2,  // 剑
    Wand = 3,  // 魔杖
    Staff = 4  // 法杖
}
