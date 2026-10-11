using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FypGreybox
{
    /// <summary>一句话生成解析出的结构化意图，不含任何生成逻辑。</summary>
    [Serializable]
    public sealed class LevelIntent
    {
        public string anchorId;
        public float requestedGap = -1f;
        public bool wantsEasier;
        public bool wantsHarder;
        public string raw;
    }

    /// <summary>关键词加数字的确定性解析，失败时返回人类可读原因。</summary>
    public static class LevelIntentParser
    {
        static readonly Regex Number = new Regex(@"(?<v>\d+(\.\d+)?)\s*(m|米)?", RegexOptions.Compiled);

        /// <summary>解析描述文本，成功返回意图，失败返回原因。</summary>
        public static bool TryParse(string text, LevelData level, out LevelIntent intent, out string reason)
        {
            intent = new LevelIntent { raw = text ?? "" };
            reason = "";
            if (level == null || level.platforms == null || level.platforms.Count == 0)
            {
                reason = "当前没有可用的关卡。";
                return false;
            }
            string t = intent.raw.Trim();
            if (t.Length == 0)
            {
                reason = "请先输入一句话描述，比如：在 P02 后面加一个 2 米的跳跃。";
                return false;
            }
            intent.anchorId = null;
            foreach (var p in level.platforms)
            {
                if (p != null && !string.IsNullOrEmpty(p.id) && t.Contains(p.id)) { intent.anchorId = p.id; break; }
            }
            if (intent.anchorId == null)
            {
                var m = Regex.Match(t, @"P\d+", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    string up = m.Value.ToUpperInvariant();
                    if (level.Find(up) != null) intent.anchorId = up;
                }
            }
            if (intent.anchorId == null) intent.anchorId = level.platforms[level.platforms.Count - 2].id;
            var matches = Number.Matches(t);
            float v = 2f;
            bool hasNumber = false;
            foreach (Match cand in matches)
            {
                int idx = cand.Index;
                bool insideId = idx > 0 && (t[idx - 1] == 'P' || t[idx - 1] == 'p');
                if (insideId) continue;
                if (float.TryParse(cand.Groups["v"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                {
                    v = parsed;
                    hasNumber = true;
                }
            }
            intent.requestedGap = hasNumber ? v : 2f;
            intent.wantsEasier = t.Contains("简单") || t.Contains("容易") || t.Contains("近");
            intent.wantsHarder = t.Contains("难") || t.Contains("远") || t.Contains("极限");
            if (intent.wantsEasier && intent.requestedGap > 2f) intent.requestedGap = 2f;
            if (intent.wantsHarder && intent.requestedGap < 3f) intent.requestedGap = 3f;
            return true;
        }
    }
}
