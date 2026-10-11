# 本机 FYP 相关技能安装记录

日期：2026-10-11（Asia/Hong_Kong）；设备：Windows PC。
用户授权：寻找并安装适合当前 Unity FYP 的技能。未修改 Unity 工程、场景或 MCP 配置。

## 已安装与验证

| 技能声明名称 | 安装目录 | 来源与固定版本 | 本项目用途 |
| --- | --- | --- | --- |
| unity-mcp-orchestrator | C:/Users/Administrator/.codex/skills/unity-mcp-skill | CoplayDev/unity-mcp，v10.3.0，unity-mcp-skill | MCP 资源读取、对象/脚本操作、编译与 Console 检查、测试及工具路由 |
| level-design | C:/Users/Administrator/.codex/skills/level-design | gamedev-skills/awesome-gamedev-agent-skills，commit 0a70cfc64672d512c5b5b6006b5dcb5cf90dbdbd，skills/disciplines/level-design | 角色尺度、灰盒、路线、节奏、引导和试玩迭代 |
| save-systems | C:/Users/Administrator/.codex/skills/save-systems | 同一 gamedev 固定 commit，skills/disciplines/save-systems | 关卡数据保存/加载、格式版本、迁移、备份及损坏恢复 |

三项共 9 个 Markdown 文件，安装文件与事先下载审阅的固定源版本逐文件 SHA-256 一致。技能声明、引用文件存在性已检查；系统 quick_validate 在 UTF-8 模式下三项均返回 Skill is valid。

之前 unity-mcp-skill 目录只有 .unity-mcp-skill-sync，没有 SKILL.md；已保留至 C:/Users/Administrator/.codex/skill-backups/unity-mcp-skill-incomplete-20261011-180528，再用固定 tag 补齐安装。

技能可在后续 turn 被发现/使用；本轮验证的是安装和格式，不是它们带来的开发效果或 Unity 玩法功能。

## 使用边界

- Unity MCP 技能与当前插件版本 v10.3.0 对齐；示例工具参数仍要根据实际 tools/list/schema、Unity 2022.3 API、项目资源和 Console 检查。不能用技能示例代替运行验证。
- level-design 与 save-systems 是引擎中立的设计指南，包含 GDScript/Python 示意；Unity 实现要适配 C# 与所选版本，不直接照搬示例语言。
- 关卡尺度和难度比例属于设计示意，必须按实际控制器、物理条件和试玩校准；不能据有限测量或示意 SAFE_GAP/HARD_GAP 就宣称整体可通关/好玩。
- 关卡设计数据和试玩临时状态继续分开；不能因通用存档指南而把试玩中的角色临时状态覆盖进作者关卡。
- 最终交互界面位置、多玩法范围和引擎决定遵循用户项目指令，不由安装的通用 skill 决定。
- 本轮未安装其他 MCP/引擎插件、云服务或 Unity CLI。Unity-Technologies 的部分技能及社区 Unity C# 技能要求较新的工作流/Unity 6，暂未安装；现有 MCP 足以开展下一步操作验证。

## Sources

- [Coplay 固定版技能](https://github.com/CoplayDev/unity-mcp/tree/v10.3.0/unity-mcp-skill)
- [关卡设计技能](https://github.com/gamedev-skills/awesome-gamedev-agent-skills/tree/0a70cfc64672d512c5b5b6006b5dcb5cf90dbdbd/skills/disciplines/level-design)
- [保存系统技能](https://github.com/gamedev-skills/awesome-gamedev-agent-skills/tree/0a70cfc64672d512c5b5b6006b5dcb5cf90dbdbd/skills/disciplines/save-systems)
