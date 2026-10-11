# Unity MCP 候选核查

日期：2026-10-11（Asia/Hong_Kong）；设备：Windows PC。
状态：检索与推荐完成，未安装、未编译或连接验证。用户报告当前工程 Window 菜单没有 MCP。

后续更新（同日）：用户已导入插件，截图为 MCP for Unity v10.3.0。通过本机 MCP SDK 实际完成 initialize、工具/资源发现、工程实例及 project/info 读取，目标 My project (2)，Unity 2022.3.62f1c1。只读连接已验证；编辑、Play/Test、构建和整体兼容性尚未验证。以上“未安装”状态保留为本日早些时候的检索背景。

## 推荐

优先验证 **CoplayDev/unity-mcp v10.3.0**，锁定正式 tag，不使用浮动 main/beta。当前截图/文件记录的环境是 Unity 2022.3.62f1c1；项目声明覆盖 2022.3，但没有获得这一个 China build 的实测结果。Unity 的安全修复版本选择仍须独立处理，见 UNITY_VERSION_CHECK_2026-10-11.md。

安装 URL：`https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#v10.3.0`

## GitHub 实时核查

公开 GitHub API 于本轮查询；Stars 只是关注度代理，不能当成真实用户数或稳定性证明。

| 项目 | Stars / Forks | 最新正式 Release | 兼容证据与判断 |
| --- | --- | --- | --- |
| CoplayDev/unity-mcp | 14837 / 1541 | v10.3.0，2026-10-04 | 正式 tag package.json unity=2021.3；README 声明 2021.3 LTS→6.x；范围覆盖 2022.3，优先验证 |
| IvanMurzak/Unity-MCP | 4418 / 392 | 0.93.3，2026-10-10 | 正式 tag 包 unity=2022.3；扩展工具、反射与 runtime 为项目主打能力，保留候选 |
| CoderGamester/mcp-unity | 1926 / 245 | 1.5.0，2026-09-03 | 正式包 unity=2022.3，但当前 README Requirements 写 Unity 6 or later，支持声明不一致；不优先推荐给当前 2022.3 环境 |

三者均未 archived。此为查阅时状态，不是长期固定数据。

## 功能与 FYP 适配

Coplay v10.3.0 README 列出 48 个工具入口。作者文档覆盖场景/GameObject、组件、Prefab/资产、脚本、Console、测试、profiling/build，并提供自定义工具扩展。不同工具组或附加能力可能需要启用/安装额外依赖；AI 资产生成需要外部服务及凭据，首阶段不需要。

对本 FYP，先验证读取场景、创建/修改对象、读取 Console、Play/Test、脚本/测试等核心能力；schema 范围检查可以作为项目自定义逻辑/工具后续接入，不将它写成插件已经自带的关卡可达性判断。

这是开发用编辑器桥接的优先推荐，不能据此锁定最终用户界面或认定独立 Player 中的编辑/AI 接口已完成。

## PC 依赖与当前限制

- 已只读确认 Git 2.54.0.windows.1、uv 0.11.15、Python 3.12.10 可运行，满足推荐项目文档中的基础版本要求。
- Codex 此前已有 unityMCP HTTP 配置，http://127.0.0.1:8080/mcp；检查时端口不可连接。新包导入后需启动服务器并核对其实际地址与目标工程。
- 用户前一截图显示 packages.unity.cn / ECONNRESET。Git URL 可从 GitHub 拉插件，但 Newtonsoft.Json、Test Framework 等依赖仍可能访问 Unity registry；不能保证避开该网络问题。
- 当前 Unity 工程路径未取得，没有改其 manifest、安装插件或全局 MCP 配置。

## 安装后的最小验证

Package Manager → + → Add package from git URL → 粘贴固定 tag URL。
编译通过后打开 Window → MCP for Unity，核对依赖并启动本机连接，再重载 Codex 的 MCP。
先读取当前工程/场景并确认目标；在单独测试场景验证一个对象的创建/修改、保存/重开、Console 与 Play/Test。只有这些通过，才记录“本机验证可用”。

## Primary sources

- [Coplay 项目](https://github.com/CoplayDev/unity-mcp)
- [Coplay v10.3.0](https://github.com/CoplayDev/unity-mcp/releases/tag/v10.3.0)
- [Coplay 固定版包清单](https://github.com/CoplayDev/unity-mcp/blob/v10.3.0/MCPForUnity/package.json)
- [Coplay 安装说明](https://coplaydev.github.io/unity-mcp/getting-started/install)
- [Coplay 工具目录](https://coplaydev.github.io/unity-mcp/reference/tools/)
- [Coplay 自定义工具](https://coplaydev.github.io/unity-mcp/guides/custom-tools/)
- [Ivan 项目](https://github.com/IvanMurzak/Unity-MCP)
- [Ivan 固定版包清单](https://github.com/IvanMurzak/Unity-MCP/blob/0.93.3/Unity-MCP-Plugin/Packages/com.ivanmurzak.unity.mcp/package.json)
- [CoderGamester 项目](https://github.com/CoderGamester/mcp-unity)
- [CoderGamester 固定版包清单](https://github.com/CoderGamester/mcp-unity/blob/1.5.0/package.json)
