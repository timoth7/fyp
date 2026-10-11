using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FypGreybox.Editor
{
    public static class GreyboxSetup
    {
        public const string ScenePath = "Assets/FypGreybox/Scenes/GreyboxLab.unity";
        [MenuItem("FYP/Create or Open Greybox Lab")]
        public static void Open()
        {
            if (EditorApplication.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (File.Exists(ScenePath)) EditorSceneManager.OpenScene(ScenePath);
            else Create();
            GreyboxWorkbench.Open();
        }
        public static void Create()
        {
            if (File.Exists(ScenePath)) throw new IOException("GreyboxLab already exists; use Open to preserve it.");
            Directory.CreateDirectory("Assets/FypGreybox/Scenes");
            Directory.CreateDirectory("Assets/FypGreybox/Materials");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetFloat("_Smoothness", .3f);
            AssetDatabase.CreateAsset(material, "Assets/FypGreybox/Materials/Blockout.mat");
            var workspace = new GameObject("Greybox Workspace").AddComponent<GreyboxWorkspace>();
            workspace.blockMaterial = material;
            workspace.startMaterial = ColorMaterial(material, "Start", new Color(.2f, .73f, .62f));
            workspace.goalMaterial = ColorMaterial(material, "Goal", new Color(1f, .64f, .25f));
            workspace.platformMaterial = ColorMaterial(material, "Platform", new Color(.39f, .50f, .65f));
            workspace.SetData(LevelData.Example());
            workspace.Rebuild();
            var camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.gameObject.AddComponent<AudioListener>();
            camera.transform.position = new Vector3(24, 30, -20);
            camera.transform.LookAt(new Vector3(0, 0, 12));
            camera.fieldOfView = 48; camera.farClipPlane = 300;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.035f, .055f, .09f);
            workspace.viewCamera = camera;
            var light = new GameObject("Key Light").AddComponent<Light>();
            light.type = LightType.Directional; light.intensity = 1.4f; light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(48, -35, 0);
            RenderSettings.ambientLight = new Color(.4f, .47f, .57f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.LookAt(new Vector3(0, 0, 12), Quaternion.Euler(48, -30, 0), 25);
        }
        static Material ColorMaterial(Material source, string name, Color color)
        {
            var material = new Material(source); material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, "Assets/FypGreybox/Materials/" + name + ".mat");
            return material;
        }
    }
}
