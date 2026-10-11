# FYP 跨设备交接
最新状态（2026-10-11，Windows PC）：MCP 只读接通，3 个技能已安装；尚无编辑/试玩/打包验证。版本安全核查与此前网络/沙箱问题见 UNITY_VERSION_CHECK_2026-10-11.md，不能用早些时候的端口失败覆盖后续接通证据。
版本：2026-10-11.5
时间：2026-10-11（Asia/Hong_Kong）
设备：Windows PC（本次访问排查）；下方原有资料整理与云端核验由 MacBook 完成。

## 最新交接：2026-10-11 FYP 技能已安装
- 最新制作顺序建议：先用占位几何搭一个玩法的编辑→检查→试玩反馈→修改→保存重开流程，平台基础随此流程建立；玩法控制器/规则优先复用已有 prototype，视觉 Asset 随后按缺口选择。详见 PSEUDO_SANDBOX_FRAMEWORK.md 第 12 节；首个模板和原型路径未定，未开始游戏修改。
- 用户授权搜索并安装技能。已补齐 Coplay v10.3.0 对应 unity-mcp-orchestrator（unity-mcp-skill 目录），并安装引擎中立的 level-design、save-systems；来源及固定 commit 见 INSTALLED_SKILLS.md。
- 全部 9 个 Markdown 文件与审阅源版本 SHA-256 一致，三项 quick_validate 通过。旧空技能目录只含同步标记，已保留到 .codex/skill-backups；未覆盖已有有效技能。
- 可在后续 turn 使用。尚未通过这些技能修改场景、试玩或构建；MCP 已有只读接通证据见下方。没有新增其他 MCP、云服务或 Unity CLI。
- 提醒：通用设计示例含 GDScript/Python，Unity 实现需适配 C#；阈值/可达性、界面形态和多玩法范围遵循当前用户需求，不能照抄技能常数作为事实。

## 本日较早交接：2026-10-11 Unity MCP 已接通（只读）
- 用户当前截图：MCP for Unity v10.3.0；HTTPLocal http://127.0.0.1:8080，Session Active，Codex Configured。客户端使用完整地址 http://127.0.0.1:8080/mcp。
- Windows PC 实测 initialize/tools/list/resources/list 通过，Unity instances 与 project/info 成功读回。原生会话工具清单仍未加载 Unity namespace，本轮通过 MCP SDK 完成验证。
- 当前目标 My project (2)：C:/Users/Administrator/My project (2)，Unity 2022.3.62f1c1，StandaloneWindows64；工程位于 fypppc 工作目录外，后续写入需遵守授权/权限。
- 没有编辑场景、启动 Play/Test、构建或验证玩法。只读接通不等于所有工具和 c1 环境完全兼容，安全修复版本事项仍待处理。
- 接续：保留 Unity 与 MCP 服务运行，必要时重载 Codex 原生工具；开始操作前确认目标工程，并在测试场景验证第一条编辑/试玩闭环。

## 本日较早交接：2026-10-11 Unity 模板与 MCP 接续
- 最新：用户要求 GitHub MCP 选型。已核查 3 个候选、正式版包清单和 PC 依赖，推荐 CoplayDev/unity-mcp v10.3.0；详情见 UNITY_MCP_EVALUATION.md。仅为推荐，未安装/编译/连接测试；c1 精确兼容性待测。
- 推荐固定安装 URL：https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#v10.3.0 。Git/uv/Python 已可运行；当前工程路径未取得，registry 网络问题仍可能影响依赖下载。
- 建议 Universal 3D 空白 URP 模板；未确认实际选择、项目路径及完整版本。用户截图显示在线 Package Manager 搜索 ECONNRESET（packages.unity.cn），网络原因未确定。
- Codex 已有启用的 unityMCP HTTP 地址 http://127.0.0.1:8080/mcp；只读 TCP 检查不可连接，未做 MCP handshake 或场景编辑。当前工程插件状态未取得。
- Unity 工程侧需有 MCP 包并启动当前工程连接；Codex 全局配置在地址/传输一致时可复用。接入后确认实际目标工程再编辑。
- 此前 sandbox helper 错误阻止普通本机检查；本轮通过获授权的提升权限完成只读配置/端口核查，未修改全局配置。
- 接续：确认版本已修复安全问题、当前工程依赖可加载、MCP 包安装及服务启动，再重载客户端并做只读场景检查。尚未开始平台玩法实现。

