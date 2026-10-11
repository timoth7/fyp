$ErrorActionPreference = 'Stop'
Write-Host 'Use the existing Feishu developer application from your Mac.'
Write-Host 'Its OAuth redirect URL must include http://localhost:3000/callback.'
$taskAppId = (Read-Host 'App ID (cli_...)').Trim()
if ($taskAppId -notmatch '^cli_[A-Za-z0-9]+$') { throw 'Invalid App ID format.' }
$taskSecret = Read-Host 'App Secret (hidden input; do not paste it in chat)' -AsSecureString
if ($taskSecret.Length -eq 0) { throw 'App Secret is required.' }
$taskCredentialDir = Join-Path $env:LOCALAPPDATA 'Codex\feishu-mcp'
$taskCredentialPath = Join-Path $taskCredentialDir 'credentials.xml'
New-Item -ItemType Directory -Path $taskCredentialDir -Force | Out-Null
$taskCredential = New-Object System.Management.Automation.PSCredential($taskAppId, $taskSecret)
$taskCredential | Export-Clixml -LiteralPath $taskCredentialPath
Write-Host 'Credentials saved locally; the secret is encrypted for this Windows user.'
$taskRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$taskOldId = $env:APP_ID
$taskOldSecret = $env:APP_SECRET
try {
    $env:APP_ID = $taskCredential.UserName
    $env:APP_SECRET = $taskCredential.GetNetworkCredential().Password
    & 'C:\nvm4w\nodejs\node.exe' (Join-Path $PSScriptRoot 'check-access.cjs')
    $taskAccessExit = $LASTEXITCODE
} finally {
    $env:APP_ID = $taskOldId
    $env:APP_SECRET = $taskOldSecret
    $taskCredential = $null
}
if ($taskAccessExit -ne 0) { throw 'Application access check failed. Check credentials and the existing document permissions.' }
$taskConfigPath = Join-Path $env:USERPROFILE '.codex\config.toml'
$taskConfig = [System.IO.File]::ReadAllText($taskConfigPath)
$taskPattern = '(?ms)(^\[mcp_servers\.feishu\]\s*\r?\n)(.*?)(?=^\[|\z)'
$taskMatch = [regex]::Match($taskConfig, $taskPattern)
if (-not $taskMatch.Success) { throw 'The prepared Codex feishu entry was not found.' }
$taskBlock = $taskMatch.Value
if ($taskBlock -match '(?m)^enabled\s*=\s*true\s*$') { Write-Host 'Feishu MCP is already enabled. Restart Codex after changing credentials.'; exit 0 }
if ($taskBlock -notmatch '(?m)^enabled\s*=\s*false\s*$') { throw 'The feishu entry changed. Check its configuration before enabling it.' }
$taskUpdatedBlock = [regex]::Replace($taskBlock, '(?m)^enabled[ \t]*=[ \t]*false[ \t]*(?=\r?$)', 'enabled = true')
$taskBackup = $taskConfigPath + '.before-feishu-login-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.bak'
Copy-Item -LiteralPath $taskConfigPath -Destination $taskBackup
$taskConfig = $taskConfig.Substring(0, $taskMatch.Index) + $taskUpdatedBlock + $taskConfig.Substring($taskMatch.Index + $taskMatch.Length)
[System.IO.File]::WriteAllText($taskConfigPath, $taskConfig, (New-Object System.Text.UTF8Encoding($false)))
Write-Host 'Feishu MCP enabled. Restart Codex, then request the Sketchbook update in this chat.'
