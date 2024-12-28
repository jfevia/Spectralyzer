param (
    [string]$Configuration
)

$baseFileNames = @(
    "Components",
    "Folders",
    "Package"
)

foreach ($baseFileName in $baseFileNames) {
    $fileName = $baseFileName

    if ($Configuration -eq "Debug") {
        $fileName += ".Development"
    }

    $filePath = "..\..\src\Installer\$fileName.tt"
    Write-Host "Processing file: $filePath"
    t4 $filePath
}
