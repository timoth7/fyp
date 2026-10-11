param([ValidateSet('mcp', 'login')][string]$Mode = 'mcp')
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$taskCli = Join-Path $taskRoot '.local\lark-mcp\node_modules\@larksuiteoapi\lark-mcp\dist\cli.js'
$taskCredentialPath = Join-Path $env:LOCALAPPDATA 'Codex\feishu-mcp\credentials.xml'
if (-not (Test-Path -LiteralPath $taskCredentialPath)) {
    throw 'Feishu credentials are not configured. Run tools\feishu-mcp\setup.cmd first.'
}
$taskCredential = Import-Clixml -LiteralPath $taskCredentialPath
$taskOldId = $env:APP_ID
$taskOldSecret = $env:APP_SECRET
try {
    $env:APP_ID = $taskCredential.UserName
    $env:APP_SECRET = $taskCredential.GetNetworkCredential().Password
    if ($Mode -eq 'login') {
        & 'C:\nvm4w\nodejs\node.exe' $taskCli login --domain https://open.feishu.cn --scope 'offline_access docx:document wiki:wiki:readonly'
    } else {
        $taskTools = 'wiki.v2.space.getNode,docx.v1.document.get,docx.v1.documentBlock.list,docx.v1.documentBlockChildren.create,docx.v1.documentBlock.batchUpdate'
        & 'C:\nvm4w\nodejs\node.exe' $taskCli mcp --domain https://open.feishu.cn --token-mode tenant_access_token --language zh --tools $taskTools
    }
    $taskExit = $LASTEXITCODE
} finally {
    $env:APP_ID = $taskOldId
    $env:APP_SECRET = $taskOldSecret
    $taskCredential = $null
}
exit $taskExit
