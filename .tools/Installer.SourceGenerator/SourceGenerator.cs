// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Spectralyzer.Installer.SourceGenerator;

public static partial class SourceGenerator
{
    public static void Generate(string outputDirectory, string manufacturerName, string productName, string version)
    {
        GenerateComponents(outputDirectory, manufacturerName, productName, version);
        GenerateFolders(outputDirectory, manufacturerName, productName);
        GenerateRemovals(outputDirectory, manufacturerName, productName);
        GenerateShortcuts(outputDirectory, manufacturerName, productName);
        GeneratePackage(outputDirectory, manufacturerName, productName, version);
    }

    private static void GenerateComponentNodes(string outputDirectory, string directory, string manufacturerName, string productName, StringBuilder stringBuilder, int level)
    {
        var relativePath = GetRelativePath(outputDirectory, directory);
        var name = GetName(relativePath);
        var componentGroupIndentation = new string(' ', level * 4);

        stringBuilder.AppendLine($"{componentGroupIndentation}<ComponentGroup Id=\"ComponentGroup_{name}\" Directory=\"Folder_{name}\">");

        foreach (var file in Directory.GetFiles(directory))
        {
            var fileName = Path.GetFileName(file);
            var fileGuid = Guid.NewGuid().ToString("D");
            var fileId = fileGuid.Replace("-", string.Empty).ToUpperInvariant();
            var componentIndentation = new string(' ', (level + 1) * 4);
            var objectIndentation = new string(' ', (level + 2) * 4);

            stringBuilder.AppendLine($"{componentIndentation}<Component Bitness=\"always32\" Guid=\"{fileGuid}\">");
            stringBuilder.AppendLine($"{objectIndentation}<File Id=\"File_{fileId}\" Name=\"{fileName}\" Source=\"{file}\" />");
            stringBuilder.AppendLine($"{objectIndentation}<RegistryValue Root=\"HKCU\" Key=\"Software\\{manufacturerName}\\{productName}\\Components\" Name=\"File_{fileId}\" Type=\"string\" Value=\"Installed\" KeyPath=\"yes\" />");

            if (fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                stringBuilder.AppendLine($"{objectIndentation}<RegistryValue Root=\"HKCU\" Key=\"Software\\{manufacturerName}\\{productName}\\Components\" Name=\"Shortcut_{fileId}\" Type=\"string\" Value=\"Installed\" />");
                stringBuilder.AppendLine($"{objectIndentation}<Shortcut Id=\"DesktopShortcut_{fileId}\" Directory=\"DesktopFolder\" Name=\"{fileNameWithoutExtension}\" Target=\"[Folder_{name}]{fileName}\" WorkingDirectory=\"Folder_{name}\" />");
                stringBuilder.AppendLine($"{objectIndentation}<Shortcut Id=\"StartMenuShortcut_{fileId}\" Directory=\"ProgramMenuFolder\" Name=\"{fileNameWithoutExtension}\" Target=\"[Folder_{name}]{fileName}\" WorkingDirectory=\"Folder_{name}\" />");
            }

            stringBuilder.AppendLine($"{componentIndentation}</Component>");
        }

        stringBuilder.AppendLine($"{componentGroupIndentation}</ComponentGroup>");
    }

    private static void GenerateComponentRefNodes(string outputDirectory, string directory, StringBuilder stringBuilder, int level)
    {
        var relativePath = GetRelativePath(outputDirectory, directory);
        var name = GetName(relativePath);
        var indentation = new string(' ', level * 4);

        stringBuilder.AppendLine($"{indentation}<ComponentGroupRef Id=\"ComponentGroup_{name}\" />");
    }

