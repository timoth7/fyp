# FYP 项目上下文
最新状态（2026-10-11）：Unity MCP 已在 Windows PC 完成只读连接及工程信息核查；3 个 FYP 相关技能已安装。尚无场景编辑、玩法或打包测试证据。当前正在收敛首个玩法与制作顺序，界面形态仍开放；先前安装/端口/沙箱问题见下方历史记录。
版本：2026-10-11.5
更新日期：2026-10-11（Asia/Hong_Kong）
最近的方向讨论：2026-10-09，完善伪沙盒平台及关卡调试需求，交互界面形态保持开放；跨设备安排由用户在 2026-10-07 明确。
本文件是当前状态入口。CHAT_ARCHIVE.md 是历史依据，不能用旧建议覆盖较新的用户决定。

## 项目是什么
TianXi 的 2026–27 FYP / CAT302TC，方向为 AI-assisted rapid greyboxing / level design。
目标是基于已有游戏 prototype，做一个能通过描述、直接编辑和试玩来快速创作、迭代关卡的工具，并总结其 methodology。
用户用“有一些规定的 Roblox”描述当前概念：提供不同玩法类型，各有明确规则和可编辑范围。
用户自己也要使用这个界面创作内容；界面是核心交付物。多种玩法和个人创作兴趣需要保留。
Rapid 指缩短想法 → 可试玩 → 修改的循环；速度、控制、修补成本和质量要通过实践证明。

## 已明确的方向
- 重心为 level design；不以批量生产小游戏作为目标。
- 基础 prototype 提供已有玩法，再开放关卡设计；不默认支持任意游戏直接导入。
- 描述、拖拽/直接编辑、Play、修改是创作循环的核心。
- 需要玩法多样性；不能擅自收缩成唯一的几何冲刺/跑酷模板。
- 展览是演示与体验场景，不应反过来主导毕业项目。
- 早期“浏览器 vs Unity/MCP 双路线 + 展览盲测”是历史方案，不是当前主方向。
- 2026-10-07：MacBook 用于代码、研究和文档等任务；Windows PC 主要用于 Unity 调试和试玩。两边需要共用项目上下文。
- 2026-10-09：先完善伪沙盒平台与“用户调试自己关卡”的需求框架，再评估具体引擎；界面是否游戏内 built-in、引擎编辑器工具、独立工具接入均未决定。
- 2026-10-09：用户希望将间距等预设范围放入 schema，生成前读取并提醒超范围意图；同时检查生成候选，避免把不符合模板范围的结果直接应用到场景。

## 最新流程草案
选玩法模板 → 先试玩示例 → 描述设计意图 → AI 生成草稿 → 局部对话/手动编辑 → Play → 根据体验修改 → 保存与使用。
AI 应读取当前关卡、选择区域和受保护的内容；不能只依赖聊天历史。
AI 修改需支持检查、应用及撤销。Schema、检查程序、引擎是不同职责；数据格式本身不保证可通关或好玩。
关卡调试建议扩展为：检查配置 → 实际试玩 → 记录相关问题/事件 → 定位对象或区域 → 局部修改 → 复测。创作界面与游戏执行部分不必在同一界面内，见 PSEUDO_SANDBOX_FRAMEWORK.md。
以上细节是讨论形成的实现建议，尚未验证为可运行功能。

## 已有材料与实际进度
- 相关历史聊天已归档：CHAT_ARCHIVE.md，三个来源合并后 61 个 turns / 140 条用户及助手文字消息。原始本地聊天保留。
- 历史记录显示飞书 Sketchbook 已写入 DIRECTION LOG 的 Shift 1–4，以及 PRODUCT HANDBOOK。
- 手册含界面示意、使用流程、模板/schema 草案、架构、评价方式、路线图及风险。写入发生在历史会话中；此次没有重新检查飞书正文。
- 2026-10-07 本地检查：原工作目录只有 AGENTS.md 及 .claude 下研究技能/agent 材料，未看到 Unity 工程或工具应用代码。
- 当前按“方向已定、范围与流程细化、等待第一条运行链路验证”理解。不能称为已部署、已被采用或已验证的产品。
- 未取得可运行 prototype、Unity 调试结果、生产效率测量或用户测试证据。

