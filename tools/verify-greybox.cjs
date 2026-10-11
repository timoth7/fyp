// End-to-end smoke check against the running editor. No user-created scene is replaced.
const fs = require('fs');
const path = require('path');
const assert = require('assert/strict');
const sdk = path.resolve(__dirname, '../.local/lark-mcp/node_modules/@modelcontextprotocol/sdk/dist/cjs');
const { Client } = require(path.join(sdk, 'client/index.js'));
const { StreamableHTTPClientTransport } = require(path.join(sdk, 'client/streamableHttp.js'));
(async () => {
  const c = new Client({ name: 'fyp-acceptance-check', version: '1.0' });
  const receipt = { device: 'Windows PC', steps: [] };
  async function call(name, args) {
    const raw = await c.callTool({ name, arguments: args }, undefined, { timeout: 30000 });
    const result = raw.structuredContent ?? JSON.parse(raw.content.find(x => x.type === 'text').text);
    if (result.success === false) throw new Error(JSON.stringify(result));
    return result;
  }
  const code = text => call('execute_code', { action: 'execute', code: text });
  const current = async () => (await code('return UnityEngine.Object.FindObjectOfType<FypGreybox.GreyboxWorkspace>().Json;')).data.result;
  await c.connect(new StreamableHTTPClientTransport(new URL('http://127.0.0.1:8080/mcp')));
  try {
    const state = JSON.parse((await c.readResource({ uri: 'mcpforunity://editor/state' })).contents[0].text).data;
    assert.equal(state.editor.active_scene.path, 'Assets/FypGreybox/Scenes/GreyboxLab.unity');
    assert.equal(state.editor.play_mode.is_playing, false);
    receipt.unityVersion = state.unity.unity_version;
    const before = await current();
    const result = await code('var ws = UnityEngine.Object.FindObjectOfType<FypGreybox.GreyboxWorkspace>(); var candidate = ws.Data.Copy(); candidate.platforms[1].position.x += .25f; var w = UnityEditor.EditorWindow.GetWindow<FypGreybox.Editor.GreyboxWorkbench>(); typeof(FypGreybox.Editor.GreyboxWorkbench).GetMethod("Commit", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(w, new object[]{candidate,"Acceptance edit",false}); return ws.Json;');
    assert.notEqual(result.data.result, before); receipt.steps.push('Workbench apply changed design');
    await call('manage_editor', { action: 'undo' });
    assert.equal(await current(), before); receipt.steps.push('Undo restored exact design JSON');
    await call('manage_editor', { action: 'redo' });
    assert.equal(await current(), result.data.result); receipt.steps.push('Redo restored exact edited design JSON');
    await call('manage_editor', { action: 'undo' });
    assert.equal(await current(), before);
    const save = await code('var ws = UnityEngine.Object.FindObjectOfType<FypGreybox.GreyboxWorkspace>(); FypGreybox.LevelStore.Save(FypGreybox.LevelStore.DefaultPath, ws.Data, ws.Rules); UnityEditor.SceneManagement.EditorSceneManager.SaveScene(ws.gameObject.scene); return FypGreybox.LevelStore.DefaultPath;');
    receipt.savePath = save.data.result;
    await code('UnityEditor.SceneManagement.EditorSceneManager.OpenScene(FypGreybox.Editor.GreyboxSetup.ScenePath); return "reopened";');
    assert.equal(await current(), before); receipt.steps.push('Scene reopened with exact design');
    const loaded = await code('bool recovered; var ws = UnityEngine.Object.FindObjectOfType<FypGreybox.GreyboxWorkspace>(); var level = FypGreybox.LevelStore.Load(FypGreybox.LevelStore.DefaultPath, ws.Rules, out recovered); return UnityEngine.JsonUtility.ToJson(level, true);');
    assert.equal(loaded.data.result, before); receipt.steps.push('Saved JSON reloaded with exact design');
    receipt.status = 'passed';
  } catch (e) { receipt.status = 'failed'; receipt.error = e.message; process.exitCode = 1; }
  finally {
    fs.writeFileSync(path.resolve(__dirname, '../.local/greybox-smoke-result.json'), JSON.stringify(receipt, null, 2));
    console.log(JSON.stringify(receipt)); await c.close();
  }
})();
