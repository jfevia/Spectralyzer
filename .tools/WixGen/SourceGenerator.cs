// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Spectralyzer.WixGen;

public static partial class SourceGenerator
{
    public static void GenerateFolders(string outputDir)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<Wix xmlns=\"http://wixtoolset.org/schemas/v4/wxs\">");
        stringBuilder.AppendLine("<Fragment>");
        stringBuilder.AppendLine("<StandardDirectory Id=\"LocalAppDataFolder\">");
        stringBuilder.AppendLine("<Directory Id=\"ManufacturerFolder\" Name=\"Spectralyzer\">");
        stringBuilder.AppendLine("<Directory Id=\"ProductFolder\" Name=\"Spectralyzer\">\"");
        stringBuilder.AppendLine("<Directory Id=\"AppFolder\" Name=\"App\">\"");

        var stack = new Stack<(TreeNode node, bool isClosing, int depth)>();

        var rootTree = GetFolders(outputDir);

        stack.Push((rootTree, false, 0));

        while (stack.Count > 0)
        {
            var (currentNode, isClosing, depth) = stack.Pop();
            var indentation = new string(' ', depth * 2);

            if (isClosing)
            {
                stringBuilder.AppendLine($"{indentation}</Directory>");
            }
            else
            {
                var relativePath = GetRelativePath(outputDir, currentNode.Path);
                var directoryName = Path.GetFileName(currentNode.Path.TrimEnd(Path.DirectorySeparatorChar));
                var name = GetName(relativePath);

                stringBuilder.AppendLine($"{indentation}<Directory Id=\"Folder_{name}\" Name=\"{directoryName}\" />");
                
                // Push the closing tag for this node
                stack.Push((currentNode, true, depth));

                // Push all children (in reverse order to maintain the correct order in XML)
                for (var i = currentNode.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push((currentNode.Children[i], false, depth + 1));
                }
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

    private static TreeNode GetFolders(string directory)
    {
        var stack = new Stack<TreeNode>();
        var rootTree = new TreeNode(directory);
        stack.Push(rootTree);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();

            foreach (var subDirectory in Directory.GetDirectories(currentNode.Path))
            {
                var childNode = new TreeNode(subDirectory);
                currentNode.Children.Add(childNode);
                stack.Push(childNode);
            }
        }

        return rootTree;
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
    private static partial Regex MyRegex();

    public class TreeNode
    {
        public List<TreeNode> Children { get; } = [];
        public string Path { get; }

        public TreeNode(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }
}