## 未决定 / 待验证
- 第一组模板：讨论过车道跑酷、侧视跳跃、俯视躲避、迷你高尔夫、3D 闯关等，未最终选定数量或组合。
- 固定与开放边界：模板规则、可编辑对象、参数范围、统一美术语言。
- 内容用途：仅在工具内保存/试玩、导出独立作品、还是回写引擎项目。
- 技术形态：本日较早的 UE 5.8 优先、Unity 2022.3 / 团结 fallback 候选记录保留于 ENGINE_SELECTION.md。用户最新要求先完善产品需求和界面形态，再评估 Unity / UE 等引擎，尚未锁定选型或必须先安装 UE 的顺序。游戏内 built-in、编辑器工具和外部接入工具均为候选；agent/model、连接方式和 MCP 位置待定。
- 核心研究问题与评价方法：schema、人和 AI 分工、局部修改、降低门槛等都是候选；不能擅自把其中一个写成已确认研究贡献。
- 展览日期、试玩者数据/伦理要求、项目名称等需进一步确认。
- 历史中的“20 秒”“20–30 关”、可通关校验等均为规划假设。

## 下一步建议（尚未获得具体范围决定）
先用 PSEUDO_SANDBOX_FRAMEWORK.md 收敛首条创作者任务、首个玩法模板、可编辑边界、调试反馈和内容用途；评估交互界面承载形态，再让候选引擎执行同一个小任务的 spike。建议先完成手动编辑/试玩/定位问题/修改/保存闭环，再接一个 AI 局部修改任务。一个模板的 spike 不收缩最终玩法多样性。独立 runtime 验证是否成为首版门槛，取决于最终形态与首版需求，当前未确认。
跨设备接续先读本文件和 HANDOFF.md；不要从旧聊天重新推导全部方向。

## 项目资料入口
云端项目：https://chatgpt.com/g/g-p-6ac5661e55dc819189de62d9e99a5d26/project
唯一飞书 Sketchbook：https://tianxi57.feishu.cn/wiki/KVV2w0qRzinNuGkjCw8cvGNTnFb
历史已验证 docx ID：ADQudjIVWob0nbxupX4cJmMWnRd。未来操作仍需先解析 wiki 链接。
Mac 本地目录：/Users/timoxi/fyp（历史记录，尚未在本轮访问）。Windows 本地目录：C:\Users\Administrator\Desktop\fypppc（2026-10-07 PC 当前工作目录核验）。
用户指定只维护这份 Sketchbook，不按标题寻找同名副本，也不新建权限测试文档。未经当前任务授权不改学术正文。

## 2026-10-07 跨设备访问排查
- PC 应用项目列表确认：fypppc 是 Windows 本地项目，fyppc 是 ChatGPT 云端项目（ID：g-p-6ac572b1ce4c8191aa9f577e6435025a）；二者在应用中是独立对象。另有 FYP · AI 关卡设计 云端项目，不能与 fyppc 混同。
- 用户报告：PC 本地项目在 Mac/手机不可见；fyppc 在 Mac 可见，但只能选 Chat，不能选 Work。本轮未直接检查 Mac 界面，不能确定 Work 不可用的具体原因。
- 官方 Remote connections 文档支持 Mac/手机连接 Windows 主机并继续其项目和聊天。建议优先配置远程连接；尚未配对或实测，用户尚未选定方案。
- 若要求 PC 关机后仍在 Mac 本地开发，需要另行同步工程文件并在 Mac 添加本地项目；文件同步不等于原生聊天同步。当前未配置 Git 同步。
- 官方来源：https://learn.chatgpt.com/docs/remote-connections 、https://learn.chatgpt.com/docs/projects?surface=app （2026-10-07 查阅）。

## 状态更新规则
完成实质工作或获得新决定后，更新本文件对应段落、版本号和 HANDOFF.md。
明确区分：用户决定、助手建议、实际完成、测试证据、待验证。
有冲突时采用较新的用户明确决定；同名云端资料如未替换只是旧快照。
不要写入密码、token、API key、私人参与者原始数据或无关个人信息。


## 2026-10-09 引擎候选与 PC 核查
- 用户授权更新本地资料及唯一飞书 Sketchbook；确认当前正在 Windows PC 使用本项目。
- 候选状态：UE 5.8 系列优先，Unity 2022.3 / 团结为 fallback，Godot 暂不选；尚未完成 engine spike，未正式锁定引擎。
- 本机已核验 UE 5.7.1（Changelist 48512491），安装于 C:\Program Files\Epic Games\UE_5.7；Build.version 与 Launcher 清单一致。
- 官方已发布 UE 5.8；本轮最新查到的正式热修复公告为 5.8.3（2026-09-22）。候选版本与本机安装版本不同，不能宣称本机已有 5.8 官方 MCP。
- 测试证据：只检查安装文件与官方来源，未运行编辑器、MCP、Play/Test、打包或 runtime 验证。
- 已保存 ENGINE_SELECTION.md，保留用户总结并补入本轮证据、来源和核查范围。
- 飞书同步：尚未完成，当前会话没有可调用的飞书文档解析/块操作工具；待连接能力恢复后解析唯一 wiki URL，再更新及读回。云端上传快照本轮未替换。