## 本日较早记录：2026-10-09 伪沙盒需求框架
- 最新引擎评估：助手针对规则/schema、编辑、试玩反馈与版本管理，建议 Unity 优先、UE 保留候选；用户未正式确认。ENGINE_SELECTION.md 第 10 节记录理由与验证条件，原 UE 优先总结为本日较早参考。
- PC 静态检查见 Unity 2022.3.62f1c1 的实际可执行文件（目录 2022.3.62f1），另有 6000.2.13f1 文件；未启动、授权、编译/打包或插件测试。该安装证据不能推翻用户此前报告的地区/版本约束。
- 最新补充：用户要求 schema 预设间距等范围，生成前提前提醒超范围意图。PSEUDO_SANDBOX_FRAMEWORK.md 已加入阈值分级、请求/候选检查、定位反馈及规则适用条件；具体阈值未测定，未实现或同步飞书。
- 用户要求先完善平台与关卡调试需求，再评估引擎。明确交互界面未决定，不能默认游戏内 built-in 或创作/试玩共用同一界面。
- 新增 context/PSEUDO_SANDBOX_FRAMEWORK.md：能力、创作与调试流程、三种界面形态、功能关系、制作阶段与验收任务；均是提案，未实现。
- 产品保留多玩法与描述/直接编辑/试玩/修改；首个模板用于验证，不能当作最终单一玩法方向。目标创作者、模板、编辑范围、作品用途和 runtime AI 首版门槛未定。
- 接续：先收敛一条用户任务和界面候选，随后用同一任务评估引擎；原 UE 候选记录保留，但不覆盖用户最新的产品需求优先顺序。
- 本轮设备 Windows PC；未新增游戏代码，未启动/测试 UE 或 Unity。本轮新框架未同步飞书及云端，此前飞书引擎 checkpoint 的同步证据见下方。

## 本日较早交接：2026-10-09 PC 飞书接通及同步完成
- 官方 Lark MCP 0.5.1 / Node v24.15.0，Windows keytar 已补齐。Codex feishu 已启用，应用身份 tenant_access_token。
- 用户提供原应用凭据，已在本机私有用户目录按 DPAPI 加密保存；不得同步凭据或 token 缓存。应用已有指定文档权限，不依赖用户 OAuth；截图 20029 为回调地址错误。
- 实测官方 MCP initialize/tools/list、唯一 wiki 解析、文档读取、块写入和读回通过，5 个文档工具可发现。
- 飞书已更新：DIRECTION LOG 新增 Shift 5；PRODUCT HANDBOOK 引擎候选 checkpoint / v1.1。revision 281 → 284；新增 24 块、更新 1 段；文字及归属读回一致，原有块与根子块顺序保留。
- 助手启用配置时漏写换行曾导致 Codex 启动 TOML 错误；用户已修复，codex mcp get feishu 成功显示 enabled: true。辅助脚本换行处理也已修复。
- 本次同步任务已完成，不要重复运行 sketchbook.cjs --apply。FEISHU_UPDATE_PENDING.md 已标记同步完成；云端项目上传快照未替换。
- 接下来做 UE engine spike；尚无引擎运行/打包/玩法验证。如果当前聊天工具列表未刷新，重启 Codex 后接续。

