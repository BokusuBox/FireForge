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

public enum MaterialTag
{
    Weapon = 1,  // 武器类底材
    Sword = 2,  // 剑类底材
    Dagger = 3,  // 匕首类底材
    Wand = 4,  // 法杖类底材
    Staff = 5,  // 长杖类底材
    BodyArmor = 6,  // 胸甲类底材
    Helmet = 7,  // 头盔类底材
    Gloves = 8,  // 手套类底材
    Boots = 9,  // 靴子类底材
    Ring = 10,  // 戒指类底材
    Amulet = 11  // 项链类底材
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

public enum SceneType
{
    Arena = 1,  // 竞技场-标准战斗
    Fortress = 2,  // 堡垒-防守战
    Endurance = 3,  // 耐力试炼-生存挑战
    Swarm = 4,  // 虫群-大量敌人
    Boss = 5,  // Boss战-单体强敌
    Dungeon = 6  // 地下城-综合挑战
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
    Active = 1,  // 主动
    Passive = 2  // 被动
}

public enum SquadRole
{
    MainDps = 1,  // 主输出
    RangerDps = 2,  // 远程输出
    MageDps = 3,  // 法师输出
    Tank = 4,  // 坦克
    Support = 5,  // 辅助
    Healer = 6  // 治疗
}

public enum StatType
{
    Attack = 1,  // 攻击力-基础点伤
    Armor = 2,  // 护甲-物理减伤
    MaxHp = 3,  // 最大生命值
    AttackSpeed = 4,  // 攻击速度-动作速率
    MoveSpeed = 5,  // 移动速度
    CritRate = 6,  // 暴击率
    CritDmgMultiplier = 7,  // 暴击伤害倍率
    CooldownReduction = 8,  // 冷却缩减
    IncreasedDamage = 9,  // A类增伤(同类相加)
    MoreDamage = 10,  // B类独立乘区伤害
    IncreasedArmor = 11,  // A类增防(同类相加)
    MoreArmor = 12,  // B类独立乘区防御
    HealPower = 13,  // 治疗强度
    MoreHeal = 14,  // B类独立乘区治疗
    FireResist = 15,  // 火焰抗性
    IceResist = 16,  // 冰霜抗性
    PoisonResist = 17,  // 毒素抗性
    DotDamage = 18,  // 持续伤害
    AoERadius = 19  // AOE范围
}

public enum TraitEffectType
{
    StatMultiplier = 1,  // 属性乘区(B类独立)
    StatAdditive = 2,  // 属性加成(A类相加)
    ForceCrit = 3,  // 强制暴击
    Immunity = 4,  // 免疫/无敌
    Dot = 5,  // 持续伤害
    Heal = 6,  // 治疗
    Shield = 7,  // 护盾
    Aura = 8,  // 光环(全队)
    Summon = 9,  // 召唤
    Dispel = 10  // 驱散
}

public enum TraitTriggerType
{
    Always = 1,  // 常驻被动
    OnCombatStart = 2,  // 战斗开始
    OnCrit = 3,  // 暴击时
    OnHit = 4,  // 命中时
    OnKill = 5,  // 击杀时
    OnDamageTaken = 6,  // 受伤时
    OnLowHp = 7,  // 低血量时
    OnHeal = 8,  // 治疗时
    OnDeath = 9  // 死亡时
}

public enum TraitType
{
    IronWill = 1,  // 坚韧意志-受伤减免
    FirstStrike = 2,  // 先手优势-开局爆发
    ArcaneAffinity = 3,  // 奥术亲和-法术增幅
    LethalStrike = 4,  // 致命一击-必定暴击
    CritMultiplier = 5,  // 暴伤翻倍-暴击伤害乘区
    ArmorCounter = 6,  // 破甲反击-受击触发反击
    Undying = 7,  // 不死之身-低血量无敌护盾
    PoisonBlade = 8,  // 毒刃-命中附加毒伤
    Regeneration = 9,  // 生命涌泉-持续回复
    WarCry = 10  // 战吼-全队增伤光环
}
