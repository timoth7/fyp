using System;
using UnityEngine;

namespace FypGreybox
{
    /// <summary>由意图生成候选关卡，复用插入逻辑与边缘距离校验。</summary>
    public static class LevelGenerator
    {
        /// <summary>在锚点后插入一个平台，返回候选与说明，失败返回原因。</summary>
        public static bool TryGenerate(LevelData baseLevel, LevelIntent intent, TemplateRules rules, out LevelData candidate, out string summary, out string error)
        {
            candidate = null;
            summary = "";
            error = "";
            if (baseLevel == null || intent == null || rules == null)
            {
                error = "缺少关卡、意图或规则。";
                return false;
            }
            var requestIssues = LevelValidator.CheckRequestedGap(intent.requestedGap, rules, intent.anchorId);
            if (LevelValidator.HasErrors(requestIssues))
            {
                error = requestIssues[0].message;
                return false;
            }
            candidate = baseLevel.Copy();
            int at = candidate.platforms.FindIndex(p => p.id == intent.anchorId);
            if (at < 0 || at >= candidate.platforms.Count - 1)
            {
                error = "锚点不在路线中间，无法在其后插入。";
                return false;
            }
            var a = candidate.platforms[at];
            if (a.locked)
            {
                error = a.id + " 已锁定，换一个锚点再生成。";
                return false;
            }
            if (candidate.platforms.Count >= rules.maxPlatforms)
            {
                error = "平台数量已达上限。";
                return false;
            }
            string id = NextPlatformId(candidate);
            var size = new Vector3(4, 1, 4);
            float z = a.position.z + (a.size.z + size.z) * 0.5f + intent.requestedGap;
            var inserted = new PlatformData(id, new Vector3(a.position.x, a.position.y, z), size);
            candidate.platforms.Insert(at + 1, inserted);
            float shift = size.z + intent.requestedGap;
            for (int i = at + 2; i < candidate.platforms.Count; i++)
            {
                var p = candidate.platforms[i];
                p.position = new Vector3(p.position.x, p.position.y, p.position.z + shift);
            }
            var issues = LevelValidator.Check(candidate, rules);
            if (LevelValidator.HasErrors(issues))
            {
                error = issues[0].message;
                candidate = null;
                return false;
            }
            summary = "在 " + a.id + " 后插入 " + id + "，边缘间距 " + intent.requestedGap.ToString("0.00") + " m；橙色为预览，应用后可撤销。";
            return true;
        }
        /// <summary>按 P01、P02 顺序取下一个可用编号，避免随机 ID。</summary>
        static string NextPlatformId(LevelData level)
        {
            int max = 0;
            foreach (var p in level.platforms)
            {
                if (p == null || string.IsNullOrEmpty(p.id) || p.id.Length < 2 || p.id[0] != 'P') continue;
                if (int.TryParse(p.id.Substring(1), out int n) && n > max) max = n;
            }
            string candidateId;
            do
            {
                max++;
                candidateId = "P" + max.ToString("D2");
            } while (level.Find(candidateId) != null);
            return candidateId;
        }
    }
}
