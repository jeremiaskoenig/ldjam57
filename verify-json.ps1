Get-ChildItem -Path "assets" -Filter *.json | ForEach-Object {
    $fileName = $_.FullName
    try {
        $content = Get-Content -Path $_.FullName -Raw
        $json = $content | ConvertFrom-Json
    } catch {
        Write-Host "$fileName is invalid JSON. Error: $($_.Exception.Message)" 
    }
}
Read-Host