## 本日较早交接：2026-10-09 PC 飞书官方 MCP 准备
- 用户授权配置 PC 飞书 MCP，要求沿用 Mac 已有方式；用户记忆为官方 Lark。Mac 配置不可从 PC 直接读取，未确认原应用/配置详情。
- 完成：官方 @larksuiteoapi/lark-mcp 0.5.1 安装；PC Node v24.15.0；Codex feishu 注册，disabled；原配置已备份；启动与登录辅助脚本位于 tools/feishu-mcp。
- 验证：CLI 返回 0.5.1；辅助脚本语法通过；codex mcp get feishu 显示 disabled。尚未提供凭据或完成 OAuth，未完成 MCP handshake/tools/list 或文档读写。
- 本机交互凭据窗口启动后未保持运行；当前未生成凭据文件。用户可双击 tools/feishu-mcp/setup.cmd 输入 App ID、App Secret（隐藏输入）并完成浏览器授权。成功登录后辅助脚本启用条目；重启 Codex 后继续本聊天。
- 凭据不在项目中；App Secret 使用当前 Windows 用户 DPAPI 加密，OAuth token 由官方工具管理。不要同步凭据缓存。
- 官方 wiki 解析工具为 wiki.v2.space.getNode；AGENTS.md 已适配官方与社区实现的解析工具名。唯一 wiki URL 与历史 docx ID 仍不变，操作前必须重新解析。
- 飞书正文更新仍待执行：读取指定 Sketchbook 后加入 context/FEISHU_UPDATE_PENDING.md 的内容并读回确认。云端项目快照未更新。
## 本日较早交接：2026-10-09 引擎候选与 PC 环境
- 当前设备：Windows PC，用户本轮明确；保留此前 MacBook 负责代码/研究/文档、PC 负责引擎调试/试玩的协作安排。现阶段也在 PC 更新文档。
- 用户授权：更新本地项目资料及唯一飞书 Sketchbook。
- 新候选：UE 5.8 系列优先验证；Unity 2022.3 / 团结作为分别验证的 fallback；Godot 暂不选。尚未正式选定引擎。
- 已核验本机 UE 5.7.1，Changelist 48512491；Build.version 与 Launcher 安装记录一致。官方最新查到 5.8.3 热修复公告，不等于已在本机安装。
- 文件：新增 ENGINE_SELECTION.md；更新 PROJECT_CONTEXT.md 与本交接。原 .claude 材料保留。
- 测试：仅静态安装核查和官方资料核查；未启动 UE、未接 MCP、未打包或试玩，没有可运行 prototype 证据。
- 下一步建议：保留 5.7.1，准备独立 5.8 正式版本并记录完整版本；按 ENGINE_SELECTION.md 做限时 spike，三轮编辑/测试/重置及独立 runtime 验证分别记录。
- 飞书：本轮尚未写入；当前未发现可调用的 get_feishu_document_info 和文档块工具。待连接恢复后解析唯一 wiki URL，读取现有 DIRECTION LOG / PRODUCT HANDBOOK，再写入并读回；不另建文档。
- 云端：本轮没有替换上传快照。跨设备请同步 ENGINE_SELECTION.md、PROJECT_CONTEXT.md、HANDOFF.md。
## 历史交接：2026-10-07 跨设备访问排查
- 应用列表核验：fypppc 为 PC 本地目录 C:\Users\Administrator\Desktop\fypppc；fyppc 为另一 ChatGPT 云端项目。二者不是一个应用项目对象。
- 已查官方文档：可通过 Settings > Connections 配置 Mac/手机远程访问 PC 的项目及聊天；PC 需保持在线且应用运行。
- 建议先用远程连接解决同一聊天接续；如需 Mac 离线于 PC 独立开发，再配置工程文件同步。建议未被用户确认，连接未实测。
- fyppc 不能选 Work 的现象由用户报告；原因未验证，不将它概括为所有 ChatGPT 项目都不支持 Work。
- 本轮仅更新本地 context 状态；未更改应用设置，未上传云端资料，未进行 Unity 测试。云端 HANDOFF-2026-10-07-v2.md 仍为较早快照。

## 刚完成
- 整理当前 FYP 定位、范围和待定项到 PROJECT_CONTEXT.md。
- 归档三个相关本地聊天的用户/助手文字；重复 turns 合并。
- 建立 ChatGPT 云端项目“FYP · AI 关卡设计”，选择 Project-only memory。
- 本轮云端资料清单：PROJECT_CONTEXT.md、HANDOFF.md、CHAT_ARCHIVE.md、PROJECT_INSTRUCTIONS.md、CROSS_DEVICE_GUIDE.md。接续时以项目 Sources 中最新文件版本为准。
- 用户在 2026-10-07 明确授权在此 ChatGPT 项目内核验五份 FYP 资料并更新交接文件。
- 五份基础资料已逐份打开预览读回；历史档案的 61 轮记录 ID、140 条消息及结尾均可读取。当前进度、协作指令和使用说明的全部标题与结尾检查通过。这是页面正文核验，不是下载原文件的字节校验。
- 项目指令已点击 Save 保存，重新打开设置后与本地 PROJECT_INSTRUCTIONS.md 的正文一致；Project-only memory 已确认。
- 本次新版交接在云端使用文件名 HANDOFF-2026-10-07-v2.md，旧 HANDOFF.md 留作历史快照。开始工作时优先读新版交接；本地对应文件仍为 context/HANDOFF.md。

## 代码与工程状态
- 尚未发现 Unity 工程或应用代码；没有代码改动和 Unity 测试结果。
- 本地目录尚不是 Git 仓库，没有设置工程的自动跨设备同步。
- 云端项目文件是上传快照，不会自动同步本地文件或原生 Codex 聊天。

## 下次从哪里接
先读 PROJECT_CONTEXT.md 和 ENGINE_SELECTION.md。先区分本机 5.7.1 与候选 5.8，再准备最小 engine spike；首个玩法模板、可编辑范围与内容导出方式仍待确定。
打开云端项目：https://chatgpt.com/g/g-p-6ac5661e55dc819189de62d9e99a5d26/project
Windows 上开始本地开发前，把最新接续资料放到工程中，并让 Codex 读取它。

## 本次没有做的验证
尚未在 Windows 登录/下载测试，尚未运行 Unity，未重新读取飞书手册。

## 后续每次交接填写
- 时间 / 设备：
- 做了什么（文件、关卡或文档）：
- 用户新决定：
- 测试环境与结果（Unity/引擎版本，实际观察）：
- 未解决的问题：
- 下一步：
- 需要同步的文件 / Git 分支与 commit（如已有仓库）：
- 云端资料是否替换为本次版本：



