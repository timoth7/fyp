// Read-only credential and canonical wiki access probe. Never prints tokens.
async function run() {
  const authResponse = await fetch('https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal', {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ app_id: process.env.APP_ID, app_secret: process.env.APP_SECRET }),
    signal: AbortSignal.timeout(15000)
  });
  const auth = await authResponse.json();
  console.log(JSON.stringify({ step: 'application_authentication', code: auth.code, success: auth.code === 0 }));
  if (auth.code !== 0) { process.exitCode = 1; return; }
  const wikiResponse = await fetch('https://open.feishu.cn/open-apis/wiki/v2/spaces/get_node?token=KVV2w0qRzinNuGkjCw8cvGNTnFb', {
    headers: { Authorization: 'Bearer ' + auth.tenant_access_token }, signal: AbortSignal.timeout(15000)
  });
  const wiki = await wikiResponse.json();
  console.log(JSON.stringify({ step: 'canonical_wiki_resolution_as_application', code: wiki.code, success: wiki.code === 0,
    documentType: wiki.data?.node?.obj_type, documentId: wiki.data?.node?.obj_token }));
  if (wiki.code !== 0) process.exitCode = 1;
}
run().catch(() => { console.error('Read-only probe failed; credential and response details withheld.'); process.exitCode = 1; });
