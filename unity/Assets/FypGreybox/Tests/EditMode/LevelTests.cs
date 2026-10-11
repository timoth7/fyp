using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace FypGreybox.Tests
{
    public class LevelTests
    {
        TemplateRules Rules => TemplateRules.Load();
        [Test] public void DefaultRouteHasNoErrorsOrWarnings()
        { Assert.That(LevelValidator.Check(LevelData.Example(), Rules), Is.Empty); }
        [Test] public void GapUsesEdgesAndHandlesDiagonalSeparation()
        {
            var a = new PlatformData("a", Vector3.zero, new Vector3(2, 1, 2));
            var b = new PlatformData("b", new Vector3(5, 0, 6), new Vector3(2, 1, 2));
            Assert.That(LevelValidator.Gap(a, b), Is.EqualTo(5).Within(.001f));
        }
        [Test] public void NumericRequestAndCandidateUseSameWarningAndErrorThresholds()
        {
            foreach (float gap in new[] { 2.6f, 3f, 4.5f, 4.6f })
            {
                var level = LevelData.Example();
                float delta = gap - LevelValidator.Gap(level.platforms[0], level.platforms[1]);
                for (int i = 1; i < level.platforms.Count; i++) level.platforms[i].position.z += delta;
                var request = LevelValidator.CheckRequestedGap(gap, Rules, "P02");
                // Candidate checks also add a separate physics estimate. Compare only the shared gap policy.
                var candidate = LevelValidator.Check(level, Rules).Where(i => i.objectId == "P02" && i.message.StartsWith("间距")).ToList();
                Assert.AreEqual(LevelValidator.HasErrors(request), LevelValidator.HasErrors(candidate));
                Assert.AreEqual(request.Any(i => i.severity == IssueSeverity.Warning), candidate.Any(i => i.severity == IssueSeverity.Warning));
            }
        }
        [Test] public void InvalidCandidateDoesNotChangeWorkspace()
        {
            var go = new GameObject("test");
            try
            {
                var workspace = go.AddComponent<GreyboxWorkspace>(); workspace.SetData(LevelData.Example());
                string before = workspace.Json; var invalid = workspace.Data.Copy(); invalid.platforms[1].position.z = 70;
                Assert.IsFalse(workspace.TryApply(invalid, out _)); Assert.AreEqual(before, workspace.Json);
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
        [Test] public void LocksRejectModificationAndDeletionButAllowOtherObjects()
        {
            var before = LevelData.Example(); before.platforms[1].locked = true;
            var candidate = before.Copy(); candidate.platforms[2].position.x += .25f;
            Assert.IsTrue(LevelValidator.PreservesLocked(before, candidate));
            candidate.platforms[1].position.z += 1; Assert.IsFalse(LevelValidator.PreservesLocked(before, candidate));
            candidate = before.Copy(); candidate.platforms.RemoveAt(1); Assert.IsFalse(LevelValidator.PreservesLocked(before, candidate));
        }
        [Test] public void RejectsDuplicateIdsNaNAndUnknownVersions()
        {
            var level = LevelData.Example(); level.platforms[1].id = level.startId;
            Assert.IsTrue(LevelValidator.HasErrors(LevelValidator.Check(level, Rules)));
            level = LevelData.Example(); level.platforms[1].position.x = float.NaN;
            Assert.IsTrue(LevelValidator.HasErrors(LevelValidator.Check(level, Rules)));
            level = LevelData.Example(); level.version = 2;
            Assert.Throws<InvalidDataException>(() => LevelStore.Parse(JsonUtility.ToJson(level), Rules));
            Assert.Throws<InvalidDataException>(() => LevelStore.Parse("{}", Rules));
        }
        [Test] public void ElevationChangesRequirePlaytestWarning()
        {
            var level = LevelData.Example(); level.platforms[1].position.y = .5f;
            Assert.That(LevelValidator.Check(level, Rules).Any(i => i.severity == IssueSeverity.Warning && i.message.Contains("高差")), Is.True);
        }
        [Test] public void IntentGeneratorInsertsAfterAnchorAndStaysDeterministic()
        {
            var level = LevelData.Example();
            Assert.IsTrue(LevelIntentParser.TryParse("在 P02 后面加一个 2 米的跳跃", level, out var intent, out _));
            Assert.AreEqual("P02", intent.anchorId);
            Assert.IsTrue(LevelGenerator.TryGenerate(level, intent, Rules, out var a, out _, out _));
            Assert.IsTrue(LevelGenerator.TryGenerate(level, intent, Rules, out var b, out _, out _));
            Assert.AreEqual(a.platforms.Count + 0, level.platforms.Count + 1);
            Assert.AreEqual(JsonUtility.ToJson(a).Length, JsonUtility.ToJson(b).Length);
            Assert.IsFalse(LevelValidator.HasErrors(LevelValidator.Check(a, Rules)));
        }
        [Test] public void IntentGeneratorRejectsOutOfRangeAndLockedAnchor()
        {
            var level = LevelData.Example();
            Assert.IsTrue(LevelIntentParser.TryParse("在 P02 后面加一个 8 米", level, out var big, out _));
            Assert.IsFalse(LevelGenerator.TryGenerate(level, big, Rules, out _, out _, out string err));
            Assert.IsTrue(err.Contains("超出模板范围"));
            level.platforms[1].locked = true;
            Assert.IsTrue(LevelIntentParser.TryParse("在 P02 后面加一个 2 米", level, out var locked, out _));
            Assert.IsFalse(LevelGenerator.TryGenerate(level, locked, Rules, out _, out _, out string lockErr));
            Assert.IsTrue(lockErr.Contains("锁定"));
        }
        [Test] public void IntentParserFallsBackToSensibleDefaults()
        {
            var level = LevelData.Example();
            Assert.IsTrue(LevelIntentParser.TryParse("加一个跳跃", level, out var intent, out _));
            Assert.AreEqual("P04", intent.anchorId);
            Assert.AreEqual(2f, intent.requestedGap);
        }
        [Test] public void SaveReloadAndCorruptionRecoveryPreserveDesign()
        {
            string dir = Path.Combine(Path.GetTempPath(), "FypGreyboxTest-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(dir, "level.json");
            try
            {
                var level = LevelData.Example(); LevelStore.Save(path, level, Rules);
                var reloaded = LevelStore.Load(path, Rules, out bool recovered);
                Assert.IsFalse(recovered); Assert.AreEqual(JsonUtility.ToJson(level), JsonUtility.ToJson(reloaded));
                var changed = level.Copy(); changed.platforms[2].position.x += .25f; LevelStore.Save(path, changed, Rules);
                Assert.IsFalse(File.Exists(path + ".tmp"));
                File.WriteAllText(path, "{broken");
                var backup = LevelStore.Load(path, Rules, out recovered);
                Assert.IsTrue(recovered); Assert.AreEqual(JsonUtility.ToJson(level), JsonUtility.ToJson(backup));
            }
            finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
        }
    }
}
