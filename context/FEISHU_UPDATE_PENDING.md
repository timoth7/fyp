# Sketchbook 待同步补充：引擎候选与 PC 环境

日期：2026-10-09（Asia/Hong_Kong）  
状态：2026-10-09 已同步并读回核验。保留本文件名作为本次草案来源，避免旧交接引用失效；不再是待执行任务。
目标：[唯一 Sketchbook](https://tianxi57.feishu.cn/wiki/KVV2w0qRzinNuGkjCw8cvGNTnFb)

同步证据：通过官方 Lark MCP 应用身份重新解析指定 wiki URL，读取 docx `ADQudjIVWob0nbxupX4cJmMWnRd`。按原文档英文格式更新 DIRECTION LOG（Shift 5）和 PRODUCT HANDBOOK（engine checkpoint 与状态段落）。revision 281 → 284；新增 24 块、更新 1 个状态段落；新增文字与更新段落读回一致，原有块全部保留，根节点子块顺序不变。以下中文草案是事实来源，实际写入版本为英文。

## Engine selection update — candidate decision

当前 FYP 继续围绕 AI-assisted rapid greyboxing / level design：基于已有玩法 prototype，让创作者通过描述、直接编辑、试玩和修改快速迭代关卡。最终工具保留多种玩法；第一轮技术验证只用一个玩法模板控制范围。

当前引擎候选优先级为 UE 5.8 系列、Unity 2022.3 / 团结作为备选，Godot 暂不选。此为候选决策，尚未完成 engine spike，也未正式锁定引擎。UE 官方 MCP 的存在提供了验证基础，但不能据此认定其更稳定、开发更快或已支持完整发布版 AI 编辑。

2026-10-09 在 Windows PC 核查到已安装 UE 5.7.1（Changelist 48512491），与 Epic Launcher 安装记录一致。本机现有版本和计划验证的 5.8 环境不同。Epic 已发布 UE 5.8，本轮最新查到的正式热修复公告为 5.8.3（2026-09-22）；准备环境时应再次核对可用补丁版本。

本轮只进行了安装文件和官方资料核查，没有启动引擎、连接 MCP、创建可玩 prototype、运行 Play/Test、打包或验证 runtime。因此尚无本机稳定性、迭代效率或 runtime AI 修改能力的测试证据。

下一步建议保留现有 5.7.1，另行准备 5.8 正式版本，在 2–3 个工作日内执行最小 spike：读取场景 → 修改对象或参数 → Play/Test → 检查实际结果 → 重置，连续三轮；另测一个自定义工具的参数拒绝与恢复，以及独立打包程序的只读状态和受控修改接口。完成后用证据决定正式采用 UE 或验证 Unity / 团结备选。编辑器验证通过不能替代独立 runtime 验证；首版若必须支持 runtime AI 修改，该项失败就不能有条件通过。

当前设备为 Windows PC，本轮文档更新也在 PC 完成。跨设备文件与云端资料仍采用显式交接；本地更新不会自动替换云端快照。

来源：
- [Epic 官网：UE 5.8](https://www2.unrealengine.com/)
- [Epic 5.8.3 热修复公告](https://forums.unrealengine.com/t/5-8-3-hotfix-released/2833315)
- [Epic Unreal MCP 文档：Experimental、工具扩展及 runtime 限制](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)

TianXi
