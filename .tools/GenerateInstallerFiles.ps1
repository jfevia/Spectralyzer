param (
    [string]$Configuration = "Release",
    [string]$OutputDir = "bin\Release"
)

function Get-ShortHash {
    param(
        [string]$OriginalValue
    )
    $hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash([System.Text.Encoding]::UTF8.GetBytes($OriginalValue))
    $hashArray = [BitConverter]::ToString($hash)[0..32]
    return $hashArray -join ''
}

function Get-Name {
    param(
        [string]$OriginalName
    )
    $Name = $OriginalName -replace '[\\\/\.\:\-]', ""
    $Name = Get-ShortHash -OriginalValue $Name
    $Name = $Name -replace '-', ""
    $Name = $Name -replace 'Debug', ""
    $Name = $Name -replace 'Release', ""
    return $Name
}

function Get-RelativePath {
    param (
        [string]$FromPath,
        [string]$ToPath
    )
    $fromUri = [uri](Resolve-Path $FromPath).Path
    $toUri = [uri](Resolve-Path $ToPath).Path
    $relativeUri = $fromUri.MakeRelativeUri($toUri)
    return [uri]::UnescapeDataString($relativeUri.ToString())
}

function Get-FilesByDirectory {
    param ([string]$DirectoryPath)
    $directoriesToScan = @($DirectoryPath)
    $filesByDirectory = @()

    while ($directoriesToScan.Count -gt 0) {
        $currentDirectory = $directoriesToScan | Select-Object -First 1
        $directoriesToScan = $directoriesToScan | Where-Object { $_ -ne $currentDirectory }

        if (-not (Test-Path $currentDirectory -PathType Container)) { continue }

        $relativePath = Get-RelativePath -FromPath $DirectoryPath -ToPath $currentDirectory
        $files = Get-ChildItem -Path $currentDirectory -File | ForEach-Object { $_.FullName }
        $filesByDirectory += [PSCustomObject]@{
            RelativePath = $relativePath
            Files        = $files
        }

        $subdirectories = Get-ChildItem -Path $currentDirectory -Directory | ForEach-Object { $_.FullName }
        if ($null -eq $directoriesToScan) {
            $directoriesToScan = @()
        }

        $directoriesToScan += @($subdirectories)
    }

    return $filesByDirectory
}

function Generate-Components {
    param (
        [string]$OutputDir
    )

    $absoluteDirectory = Resolve-Path $OutputDir
    if (-not (Test-Path $absoluteDirectory -PathType Container)) {
        throw "Could not find absolute directory '$absoluteDirectory'"
    }

    $OutputFile = "Components.wxs"
    $FileById = @{"Spectralyzer.App.Host.exe" = "AppExecutable" }
    $filesByDirectory = Get-FilesByDirectory -DirectoryPath $absoluteDirectory

    # Generate WiX fragment
    $wixContent = "<Wix xmlns=""http://wixtoolset.org/schemas/v4/wxs"">
    <Fragment>"

    foreach ($kvp in $filesByDirectory | Where-Object { $_.RelativePath -ne "" }) {
        $path = $kvp.RelativePath
        $name = Get-Name -OriginalName $path
        $wixContent += "
        <ComponentGroup Id=""ComponentGroup_${name}"" Directory=""Folder_${name}"">"

        foreach ($file in $kvp.Files) {
            $path = $file# -replace $absoluteDirectory, $RelativePath
            $fileName = Split-Path -Path $file -Leaf
            $fileGuid = [guid]::NewGuid()
            
            if ($null -ne $FileById[$fileName]) {
                $fileId = $FileById[$fileName]
            }
            else {
                $fileId = "FileId_$($fileGuid.Guid.Replace('-', ''))"
            }

            $wixContent += "
            <Component Bitness=""always32"" Guid=""$fileGuid"">
                <File Id=""$fileId""
                      Name=""$fileName""
                      Source=""$path"" />
                <RegistryValue Root=""HKCU""
                               Key=""Software\Spectralyzer\Spectralyzer\Components""
                               Name=""$fileId""
                               Type=""integer""
                               Value=""1""
                               KeyPath=""yes"" />
            </Component>"
        }
        $wixContent += "
        </ComponentGroup>"
    }

    $wixContent += "
    </Fragment>
</Wix>"

    # Output to file
    Set-Content -Path $OutputFile -Value $wixContent
    Write-Host "File generated: $OutputFile"
}

