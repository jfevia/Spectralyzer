# Paths
Set-Location ".\Assets\"
$ModulesPath = "node_modules"
$CacheFile = "node_modules\build.cache"

if (-Not (Test-Path $ModulesPath)) {
    Write-Output "Installing npm modules..."
    npm install
}

if (-Not (Test-Path $CacheFile)) {
    Write-Output "Building npm modules..."
    npm run build
    New-Item $CacheFile -ItemType File | Out-Null
}
