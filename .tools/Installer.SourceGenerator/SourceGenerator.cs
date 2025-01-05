// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Spectralyzer.Installer.SourceGenerator;

public static partial class SourceGenerator
{
    public static void Generate(string outputDirectory)
    {
        GenerateComponents(outputDirectory);
        GenerateFolders(outputDirectory);
        GenerateRemovals(outputDirectory);
        GeneratePackage(outputDirectory);
    }

    private static void GenerateComponentNodes(string outputDirectory, string directory, StringBuilder stringBuilder, int level)
    {
        var relativePath = GetRelativePath(outputDirectory, directory);
        var name = GetName(relativePath);
        var componentGroupIndentation = new string(' ', level * 4);

        stringBuilder.AppendLine($"{componentGroupIndentation}<ComponentGroup Id=\"ComponentGroup_{name}\" Directory=\"Folder_{name}\">");

        foreach (var file in Directory.GetFiles(directory))
        {
            var fileName = Path.GetFileName(file);
            var fileGuid = Guid.NewGuid().ToString("D");
            var fileId = fileGuid.Replace("-", string.Empty);
            var componentIndentation = new string(' ', (level + 1) * 4);
            var fileAndRegistryIndentation = new string(' ', (level + 2) * 4);

            stringBuilder.AppendLine($"{componentIndentation}<Component Bitness=\"always32\" Guid=\"{fileGuid}\">");
            stringBuilder.AppendLine($"{fileAndRegistryIndentation}<File Id=\"File_{fileId}\" Name=\"{fileName}\" Source=\"{file}\" />");
            stringBuilder.AppendLine($"{fileAndRegistryIndentation}<RegistryValue Root=\"HKCU\" Key=\"Software\\Spectralyzer\\Spectralyzer\\Components\" Name=\"File_{fileId}\" Type=\"string\" Value=\"Installed\" KeyPath=\"yes\" />");
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

    private static void GenerateComponents(string outputDirectory)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");

        foreach (var directory in Directory.GetDirectories(outputDirectory, "*", SearchOption.AllDirectories))
        {
            GenerateComponentNodes(outputDirectory, directory, stringBuilder, 2);
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

    private static void GenerateFolders(string outputDirectory)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine("            <Directory Id=\"ManufacturerFolder\" Name=\"Spectralyzer\">");
        stringBuilder.AppendLine("                <Directory Id=\"ProductFolder\" Name=\"Spectralyzer\">\"");

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

    private static void GeneratePackage(string outputDirectory)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Package Name=\"!(loc.Product)\"");
        stringBuilder.AppendLine("             Manufacturer=\"!(loc.Manufacturer)\"");
        stringBuilder.AppendLine("             Version=\"1.0.0.0\"");
        stringBuilder.AppendLine("             Scope=\"perUser\"");
        stringBuilder.AppendLine("             UpgradeCode=\"aebb7914-3502-40db-9d67-e06b9ba8570b\">");
        stringBuilder.AppendLine("        <MajorUpgrade DowngradeErrorMessage=\"!(loc.DowngradeError)\" />");
        stringBuilder.AppendLine("        <Media Id=\"1\" CompressionLevel=\"high\" EmbedCab=\"yes\" Cabinet=\"media.cab\" />");
        stringBuilder.AppendLine("        <Feature Id=\"Main\">");
        stringBuilder.AppendLine("            <ComponentRef Id=\"RemoveComponent\" />");
        stringBuilder.AppendLine("            <ComponentRef Id=\"ShortcutsComponent\" />");

        foreach (var directory in Directory.GetDirectories(outputDirectory, "*", SearchOption.AllDirectories))
        {
            GenerateComponentRefNodes(outputDirectory, directory, stringBuilder, 4);
        }

        stringBuilder.AppendLine("        </Feature>");
        stringBuilder.AppendLine("    </Package>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Package.wxs", stringBuilder.ToString());
    }

    private static void GenerateRemovals(string outputDirectory)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("    <Fragment>");
        stringBuilder.AppendLine("        <StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine("            <Component Id=\"RemoveComponent\" Guid=\"e377d524-71e2-4a09-a3b2-ef4fc08dc323\">");
        stringBuilder.AppendLine("                <RegistryValue Root=\"HKCU\"");
        stringBuilder.AppendLine("                               Key=\"Software\\Spectralyzer\\Spectralyzer\"");
        stringBuilder.AppendLine("                               Name=\"State\"");
        stringBuilder.AppendLine("                               Type=\"string\"");
        stringBuilder.AppendLine("                               Value=\"Installed\"");
        stringBuilder.AppendLine("                               KeyPath=\"yes\" />");
        stringBuilder.AppendLine("                <RemoveRegistryKey Root=\"HKCU\"");
        stringBuilder.AppendLine("                                   Key=\"Software\\Spectralyzer\\Spectralyzer\"");
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