// Local bridge for this workspace when native MCP discovery has not refreshed.
// Usage: node tools/unity-mcp.cjs resource <uri> | tool <name> <json-file> | schema <name>
const path = require('path');
const fs = require('fs');
const sdk = path.resolve(__dirname, '../.local/lark-mcp/node_modules/@modelcontextprotocol/sdk/dist/cjs');
const { Client } = require(path.join(sdk, 'client/index.js'));
const { StreamableHTTPClientTransport } = require(path.join(sdk, 'client/streamableHttp.js'));
(async () => {
  const client = new Client({ name: 'fyp-greybox-development', version: '1.0' });
  try {
    await client.connect(new StreamableHTTPClientTransport(new URL('http://127.0.0.1:8080/mcp')), { timeout: 15000 });
    const [mode, name, file] = process.argv.slice(2);
    let result;
    if (mode === 'resource') result = await client.readResource({ uri: name });
    else if (mode === 'schema') result = (await client.listTools()).tools.find(t => t.name === name);
    else if (mode === 'tool') result = await client.callTool({ name, arguments: file ? JSON.parse(fs.readFileSync(file, 'utf8').replace(/^\uFEFF/, '')) : {} }, undefined, { timeout: 120000 });
    else throw new Error('Expected resource, schema or tool.');
    const output = result?.structuredContent ?? result;
    if (process.env.FYP_MCP_RECEIPT) fs.writeFileSync(process.env.FYP_MCP_RECEIPT, JSON.stringify(output, null, 2));
    console.log(JSON.stringify(output));
  } finally { await client.close(); }
})().catch(error => { console.error(error.message); process.exitCode = 1; });
