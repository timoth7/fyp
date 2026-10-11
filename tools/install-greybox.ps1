param([string]$Project = 'C:\Users\Administrator\My project (2)')
$ErrorActionPreference = 'Stop'
$source = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\unity\Assets\FypGreybox'))
$projectRoot = [IO.Path]::GetFullPath($Project)
if (!(Test-Path -LiteralPath (Join-Path $projectRoot 'ProjectSettings\ProjectVersion.txt'))) { throw 'Not a Unity project.' }
$destination = [IO.Path]::GetFullPath((Join-Path $projectRoot 'Assets\FypGreybox'))
if (!$destination.StartsWith($projectRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) { throw 'Destination escaped Unity project.' }
# Preserve any previous source on updates; only the dedicated FypGreybox subtree is touched.
if (Test-Path -LiteralPath $destination) {
    $backup = Join-Path $projectRoot ('FypGreyboxBackups\' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    Copy-Item -LiteralPath $destination -Destination $backup -Recurse
}
New-Item -ItemType Directory -Path $destination -Force | Out-Null
foreach ($item in Get-ChildItem -LiteralPath $source -Force) {
    # The scene and materials can contain the creator's new designs. Never replace existing copies.
    if ($item.Name -in @('Scenes','Scenes.meta','Materials','Materials.meta') -and (Test-Path -LiteralPath (Join-Path $destination $item.Name))) { continue }
    Copy-Item -LiteralPath $item.FullName -Destination $destination -Recurse -Force
}
Write-Output ('Installed source to ' + $destination)
