using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FypGreybox.Tests
{
    public class PlayLoopTests
    {
        GreyboxWorkspace workspace;
        GameObject root;
        Material material;
        [UnitySetUp] public IEnumerator Setup()
        {
            root = new GameObject("Test workspace");
            workspace = root.AddComponent<GreyboxWorkspace>();
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            workspace.blockMaterial = material; workspace.SetData(LevelData.Example());
            var camera = new GameObject("Test camera"); camera.transform.SetParent(root.transform);
            workspace.viewCamera = camera.AddComponent<Camera>();
            yield return null;
            workspace.player.externalDrive = true;
            Physics.SyncTransforms();
            for (int i = 0; i < 20; i++) workspace.player.Tick(Vector3.zero, false, 1f / 60);
        }
        [UnityTearDown] public IEnumerator Cleanup()
        { Object.Destroy(root); Object.Destroy(material); yield return null; }
        [UnityTest] public IEnumerator ForwardInputAndCameraStayAlignedWithRouteAfterRestart()
        {
            Assert.That(GreyboxMotor.InputDirection(Vector2.up), Is.EqualTo(Vector3.forward));
            Assert.That(GreyboxMotor.InputDirection(Vector2.right), Is.EqualTo(Vector3.right));
            Assert.That(GreyboxMotor.InputDirection(Vector2.one).magnitude, Is.EqualTo(1).Within(.001f));
            Vector3 before = workspace.player.transform.position;
            for (int i = 0; i < 10; i++) workspace.player.Tick(GreyboxMotor.InputDirection(Vector2.up), false, 1f / 60);
            Assert.That(workspace.player.transform.position.x, Is.EqualTo(before.x).Within(.001f));
            Assert.That(workspace.player.transform.position.z, Is.GreaterThan(before.z));
            workspace.viewCamera.transform.position = new Vector3(24, 30, -20);
            workspace.Restart(); yield return null;
            Vector3 forward = workspace.viewCamera.transform.forward; forward.y = 0; forward.Normalize();
            Assert.That(Vector3.Dot(forward, Vector3.forward), Is.GreaterThan(.999f));
            Assert.That(workspace.viewCamera.transform.position.x, Is.EqualTo(workspace.player.transform.position.x).Within(.001f));
        }
        [UnityTest] public IEnumerator CharacterCanTraverseDefaultRouteAndReachGoal()
        {
            var data = workspace.Data;
            for (int i = 1; i < data.platforms.Count; i++)
            {
                var from = data.platforms[i - 1]; var to = data.platforms[i];
                Vector3 launch = new Vector3(to.position.x, 0, from.position.z + from.size.z * .5f - .3f);
                for (int t = 0; t < 180; t++)
                {
                    Vector3 delta = launch - workspace.player.transform.position; delta.y = 0;
                    if (delta.magnitude < .11f) break;
                    workspace.player.Tick(delta.normalized, false, 1f / 60);
                    if (t % 5 == 0) yield return null;
                }
                Assert.IsTrue(workspace.player.Grounded, "Must launch from " + from.id);
                bool landed = false;
                for (int t = 0; t < 120; t++)
                {
                    Vector3 delta = to.position - workspace.player.transform.position; delta.y = 0;
                    workspace.player.Tick(delta.magnitude > .1f ? delta.normalized : Vector3.zero, t == 0, 1f / 60);
                    if (t > 10 && workspace.player.Grounded && workspace.player.lastPlatform == to.id) { landed = true; break; }
                    if (t % 5 == 0) yield return null;
                }
                Assert.IsTrue(landed, "Failed to land on " + to.id);
            }
            Assert.IsTrue(workspace.completed);
            Assert.AreEqual(0, workspace.falls);
            Assert.IsTrue(workspace.events.Exists(e => e.kind == "COMPLETE"));
        }
        [UnityTest] public IEnumerator FallReportsObjectAndRespawnsWithoutChangingDesign()
        {
            string json = workspace.Json;
            workspace.player.Teleport(new Vector3(-30, 1, 0));
            for (int i = 0; i < 180 && workspace.falls == 0; i++)
            { workspace.player.Tick(Vector3.zero, false, 1f / 60); if (i % 5 == 0) yield return null; }
            Assert.AreEqual(1, workspace.falls);
            Assert.That(workspace.player.transform.position.z, Is.EqualTo(0).Within(.1f));
            Assert.IsTrue(workspace.events.Exists(e => e.kind == "FALL" && e.platformId == "P01"));
            Assert.AreEqual(json, workspace.Json);
            workspace.Restart();
            Assert.AreEqual(0, workspace.falls); Assert.IsFalse(workspace.completed);
        }
    }
}
