# 《通货与铁砧》项目开发进度表 (Project Schedule)

## 技术栈确认

| 项目 | 决策 |
|------|------|
| 引擎 | Godot 4.6 (.NET/C#) |
| 编程语言 | C# |
| 战斗模式 | 实时自动战斗（俯视角 Top-down） |
| 数据配置 | JSON 外部数据表（后续配套 Excel→JSON 转换工具） |
| 视角 | 2D 俯视角 |

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

### 1.1 项目目录结构搭建
- [x] 创建 `tools/` 目录（导表工具）
- [x] 创建 `data/` 目录（xlsx 源文件，按系统分子文件夹）
- [x] 创建 `demo/data/` 目录（JSON 输出）
- [x] 创建 `demo/scripts/data/generated/` 目录（C# 自动生成代码）
- [ ] 创建 `scripts/core/` 目录及全局管理器骨架
- [ ] 创建 `scripts/crafting/` 目录
- [ ] 创建 `scripts/combat/` 目录
- [ ] 创建 `scripts/shop/` 目录
- [ ] 创建 `scripts/order/` 目录
- [ ] 创建 `scripts/ui/` 目录
- [ ] 创建 `resources/` 目录（Godot Resource）

### 1.2 核心数据模型定义
- [ ] `AffixData.cs` - 词缀数据模型（Group/Tier/iLvl门槛/Weight 四维结构）
- [ ] `EquipmentData.cs` - 装备数据模型（底材名/iLvl/MaxAP/基础白值/前后缀槽位）
- [ ] `AdventurerData.cs` - 冒险者数据模型（等级/HP/基础攻击/护甲/攻速/移速/暴击/暴伤/CDR/被动特质）
- [ ] `OrderData.cs` - 订单数据模型（顾客引用/需求描述/目标技能阈值/考核分数/赏金）
- [ ] `CurrencyData.cs` - 通货数据模型（名称/AP消耗/效果类型枚举）
- [ ] `SkillData.cs` - 技能数据模型（技能名/等级/阈值坎/效果描述）

### 1.3 词缀库引擎
- [ ] `AffixDb.cs` - 词缀数据库加载器（从 JSON 读取并缓存）
- [ ] `AffixRoller.cs` - 词缀抽取漏斗引擎（iLvl过滤 → 同组去重 → 权重Roll点 → 三选一生成）
- [ ] 创建最小词缀库 JSON（3-4个词缀组 × 3个Tier）

### 1.4 全局管理器
- [ ] `GameManager.cs` - 游戏全局状态管理（Autoload 单例）
- [ ] `ResourceManager.cs` - 金币/通货/素材资源管理
- [ ] `ReputationManager.cs` - 声望等级管理

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