    private static void GenerateComponents(string outputDirectory, string manufacturerName, string productName, string version)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <Component Id=\"InstallationFolderComponent\" Directory=\"ProductFolder\">");
        stringBuilder.AppendLine($"            <RegistryValue Root=\"HKCU\" Key=\"Software\\{manufacturerName}\\{productName}\" Name=\"InstallationFolder\" Type=\"string\" Value=\"[ProductFolder]\" KeyPath=\"yes\" />");
        stringBuilder.AppendLine("        </Component>");
        stringBuilder.AppendLine("        <Component Id=\"ProductVersionComponent\" Directory=\"ProductFolder\">");
        stringBuilder.AppendLine($"            <RegistryValue Root=\"HKCU\" Key=\"Software\\{manufacturerName}\\{productName}\" Name=\"ProductVersion\" Type=\"string\" Value=\"{version}\" KeyPath=\"yes\" />");
        stringBuilder.AppendLine("        </Component>");

        foreach (var directory in Directory.GetDirectories(outputDirectory, "*", SearchOption.AllDirectories))
        {
            GenerateComponentNodes(outputDirectory, directory, manufacturerName, productName, stringBuilder, 2);
        }

        stringBuilder.AppendLine("    </Fragment>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Components.wxs", stringBuilder.ToString());
    }

    private static void GenerateDirectoryNodes(string outputDirectory, string directoryPath, StringBuilder stringBuilder, int level)
    {
        var relativePath = GetRelativePath(outputDirectory, directoryPath);
        var directoryName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));
        var name = GetName(relativePath);
        var indentation = new string(' ', level * 4);

        stringBuilder.AppendLine($"{indentation}<Directory Id=\"Folder_{name}\" Name=\"{directoryName}\">");

        foreach (var subDirectory in Directory.GetDirectories(directoryPath))
        {
            GenerateDirectoryNodes(outputDirectory, subDirectory, stringBuilder, level + 1);
        }

        stringBuilder.AppendLine($"{indentation}</Directory>");
    }

    private static void GenerateFolders(string outputDirectory, string manufacturerName, string productName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"DesktopFolder\" />");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"ProgramMenuFolder\" />");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine($"            <Directory Id=\"ManufacturerFolder\" Name=\"{manufacturerName}\">");
        stringBuilder.AppendLine($"                <Directory Id=\"ProductFolder\" Name=\"{productName}\">\"");

        foreach (var subDirectory in Directory.GetDirectories(outputDirectory))
        {
            GenerateDirectoryNodes(outputDirectory, subDirectory, stringBuilder, 5);
        }

        stringBuilder.AppendLine("                </Directory>");
        stringBuilder.AppendLine("            </Directory>");
        stringBuilder.AppendLine("        </StandardDirectory>");
        stringBuilder.AppendLine("    </Fragment>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Folders.wxs", stringBuilder.ToString());
    }

    private static void GeneratePackage(string outputDirectory, string manufacturerName, string productName, string version)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine($"    <Package Name=\"{productName}\"");
        stringBuilder.AppendLine($"             Manufacturer=\"{manufacturerName}\"");
        stringBuilder.AppendLine($"             Version=\"{version}\"");
        stringBuilder.AppendLine("             Scope=\"perUser\"");
        stringBuilder.AppendLine("             UpgradeCode=\"aebb7914-3502-40db-9d67-e06b9ba8570b\">");
        stringBuilder.AppendLine("        <MajorUpgrade DowngradeErrorMessage=\"!(loc.DowngradeError)\" />");
        stringBuilder.AppendLine("        <Media Id=\"1\" CompressionLevel=\"high\" EmbedCab=\"yes\" Cabinet=\"media.cab\" />");
        stringBuilder.AppendLine("        <Feature Id=\"Main\">");
        stringBuilder.AppendLine("            <ComponentRef Id=\"RemoveComponent\" />");
        stringBuilder.AppendLine("            <ComponentRef Id=\"InstallationFolderComponent\" />");
        stringBuilder.AppendLine("            <ComponentRef Id=\"ManufacturerNameComponent\" />");
        stringBuilder.AppendLine("            <ComponentRef Id=\"ProductNameComponent\" />");
        stringBuilder.AppendLine("            <ComponentRef Id=\"ProductVersionComponent\" />");

        foreach (var directory in Directory.GetDirectories(outputDirectory, "*", SearchOption.AllDirectories))
        {
            GenerateComponentRefNodes(outputDirectory, directory, stringBuilder, 3);
        }

        stringBuilder.AppendLine("        </Feature>");
        stringBuilder.AppendLine("    </Package>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Package.wxs", stringBuilder.ToString());
    }

    private static void GenerateRemovals(string outputDirectory, string manufacturerName, string productName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine("            <Component Id=\"RemoveComponent\" Guid=\"e377d524-71e2-4a09-a3b2-ef4fc08dc323\">");
        stringBuilder.AppendLine("                <RegistryValue Root=\"HKCU\"");
        stringBuilder.AppendLine($"                               Key=\"Software\\{manufacturerName}\\{productName}\"");
        stringBuilder.AppendLine("                               Name=\"State\"");
        stringBuilder.AppendLine("                               Type=\"string\"");
        stringBuilder.AppendLine("                               Value=\"Installed\"");
        stringBuilder.AppendLine("                               KeyPath=\"yes\" />");
        stringBuilder.AppendLine("                <RemoveRegistryKey Root=\"HKCU\"");
        stringBuilder.AppendLine($"                                   Key=\"Software\\{manufacturerName}\\{productName}\"");
        stringBuilder.AppendLine("                                   Action=\"removeOnUninstall\" />");
        stringBuilder.AppendLine("                <RemoveFile Id=\"RemoveAllFiles\" Name=\"*\" On=\"uninstall\" />");
        stringBuilder.AppendLine("                <RemoveFolder Id=\"RemoveManufacturerFolder\" Directory=\"ManufacturerFolder\" On=\"uninstall\" />");
        stringBuilder.AppendLine("                <RemoveFolder Id=\"RemoveProductFolder\" Directory=\"ProductFolder\" On=\"uninstall\" />");

        foreach (var subDirectory in Directory.GetDirectories(outputDirectory))
        {
            GenerateRemoveNodes(outputDirectory, subDirectory, stringBuilder);
        }

        stringBuilder.AppendLine("            </Component>");
        stringBuilder.AppendLine("        </StandardDirectory>");
        stringBuilder.AppendLine("    </Fragment>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Remove.wxs", stringBuilder.ToString());
    }

    private static void GenerateRemoveNodes(string outputDirectory, string directoryPath, StringBuilder stringBuilder)
    {
        var relativePath = GetRelativePath(outputDirectory, directoryPath);
        var name = GetName(relativePath);

        stringBuilder.AppendLine($"                <RemoveFolder Id=\"Remove{name}Folder\" Directory=\"Folder_{name}\" On=\"uninstall\" />");

        foreach (var subDirectory in Directory.GetDirectories(directoryPath))
        {
            GenerateRemoveNodes(outputDirectory, subDirectory, stringBuilder);
        }
    }

    private static void GenerateShortcutNodes(string outputDirectory, string directory, string manufacturerName, string productName, StringBuilder stringBuilder, int level)
    {
        var files = Directory.GetFiles(directory).Where(file => file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)).ToList();
        if (files.Count <= 0)
        {
            return;
        }

        var relativePath = GetRelativePath(outputDirectory, directory);
        var name = GetName(relativePath);
        var directoryIndentation = new string(' ', level * 4);
        var componentGroupIndentation = new string(' ', (level + 1) * 4);
        var componentIndentation = new string(' ', (level + 2) * 4);
        var shortcutAndRegistryIndentation = new string(' ', (level + 3) * 4);

        stringBuilder.AppendLine($"{directoryIndentation}<DirectoryRef Id=\"Folder_{name}\">");
        stringBuilder.AppendLine($"{componentGroupIndentation}<ComponentGroup Id=\"ComponentGroup_{name}\" Bitness=\"always32\">");

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            var fileGuid = Guid.NewGuid().ToString("D");
            var fileId = fileGuid.Replace("-", string.Empty).ToUpperInvariant();

            stringBuilder.AppendLine($"{componentIndentation}<Component Id=\"Shortcut_{fileId}\" Bitness=\"always32\">");
            stringBuilder.AppendLine($"{shortcutAndRegistryIndentation}<RegistryValue Root=\"HKCU\" Key=\"Software\\{manufacturerName}\\{productName}\\Components\" Name=\"Shortcut_{fileId}\" Type=\"string\" Value=\"[File_{fileId}]\" KeyPath=\"yes\" />");
            stringBuilder.AppendLine($"{shortcutAndRegistryIndentation}<Shortcut Id=\"DesktopShortcut_{fileId}\" Directory=\"DesktopFolder\" Name=\"{fileNameWithoutExtension}\" Target=\"[Folder_{name}]{fileName}\" WorkingDirectory=\"Folder_{name}\" />");
            stringBuilder.AppendLine($"{shortcutAndRegistryIndentation}<Shortcut Id=\"StartMenuShortcut_{fileId}\" Directory=\"ProgramMenuFolder\" Name=\"{fileNameWithoutExtension}\" Target=\"[Folder_{name}]{fileName}\" WorkingDirectory=\"Folder_{name}\" />");
            stringBuilder.AppendLine($"{componentIndentation}</Component>");
        }

        stringBuilder.AppendLine($"{componentGroupIndentation}</ComponentGroup>");
        stringBuilder.AppendLine($"{directoryIndentation}</DirectoryRef>");
    }

    private static void GenerateShortcuts(string outputDirectory, string manufacturerName, string productName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"DesktopFolder\" />");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"ProgramMenuFolder\" />");

        foreach (var directory in Directory.GetDirectories(outputDirectory, "*", SearchOption.AllDirectories))
        {
            GenerateShortcutNodes(outputDirectory, directory, manufacturerName, productName, stringBuilder, 3);
        }

        stringBuilder.AppendLine("    </Fragment>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Shortcuts.wxs", stringBuilder.ToString());
    }

    private static string GetName(string originalName)
    {
        var sanitizedName = InvalidCharacters().Replace(originalName, string.Empty);
        var hashedName = GetShortHash(sanitizedName);
        hashedName = hashedName.Replace("-", string.Empty)
                               .Replace("Debug", string.Empty)
                               .Replace("Release", string.Empty);

        return hashedName;
    }

    private static string GetRelativePath(string fromPath, string toPath)
    {
        // Resolve the absolute paths
        var absoluteFromPath = Path.GetFullPath(fromPath);
        var absoluteToPath = Path.GetFullPath(toPath);

        // Convert to Uri
        var fromUri = new Uri(absoluteFromPath);
        var toUri = new Uri(absoluteToPath);

        // Calculate the relative Uri
        var relativeUri = fromUri.MakeRelativeUri(toUri);

        // Return the unescaped relative path
        return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
    }

    private static string GetShortHash(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var hashString = Convert.ToHexString(hash);
        return hashString[..32];
    }

    [GeneratedRegex(@"[\\\/\.\:\-]")]
    private static partial Regex InvalidCharacters();
}