$SongsPath = "C:\Users\<username>\AppData\Local\osu!\Songs"
$ImporterPath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\osu-stable-to-lazer.exe"

$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = $SongsPath
$watcher.Filter = "*"
$watcher.IncludeSubdirectories = $false
$watcher.NotifyFilter = [System.IO.NotifyFilters]::DirectoryName
$watcher.EnableRaisingEvents = $true

$sourceIdentifier = "OsuBeatmapCreated"

Register-ObjectEvent `
    -InputObject $watcher `
    -EventName Created `
    -SourceIdentifier $sourceIdentifier | Out-Null

Write-Host "Watching: $SongsPath"
Write-Host "Importer: $ImporterPath"
Write-Host "Press Ctrl+C to stop."

try {
    while ($true) {
        $event = Wait-Event -SourceIdentifier $sourceIdentifier
        $path = $event.SourceEventArgs.FullPath

        Remove-Event -EventIdentifier $event.EventIdentifier

        Write-Host "New beatmap detected: $path"

        # stableの書き込み完了待ち
        Start-Sleep -Seconds 5

        Write-Host "Starting importer..."

        & $ImporterPath $path

        Write-Host "Importer finished. Exit code: $LASTEXITCODE"
    }
}
finally {
    Unregister-Event -SourceIdentifier $sourceIdentifier -ErrorAction SilentlyContinue
    $watcher.Dispose()
}