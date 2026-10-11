# FYP 引擎选型总结

更新日期：2026-10-09  
状态：候选决策；尚未完成 engine spike，未正式锁定引擎。

最新评估（本日需求细化之后）：助手建议优先用 **Unity** 实现首个产品闭环，UE 保留为候选；这是针对当前需求的工程判断，不是用户已确认的选型，也不是本机运行验证结果。理由和条件见第 10 节。下面第 1–8 节保留本日较早的、以官方 MCP 为主要依据的候选总结。

## 1. 本日较早候选结论（尚未锁定）

- **Primary candidate：Unreal Engine 5.8。** 优先验证官方 MCP、agent 操作编辑器和后续 runtime 接口能否支撑项目。
- **Fallback：Unity 2022.3 / 团结引擎。** 如果 UE 的连接、扩展或迭代成本不达标，切回更熟悉的 Unity 工作流；两者是分别验证的备选环境，不能默认兼容。
- **Godot：暂不选。** 保留为需求变化后的候选，尤其是 Web-first、轻量部署或开源可控性成为关键要求时。

这是一份针对本 FYP 的判断，不是通用引擎排名。**UE 5.8 官方 MCP 仍是 Experimental，必须先做最小 engine spike，再决定是否正式采用。** 官方身份提供了可研究的接口基础，并不证明它比社区方案更稳定或更省时间。[Epic 官方 Unreal MCP 文档](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)

## 2. 项目目标与决策依据

近期先完成一个小型 pseudo-sandbox（伪沙盒）可玩原型：一个玩法模板、少量可编辑对象、明确的编辑范围，以及 Play/Test loop。AI/agent 应能读取场景、执行修改、检查实际结果，并在失败后获得可用于修正的反馈。

本阶段优先级为：

1. AI/agent 与引擎之间能否形成可重复的操作—验证闭环。
2. 本机与中国使用环境下，安装、依赖和版本是否可持续维护。
3. 能否快速产出可展示的 3D 原型，并支持受控的关卡编辑。
4. 自定义工具与 runtime 接口的扩展成本。
5. 部署方式、学习成本与 FYP 时间预算。

先验证本地工程，不让本机与云端同步成为第一个可玩原型的前置条件。当前按桌面原型规划；Web-first 尚不是硬需求。

## 3. 三种引擎比较

下表中的“更适合”“成本较高”等是本项目的工程判断，需由 spike 验证；具体接口与导出能力见后文来源。

| 维度 | Unreal Engine 5.8 | Unity 2022.3 / 团结 | Godot 4.x |
| --- | --- | --- | --- |
| AI/agent 兼容 | 官方 MCP 可让 agent 调用编辑器功能；实际客户端与工具覆盖待测 | C# 工作流熟悉；社区 MCP 覆盖场景、脚本、测试等操作 | 文本场景与脚本便于 agent 编辑；社区桥接可补充引擎反馈 |
| MCP 生态 | 官方入口有吸引力，但 Experimental 的接口变化与缺口需评估 | 社区方案相对成熟、可作为近期备选；成熟不等于在本机稳定 | 有多种社区方案，需逐个核查版本、功能与维护情况 |
| 官方 MCP | 已有 Epic 官方 Unreal MCP，Experimental | 本次证据主要来自社区方案，未据此认定有同等官方通用接口 | 本次证据主要来自社区方案，未据此认定有同等官方通用接口 |
| Runtime MCP 潜力 | 有官方 runtime 模块基础；打包后工具仍需显式注册和验证 | 编辑器 MCP 不能直接等同于独立 Player 接口；通常需另做桥接 | 可以扩展 runtime 接口；编辑器调试桥接也不能等同于发布版支持 |
| 中国区版本限制 | 本次未确认同类版本分叉限制；本机下载、依赖与授权仍须验证 | 用户已报告无法使用国际最新版本；2022.3 与团结须分别锁版本，长期兼容风险较高 | 本次未发现同类区域分叉证据；下载和插件依赖仍须实测 |
| 快速原型 | 模板可加速，但工程、编译和工具接入可能消耗更多时间 | 结合现有熟悉度，预计最容易快速形成可玩原型 | 引擎较轻，但新增学习与迁移成本可能抵消优势 |
| 3D greyboxing / level design | 适合作为本项目优先验证的 3D 编辑与关卡工作流 | 足以完成小型 3D 伪沙盒，现有经验有利于快速推进 | 能完成小型 3D 原型；是否更省时间暂无本项目证据 |
| Web 部署 | 若采用 Pixel Streaming，游戏运行在服务器，需承担服务与网络成本 | 有 WebGL 路线；须测试所选版本、目标浏览器与资源限制 | 有 Web 导出；渲染方式和语言限制需提前核查 |
| 学习 / 迁移成本 | 有一定 UE 经验，但恢复工作流、自定义工具及 C++ 扩展仍有成本 | 现有经验最多；团结适配与未来版本迁移另计 | 需重新熟悉 GDScript、节点体系与插件流程 |

