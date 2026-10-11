# Unity 版本与 MCP 接续核查

日期：2026-10-11（Asia/Hong_Kong）；设备：Windows PC。

用户截图：2022.3.62f1 显示 Security Alert；2022.3.62f1c1 无该标；6000.2.13f1 显示 Warning。没有 badge 不能作为已修复的证据。

官方来源核查：
- `c1` 版本是 Unity China build；它与基础版本号的安全修复状态应分别判断。[Unity 官方员工答复](https://discussions.unity.com/t/unity-was-upgraded-from-version-2022-3-62f1c1-to-2022-3-62f2c1-and-google-play-reported-that-there-were-security-vulnerabilities/1689930/5)
- 国际 2022.3 LTS 的 CVE-2025-59489 修复版本为 2022.3.62f2；漏洞涉及受影响版本构建的应用。[Unity 安全公告](https://unity.com/security/sept-2025-01)
- Unity 官方员工说明升级到 2022.3.62f2c1 应足以处理该问题；没有据此宣称当前安装的 f1c1 已修复。[官方答复](https://discussions.unity.com/t/unity-was-upgraded-from-version-2022-3-62f1c1-to-2022-3-62f2c1-and-google-play-reported-that-there-were-security-vulnerabilities/1689930/9)
- 6000.2.13f1 属于官方列出的 Package Manager 错误地判定包签名无效的受影响版本，官方要求改用修复版本；这与前述红色漏洞提醒是不同问题。[官方 Package Manager 通告](https://discussions.unity.com/t/package-manager-error-resolved-new-hub-badge-warning/1706569)

建议：新工程选择该发行分支已修复版本；不要因为 2022.3.62f1c1 没有 badge 就锁定它。尚未下载/升级编辑器、创建工程或验证插件。

MCP 问题：当前聊天没有可发现的 Unity 工具。PC 本机检查未成功：shell 和 node_repl 均报 Windows sandbox setup refresh errors；这不是 Unity 或 Unity MCP 已失败的证据，也未检查到实际服务器地址或连通性。官方 Codex 文档说明保存 MCP 配置后应重启/重载，可以回来继续原聊天；尚未完成本机连接测试。[官方 MCP 文档](https://learn.chatgpt.com/docs/extend/mcp)

## 同日后续检查：Package Manager 和项目复用

- 用户截图报 `packages.unity.cn` 的 `ECONNRESET`，是在线包服务连接重置，不是图中已出现代码编译错误的证据。已有依赖若加载正常可继续本地编辑，但在线搜索、安装、更新及缺失依赖下载可能受阻；本轮没有确定具体网络原因或修复设置。
- 经获授权的只读检查（提升权限以绕开先前的 sandbox helper 故障），PC Codex 已有启用的 `unityMCP` HTTP 条目，地址为 `http://127.0.0.1:8080/mcp`。
- 对本机 8080 的 TCP 连接检查失败；只证明检查时该配置端口不可连接，没有完成 MCP initialize、tools/list 或任何 Unity 编辑。
- 未取得当前 Unity 工程路径及其 manifest，不能宣称新工程已装或未装插件。按 Coplay MCP for Unity 工作流，Unity 侧包属于每个工程的依赖；Codex 侧全局条目可在地址/传输一致时复用，需启动当前工程连接并确认实际目标实例。[项目安装说明](https://github.com/CoplayDev/unity-mcp)
