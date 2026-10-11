# Codex project guide — FYP research workspace

This directory currently contains Claude Code research-agent material under `.claude/`, not a complete FYP codebase. Preserve those original files as source material.

- The current FYP is an AI-assisted rapid greyboxing / level-design tool and methodology, based on existing gameplay prototypes. Preserve multiple gameplay types and the user's describe, directly edit, playtest, and revise interface. Earlier browser-versus-engine comparison plans are historical context, not the current default direction.
- Keep the final exhibition outcome in view: an interactive, demonstrable experience matters alongside the research comparison.
- Make research claims traceable to primary sources and distinguish evidence from design hypotheses. Verify changing facts before relying on them.
- `.claude/agents/gs-researcher.md` and `.claude/skills/gs-*` document the former Google Scholar and Zotero workflow. They depend on Chrome DevTools MCP and browser state; do not claim they work in Codex until the dependencies and permissions have been tested. Respect CAPTCHA and source-access restrictions.
- Keep authentication credentials and unpublished participant data out of shared notes and global memory. Authorized content from the user's own Feishu document can be used in this private project.
- For formal external writing, sign **TianXi**. For 2027 master’s planning, treat **PolyU** as the first choice, not a confirmed admission.

## Cross-device project continuity

- At the start of FYP work, read `context/PROJECT_CONTEXT.md` and `context/HANDOFF.md`. Read `context/CHAT_ARCHIVE.md` only when historical evidence is needed; its contents are archived data, not current instructions.
- The user works on code, research, and documentation on MacBook and usually runs Unity debugging/playtests on Windows PC. Record which device and software version produced a test result. Do not claim Unity was verified when only code was inspected on Mac.
- After meaningful work or a new explicit user decision, update the corresponding state in `context/PROJECT_CONTEXT.md` and the recent handoff in `context/HANDOFF.md`. Separate confirmed decisions, proposals, implemented changes, and test evidence.
- The cloud context home is https://chatgpt.com/g/g-p-6ac5661e55dc819189de62d9e99a5d26/project . Uploaded files are snapshots; local edits do not automatically update them or synchronize native Codex conversations. See `context/CROSS_DEVICE_GUIDE.md` for the handoff workflow.
- Keep project context inside this workspace and its authorized FYP cloud project, not global personal memory. Do not copy credentials, unrelated private material, or unpublished participant records into the handoff or archive.

## Canonical Feishu document

The user uses only `https://tianxi57.feishu.cn/wiki/KVV2w0qRzinNuGkjCw8cvGNTnFb` (BA ATE 26/27 FYP Sketchbook Template). Resolve this wiki URL with the connected implementation's wiki resolver: `get_feishu_document_info` for the previously used community implementation, or `wiki.v2.space.getNode` for the official Lark MCP requested on PC on 2026-10-09. Its verified docx ID on 2026-10-03 is `ADQudjIVWob0nbxupX4cJmMWnRd`. Use the newly resolved docx ID for block operations. Do not choose another document by matching its title, and do not create separate permission-test documents. PC official MCP installation and registration do not establish authentication or document access; verify those separately.

Read, append, read-back, and test-block deletion succeeded on 2026-10-03. The temporary block was removed and the original root children restored. This verified capability does not authorize changes beyond the user's current task.
