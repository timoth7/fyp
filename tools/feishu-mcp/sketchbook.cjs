const fs = require('fs');
const path = require('path');
const root = path.resolve(__dirname, '../..');
const sdk = path.join(root, '.local/lark-mcp/node_modules/@modelcontextprotocol/sdk/dist/cjs');
const { Client } = require(path.join(sdk, 'client/index.js'));
const { StdioClientTransport } = require(path.join(sdk, 'client/stdio.js'));
async function run() {
  const client = new Client({name:'fyp-sketchbook-verification',version:'1.0.0'});
  const transport = new StdioClientTransport({
    command:'C:/Windows/System32/WindowsPowerShell/v1.0/powershell.exe',
    args:['-NoLogo','-NoProfile','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'start.ps1')],
    stderr:'pipe',
    env: { ...process.env }
  });
  try {
    await client.connect(transport);
    const { tools } = await client.listTools();
    console.log(JSON.stringify({step:'MCP_initialize_and_tools_list',tools:tools.map(t=>t.name)}));
    async function call(suffix,args) {
      const tool = tools.find(t=>t.name.endsWith(suffix));
      if (!tool) throw new Error('Missing tool: '+suffix);
      const response = await client.callTool({name:tool.name,arguments:args});
      const payload = response.content.filter(c=>c.type==='text').map(c=>c.text).join('\n');
      let data; try { data=JSON.parse(payload); } catch { throw new Error('Non-JSON tool response'); }
      if (response.isError || (data.code !== undefined && data.code !== 0)) throw new Error('Tool failed: '+suffix+'; code='+data.code);
      return data;
    }
    const wiki=await call('wiki_v2_space_getNode',{params:{token:'KVV2w0qRzinNuGkjCw8cvGNTnFb'},useUAT:false});
    const wikiData=wiki.data || wiki;
    if(!wikiData.node) throw new Error('Unexpected wiki response: '+JSON.stringify(wiki));
    const docId=wikiData.node.obj_token;
    if(wikiData.node.obj_type!=='docx') throw new Error('Canonical wiki is not a docx');
    const doc=await call('docx_v1_document_get',{path:{document_id:docId},useUAT:false});
    const blocks=[]; let next;
    do {
      const data=await call('docx_v1_documentBlock_list',{path:{document_id:docId},params:{page_size:500,...(next?{page_token:next}:{})},useUAT:false});
      const body=data.data||data;
      blocks.push(...body.items); next=body.has_more ? body.page_token : undefined;
    } while(next);
    const document=(doc.data||doc).document;
    const snapshot={document,docId,blocks};
    fs.writeFileSync(path.join(root,'.local/sketchbook-before.json'),JSON.stringify(snapshot,null,2));
    const lines=blocks.map(b=>{
      const content=b.text||b.heading1||b.heading2||b.heading3||b.heading4||b.bullet||b.ordered||b.quote;
      const text=content?.elements?.map(e=>e.text_run?.content||'').join('')||'';
      return {id:b.block_id,type:b.block_type,text};
    });
    fs.writeFileSync(path.join(root,'.local/sketchbook-before-text.json'),JSON.stringify(lines,null,2));
    console.log(JSON.stringify({step:'canonical_document_read',document,blockCount:blocks.length}));
    if (process.argv.includes('--apply')) {
      const operations=JSON.parse(fs.readFileSync(path.join(__dirname,'sketchbook-update.json'),'utf8'));
      const originalRoot=blocks.find(b=>b.block_id===docId).children;
      const receipts=[];
      for(const operation of operations) {
        if(operation.kind==='insert') {
          if(lines.some(b=>b.text===operation.marker)) throw new Error('Update marker already exists; inspect before repeating: '+operation.marker);
          const result=await call('docx_v1_documentBlockChildren_create',{path:{document_id:docId,block_id:operation.parent},data:{children:operation.children,index:operation.index},useUAT:false});
          const created=(result.data||result).children;
          receipts.push({kind:'insert',parent:operation.parent,children:created.map(b=>b.block_id),expected:operation.children});
        } else if(operation.kind==='update') {
          const old=lines.find(b=>b.id===operation.blockId);
          if(old?.text!==operation.expectedOld) throw new Error('Existing paragraph changed; update aborted');
          await call('docx_v1_documentBlock_batchUpdate',{path:{document_id:docId},data:{requests:[{block_id:operation.blockId,update_text_elements:{elements:[{text_run:{content:operation.text}}]}}]},useUAT:false});
          receipts.push({kind:'update',blockId:operation.blockId,text:operation.text});
        }
        fs.writeFileSync(path.join(root,'.local/sketchbook-update-receipts.json'),JSON.stringify(receipts,null,2));
      }
      const after=[]; next=undefined;
      do { const data=await call('docx_v1_documentBlock_list',{path:{document_id:docId},params:{page_size:500,...(next?{page_token:next}:{})},useUAT:false});const body=data.data||data;after.push(...body.items);next=body.has_more?body.page_token:undefined;}while(next);
      function content(b){const p=b?.text||b?.heading1||b?.heading2||b?.heading3;return p?.elements?.map(e=>e.text_run?.content||'').join('')||'';}
      const afterMap=new Map(after.map(b=>[b.block_id,b]));
      for(const receipt of receipts){if(receipt.kind==='update'){if(content(afterMap.get(receipt.blockId))!==receipt.text)throw new Error('Updated paragraph readback mismatch');}else{
        receipt.children.forEach((id,i)=>{if(content(afterMap.get(id))!==content(receipt.expected[i]))throw new Error('Inserted text readback mismatch');});
        const parent=afterMap.get(receipt.parent);if(!receipt.children.every(id=>parent.children.includes(id)))throw new Error('Inserted block parent mismatch');
      }}
      if(JSON.stringify(afterMap.get(docId).children)!==JSON.stringify(originalRoot))throw new Error('Original root child order changed');
      if(blocks.some(b=>!afterMap.has(b.block_id)))throw new Error('An original block is missing');
      const updated=await call('docx_v1_document_get',{path:{document_id:docId},useUAT:false});
      fs.writeFileSync(path.join(root,'.local/sketchbook-after.json'),JSON.stringify({document:(updated.data||updated).document,blocks:after},null,2));
      console.log(JSON.stringify({step:'write_and_readback_verified',revision:(updated.data||updated).document.revision_id,inserted:receipts.filter(r=>r.kind==='insert').reduce((n,r)=>n+r.children.length,0),updated:receipts.filter(r=>r.kind==='update').length,originalBlocksPreserved:true,rootOrderPreserved:true}));
      return;
    }
    console.log(JSON.stringify({headings:lines.filter(b=>b.type>=3&&b.type<=11&&b.text),tail:lines.filter(b=>b.text).slice(-18)},null,2));
  } finally {await client.close();}
}
run().catch(e=>{ console.error(e.message); process.exitCode=1; });
