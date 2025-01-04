// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Spectralyzer.WixGen;

public static partial class SourceGenerator
{
    public class TreeNode
    {
        public string Path { get; }
        public List<TreeNode> Children { get; } = [];

        public TreeNode(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    private static TreeNode GetFolders(string directory)
    {
        var stack = new Stack<TreeNode>();
        var rootTree = new TreeNode(directory);
        stack.Push(rootTree);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                var childNode = new TreeNode(subDirectory);
                currentNode.Children.Add(childNode);
                stack.Push(childNode);
            }
        }

        return rootTree;
    }

    public static void GenerateFolders(string outputDir)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("<Fragment>");
        stringBuilder.AppendLine("<StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine("<Directory Id=\"ManufacturerFolder\" Name=\"Spectralyzer\">");
        stringBuilder.AppendLine("<Directory Id=\"ProductFolder\" Name=\"Spectralyzer\">\"");
        stringBuilder.AppendLine("<Directory Id=\"AppFolder\" Name=\"App\">\"");

        var stack = new Stack<string>();
        var dictionary = new Dictionary<string, HashSet<string>>();

        stack.Push(outputDir);

        while (stack.Count > 0)
        {
            var currentDirectory = stack.Pop();

            if (dictionary.TryGetValue(currentDirectory, out var subdirectories) && !string.Equals(currentDirectory, outputDir, StringComparison.OrdinalIgnoreCase))
            {
                subdirectories.Remove(currentDirectory);

                if (subdirectories.Count == 0)
                {
                    stringBuilder.AppendLine("</Directory>");
                }
            }
            else
            {
                dictionary[currentDirectory] = subdirectories = Directory.GetDirectories(currentDirectory).ToHashSet();

                foreach (var subdirectory in subdirectories)
                {
                    stack.Push(subdirectory);
                }
            }

            var relativePath = GetRelativePath(outputDir, currentDirectory);
            var directoryName = Path.GetFileName(currentDirectory.TrimEnd(Path.DirectorySeparatorChar));
            var name = GetName(relativePath);

            if (subdirectories.Count == 0)
            {
                stringBuilder.AppendLine($"<Directory Id=\"Folder_{name}\" Name=\"{directoryName}\" />");
            }
            else
            {
                stringBuilder.AppendLine($"<Directory Id=\"Folder_{name}\" Name=\"{directoryName}\">");
            }
        }

        stringBuilder.AppendLine("</Directory>");
        stringBuilder.AppendLine("</Directory>");
        stringBuilder.AppendLine("</Directory>");
        stringBuilder.AppendLine("</StandardDirectory>");
        stringBuilder.AppendLine("</Fragment>");
        stringBuilder.AppendLine("</Wix>");

        File.WriteAllText("Folders.wxs", stringBuilder.ToString());
    }

    public static string GetRelativePath(string fromPath, string toPath)
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

    private static string GetName(string originalName)
    {
        var sanitizedName = MyRegex().Replace(originalName, string.Empty);
        var hashedName = GetShortHash(sanitizedName);
        hashedName = hashedName.Replace("-", string.Empty)
                               .Replace("Debug", string.Empty)
                               .Replace("Release", string.Empty);

        return hashedName;
    }

    private static string GetShortHash(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var hashString = Convert.ToHexString(hash);
        return hashString[..32];
    }

    [GeneratedRegex(@"[\\\/\.\:\-]")]
    private static partial Regex MyRegex();
}