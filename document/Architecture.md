# 《通货与铁砧》项目架构文档 (Architecture)

> 本文档记录项目所有脚本的定位、功能与依赖关系。每当完成一个新脚本时，必须同步更新此文档。

---

## 一、整体架构分层

```
┌─────────────────────────────────────────────────────────────┐
│                      UI 层 (scripts/ui/)                     │
│        铁砧界面 / 三选一弹窗 / 战斗HUD / 订单面板 / 集市界面    │
├─────────────────────────────────────────────────────────────┤
│                  管理器层 (Autoload 单例)                      │
│  GameRoot (唯一入口)                                          │
│  ├─ SaveManager          ├─ ResourceManager                  │
│  ├─ ReputationManager    ├─ CraftingManager [规划]            │
│  ├─ CombatManager [规划]  └─ ShopManager [规划]               │
├─────────────────────────────────────────────────────────────┤
│                  业务引擎层 (按模块隔离)                        │
│  AffixRegistry | AffixRoller | ThresholdAggregator | SkillEngine   │
│  CombatAI | OrderGenerator | TraitConditionParser            │
├─────────────────────────────────────────────────────────────┤
│                  数据模型层 (运行时业务模型)                     │
│  AdventurerData | EquipmentData | AffixData | TraitData      │
│  OrderData | ArchetypeData | CurrencyData | SkillData        │
├─────────────────────────────────────────────────────────────┤
│                  数据访问层 (自动生成)                          │
│  TableManager → TableData → 强类型包装 (XxxRow / XxxTable)   │
│  BeanConverter + __bean__.cs + __enum__.cs                   │
├─────────────────────────────────────────────────────────────┤
│                  数据源层 (外部配置)                            │
│  .xlsx → (Python xlsx2json) → .json                         │
└─────────────────────────────────────────────────────────────┘
```

**核心原则**：
- 配置与逻辑分离：所有数值走 Excel → JSON → 通用表加载器，代码只管逻辑
- 管理器单例协调：各系统通过 GameRoot 统一管理，Autoload 仅设 GameRoot 一个入口
- 模块高内聚低耦合：锻造/战斗/集市/订单各自闭环，通过 EventBus 信号通信
- 通用化代码生成：固定 C# 文件 + Bean 扩展 + 强类型包装，新增表零手写代码

---

## 二、目录结构

```
demo/scripts/
├── core/               # 核心系统
│   ├── manager/        #   全局管理器（GameRoot, SaveManager, ResourceManager, ReputationManager）
│   ├── model/          #   数据模型（AdventurerData, EquipmentData, AffixData, TraitData 等）
│   ├── EventBus.cs     #   全局事件总线（静态工具类）
│   ├── ISaveable.cs    #   存档接口
│   └── ObjectPool.cs   #   通用对象池（泛型工具类）
├── crafting/           # 锻造模块
├── combat/             # 战斗模块
├── order/              # 订单模块
├── shop/               # 集市模块 [规划]
├── ui/                 # UI 模块 [规划]
└── data/
    └── generated/      # 自动生成代码（禁止手动修改）

data/                   # xlsx 配表源文件（按系统分子文件夹）
tools/                  # Python 导表工具
document/               # 项目文档
```

---

## 三、脚本清单

### 3.1 核心系统 (`scripts/core/`)

#### 🏗️ 全局管理器 (`scripts/core/manager/`)

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `GameRoot.cs` | ✅ 已完成 | Autoload 唯一入口 | 管理所有子 Manager 生命周期与初始化顺序；窗口关闭时自动存档 | SaveManager, ResourceManager, ReputationManager |
| `EventBus.cs` | ✅ 已完成 | 静态工具类（非节点） | 全局发布/订阅事件总线，Manager 间解耦通信；`GameEvents` 常量集合统一事件名 | 无 |
| `ISaveable.cs` | ✅ 已完成 | 纯接口（非节点） | 存档契约：`SaveKey`（唯一标识）/ `Serialize()`（序列化）/ `Deserialize()`（反序列化） | Godot.Collections.Dictionary |
| `SaveManager.cs` | ✅ 已完成 | GameRoot 子节点 | 中心化存档管理：注册 ISaveable → 遍历序列化 → JSON 写入；支持多存档位/存档信息查询/删除 | ISaveable, EventBus |
| `ResourceManager.cs` | ✅ 已完成 | GameRoot 子节点 (ISaveable) | 金币与通货资源管理：增减操作带余额校验，变更自动广播 EventBus 事件 | EventBus, ISaveable |
| `ReputationManager.cs` | ✅ 已完成 | GameRoot 子节点 (ISaveable) | 声望等级管理：从 reputation.xlsx 配表加载阈值/等级名/订单解锁；进度查询/升级检测 | EventBus, ISaveable, TableManager |
| `ObjectPool.cs` | ✅ 已完成 | 泛型工具类（非节点） | 通用对象池：PackedScene/new T() 双创建模式；预热/归还/容量上限/进程控制 | Godot.Node |

