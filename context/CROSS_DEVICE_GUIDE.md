# FYP 两台设备怎么接着用
云端入口：https://chatgpt.com/g/g-p-6ac5661e55dc819189de62d9e99a5d26/project

当前最新云端交接：HANDOFF-2026-10-07-v2.md（版本 2026-10-07.2）。下载到本地时可保存为 context/HANDOFF.md。旧云端 HANDOFF.md 是历史快照；项目指令已指向新版。

## 日常讨论：任何设备
登录同一个 ChatGPT 账号，打开“FYP · AI 关卡设计”，在这个项目内开启或继续聊天。
开场可说：“先读 PROJECT_CONTEXT.md 和 HANDOFF.md，告诉我现在从哪里接，再做本次任务。”
项目采用 Project-only memory；项目资料留在项目范围。页面提示这种项目不支持 Work in the cloud；这里用 Chat 做讨论与资料接续，本地代码任务交给 Codex。

## MacBook：代码 / 研究 / 文档
打开 fyp 本地文件夹，Codex 按 AGENTS.md 读取 context 中的当前状态。
完成任务后记录改变和测试结果，生成最新的 PROJECT_CONTEXT.md、HANDOFF.md。
本地资料改动不会自动传到 ChatGPT。把更新后的文件替换到项目 Sources，并核对版本号。

## Windows PC：Unity / 调试 / 试玩
初次从项目 Sources 下载最新 PROJECT_CONTEXT.md、HANDOFF-2026-10-07-v2.md、PROJECT_INSTRUCTIONS.md；把新版交接另存为 HANDOFF.md。
放在 Windows 工程的 context/ 目录。工程根 AGENTS.md 加入：
“开始先读 context/PROJECT_CONTEXT.md 和 context/HANDOFF.md；完成实质工作后更新交接，区分用户决定、建议与测试事实。”
如果没有本地文件，也可把最新交接文字贴给 Windows Codex，但要明确这不提供工程文件。
记录 Unity 版本、测试关卡、操作、期望/实际结果。结束后更新交接文件，再上传到同一个云端项目。

## 如何避免两边进度打架
一次让一台设备编辑同一份状态，结束后更新版本号，另一台先获取最新版本。
同名云端文件只保留最新有效状态；旧版本可以保留在本地历史/版本管理中。
需要更新云端时可以对当前 AI 说：
“把本次工作更新到 PROJECT_CONTEXT.md 和 HANDOFF.md，并将云端 FYP 项目的旧状态文件替换为最新版本；若你不能访问项目，请交付可上传文件并说明。”
未来工程代码通过 Git 等方式单独同步；目前没有配置工程同步服务。

## 已保存与未实现的区别
CHAT_ARCHIVE.md 保存旧聊天文字，是可搜索的资料；不会变成 Windows 上原生可恢复的旧 Codex 聊天。
云端 Project 的 Sources、项目指令与原生 Codex 会话是不同层；上传资料不会把本地插件、凭证、工程或应用一起搬过去。
PROJECT_CONTEXT.md 是当前事实，HANDOFF.md 是最近一次工作的交接；不能只靠 AI 自动记忆来保持进度。