## 4. 关键证据与风险

### UE：官方基础明确，实际闭环尚未验证

Epic 文档列出了 actor、灯光、材质和自动化测试等工具能力，也提醒功能不完整、API 可能变化。`ModelContextProtocol` 与 `ModelContextProtocolEngine` 为 runtime 模块，但 cooked/shipping build 不会自动发现编辑器注册表中的工具，需要显式注册。默认接口面向本机且无认证，因此 runtime 验证先限制在本机。**“有 runtime 模块”不等于“已有可直接发布的 runtime 编辑系统”。** [Epic 官方 Unreal MCP 文档](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)

### Unity / 团结：近期效率高，长期版本兼容需管理

CoplayDev 的 MCP for Unity 提供场景、脚本、资产和测试等操作；项目声明支持 Unity 2021.3 LTS 至 6.x，范围包含 2022.3。因此将其视为相对成熟的社区备选有依据，但 README 的支持范围不能替代本机测试，也没有自动保证团结兼容。[CoplayDev 项目与版本要求](https://github.com/CoplayDev/unity-mcp)

用户无法使用国际最新 Unity 是本项目的实际约束。本次核查中，中国官网发布入口跳转到团结引擎；官方 FAQ 也说明团结的区域适用范围。但这些证据不足以推导所有中国用户都无法使用国际版，具体下载、账号与授权情况应在本机确认。[中国官网发布入口](https://unity.cn/releases)、[团结引擎官方 FAQ](https://docs.unity.cn/cn/tuanjiemanual/Manual/AssetstoreFAQ.html)

长期风险是版本分叉后，国际版教程、Package、Editor API 与 MCP 更新可能不再适配选定环境。这是工程风险推断，不是认定插件必然失效。若采用 fallback，必须记录完整编辑器版本、MCP tag/commit 和依赖版本，并先验证编译、场景修改及测试。

### Godot：暂不选，不代表 AI 能力不足

已有社区 MCP 提供场景与脚本编辑、运行和编辑器反馈等能力，因此不能仅以“没有官方 MCP”排除 Godot。[Godot 社区 MCP 示例](https://github.com/blentz/godot-mcp)

当前暂不选择它，主要因为本项目优先验证 UE 官方接口，并保留已有经验较多的 Unity 作为备选。再引入第三套工作流，会增加学习和验证范围。

## 5. Web-first 会改变排序

如果“用户通过链接直接在浏览器本地运行、无需安装客户端，也不依赖 GPU 云服务器”成为硬需求，**Unity / Godot 的权重上升，应重做决策。**

- Unity 有 WebGL 发布路线，但应按实际选择的 2022.3 / 团结版本测试，不能套用国际最新版的能力承诺。[Unity 官方 WebGL 发布教程](https://learn.unity.com/learn/tutorial/create-and-publish-webgl-builds)
- Godot 4 官方 Web 导出使用 Compatibility 渲染器；当前 stable 文档仍说明 C# 项目不能导出 Web。若选 Godot，应先验证 GDScript 路线与目标浏览器。[Godot 官方 Web 导出文档](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_web.html)
- UE 的 Pixel Streaming 让服务器运行游戏并向浏览器传输画面；它需要另行评估 GPU、带宽、延迟与并发成本，不能按浏览器本地运行来估算。[Epic 官方 Pixel Streaming 文档](https://dev.epicgames.com/documentation/en-us/unreal-engine/pixel-streaming-in-unreal-engine)

Web 能运行游戏，也不等于浏览器可直接承载原有 MCP 工作流；浏览器与 agent/backend 的连接需要单独设计。

## 6. 最小 engine spike checklist

建议限时 **2–3 个工作日**。这是一项待执行验证，以下勾选框目前均未完成。只做一个本地小场景：地面、玩家、障碍物、目标点和一条可编辑参数。

- [ ] **固定环境**：安装并记录 UE 5.8 完整版本、操作系统、硬件、agent/client 与依赖版本；确认本机可启动、保存、重开及打包。
- [ ] **接通 MCP**：按官方文档启用 Unreal MCP 与所需 toolsets；实际客户端完成工具发现，并读取当前场景信息。
- [ ] **验证编辑闭环**：让 agent 创建或修改障碍物、位置及颜色；检查结果正确，保存后重开仍一致。不能只看工具返回“成功”。
- [ ] **验证可玩闭环**：玩家可移动、有碰撞、可到达目标；修改参数后再次 Play/Test，并记录运行反馈。
- [ ] **验证一个自定义工具**：限定范围修改一个玩法参数或摆放一个对象；检查非法参数能返回清晰错误，场景可重置。
- [ ] **验证可重复性与恢复**：从基线连续执行三轮“读取 → 修改 → 测试 → 重置”；模拟一次断连，重连后重新读取状态，避免重复生成对象。
- [ ] **验证 runtime 最小接口**：在独立打包程序中测试一个只读状态工具和一个受控参数修改工具；记录构建类型及显式注册方式。不能用编辑器 Play 模式代替。
- [ ] **留下证据**：可玩 build、短录屏、调用与错误记录、版本清单、实际耗时，以及仍需人工处理的步骤。

## 7. Go / No-go 标准

以下是本项目建议采用的验收门槛，不是引擎厂商保证。

| 决策 | 标准 | 后续动作 |
| --- | --- | --- |
| **Go：正式采用 UE** | 限时内完成本机安装与打包；实际 agent 可读取并修改场景；三轮核心闭环均通过，无需手工修复数据；自定义工具可验证结果、拒绝错误参数并重置；迭代耗时可接受 | 锁定版本，进入第一个伪沙盒 prototype |
| **有条件 Go** | 编辑器闭环通过，但独立 build 的 runtime MCP 尚未通过 | 仅在首阶段明确限定为编辑器辅助开发时采用；记录 runtime 缺口，暂不承诺独立游戏内 AI 编辑 |
| **No-go：切换 fallback** | 限时内不能稳定连接或重复完成闭环；关键操作缺失且需大量引擎改造；频繁崩溃或数据损坏；构建与学习成本明显妨碍每周交付 | 在 Unity 2022.3 或所选团结版本上复用同一最小场景和验收标准，先验证社区 MCP |
| **重做部署决策** | Web-first 成为硬需求，且不能接受 Pixel Streaming 的服务器成本或延迟 | 提高 Unity / Godot 权重，先做浏览器导出 spike 再选引擎 |

若 runtime AI 修改是首版的硬需求，runtime 验证失败就不能“有条件 Go”。若 fallback 也未通过，不应继续扩大基础设施范围；应缩小 MCP 操作面，先保证可玩原型与可验证的编辑边界。

## 8. 下一步

先执行 UE 5.8 engine spike，并用上面的证据决定 Go / No-go。通过后再制作第一版可玩伪沙盒；失败则切换 Unity 2022.3 / 团结，不继续无限投入连接与同步问题。

**当前可以确认的是候选优先级；尚不能确认的是本机稳定性、实际开发速度及发布版 runtime MCP 可用性。**

## 9. Windows PC 环境核查（2026-10-09）

- 当前设备：Windows PC；用户本轮确认正在使用 PC。
- 本机实际安装：UE **5.7.1**，Changelist **48512491**，分支 `++UE5+Release-5.7`。由本机 `Engine/Build/Build.version` 与 Epic Launcher 安装清单交叉核验。
- 安装目录：`C:\Program Files\Epic Games\UE_5.7`。本次在 Launcher 安装清单中未发现 UE 5.8；这不等于排除其他磁盘或源码安装。
- 官方版本核查：Epic 官网已宣布 UE 5.8 可用；官方公告发布了 **5.8.3** 热修复（2026-09-22）。截至本轮核查，最新查到的正式热修复为 5.8.3；不将它视为永久固定版本。
- 当前候选仍为 UE 5.8 系列；具体 spike 版本建议在安装时核对 Launcher，优先验证当时的 5.8 正式补丁版本。
- 本机现有 5.7.1 不等于候选 5.8 官方 MCP 环境。本次按插件文件名检查未找到 ModelContextProtocol/MCP 插件；未启动编辑器检查，不能把此检查当作完整功能验证。
- 本轮仅进行了文件与官方来源核查；没有安装或升级引擎，没有运行编辑器、创建 prototype、打包、连接 MCP 或测试 runtime。
- 后续建议：保留 5.7.1，另行安装 5.8 正式版本用于隔离的最小 spike；安装、下载权限、依赖、启动和打包仍待验证。
- 首个 spike 用一个玩法模板控制验证范围；最终工具仍须保留多种玩法、描述、直接编辑、试玩与修改的创作循环。

来源：
- [Epic 官网](https://www2.unrealengine.com/)
- [Epic 5.8.3 Hotfix 公告](https://forums.unrealengine.com/t/5-8-3-hotfix-released/2833315)
- [Epic Unreal MCP 文档](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)

核查范围说明：本轮重新核查了 UE 版本与官方 MCP 文档；原总结中的 Unity、团结、Godot 与社区插件比较保留为此前资料，本轮未逐项复核其最新状态。

## 10. 按产品与调试需求重新评估（2026-10-09）

### 建议与决策状态

助手建议：**Unity 优先实现首个闭环，UE 保留为候选**。用户尚未确认引擎，交互界面仍未确定。较早的 UE 候选优先级主要基于官方 MCP；新需求把模板规则、schema 检查、手动编辑、试玩反馈和版本管理置于更重要的位置，因而重新分配权重。

### 对本项目的判断

- Schema 预设阈值与空间检查两者都能实现，需要自行开发对应检查程序；这一需求不构成 UE 或 Unity 独有优势。
- 结合项目资料记载的 Unity 熟悉度，预计以 C# 组织模板规则、对象 ID、操作记录、保存/撤销和反馈会更利于首阶段推进。这是工程预估，未测量开发耗时。
- Unity 2022.3 官方支持运行时 UI 和自定义编辑器窗口；UE 也有运行时 UI 与 Editor Utility Widgets。交互位置未决定不构成排除某一引擎的理由，编辑器专用接口不能直接当作打包程序能力。
- UE 官方 MCP 是值得验证的优势，但仍 Experimental；runtime 的工具注册与接口仍需单独完成。开发 agent 能控制编辑器，不等于用户产品的创作/调试流程已经成立。
- 如果交付重点之后转向 UE 编辑器内的创作插件、主要资产/玩法已在 UE、或高度依赖 UE 特定功能，可以把 UE 提升为优先候选。当前未取得这些前提的确认。
- 外部独立工具形态在两者中都需要定义内容读写、执行测试、反馈及版本一致性接口；不能单凭编辑器 MCP 的存在默认完成。

### PC 静态安装核查

- `C:\Program Files\Unity\Hub\Editor\2022.3.62f1\Editor\Unity.exe` 的 ProductVersion 为 **2022.3.62f1c1_b0109b07edb8**。
- 另有 `6000.2.13f1` 编辑器文件；本轮不以文件存在推翻用户此前报告的版本/区域使用限制。
- 当前 UE 文件核查版本仍为 5.7.1。
- 本轮没有启动或激活 Unity，没有编译、打包、MCP 或玩法测试。`c1` 环境、国际版文档和社区插件的兼容性仍需实际检查；不能默认与团结兼容。

### 正式选型前的最小证据

先在能够实际启动并获得授权的 Unity 完整版本上，验证：读取同一小关卡 → 预设间距规则提示 → 手动/受控操作修改 → 真实试玩并返回定位反馈 → 撤销 → 保存重开。记录 Windows PC、完整版本、耗时和失败步骤。UI 承载仍可先采用临时测试面板，不将它当作用户最终界面的决定。

若基础环境、关键依赖或核心闭环无法成立，回到 UE 执行相同任务。没有必要同时建设两套完整平台。

本轮核对的官方依据：
- [Unity 2022.3 UI 系统比较](https://docs.unity3d.com/2022.3/Documentation/Manual/UI-system-compare.html)
- [Unity 2022.3 自定义编辑器窗口](https://docs.unity3d.com/2022.3/Documentation/Manual/UIE-HowTo-CreateEditorWindow.html)
- [UE 编辑器 Blueprint 工具及 Editor Utility Widgets](https://dev.epicgames.com/documentation/en-us/unreal-engine/scripting-the-unreal-editor-using-blueprints)
- [UE Unreal MCP：Experimental 与 runtime 限制](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)
