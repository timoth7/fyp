using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FypGreybox.Editor
{
    public class GreyboxWorkbench : EditorWindow
    {
        const string ReportKey = "FypGreybox.LastPlayReport";
        GreyboxWorkspace workspace;
        LevelData draft;
        string observedJson, selected = "P02", status = "选择平台，修改候选，检查后应用。";
        string intentText = "在 P02 后面加一个 2 米的跳跃", intentSummary = "";
        bool pending, acceptedWarnings, forceErrors;
        Vector2 scroll;
        float requestedGap = 2;
        List<LevelIssue> requestIssues;
        GUIStyle heading, small;

        [MenuItem("FYP/Greybox Workbench")]
        public static void Open()
        {
            var window = GetWindow<GreyboxWorkbench>();
            window.titleContent = new GUIContent("Greybox Lab");
            window.minSize = new Vector2(360, 520);
            window.Show();
        }
        void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndo;
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.playModeStateChanged += OnPlayChanged;
        }
        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo;
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.playModeStateChanged -= OnPlayChanged;
        }
        void OnPlayChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                var live = FindObjectOfType<GreyboxWorkspace>();
                if (live != null) SessionState.SetString(ReportKey, JsonUtility.ToJson(new PlayReport { events = new List<PlayEvent>(live.events) }));
            }
            if (state == PlayModeStateChange.EnteredEditMode) { workspace = null; draft = null; }
            Repaint();
        }
        void OnUndo()
        {
            if (EditorApplication.isPlaying) return;
            Bind();
            if (workspace != null) { workspace.Rebuild(); ResetDraft(); EditorSceneManager.MarkSceneDirty(workspace.gameObject.scene); }
            Repaint();
        }
        void Bind()
        {
            if (workspace == null) workspace = FindObjectOfType<GreyboxWorkspace>();
            if (workspace != null && (draft == null || (!pending && observedJson != workspace.Json))) ResetDraft();
        }
        void ResetDraft()
        {
            draft = workspace.Data.Copy(); observedJson = workspace.Json;
            pending = false; acceptedWarnings = false; forceErrors = false; requestIssues = null;
            if (draft.Find(selected) == null) selected = draft.platforms[0].id;
        }
        void Changed() { pending = true; acceptedWarnings = false; forceErrors = false; requestIssues = null; SceneView.RepaintAll(); Repaint(); }
        void Commit(LevelData candidate, string name, bool explicitUnlock = false, bool forceErrors = false)
        {
            if (workspace == null) workspace = FindObjectOfType<GreyboxWorkspace>();
            if (workspace == null) { status = "应用失败：找不到关卡。"; return; }
            var issues = LevelValidator.Check(candidate, workspace.Rules);
            if (LevelValidator.HasErrors(issues) && !forceErrors) { status = "应用失败：请先修复红色问题，或勾选强制应用并自担风险。"; return; }
            if (!explicitUnlock && !forceErrors && !LevelValidator.PreservesLocked(workspace.Data, candidate)) { status = "应用失败：受保护的平台已被修改。"; return; }
            Undo.RegisterCompleteObjectUndo(workspace, name);
            if (!workspace.TryApply(candidate, out issues, explicitUnlock || forceErrors, forceErrors)) { status = "应用失败；原关卡保留。"; return; }
            EditorUtility.SetDirty(workspace);
            EditorSceneManager.MarkSceneDirty(workspace.gameObject.scene);
            ResetDraft(); SceneView.RepaintAll(); status = name + "；可用撤销恢复。";
        }
        void Section(string text) { GUILayout.Space(14); GUILayout.Label(text, EditorStyles.boldLabel); }
        void OnGUI()
        {
            Bind();
            heading = heading ?? new GUIStyle(EditorStyles.boldLabel) { fontSize = 23 };
            small = small ?? new GUIStyle(EditorStyles.wordWrappedLabel) { fontSize = 11 };
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUILayout.Space(12);
            GUILayout.Label("GREYBOX LAB", heading);
            GUILayout.Label("01  /  PLATFORM CROSSING", EditorStyles.miniLabel);
            GUILayout.Space(8);
            EditorGUILayout.HelpBox("流程验证原型 · 临时编辑器面板\n编辑 → 检查 → 应用 → 试玩 → 修改 → 保存", MessageType.None);
            if (workspace == null)
            {
                if (GUILayout.Button("打开示例关卡", GUILayout.Height(34))) GreyboxSetup.Open();
                EditorGUILayout.EndScrollView(); return;
            }
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("点击 Game 画面后：WASD / 方向键移动，空格跳跃，R 重试。停止试玩后查看下方反馈。", MessageType.Info);
                if (GUILayout.Button("停止试玩 / 返回编辑", GUILayout.Height(34))) EditorApplication.isPlaying = false;
                DrawEvents(workspace.events); EditorGUILayout.EndScrollView(); Repaint(); return;
            }
            var rules = workspace.Rules;
            Section("0  一句话生成草稿");
            GUILayout.Label("输入描述生成候选，橙色预览，应用后可撤销。", small);
            intentText = EditorGUILayout.TextField("描述", intentText);
            if (GUILayout.Button("生成候选", GUILayout.Height(30)))
            {
                if (LevelIntentParser.TryParse(intentText, draft, out var intent, out string reason))
                {
                    if (LevelGenerator.TryGenerate(draft, intent, rules, out var generated, out string summary, out string genError))
                    {
                        draft = generated; selected = generated.platforms[Mathf.Max(0, generated.platforms.FindIndex(p => p.id == intent.anchorId) + 1)].id;
                        intentSummary = summary; Changed();
                    }
                    else { intentSummary = "生成失败：" + genError; }
                }
                else { intentSummary = reason; }
            }
            if (!string.IsNullOrEmpty(intentSummary)) GUILayout.Label(intentSummary, small);
            Section("1  路线 / 平台");
            GUILayout.Label("起点 → 按列表顺序行进 → 终点。可在 Scene 拖动候选手柄。", small);
            string[] names = draft.platforms.ConvertAll(p => p.id + (p.id == draft.startId ? "  起点" : p.id == draft.goalId ? "  终点" : "") + (p.locked ? "  [锁定]" : "")).ToArray();
            int index = Mathf.Max(0, draft.platforms.FindIndex(p => p.id == selected));
            int next = EditorGUILayout.Popup("选中平台", index, names);
            if (next != index) { selected = draft.platforms[next].id; requestIssues = null; SceneView.RepaintAll(); }
            var platform = draft.Find(selected);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("定位平台")) Focus(platform);
            using (new EditorGUI.DisabledScope(pending))
                if (GUILayout.Button(platform.locked ? "解锁平台" : "保护平台"))
                {
                    var copy = draft.Copy(); copy.Find(selected).locked = !platform.locked;
                    Commit(copy, platform.locked ? "解锁平台" : "保护平台", true);
                    platform = draft.Find(selected);
                }
            EditorGUILayout.EndHorizontal();
            using (new EditorGUI.DisabledScope(platform.locked))
            {
                EditorGUI.BeginChangeCheck();
                Vector3 position = EditorGUILayout.Vector3Field("位置 (m)", platform.position);
                float width = EditorGUILayout.Slider("宽度 X (m)", platform.size.x, rules.minSize, rules.maxSize);
                float depth = EditorGUILayout.Slider("深度 Z (m)", platform.size.z, rules.minSize, rules.maxSize);
                if (EditorGUI.EndChangeCheck()) { platform.position = position; platform.size = new Vector3(width, 1, depth); Changed(); }
                int currentIndex = draft.platforms.FindIndex(p => p.id == selected);
                if (currentIndex > 0)
                {
                    GUILayout.Space(6);
                    GUILayout.Label($"与前一平台边缘间距：{LevelValidator.Gap(draft.platforms[currentIndex - 1], platform):0.00} m", small);
                    requestedGap = EditorGUILayout.FloatField("请求间距 (m)", requestedGap);
                    if (GUILayout.Button("检查数值并生成候选位置"))
                    {
                        requestIssues = LevelValidator.CheckRequestedGap(requestedGap, rules, selected);
                        if (!LevelValidator.HasErrors(requestIssues))
                        {
                            var previous = draft.platforms[currentIndex - 1];
                            platform.position.x = previous.position.x;
                            platform.position.z = previous.position.z + (previous.size.z + platform.size.z) * .5f + requestedGap;
                            // Remaining objects stay untouched; candidate validation can expose new gaps/overlap.
                            pending = true; acceptedWarnings = false; SceneView.RepaintAll();
                        }
                    }
                }
            }
            if (requestIssues != null) DrawIssues(requestIssues);
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(draft.platforms.Count >= rules.maxPlatforms || selected == draft.goalId))
                if (GUILayout.Button("后方插入平台"))
                {
                    int at = draft.platforms.FindIndex(p => p.id == selected);
                    var p = draft.platforms[at]; var q = draft.platforms[at + 1];
                    int maxId = 0;
                    foreach (var existing in draft.platforms)
                    {
                        if (existing != null && existing.id.Length >= 2 && existing.id[0] == 'P' && int.TryParse(existing.id.Substring(1), out int n) && n > maxId) maxId = n;
                    }
                    string id;
                    do { maxId++; id = "P" + maxId.ToString("D2"); } while (draft.Find(id) != null);
                    draft.platforms.Insert(at + 1, new PlatformData(id, (p.position + q.position) * .5f, new Vector3(2, 1, 2)));
                    selected = id; Changed();
                }
            using (new EditorGUI.DisabledScope(platform.locked || selected == draft.startId || selected == draft.goalId || draft.platforms.Count <= 3))
                if (GUILayout.Button("删除候选平台"))
                { draft.platforms.RemoveAll(p => p.id == selected); selected = draft.startId; Changed(); }
            EditorGUILayout.EndHorizontal();

            Section("2  检查候选");
            GUILayout.Label($"建议 ≤ {rules.recommendedGap:0.0} m   ·   模板允许 ≤ {rules.allowedGap:0.0} m", small);
            GUILayout.Label("间距为轴对齐平台最近边缘的水平距离。阈值是首版设计预设，未完成玩家难度校准。", small);
            var issues = LevelValidator.Check(draft, rules);
            DrawIssues(issues);
            if (issues.Count == 0) EditorGUILayout.HelpBox("配置检查通过；可通关性和体验仍需试玩。", MessageType.Info);
            bool warnings = issues.Exists(i => i.severity == IssueSeverity.Warning);
            bool errors = LevelValidator.HasErrors(issues);
            if (warnings && pending) acceptedWarnings = EditorGUILayout.ToggleLeft("保留警告，应用后实际试玩", acceptedWarnings);
            if (errors && pending) forceErrors = EditorGUILayout.ToggleLeft("已知有红色错误，仍强制应用（自担风险）", forceErrors);
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(!pending || (errors && !forceErrors) || (warnings && !acceptedWarnings && !forceErrors)))
                if (GUILayout.Button(errors && forceErrors ? "强制应用候选" : "应用候选", GUILayout.Height(30))) Commit(draft.Copy(), errors && forceErrors ? "强制应用关卡修改" : "应用关卡修改", false, errors && forceErrors);
            using (new EditorGUI.DisabledScope(!pending))
                if (GUILayout.Button("丢弃候选", GUILayout.Height(30))) { ResetDraft(); SceneView.RepaintAll(); }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(pending))
            {
                if (GUILayout.Button("撤销")) Undo.PerformUndo();
                if (GUILayout.Button("重做")) Undo.PerformRedo();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label(pending ? "● 有候选改动；橙色线框是预览，实心几何尚未改变。" : "● 显示的是已应用关卡。", small);

            Section("3  试玩与文件");
            using (new EditorGUI.DisabledScope(pending))
            {
                if (GUILayout.Button("▶  开始试玩", GUILayout.Height(36)))
                { EditorSceneManager.SaveScene(workspace.gameObject.scene); EditorApplication.isPlaying = true; }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("保存关卡 JSON"))
                    TryFile(() => { LevelStore.Save(LevelStore.DefaultPath, workspace.Data, rules); EditorSceneManager.SaveScene(workspace.gameObject.scene); status = "关卡已保存。"; });
                if (GUILayout.Button("重新载入"))
                {
                    if (EditorUtility.DisplayDialog("重新载入", "用最后保存的关卡替换当前设计？载入可撤销。", "载入", "取消"))
                        TryFile(() => { bool recovered; var data = LevelStore.Load(LevelStore.DefaultPath, rules, out recovered); Commit(data, "载入关卡", true); if (recovered) status = "主文件损坏，已从 .bak 恢复；请检查后保存。"; });
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("导出 JSON 到指定位置…"))
                {
                    string path = EditorUtility.SaveFilePanel("导出关卡", "", "greybox-level", "json");
                    if (!string.IsNullOrEmpty(path)) TryFile(() => { LevelStore.Save(path, workspace.Data, rules); status = "已导出：" + path; });
                }
            }
            EditorGUILayout.SelectableLabel(LevelStore.DefaultPath, small, GUILayout.Height(34));
            EditorGUILayout.HelpBox(status, MessageType.None);
            Section("4  最近一次试玩反馈");
            string report = SessionState.GetString(ReportKey, "");
            if (!string.IsNullOrEmpty(report)) DrawEvents(JsonUtility.FromJson<PlayReport>(report).events);
            else GUILayout.Label("试玩并停止后，掉落和到达终点事件会显示在这里。", small);
            GUILayout.Space(12);
            EditorGUILayout.EndScrollView();
        }
        void TryFile(Action action)
        { try { action(); } catch (Exception e) { status = "文件操作失败；当前设计保留。 " + e.Message; Debug.LogWarning("FYP: " + status); } }
        void DrawIssues(List<LevelIssue> issues)
        {
            foreach (var issue in issues)
            {
                EditorGUILayout.HelpBox((string.IsNullOrEmpty(issue.objectId) ? "" : issue.objectId + " · ") + issue.message,
                    issue.severity == IssueSeverity.Error ? MessageType.Error : issue.severity == IssueSeverity.Warning ? MessageType.Warning : MessageType.Info);
                if (!string.IsNullOrEmpty(issue.objectId) && GUILayout.Button("定位 " + issue.objectId, EditorStyles.miniButton))
                { selected = issue.objectId; Focus(draft.Find(selected)); }
            }
        }
        void DrawEvents(List<PlayEvent> events)
        {
            for (int i = events.Count - 1; i >= Mathf.Max(0, events.Count - 12); i--)
            {
                var item = events[i];
                GUILayout.Label($"{item.time:0.0}s  {item.kind}  /  {item.platformId}\n{item.message}", EditorStyles.wordWrappedLabel);
                if (!EditorApplication.isPlaying && GUILayout.Button("定位 " + item.platformId, EditorStyles.miniButton))
                { selected = item.platformId; Focus(draft.Find(selected)); }
                GUILayout.Space(5);
            }
        }
        void Focus(PlatformData p)
        {
            if (p == null) return;
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null) { view.Frame(new Bounds(p.position, p.size + Vector3.one * 5), false); view.Focus(); }
        }
        void OnSceneGUI(SceneView view)
        {
            Bind();
            if (workspace == null || draft == null || EditorApplication.isPlaying) return;
            for (int i = 0; i < draft.platforms.Count; i++)
            {
                var p = draft.platforms[i];
                Handles.color = pending ? new Color(1, .7f, .25f) : new Color(.3f, .9f, .85f);
                Handles.DrawWireCube(p.position, p.size + Vector3.one * .025f);
                if (i > 0)
                {
                    var a = draft.platforms[i - 1];
                    Handles.DrawDottedLine(a.position + Vector3.up, p.position + Vector3.up, 5);
                    Handles.Label((a.position + p.position) * .5f + Vector3.up, LevelValidator.Gap(a, p).ToString("0.00") + " m");
                }
            }
            var selectedPlatform = draft.Find(selected);
            if (selectedPlatform == null || selectedPlatform.locked) return;
            EditorGUI.BeginChangeCheck();
            Vector3 position = Handles.PositionHandle(selectedPlatform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) { selectedPlatform.position = position; Changed(); }
        }
    }
}