#### 📦 运行时实例模型 (`scripts/core/model/`)

| 脚本 | 状态 | 定位 | 功能 | 关联配表 |
|------|------|------|------|----------|
| `Adventurer.cs` | ✅ 已完成 | 冒险者运行时实例 | 冒险者属性：等级/HP/基础攻防/攻速/移速/暴击/暴伤/CDR/被动特质ID列表；装备槽位管理；属性聚合计算 | adventurer.xlsx |
| `Equipment.cs` | ✅ 已完成 | 装备运行时实例 | 装备属性：底材名/iLvl/MaxAP/基础白值/前后缀槽位/技能池；AP消耗状态；词缀槽位管理 | equipment.xlsx |
| `TraitContext.cs` | ✅ 已完成 | 特质触发上下文 | 提供 hp/maxhp/combat_time/enemy_count 等变量供条件表达式读取 | 无 |
| `TraitConditionParser.cs` | ✅ 已完成 | 条件表达式解析器 | 解析 `hp:<30&enemyhp:>50` 格式，支持变量:操作符:值，支持 &/ 逻辑组合 | 无 |
| `AffixRegistry.cs` | ✅ 已完成 | 词缀注册表 | 从 Tables.Affix 加载词缀数据，按 Group/Slot/Id 建立索引；提供候选词缀查询 | affix.xlsx |
| `AffixRoller.cs` | ✅ 已完成 | 词缀抽取漏斗引擎 | iLvl过滤 → 同组去重 → 权重Roll点；提供单抽/三选一/指定组/指定Tag抽取 | AffixRegistry |

#### 📦 订单模块运行时实例 (`scripts/order/`)

| 脚本 | 状态 | 定位 | 功能 | 关联配表 |
|------|------|------|------|----------|
| `Order.cs` | ✅ 已完成 | 订单运行时实例 | 订单属性：名称/类型/难度/最低声望/流派ID/变体/冒险者/装备数/奖励；状态流转；奖励计算 | order.xlsx |

#### 📋 配表

| 配表 | 状态 | 字段概要 | 消费方 |
|------|------|----------|--------|
| `reputation.xlsx` | ✅ 已完成 | id, level, level_name, threshold, order_unlock(List\<OrderType\>), description | ReputationManager |

---

### 3.2 锻造模块 (`scripts/crafting/`)

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `CraftingManager.cs` | 🔲 规划中 | 锻造流程控制器 | 状态机：空闲→放入底材→选择通货→三选一→确认/超限；AP扣减/通货消耗/0AP超限腐化 | ResourceManager, AffixRoller, EventBus |

---

### 3.3 战斗模块 (`scripts/combat/`)

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `CombatManager.cs` | 🔲 规划中 | 战斗流程控制器 | 初始化→实时循环→结束判定 | EventBus, ObjectPool |
| `CombatUnit.cs` | 🔲 规划中 | 战斗单位 | 挂载 CharacterBody2D，俯视角移动+攻击 | Adventurer, Equipment |
| `ThresholdAggregator.cs` | 🔲 规划中 | 阈值汇总器 | 遍历全身装备，汇总技能等级，判断阈值坎 | Equipment, SkillRow |
| `SkillEngine.cs` | 🔲 规划中 | 技能执行引擎 | 根据阈值激活技能效果 | SkillRow, TraitRow |
| `CombatAI.cs` | 🔲 规划中 | 自动战斗 AI | 优先级决策树：有技能放技能→普攻→低血防御 | CombatUnit |
| `CombatResult.cs` | 🔲 规划中 | 战斗结果数据 | 得分/是否达标/DPS统计 | 无 |

---

### 3.4 订单模块 (`scripts/order/`)

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `OrderGenerator.cs` | 🔲 规划中 | 订单生成引擎 | 根据声望等级生成订单 | ReputationManager, TableManager |

---