## 2026-10-09 PC 飞书官方 MCP 配置
- 用户要求在 PC 配置飞书 MCP；Mac 曾配置过，用户记忆为官方 Lark MCP，未取得 Mac 配置文件核验。
- 已安装官方 @larksuiteoapi/lark-mcp 0.5.1 至 .local/lark-mcp，PC Node v24.15.0；官方 CLI 版本检查通过。
- 已备份并更新 PC Codex config.toml，新增 feishu 条目；凭据与 OAuth 未完成前保持 disabled。Codex 已识别该条目，PowerShell 脚本语法检查通过。
- tools/feishu-mcp/setup.cmd 提供本机 App ID 与隐藏 App Secret 输入和 OAuth 登录；App Secret 以当前 Windows 用户 DPAPI 加密保存到用户本机目录，不写入项目。尝试启动交互窗口后进程未保持运行，核查未生成凭据文件；仍需用户双击 setup.cmd 输入凭据并授权。
- 官方实现使用 wiki.v2.space.getNode 解析指定 wiki URL，不提供社区实现的 get_feishu_document_info。本次未解析 docx ID、未发现远端工具或读写文档；原文档仍未同步。
- 下一步：用 Mac 上原应用的凭据完成 PC 授权，再启用/重载 MCP、解析唯一 wiki 链接、读取 Sketchbook、更新并读回 FEISHU_UPDATE_PENDING.md 中的已授权内容。

## 2026-10-09 最终状态：PC 飞书接通及文档同步完成
- 用户提供原应用凭据用于本机配置，已按 Windows 用户 DPAPI 加密保存到私有用户目录，不记录其值于项目或全局记忆。
- 应用凭据及唯一 wiki 解析成功；应用已有指定 Sketchbook 的访问权限，当前使用 tenant_access_token 应用身份，无需用户 OAuth。截图 20029 为 redirect URL 错误，不是设备数量限制的证据。
- 官方 Lark MCP 0.5.1 已实测 initialize、tools/list、wiki 解析、文档和全部块读取、创建/更新及读回；5 个文档工具可发现。Windows keytar 依赖已补齐。
- 飞书已完成更新：DIRECTION LOG 新增 Shift 5；PRODUCT HANDBOOK 加入 engine checkpoint 并更新 v1.1 状态。revision 281 → 284；24 个新增块和 1 个更新段落逐项读回一致，原有块及根子块顺序保留。正式英文内容签 TianXi。
- Codex feishu 已启用。助手启用时漏写换行曾导致 TOML 解析失败，用户修复后 codex mcp get feishu 已成功解析并显示 enabled: true；辅助脚本同类替换已修复以保留换行。
- 文档同步任务已完成；之前的“待凭据/待同步”段落是本日早些时候的状态。云端上传快照未替换；没有 UE/Unity 运行、打包或玩法测试证据。

## 2026-10-09 需求框架与交互形态澄清
- 用户要求完善伪沙盒平台能力，重点支持创作者调试自己的关卡；下一阶段再选引擎。
- 用户明确：交互界面没有确定，不等于必须融合到游戏界面。built-in、引擎工具或外部工具接入均待评估。
- 已整理 PSEUDO_SANDBOX_FRAMEWORK.md：产品定位、受控自由、多玩法模板、编辑/试玩/反馈/版本闭环、问题分类、三种界面候选、功能框架、阶段顺序及验收任务。
- 这些是需求/设计提案，不是实现或验证结果；第一模板、目标创作者、作品交付方式、runtime AI 必要性仍未定。
- 本轮只更新本地框架和交接，未新增游戏代码、未运行引擎；未把这份新框架写入飞书或替换云端快照。飞书当前已同步的是此前引擎 checkpoint，而非本次界面形态澄清。

## 2026-10-09 schema 预设规则补充
- 用户新增需求：间距等参数预设阈值；生成前读 schema，发现超范围请求时提前提醒。
- 框架补入请求预检查、候选数据应用前检查、共享确定性检查程序；区分建议阈值、模板允许范围及有条件的角色能力诊断。
- 数值尚未测定，3/4 米等仅为说明例子。阈值需与模板、控制器和适用条件对应；不能以 schema 范围或有限测试结果直接证明整体不可通关。
- 本轮只更新本地需求框架，无实现、引擎测试或新增飞书同步。

