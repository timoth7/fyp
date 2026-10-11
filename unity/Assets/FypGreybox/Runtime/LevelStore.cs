using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace FypGreybox
{
    public static class LevelStore
    {
        public static string DefaultPath => Path.Combine(Application.persistentDataPath, "FypGreybox", "level.json");
        public static LevelData Parse(string json, TemplateRules rules)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Length > 1000000 || !json.Contains("\"version\"") || !json.Contains("\"template\""))
                throw new InvalidDataException("关卡缺少版本/模板，或文件过大。");
            LevelData data;
            try { data = JsonUtility.FromJson<LevelData>(json); }
            catch (Exception e) { throw new InvalidDataException("JSON 无法解析。", e); }
            var issues = LevelValidator.Check(data, rules);
            if (LevelValidator.HasErrors(issues)) throw new InvalidDataException(issues.Find(i => i.severity == IssueSeverity.Error).message);
            return data;
        }
        public static void Save(string path, LevelData data, TemplateRules rules)
        {
            string json = JsonUtility.ToJson(data, true);
            Parse(json, rules);
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            string temp = path + ".tmp";
            byte[] bytes = new UTF8Encoding(false).GetBytes(json);
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
            if (File.Exists(path)) File.Replace(temp, path, path + ".bak");
            else File.Move(temp, path);
        }
        public static LevelData Load(string path, TemplateRules rules, out bool recovered)
        {
            recovered = false;
            try { return Read(path, rules); }
            catch (Exception original) when (original is InvalidDataException || original is IOException || original is UnauthorizedAccessException || original is ArgumentException)
            {
                if (!File.Exists(path + ".bak")) throw;
                var backup = Read(path + ".bak", rules);
                recovered = true;
                return backup;
            }
        }
        static LevelData Read(string path, TemplateRules rules)
        {
            if (new FileInfo(path).Length > 1000000) throw new InvalidDataException("关卡文件过大。");
            return Parse(File.ReadAllText(path), rules);
        }
    }
}
