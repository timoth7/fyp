# Windows PC 飞书官方 MCP

本机准备版本：`@larksuiteoapi/lark-mcp@0.5.1`，Node v24.15.0（2026-10-09）。
2026-10-09 已验证应用身份的 MCP 工具发现、指定 wiki 解析、文档读取、写入及读回。Codex 的 `feishu` 条目已启用；当前会话的工具清单需要重启/重载后刷新。

若需更换凭据，双击 `setup.cmd`，使用已有飞书开发者应用的 App ID 和 App Secret（密钥输入隐藏）。脚本只读核查应用身份及指定 wiki 的访问权限。不需要新建应用或文档。当前应用已经有访问指定 Sketchbook 的权限，无需用户 OAuth。

用户先前截图中的 20029 是 OAuth redirect URL 错误，与设备数量无关。以后若确实需要用户身份，官方 CLI 的默认回调是 `http://localhost:3000/callback`，需与开发者后台一致。当前应用身份路线不经过该回调。

App Secret 使用当前 Windows 用户的 DPAPI 加密，保存在 `%LOCALAPPDATA%\Codex\feishu-mcp\credentials.xml`，不写入项目资料。启动器仅将凭据传给本地官方进程的环境变量，不放在进程参数中。应用 token 由官方 MCP 管理。

只读核查成功后，脚本启用已准备的 `mcp_servers.feishu` 条目并备份原配置。当前 Sketchbook 已完成此次更新，revision 281 → 284；24 个新增块、1 个状态段落更新逐项读回一致，原有块与根节点顺序均保留。

官方 MCP 的 wiki 解析工具为 `wiki.v2.space.getNode`（通常显示为 `wiki_v2_space_getNode`）；此前 `get_feishu_document_info` 来自不同实现，不能照搬名称。始终使用指定 wiki token 解析实际 docx ID，不按标题选择其他文档。

当前启用范围仅为知识库节点解析、文档读取、块读取、创建子块及更新块；没有配置消息发送工具。

官方来源：[Lark MCP](https://github.com/larksuite/lark-openapi-mcp)、[配置说明](https://github.com/larksuite/lark-openapi-mcp/blob/main/docs/usage/configuration/configuration-zh.md)、[Codex MCP](https://learn.chatgpt.com/docs/extend/mcp?surface=cli)。
