# 《通货与铁砧》项目开发进度表 (Project Schedule)

## 技术栈确认

| 项目 | 决策 |
|------|------|
| 引擎 | Godot 4.6 (.NET/C#) |
| 编程语言 | C# |
| 战斗模式 | 实时自动战斗（俯视角 Top-down） |
| 数据配置 | JSON 外部数据表（后续配套 Excel→JSON 转换工具） |
| 视角 | 2D 俯视角 |
| 存档策略 | 单机本地存档（JSON 序列化） |

---

## 架构模式

本项目采用 **数据驱动 + 管理器单例 + 模块化分层** 架构：

```
┌─────────────────────────────────────────────────────┐
│                    UI 层 (scripts/ui/)               │
│          铁砧界面 / 三选一弹窗 / 战斗HUD / 订单面板    │
├─────────────────────────────────────────────────────┤
│              管理器层 (Autoload 单例)                  │
│  GameRoot (唯一入口)                                  │
│  ├─ ResourceManager  ├─ ReputationManager            │
│  ├─ CraftingManager  ├─ CombatManager                │
│  └─ ShopManager      └─ SaveManager                  │
├─────────────────────────────────────────────────────┤
│              业务引擎层 (按模块隔离)                    │
│  AffixRoller | ThresholdAggregator | SkillEngine     │
│  CombatAI | OrderGenerator                           │
├─────────────────────────────────────────────────────┤
│              数据访问层 (自动生成)                      │
│  TableManager → TableData → 强类型包装 (XxxRow)       │
│  BeanConverter + __bean__.cs + __enum__.cs           │
├─────────────────────────────────────────────────────┤
│              数据源层 (外部配置)                        │
│  .xlsx → (Python xlsx2json) → .json                 │
└─────────────────────────────────────────────────────┘
```

**核心原则**：
- 配置与逻辑分离：所有数值走 Excel → JSON → 通用表加载器，代码只管逻辑
- 管理器单例协调：各系统通过 GameRoot 统一管理，Autoload 仅设 GameRoot 一个入口
- 模块高内聚低耦合：锻造/战斗/集市/订单各自闭环，通过 EventBus 信号通信
- 通用化代码生成：固定 C# 文件 + Bean 扩展 + 强类型包装，新增表零手写代码

---

## 第一阶段 Demo 目标

完成 **阶段一核心循环**：生成订单 → 洞察与采购 → AP打造博弈 → 实战验证 → 结算与重试

---

## M1 - 数据地基层

### 1.0 导表工具（前置）
- [x] 开发 Python 导表工具 `tools/xlsx2json.py`
- [x] 支持 xlsx → JSON 转换（类型系统：int/float/double/string/bool/枚举/list）
- [x] 支持 `##` 行跳过（不导出）
- [x] 自动生成 C# 代码（`__enum__.cs` / `XxxRecord.cs` / `XxxTable.cs` / `TableManager.cs`）
- [x] 枚举值从所有行（含##行）收集，确保枚举定义完整
- [x] 待用户将 xlsx 中 D2 的 `enum` 改为 `WeaponType` 后重新导出验证
- [x] 导表工具新增强类型包装类生成：每张表自动生成 `XxxRow.cs`（行包装）和 `XxxTable.cs`（表包装）
- [x] `XxxRow` 内部持有 `TableRecord` 引用，通过属性暴露强类型访问（如 `Id` / `WeaponName` / `WeaponType`）
- [x] `XxxTable` 内部持有 `TableData` 引用，提供 `GetAll()` / `FindById()` 等强类型查询方法

### 1.1 项目目录结构搭建
- [x] 创建 `tools/` 目录（导表工具）
- [x] 创建 `data/` 目录（xlsx 源文件，按系统分子文件夹）
- [x] 创建 `demo/data/` 目录（JSON 输出）
- [x] 创建 `demo/scripts/data/generated/` 目录（C# 自动生成代码）
- [x] 创建 `scripts/core/` 目录
- [x] 创建 `scripts/crafting/` 目录
- [x] 创建 `scripts/combat/` 目录
- [x] 创建 `scripts/shop/` 目录
- [x] 创建 `scripts/order/` 目录
- [x] 创建 `scripts/ui/` 目录
- [x] 创建 `resources/` 目录（Godot Resource）

### 1.2 核心数据模型定义
- [x] `AffixData.cs` - 词缀数据模型（Group/Tier/iLvl门槛/Weight 四维结构）+ `affix.xlsx` 配表
- [x] `EquipmentData.cs` - 装备数据模型（底材名/iLvl/MaxAP/基础白值/前后缀槽位）+ `equipment.xlsx` 配表
- [x] `AdventurerData.cs` - 冒险者数据模型（等级/HP/基础攻击/护甲/攻速/移速/暴击/暴伤/CDR/被动特质）+ `adventurer.xlsx` 配表
- [x] `OrderData.cs` + `ArchetypeData.cs` - 订单数据模型 + 流派模板库（3表拆分：archetype/archetype_squad/order）+ `order.xlsx` 配表
- [x] `CurrencyData.cs` - 通货数据模型（名称/AP消耗/效果类型枚举/工作室等级闸门）+ `currency.xlsx` 配表
- [x] `SkillData.cs` - 技能数据模型（技能名/类型/触发条件/阈值坎/冷却）+ `skill.xlsx` 配表
- [x] `TraitData.cs` - 被动特质数据模型（触发条件+效果类型分离）
  - 已创建 `trait.xlsx` 配表（字段：id, trait_type, trait_name, trigger_type, trigger_condition, effect_type, stat_type, value, duration, priority, description）
  - 已创建 `TraitData.cs` 运行时模型（从 TraitRow 构建，包含效果数值/乘区归属/触发条件）
  - 已创建 `TraitContext.cs` - 特质触发上下文（提供hp/maxhp/combat_time等变量）
  - 已创建 `TraitConditionParser.cs` - 条件表达式解析器（支持变量:操作符:值格式，支持&/|组合）
  - `AdventurerData.PassiveTraitIds` 已改为 `List<int>` 引用特质ID
  - 战斗公式集成待后续开发

#### ⚠️ 暂代字段清单（后续需替换/完善）

| 所属模型 | 字段 | 当前类型 | 暂代原因 | 后续方案 |
|----------|------|----------|----------|----------|
| ArchetypeData | `CorrectAffixGroups` | `List<int>` | 引用词缀组ID，但组ID格式为101/102等，与affix表group_id字段对齐待验证 | 锻造引擎开发时验证关联查询 |
| ArchetypeData | `CalculateTargetScore()` | 运行时方法 | 公式系数(Tier乘区0.3步进/iLvl缩放)为初版估算 | 战斗系统数值平衡时调参 |
| SkillData | `ThresholdLevels` | `List<int>` (从string解析) | 阈值坎效果描述仅在description字段文字说明 | 后续建 skill_threshold.xlsx 拆分每个坎的具体效果数值 |
| SkillData | `Description` | `string` | 技能效果描述为纯文本，战斗系统无法直接解析 | 后续拆分为结构化效果字段 |

### 1.3 词缀库引擎
- [ ] `AffixDb.cs` - 词缀数据库加载器（从 JSON 读取并缓存）
- [ ] `AffixRoller.cs` - 词缀抽取漏斗引擎（iLvl过滤 → 同组去重 → 权重Roll点 → 三选一生成）
- [ ] 创建最小词缀库 JSON（3-4个词缀组 × 3个Tier）

### 1.4 全局管理器（GameRoot 架构）
- [ ] `GameRoot.cs` - 唯一 Autoload 入口，内部管理所有子 Manager 生命周期与初始化顺序
- [ ] `EventBus.cs` - 全局事件总线，Manager 间通过 Godot Signal 解耦通信（信号名采用名词+过去式动词语义）
- [ ] `ISaveable.cs` - 存档接口，统一 `SaveKey` / `Serialize()` / `Deserialize()` 契约
- [ ] `SaveManager.cs` - 存档管理器，中心化管理所有 ISaveable 的序列化/反序列化/多存档位
- [ ] `ResourceManager.cs` - 金币/通货/素材资源管理（实现 ISaveable）
- [ ] `ReputationManager.cs` - 声望等级管理（实现 ISaveable）
- [ ] `ObjectPool.cs` - 通用对象池（支持 `Get()` / `Return()` / `Prewarm()`，M3 战斗系统前置）

### 1.5 数据访问层
- [ ] `TableManager` 改为延迟加载模式：首次 `GetTable()` 时才加载 JSON，可选 `PreloadTables()` 预加载核心表
- [ ] `TableManager` 新增 `GetTable<T>()` 泛型方法，返回强类型表包装
- [ ] 业务代码统一走强类型包装，禁止直接使用 `TableRecord.GetXxx()`

### 1.6 特质配表（Trait System）
- [x] `trait.xlsx` - 特质配表（字段：id, trait_type, trait_name, trigger_type, trigger_condition, effect_type, stat_type, value, duration, priority, description）
- [x] `TraitData.cs` - 特质运行时模型（从 TraitRow 构建，包含效果数值/乘区归属/触发条件）
- [x] `TraitContext.cs` - 特质触发上下文（提供hp/maxhp/combat_time等变量）
- [x] `TraitConditionParser.cs` - 条件表达式解析器（支持变量:操作符:值格式，支持&/|组合）
- [x] `AdventurerData.PassiveTraitIds` 从 `List<TraitType>` 改为 `List<int>` 引用特质ID
- [ ] 战斗公式集成：特质效果按 multiplier_category 分发到 POE 乘区

---

## M2 - 铁砧可敲（锻造系统）

### 2.1 锻造核心逻辑
- [ ] `CraftingManager.cs` - 锻造流程控制器（状态机：空闲→放入底材→选择通货→三选一→确认/超限）
- [ ] AP 扣减逻辑
- [ ] 通货消耗逻辑
- [ ] 0AP 超限模式（腐化变异机制）

### 2.2 锻造 UI
- [ ] 铁砧主界面（装备面板 + AP进度条 + 通货选择栏）
- [ ] 三选一弹窗 UI
- [ ] 装备词条面板展示
- [ ] 腐化变异提示 UI

---

## M3 - 战斗可观（战斗验证系统）

### 3.1 战斗核心逻辑
- [ ] `CombatManager.cs` - 战斗流程控制器（初始化→实时循环→结束判定）
- [ ] `CombatUnit.cs` - 战斗单位（挂载 CharacterBody2D，俯视角移动+攻击）
- [ ] `ThresholdAggregator.cs` - 阈值汇总器（遍历全身装备，汇总技能等级，判断阈值坎）
- [ ] `SkillEngine.cs` - 技能执行引擎（根据阈值激活技能效果）

### 3.2 战斗 AI
- [ ] `CombatAI.cs` - 自动战斗 AI（优先级决策树：有技能放技能→普攻→低血防御）

### 3.3 战斗场景
- [ ] 俯视角战斗场地场景搭建
- [ ] 战斗单位预制体（CharacterBody2D + 碰撞体 + 血条）
- [ ] 战斗 HUD（双方血条 + 技能冷却指示 + 战斗计时）

### 3.4 战斗结算
- [ ] `CombatResult.cs` - 战斗结果数据结构（得分/是否达标/DPS统计）
- [ ] 结算界面 UI

---

## M4 - 闭环 Demo（订单与经济循环）

### 4.1 订单系统
- [ ] `OrderGenerator.cs` - 订单生成引擎（根据声望等级生成订单）
- [ ] 订单面板 UI（顾客信息 + 需求展示 + 赏金预览）

### 4.2 集市系统
- [ ] `ShopManager.cs` - 集市刷新与交易逻辑
- [ ] 集市界面 UI（底材列表 + 购买操作）

### 4.3 主流程串联
- [ ] 游戏主场景搭建（铁匠铺主界面）
- [ ] 场景切换流程（铁匠铺 ↔ 集市 ↔ 战斗场景）
- [ ] 主菜单 UI
- [ ] 完整闭环测试：接单→买材→打造→验货→结算

---

## 开发日志

| 日期 | 里程碑 | 完成内容 | 遇到的问题 | 解决方案 |
|------|--------|----------|------------|----------|
| 2026-04-08 | - | 项目初始化，确认技术栈与架构规划 | - | - |
| 2026-04-09 | M1-1.0 | 完成导表工具开发与验证 | xlsx中D2为`enum`（C#关键字），需改为`WeaponType` | 用户已更新xlsx，重新导出验证通过 |
| 2026-04-09 | M1-1.0 | 重构C#代码生成：合并为通用TableRecord+TableData | 每张表独立Record/Table类导致脚本膨胀 | 重构为通用字典模式，固定4个C#文件 |
| 2026-04-09 | M1-1.0 | 验证多文件夹xlsx扫描+exe打包 | - | data/test/和data/test2/两个文件夹均正确扫描 |
| 2026-04-09 | M1-1.0 | 新增Bean/结构体支持：`__bean__.xlsx`特殊表 | - | 解析Bean定义、验证引用、生成`__bean__.cs`和`BeanConverter.cs` |
| 2026-04-09 | M1-1.0 | 修复JSON输出Bug：空行产生null记录 | Bean续行（基本列全空）被当作独立记录 | 引入主行/续行模式，基本列有值时开始新记录 |
| 2026-04-09 | M1-1.0 | 修复Bean列跨度计算错误 | `bean_col_count=len(fields)`未递归展开嵌套Bean | 新增`bean_col_span()`递归计算，`build_bean_col_layout()`递归构建 |
| 2026-04-09 | M1-1.0 | 修复`parse_bean_xlsx`丢失同行字段 | full_name和第一个字段在同一行时被跳过 | 创建新Bean后检查同行是否有字段数据 |
| 2026-04-09 | M1-1.0 | 支持动态表头行数：根据Bean嵌套深度计算 | 固定从Row 6开始读数据导致普通表数据丢失 | `header_rows = 3 + (1 + max_nesting)`，无Bean时从Row 4开始 |
| 2026-04-09 | 架构 | 架构优化设计：6项优化规划写入文档 | 单机游戏缺少存档系统、数据访问层类型不安全、Manager间耦合风险 | 确定A强类型包装/B存档系统/C事件总线/D延迟加载/E GameRoot/F对象池，整合进M1里程碑 |
| 2026-04-09 | 架构 | 将架构优化设计融入正式开发目标 | "架构优化"独立章节与M1里程碑内容重复，且标注混乱 | 删除独立章节，内容直接写入M1各子节，清理所有"→ 对应架构优化X"标注 |
| 2026-04-10 | M1-1.0 | 实现强类型包装类自动生成（Tables.cs） | 枚举列表在TableData中被错误当作Bean列表处理 | 修复ParseFieldType新增IsEnumList标志，枚举列表存为List<string>，包装器用Enum.Parse转换 |
| 2026-04-10 | M1-1.6 | 完成特质系统（Trait System）开发 | trait.xlsx表头格式不正确导致未生成 | 修复表头格式（##字段名称行），成功导出TraitRow/TraitTable |
| 2026-04-10 | M1-1.6 | 创建TraitContext/TraitConditionParser | - | 支持复杂条件表达式（hp:<30&enemyhp:>50），支持&/|逻辑组合 |