function Generate-Folders {
    param (
        [string]$OutputDir
    )

    $absoluteDirectory = Resolve-Path $OutputDir
    if (-not (Test-Path $absoluteDirectory -PathType Container)) {
        throw "Could not find absolute directory '$absoluteDirectory'"
    }

    $OutputFile = "Folders.wxs"

    # Initialize directory stack
    $directoriesToScan = @($absoluteDirectory)

    # Start WiX content
    $wixContent = "<Wix xmlns=""http://wixtoolset.org/schemas/v4/wxs"">
    <Fragment>
        <StandardDirectory Id=""LocalAppDataFolder"">
            <Directory Id=""ManufacturerFolder"" Name=""Spectralyzer"">
                <Directory Id=""ProductFolder"" Name=""Spectralyzer"">"

    while ($directoriesToScan.Count -gt 0) {
        $currentDirectory = $directoriesToScan | Select-Object -First 1
        $directoriesToScan = $directoriesToScan | Where-Object { $_ -ne $currentDirectory }

        if (-not (Test-Path $currentDirectory -PathType Container)) { continue }

        $path = Get-RelativePath -FromPath $absoluteDirectory -ToPath $currentDirectory
        $directoryName = Split-Path -Path $currentDirectory -Leaf
        $name = Get-Name -OriginalName $path

        if (-not [string]::IsNullOrEmpty($name)) {
            $wixContent += "
                    <Directory Id=""Folder_${name}"" Name=""$directoryName"">"
        }

        $subdirectories = Get-ChildItem -Path $currentDirectory -Directory | ForEach-Object { $_.FullName }

        if ($subdirectories.Count -eq 0) {
            $wixContent += "
                    </Directory>"
        }

        if ($null -eq $directoriesToScan) {
            $directoriesToScan = @()
        }

        $directoriesToScan += @($subdirectories)
    }

    $wixContent += "
                </Directory>
            </Directory>
        </StandardDirectory>
    </Fragment>
</Wix>"

    # Write output to file
    Set-Content -Path $OutputFile -Value $wixContent
    Write-Host "File generated: $OutputFile"
}

function Generate-Removals {
    param (
        [string]$OutputDir
    )

    $absoluteDirectory = Resolve-Path $OutputDir
    if (-not (Test-Path $absoluteDirectory -PathType Container)) {
        throw "Could not find absolute directory '$absoluteDirectory'"
    }

    $OutputFile = "Remove.wxs"

    # Initialize directory stack
    $directoriesToScan = @($absoluteDirectory)

    # Start WiX content
    $wixContent = "<Wix xmlns=""http://wixtoolset.org/schemas/v4/wxs"">
    <Fragment>
        <StandardDirectory Id=""LocalAppDataFolder"">
            <Component Id=""RemoveComponent"" Guid=""e377d524-71e2-4a09-a3b2-ef4fc08dc323"">
                <RegistryValue Root=""HKCU""
                               Key=""Software\Spectralyzer\Spectralyzer""
                               Name=""Removed""
                               Type=""integer""
                               Value=""1""
                               KeyPath=""yes"" />
                <RemoveRegistryKey Root=""HKCU""
                                   Key=""Software\Spectralyzer\Spectralyzer""
                                   Action=""removeOnUninstall"" />
                <RemoveFile Id=""RemoveAllFiles"" Name=""*"" On=""uninstall"" />
                <RemoveFolder Id=""RemoveManufacturerFolder"" Directory=""ManufacturerFolder"" On=""uninstall"" />
                <RemoveFolder Id=""RemoveProductFolder"" Directory=""ProductFolder"" On=""uninstall"" />"

    while ($directoriesToScan.Count -gt 0) {
        $currentDirectory = $directoriesToScan | Select-Object -First 1
        $directoriesToScan = $directoriesToScan | Where-Object { $_ -ne $currentDirectory }

        if (-not (Test-Path $currentDirectory -PathType Container)) { continue }

        $path = Get-RelativePath -FromPath $absoluteDirectory -ToPath $currentDirectory
        $name = Get-Name -OriginalName $path

        if (-not [string]::IsNullOrEmpty($name)) {
            $wixContent += "
                <RemoveFolder Id=""Remove${name}Folder"" Directory=""Folder_${name}"" On=""uninstall"" />"
        }

        $subdirectories = Get-ChildItem -Path $currentDirectory -Directory | ForEach-Object { $_.FullName }

        if ($null -eq $directoriesToScan) {
            $directoriesToScan = @()
        }

        $directoriesToScan += @($subdirectories)
    }

    $wixContent += "
            </Component>
        </StandardDirectory>
    </Fragment>
</Wix>"

    # Write output to file
    Set-Content -Path $OutputFile -Value $wixContent
    Write-Host "File generated: $OutputFile"
}