### 3.5 集市模块 (`scripts/shop/`) [规划]

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `ShopManager.cs` | 🔲 规划中 | 集市管理器 | 底材刷新与交易逻辑 | ResourceManager, EventBus |

---

### 3.6 UI 模块 (`scripts/ui/`) [规划]

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| 铁砧主界面 | 🔲 规划中 | 锻造操作界面 | 装备面板 + AP进度条 + 通货选择栏 | CraftingManager |
| 三选一弹窗 | 🔲 规划中 | 词缀选择 UI | 展示3个候选词缀供玩家选择 | AffixRoller |
| 战斗 HUD | 🔲 规划中 | 战斗信息展示 | 双方血条 + 技能冷却指示 + 战斗计时 | CombatManager |
| 订单面板 | 🔲 规划中 | 订单浏览/接受 | 顾客信息 + 需求展示 + 赏金预览 | OrderGenerator |
| 集市界面 | 🔲 规划中 | 底材购买界面 | 底材列表 + 购买操作 | ShopManager |
| 结算界面 | 🔲 规划中 | 战斗结算 | 得分/达标判定/DPS统计 | CombatResult |

---

### 3.7 词缀引擎 (`scripts/core/model/`)

| 脚本 | 状态 | 定位 | 功能 | 依赖 |
|------|------|------|------|------|
| `AffixRegistry.cs` | ✅ 已完成 | 词缀注册表 | 从 Tables.Affix 加载词缀数据，按 Group/Slot/Id 建立索引 | affix.xlsx |
| `AffixRoller.cs` | ✅ 已完成 | 词缀抽取漏斗引擎 | iLvl过滤 → 同组去重 → 权重Roll点；提供单抽/三选一/指定组/指定Tag抽取 | AffixRegistry |

---

### 3.8 数据访问层 (`scripts/data/generated/`) — 自动生成，禁止手动修改

| 脚本 | 定位 | 功能 |
|------|------|------|
| `__enum__.cs` | 枚举定义 | 从 xlsx 收集的所有枚举类型（AdventurerRole, CurrencyType, OrderType 等 20 个） |
| `__bean__.cs` | 结构体定义 | Bean/结构体类型（ItemAward, ItemCost, DrawAward 等 6 个） |
| `BeanConverter.cs` | Bean 转换器 | Dictionary ↔ Bean 结构体的序列化/反序列化 |
| `TableRecord.cs` | 通用行记录 | 字典式行数据访问：GetInt/GetFloat/GetString/GetEnum/GetStringList/GetDict 等 |
| `TableData.cs` | 通用表数据 | 表级操作：GetAll/FindById/索引构建/延迟加载 |
| `TableManager.cs` | 表管理器 | 全局表注册中心：延迟加载 GetTable(name)/GetTable\<T\>()、PreloadTables()/PreloadAllTables()、类型注册 |
| `Tables.cs` | 强类型包装 | 每张表的 XxxRow（行包装）+ XxxTable（表包装），提供类型安全访问；支持 dict 类型字段 |

---

## 四、系统交互图

### 4.1 事件流

```
EventBus 全局事件常量 (GameEvents)
─────────────────────────────────
GoldChanged          ← ResourceManager 发布
CurrencyChanged      ← ResourceManager 发布
ReputationChanged    ← ReputationManager 发布
OrderAccepted        ← [规划] OrderGenerator 发布
OrderCompleted       ← [规划] OrderGenerator 发布
CombatStarted        ← [规划] CombatManager 发布
CombatEnded          ← [规划] CombatManager 发布
EquipmentCrafted     ← [规划] CraftingManager 发布
SaveCompleted        ← SaveManager 发布
LoadCompleted        ← SaveManager 发布
SceneChanged         ← [规划] 场景管理发布
```

### 4.2 存档数据流

```
GameRoot (Autoload)
  │
  ├─ _Ready()
  │   ├─ 创建 SaveManager
  │   ├─ 创建 ResourceManager → SaveManager.Register(resource)
  │   └─ 创建 ReputationManager → SaveManager.Register(reputation)
  │
  ├─ SaveGame(slot)
  │   ├─ SaveManager 遍历所有 ISaveable
  │   │   ├─ ResourceManager.Serialize() → {"gold":500, "currencies":{...}}
  │   │   └─ ReputationManager.Serialize() → {"reputation":300, "level":2}
  │   ├─ 拼合为 {"version":"0.1.0", "timestamp":"...", "resource":{...}, "reputation":{...}}
  │   └─ Json.Stringify() → 写入 user://saves/save_{slot}.sav
  │
  └─ LoadGame(slot)
      ├─ 读取 JSON → Json.Parse() → Dictionary
      ├─ SaveManager 遍历所有 ISaveable
      │   ├─ ReputationManager.Deserialize(data["reputation"])
      │   └─ ResourceManager.Deserialize(data["resource"])
      └─ 各 Manager 广播恢复事件 → UI 同步
```

