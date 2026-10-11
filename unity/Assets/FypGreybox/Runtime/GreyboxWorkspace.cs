using System;
using System.Collections.Generic;
using UnityEngine;

namespace FypGreybox
{
    public class GreyboxWorkspace : MonoBehaviour
    {
        [SerializeField, TextArea] string levelJson;
        public Material blockMaterial;
        public Material startMaterial, goalMaterial, platformMaterial;
        public Camera viewCamera;
        [NonSerialized] public readonly List<PlayEvent> events = new List<PlayEvent>();
        [NonSerialized] public GreyboxMotor player;
        [NonSerialized] public bool completed;
        [NonSerialized] public int falls;
        [NonSerialized] public float elapsed;
        public string Json => levelJson;
        public LevelData Data => string.IsNullOrEmpty(levelJson) ? LevelData.Example() : JsonUtility.FromJson<LevelData>(levelJson);
        public TemplateRules Rules => TemplateRules.Load();
        public void SetData(LevelData data) { levelJson = JsonUtility.ToJson(data, true); }
        public bool TryApply(LevelData candidate, out List<LevelIssue> issues, bool explicitUnlock = false, bool forceErrors = false)
        {
            issues = LevelValidator.Check(candidate, Rules);
            if (LevelValidator.HasErrors(issues) && !forceErrors) return false;
            if (!explicitUnlock && !LevelValidator.PreservesLocked(Data, candidate))
            { issues.Add(new LevelIssue(IssueSeverity.Error, "", "候选修改了受保护的平台。")); return false; }
            SetData(candidate);
            Rebuild();
            return true;
        }
        public void Rebuild()
        {
            var old = transform.Find("Generated Platforms");
            if (old != null)
            {
                old.gameObject.SetActive(false);
                if (Application.isPlaying) Destroy(old.gameObject); else DestroyImmediate(old.gameObject);
            }
            var parent = new GameObject("Generated Platforms").transform;
            parent.SetParent(transform, false);
            var data = Data;
            foreach (var p in data.platforms)
            {
                Color color = p.id == data.startId ? new Color(.2f, .73f, .62f) : p.id == data.goalId ? new Color(1f, .64f, .25f) : new Color(.39f, .50f, .65f);
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = p.id;
                block.transform.SetParent(parent, false);
                block.transform.position = p.position;
                block.transform.localScale = p.size;
                var renderer = block.GetComponent<Renderer>();
                renderer.sharedMaterial = p.id == data.startId ? (startMaterial ?? blockMaterial) : p.id == data.goalId ? (goalMaterial ?? blockMaterial) : (platformMaterial ?? blockMaterial);
                SetColor(renderer, color);
                block.AddComponent<PlatformMarker>().id = p.id;
                block.hideFlags = HideFlags.NotEditable;
                // A route index visible from the overview; it is decoration, not a collider.
                var label = new GameObject(p.id + " label");
                label.transform.SetParent(parent, false);
                label.transform.position = p.position + new Vector3(-p.size.x * .5f + .35f, .515f, -.5f);
                label.transform.rotation = Quaternion.Euler(90, 0, 0);
                var text = label.AddComponent<TextMesh>();
                text.text = p.id == data.startId ? "START" : p.id == data.goalId ? "FINISH" : p.id;
                text.fontSize = 48; text.characterSize = .1f; text.color = new Color(.90f, .96f, 1f);
                text.anchor = TextAnchor.MiddleLeft;
            }
        }
        public static void SetColor(Renderer renderer, Color color)
        {
            var props = new MaterialPropertyBlock();
            props.SetColor("_BaseColor", color); props.SetColor("_Color", color);
            renderer.SetPropertyBlock(props);
        }
        void Start()
        {
            Application.runInBackground = true;
            var issues = LevelValidator.Check(Data, Rules);
            if (LevelValidator.HasErrors(issues)) { Debug.LogWarning("FYP: 关卡有红色问题（强制应用），试玩可能无法通关，可撤销回退。"); }
            Rebuild();
            var go = new GameObject("Playtest character");
            go.transform.SetParent(transform, false);
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.6f; cc.radius = .28f; cc.center = new Vector3(0, .8f, 0); cc.skinWidth = .02f; cc.stepOffset = .12f;
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Character visual";
            visual.transform.SetParent(go.transform, false);
            visual.transform.localPosition = Vector3.up * .8f;
            visual.transform.localScale = new Vector3(.55f, .8f, .55f);
            Destroy(visual.GetComponent<Collider>());
            visual.GetComponent<Renderer>().sharedMaterial = blockMaterial;
            SetColor(visual.GetComponent<Renderer>(), new Color(.94f, .97f, 1f));
            player = go.AddComponent<GreyboxMotor>();
            player.workspace = this;
            Restart();
        }
        public void Restart()
        {
            completed = false; elapsed = 0; falls = 0; events.Clear();
            Respawn();
            PositionCamera(true);
            AddEvent("START", Data.startId, "开始试玩");
        }
        public void Respawn()
        {
            var p = Data.Find(Data.startId);
            player.Teleport(p.position + Vector3.up * (.5f + .08f));
            player.lastPlatform = p.id;
            PositionCamera(true);
        }
        public void Fall()
        {
            falls++;
            AddEvent("FALL", player.lastPlatform, "从此平台附近掉落；检查下一个落点和起跳时机。");
            Respawn();
        }
        public void Land(string id)
        {
            if (player.lastPlatform != id) AddEvent("LAND", id, "落地");
            player.lastPlatform = id;
            if (id == Data.goalId && !completed)
            { completed = true; AddEvent("COMPLETE", id, "已到达终点"); }
        }
        public void AddEvent(string kind, string id, string message)
        {
            if (events.Count >= 100) events.RemoveAt(0);
            events.Add(new PlayEvent { kind = kind, platformId = id, time = elapsed, message = message });
        }
        void Update()
        {
            if (player == null) return;
            if (!completed) elapsed += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.R)) Restart();
        }
        void LateUpdate() { PositionCamera(false); }
        void PositionCamera(bool immediate)
        {
            if (player == null || viewCamera == null) return;
            Vector3 target = player.transform.position + Vector3.up * .7f;
            Vector3 position = target + new Vector3(0, 10, -12);
            viewCamera.transform.position = immediate ? position : Vector3.Lerp(viewCamera.transform.position, position, 1f - Mathf.Exp(-6f * Time.deltaTime));
            // Keep camera yaw at zero while following lateral movement.
            var aligned = viewCamera.transform.position; aligned.x = target.x; viewCamera.transform.position = aligned;
            viewCamera.transform.LookAt(target + Vector3.forward * 2f);
        }
        void OnGUI()
        {
            float s = Mathf.Clamp(Screen.width / 1280f, .65f, 1.5f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1));
            var rect = new Rect(24, 24, 365, 126);
            GUI.color = new Color(.06f, .09f, .15f, .94f); GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = Color.white;
            var title = new GUIStyle(GUI.skin.label) { fontSize = 23, fontStyle = FontStyle.Bold };
            var body = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            GUI.Label(new Rect(42, 36, 330, 32), completed ? "ROUTE COMPLETE" : "GREYBOX / PLAYTEST", title);
            GUI.Label(new Rect(42, 76, 330, 22), "WASD / Arrows  move     SPACE  jump     R  reset", body);
            GUI.Label(new Rect(42, 106, 330, 22), $"{elapsed:0.0}s   /   {falls} falls   /   {player?.lastPlatform ?? "—"}", body);
            GUI.matrix = Matrix4x4.identity;
        }
    }
    [Serializable] public class PlayEvent
    { public string kind, platformId, message; public float time; }
    [Serializable] public class PlayReport
    { public List<PlayEvent> events = new List<PlayEvent>(); }
}