function Generate-Package {
    param (
        [string]$OutputDir,
        [string]$UpgradeCode = "aebb7914-3502-40db-9d67-e06b9ba8570b",
        [string]$Version = "1.0.0.0"
    )

    $absoluteDirectory = Resolve-Path $OutputDir
    if (-not (Test-Path $absoluteDirectory -PathType Container)) {
        throw "Could not find absolute directory '$absoluteDirectory'"
    }

    $OutputFile = "Package.wxs"
    $ProductName = "!(loc.Product)"
    $Manufacturer = "!(loc.Manufacturer)"

    # Initialize directory stack
    $directoriesToScan = @($absoluteDirectory)

    # Start WiX content
    $wixContent = "<Wix xmlns=""http://wixtoolset.org/schemas/v4/wxs"">
    <Package Name=""$ProductName""
             Manufacturer=""$Manufacturer""
             Version=""$Version""
             Scope=""perUser""
             UpgradeCode=""$UpgradeCode"">
        <MajorUpgrade DowngradeErrorMessage=""!(loc.DowngradeError)"" />
        <Feature Id=""Main"">
            <ComponentRef Id=""RemoveComponent"" />
            <ComponentRef Id=""ShortcutsComponent"" />"

    while ($directoriesToScan.Count -gt 0) {
        $currentDirectory = $directoriesToScan | Select-Object -First 1
        $directoriesToScan = $directoriesToScan | Where-Object { $_ -ne $currentDirectory }

        if (-not (Test-Path $currentDirectory -PathType Container)) { continue }

        $path = Get-RelativePath -FromPath $absoluteDirectory -ToPath $currentDirectory
        $name = Get-Name -OriginalName $path

        if (-not [string]::IsNullOrEmpty($name)) {
            $wixContent += "
            <ComponentGroupRef Id=""ComponentGroup_${name}"" />"
        }

        $subdirectories = Get-ChildItem -Path $currentDirectory -Directory | ForEach-Object { $_.FullName }

        if ($null -eq $directoriesToScan) {
            $directoriesToScan = @()
        }

        $directoriesToScan += @($subdirectories)
    }

    $wixContent += "
        </Feature>
        <Media Id=""1"" CompressionLevel=""high"" EmbedCab=""yes"" Cabinet=""media.cab"" />
    </Package>
</Wix>
"

    # Write output to file
    Set-Content -Path $OutputFile -Value $wixContent
    Write-Host "File generated: $OutputFile"
}

Generate-Components -OutputDir $OutputDir
Generate-Folders -OutputDir $OutputDir
Generate-Removals -OutputDir $OutputDir
Generate-Package -OutputDir $OutputDir