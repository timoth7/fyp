using System;
using System.Collections.Generic;
using UnityEngine;

namespace FypGreybox
{
    [Serializable] public class PlatformData
    {
        public string id;
        public Vector3 position;
        public Vector3 size = new Vector3(4, 1, 4);
        public bool locked;
        public PlatformData(string id, Vector3 position, Vector3 size)
        { this.id = id; this.position = position; this.size = size; }
    }

    [Serializable] public class LevelData
    {
        public int version = 1;
        public string template = "platform-jump-v1";
        public string title = "First crossing";
        public List<PlatformData> platforms = new List<PlatformData>();
        public string startId = "P01", goalId = "P05";
        public static LevelData Example()
        {
            var level = new LevelData();
            level.platforms.Add(new PlatformData("P01", Vector3.zero, new Vector3(6, 1, 6)));
            level.platforms.Add(new PlatformData("P02", new Vector3(0, 0, 7), new Vector3(4, 1, 4)));
            level.platforms.Add(new PlatformData("P03", new Vector3(1, 0, 13), new Vector3(4, 1, 4)));
            level.platforms.Add(new PlatformData("P04", new Vector3(-1, 0, 19), new Vector3(4, 1, 4)));
            level.platforms.Add(new PlatformData("P05", new Vector3(0, 0, 25.5f), new Vector3(6, 1, 5)));
            return level;
        }
        public LevelData Copy() => JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(this));
        public PlatformData Find(string id) => platforms.Find(p => p != null && p.id == id);
    }

    [Serializable] public class TemplateRules
    {
        public string version = "platform-jump-rules-v1";
        public float recommendedGap = 2.6f;
        public float allowedGap = 4.5f;
        public float moveSpeed = 6f;
        public float jumpHeight = 1.4f;
        public float gravity = -22f;
        public float minSize = 2f, maxSize = 12f, maxCoordinate = 100f, maxElevation = 3f;
        public int maxPlatforms = 24;
        public float IdealSameHeightRange => moveSpeed * 2f * Mathf.Sqrt(2f * jumpHeight / -gravity);
        public static TemplateRules Load()
        {
            var asset = Resources.Load<TextAsset>("FypPlatformRules");
            if (asset == null) throw new InvalidOperationException("Missing FypPlatformRules.json");
            var rules = JsonUtility.FromJson<TemplateRules>(asset.text);
            if (rules == null || rules.gravity >= 0 || rules.jumpHeight <= 0 || rules.moveSpeed <= 0 ||
                rules.recommendedGap <= 0 || rules.allowedGap < rules.recommendedGap || rules.minSize <= 0 ||
                rules.maxSize < rules.minSize || rules.maxPlatforms < 3)
                throw new InvalidOperationException("Invalid template rules.");
            return rules;
        }
    }

    public enum IssueSeverity { Info, Warning, Error }
    public sealed class LevelIssue
    {
        public readonly IssueSeverity severity;
        public readonly string objectId, message;
        public LevelIssue(IssueSeverity severity, string objectId, string message)
        { this.severity = severity; this.objectId = objectId; this.message = message; }
    }

    public static class LevelValidator
    {
        public static bool HasErrors(List<LevelIssue> issues) => issues.Exists(i => i.severity == IssueSeverity.Error);
        public static bool Finite(float v) => !float.IsNaN(v) && !float.IsInfinity(v);
        public static float Gap(PlatformData a, PlatformData b)
        {
            float x = Mathf.Max(0, Mathf.Abs(a.position.x - b.position.x) - (a.size.x + b.size.x) * .5f);
            float z = Mathf.Max(0, Mathf.Abs(a.position.z - b.position.z) - (a.size.z + b.size.z) * .5f);
            return Mathf.Sqrt(x * x + z * z);
        }
        // Same deterministic rule for a numeric request and a generated candidate.
        public static List<LevelIssue> CheckRequestedGap(float gap, TemplateRules rules, string id = "")
        {
            var issues = new List<LevelIssue>();
            const float tolerance = .0001f; // Metre-scale float arithmetic at exact boundaries.
            if (!Finite(gap) || gap < 0 || gap > rules.allowedGap + tolerance)
                issues.Add(new LevelIssue(IssueSeverity.Error, id, $"间距 {gap:0.00} m 超出模板范围 0–{rules.allowedGap:0.0} m；缩短间距或增加落点。"));
            else if (gap > rules.recommendedGap + tolerance)
                issues.Add(new LevelIssue(IssueSeverity.Warning, id, $"间距 {gap:0.00} m 超过建议 {rules.recommendedGap:0.0} m；可保留并试玩，不代表已证明无法通过。"));
            return issues;
        }
        public static List<LevelIssue> Check(LevelData level, TemplateRules rules)
        {
            var issues = new List<LevelIssue>();
            Action<string, string> error = (id, text) => issues.Add(new LevelIssue(IssueSeverity.Error, id, text));
            if (level == null || level.version != 1 || level.template != "platform-jump-v1")
            { error("", "无法读取此版本或模板的关卡。"); return issues; }
            if (level.platforms == null || level.platforms.Count < 3 || level.platforms.Count > rules.maxPlatforms)
            { error("", $"平台数量必须为 3–{rules.maxPlatforms}。"); return issues; }
            var ids = new HashSet<string>();
            foreach (var p in level.platforms)
            {
                if (p == null) { error("", "平台数据为空。"); continue; }
                if (string.IsNullOrWhiteSpace(p.id) || !ids.Add(p.id)) error(p.id, "平台 ID 为空或重复。");
                if (!Finite(p.position.x) || !Finite(p.position.y) || !Finite(p.position.z) ||
                    !Finite(p.size.x) || !Finite(p.size.y) || !Finite(p.size.z))
                { error(p.id, "位置/尺寸必须为有限数值。"); continue; }
                if (Mathf.Abs(p.position.x) > rules.maxCoordinate || Mathf.Abs(p.position.z) > rules.maxCoordinate || Mathf.Abs(p.position.y) > rules.maxElevation)
                    error(p.id, "位置超出此模板的编辑范围。");
                if (p.size.x < rules.minSize || p.size.z < rules.minSize || p.size.x > rules.maxSize || p.size.z > rules.maxSize || Mathf.Abs(p.size.y - 1f) > .001f)
                    error(p.id, $"宽/深允许 {rules.minSize}–{rules.maxSize} m；首版厚度固定 1 m。");
            }
            if (HasErrors(issues)) return issues;
            if (level.startId != level.platforms[0].id || level.goalId != level.platforms[level.platforms.Count - 1].id || level.startId == level.goalId)
                error("", "路线首尾必须对应不同的起点和终点 ID。");
            for (int i = 0; i < level.platforms.Count; i++)
            {
                var p = level.platforms[i];
                for (int j = i + 1; j < level.platforms.Count; j++)
                {
                    var q = level.platforms[j];
                    if (Mathf.Abs(p.position.x - q.position.x) < (p.size.x + q.size.x) * .5f - .01f &&
                        Mathf.Abs(p.position.z - q.position.z) < (p.size.z + q.size.z) * .5f - .01f &&
                        Mathf.Abs(p.position.y - q.position.y) < 1f - .01f)
                        error(q.id, $"{p.id} 与 {q.id} 平台实体重叠；请先移开。");
                }
                if (i == 0) continue;
                var a = level.platforms[i - 1];
                issues.AddRange(CheckRequestedGap(Gap(a, p), rules, p.id));
                if (Mathf.Abs(p.position.y - a.position.y) > .05f)
                    issues.Add(new LevelIssue(IssueSeverity.Warning, p.id, $"{a.id} → {p.id} 有高差，同高跳跃估计不适用；请实际试玩。"));
                else if (Gap(a, p) > rules.IdealSameHeightRange)
                    issues.Add(new LevelIssue(IssueSeverity.Warning, p.id, $"超过点质点同高估算 {rules.IdealSameHeightRange:0.00} m。未计角色宽度/起跳误差，不作为通关证明。"));
            }
            return issues;
        }
        public static bool PreservesLocked(LevelData before, LevelData after)
        {
            foreach (var p in before.platforms)
            {
                if (!p.locked) continue;
                var q = after.Find(p.id);
                if (q == null || p.position != q.position || p.size != q.size || !q.locked) return false;
            }
            return true;
        }
    }
}