## 2026-10-09 最新引擎评估（助手建议，未获选型确认）
- 按用户细化后的需求，助手建议 Unity 优先实现首个闭环，UE 保留为候选；用户仍未锁定引擎/界面形态。原因是规则检查、操作/版本管理和反馈的权重上升，结合已有 Unity 熟悉度；不是实测 Unity 开发更快。
- ENGINE_SELECTION.md 第 10 节已记录新判断，原 UE 官方 MCP 优先的较早总结保留为历史候选。
- PC Unity 静态文件核查：2022.3.62f1 目录中 Unity.exe 的 ProductVersion 为 2022.3.62f1c1_b0109b07edb8；另有 6000.2.13f1 文件。不据此宣称可激活运行，未验证版本/地区或插件兼容。
- 下一步建议用实际可用的 Unity 完整版本做同一小任务：schema 提示、编辑、试玩反馈、撤销、保存重开；失败再评估 UE。未启动/测试引擎，未同步此次评估至飞书/云端。

## 2026-10-11 Unity 创建与 MCP 接续
- 用户询问模板，助手建议 Universal 3D（URP 空白模板），用户尚未明确确认采用。截图随后显示 Unity Console 的在线包搜索错误，不能推断实际工程模板或完整版本。
- 报错为 packages.unity.cn / ECONNRESET；网络原因待排查。没有代码编译、玩法或构建测试证据。
- PC Codex 已有启用的 unityMCP 地址 http://127.0.0.1:8080/mcp；本轮 TCP 只读检查不可连接，未完成 Unity MCP handshake 或编辑。当前会话没有可发现的 Unity 工具。
- 当前工程路径与插件安装状态未取得；通常需在每个 Unity 工程装入 MCP 包，客户端全局地址一致可复用，连接后还需确认目标工程。详见 UNITY_VERSION_CHECK_2026-10-11.md。

## 2026-10-11 Unity MCP GitHub 评估
- 用户报告 Window 没有 MCP，要求找兼容当前版本、功能较全且关注度高的方案。
- 助手推荐 CoplayDev/unity-mcp v10.3.0 正式 tag，未安装。包最低 Unity 2021.3、README 覆盖 2022.3；当前 c1 环境的编译/连接仍未测试。
- 对比 IvanMurzak（包声明 2022.3）和 CoderGamester（包声明 2022.3，但 README 要求 Unity 6，支持范围不一致）。实时 Stars、正式版本、来源及验证范围记录于 UNITY_MCP_EVALUATION.md。
- PC Git、uv、Python 版本核查通过；Package Manager 的 registry 网络问题仍可能阻止依赖下载，当前工程路径未取得。未修改工程/全局配置，未新增玩法或引擎测试证据。

## 2026-10-11 Unity MCP 只读连接验证通过
- 用户截图显示 MCP for Unity v10.3.0、HTTPLocal、Session Active 与 Codex Configured。
- 本机实测：Codex unityMCP 配置成功解析，启用的地址为 http://127.0.0.1:8080/mcp；实际 MCP initialize、tools/list、resources/list、Unity instances 与 project/info 读取均通过。
- 目标工程：My project (2)，实际路径 C:/Users/Administrator/My project (2)，Unity 2022.3.62f1c1，StandaloneWindows64。它位于当前 fypppc 文档工作目录之外，后续写入必须按授权与工作区权限处理。
- 当前原生会话工具清单尚无 Unity namespace，但本轮可通过 MCP SDK 实际连接和读取；重载客户端可让原生工具重新发现。服务与工程需保持运行。
- 仅证明此版本/插件当前能启动并完成上述读取；没有场景创建/修改、Play/Test、玩法、打包或运行效率验证，不据此声明 c1 完整兼容或安全问题已修复。原端口不可连接/未安装记录是本日较早状态。

## 2026-10-11 FYP 相关 skill 安装
- 用户要求搜索并安装有用技能；已安装 unity-mcp-orchestrator（目录 unity-mcp-skill，Coplay v10.3.0）、level-design、save-systems（后两项为 gamedev-skills 固定 commit）。详细来源、目录和版本见 INSTALLED_SKILLS.md。
- 旧 Unity skill 只有同步标记没有正文，已备份后补齐。三项格式校验通过，9 个文件与审阅源版本一致；没有运行技能执行游戏开发、修改 Unity 场景或更改 MCP 配置。
- 两项通用设计 skill 的示例须适配 Unity C#；尺度/阈值仍需按实际玩法条件验证，不能把示例常数当成项目已测得结果。

## 2026-10-11 开工与 Asset 准备建议
- 用户询问架构是否清楚以及先找 Asset 还是先搭平台。助手建议用占位几何搭一条可编辑/检查/试玩/反馈/保存的最小流程，同时建立平台基础，之后按缺口挑展示资源。
- 玩法控制器/相机/碰撞/胜负规则属于早期基础，优先复用用户已有 prototype；实际工程/位置尚未取得。第一模板及最终界面形态仍待决定，不因临时测试面板而锁定 built-in。
- 已补入 PSEUDO_SANDBOX_FRAMEWORK.md 第 12 节。这是制作顺序建议，未取得新的模板/资源选择决定，也未开始修改游戏。