### 4.3 核心游戏循环（规划）

```
接单 → 采购底材 → 锻造打造 → 战斗验证 → 结算
 ↑                                    │
 └──────── 声望/金币奖励 ←────────────┘
```

---

## 五、配表清单

| 配表 | 路径 | 状态 | 消费方 |
|------|------|------|--------|
| adventurer.xlsx | data/Adventurer/ | ✅ | AdventurerData |
| affix.xlsx | data/Affix/ | ✅ | AffixData, AffixRegistry |
| archetype.xlsx | data/Archetype/ | ✅ | ArchetypeData |
| archetype_squad.xlsx | data/Archetype/ | ✅ | ArchetypeSquadData |
| currency.xlsx | data/Currency/ | ✅ | CurrencyData |
| equipment.xlsx | data/Equipment/ | ✅ | EquipmentData |
| order.xlsx | data/Order/ | ✅ | OrderData |
| reputation.xlsx | data/Reputation/ | ✅ | ReputationManager |
| skill.xlsx | data/Skill/ | ✅ | SkillData |
| trait.xlsx | data/Trait/ | ✅ | TraitData |

---

## 六、枚举清单

所有枚举由导表工具从 xlsx 自动收集生成，定义在 `__enum__.cs` 中：

| 枚举 | 值 | 来源 |
|------|-----|------|
| AdventurerRole | Warrior, Ranger, Mage, Rogue | adventurer.xlsx |
| AffixSlotType | Prefix, Suffix | affix.xlsx |
| ArchetypeDimension | PureDamage, TankSurvival, SupportHeal, CritBurst, DotSpread, HybridComposite | archetype.xlsx |
| CurrencyEffect | RerollOne, ForceTag, AddSlot, RerollValue, RerollAll, Corrupt | currency.xlsx |
| CurrencyType | Hammer, Essence, Chisel, Sandpaper, Drill, VoidCore | currency.xlsx |
| EquipmentRarity | Normal, Magic, Rare, Unique | equipment.xlsx |
| EquipmentSlot | Weapon, Helmet, BodyArmor, Gloves, Boots, Ring, Amulet | equipment.xlsx |
| Gender | NoneGender, Male, Female | adventurer.xlsx |
| MaterialTag | Weapon, Sword, Dagger, Wand, Staff, BodyArmor, Helmet, Gloves, Boots, Ring, Amulet | archetype.xlsx |
| OrderDifficulty | Easy, Normal, Hard, Extreme | order.xlsx |
| OrderType | WalkIn, Regional, Guild | order.xlsx |
| OrderVariant | SingleMissing, FullSet, Commission, SquadDrill | order.xlsx |
| SceneType | Arena, Fortress, Endurance, Swarm, Boss, Dungeon | archetype.xlsx |
| SkillTrigger | OnCooldown, OnLowHp, OnCrit, OnKill, OnHit, Always | skill.xlsx |
| SkillType | Active, Passive | skill.xlsx |
| SquadRole | MainDps, RangerDps, MageDps, Tank, Support, Healer | archetype_squad.xlsx |
| StatType | Attack, Armor, MaxHp, AttackSpeed, MoveSpeed, CritRate, CritDmgMultiplier, CooldownReduction, IncreasedDamage, MoreDamage, IncreasedArmor, MoreArmor, HealPower, MoreHeal, FireResist, IceResist, PoisonResist, DotDamage, AoERadius | 多表共用 |
| TraitEffectType | StatMultiplier, StatAdditive, ForceCrit, Immunity, Dot, Heal, Shield, Aura, Summon, Dispel | trait.xlsx |
| TraitTriggerType | Always, OnCombatStart, OnCrit, OnHit, OnKill, OnDamageTaken, OnLowHp, OnHeal, OnDeath | trait.xlsx |
| TraitType | IronWill, FirstStrike, ArcaneAffinity, LethalStrike, CritMultiplier, ArmorCounter, Undying, PoisonBlade, Regeneration, WarCry | trait.xlsx |
