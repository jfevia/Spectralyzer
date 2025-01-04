using System.Diagnostics;
using CommandLine;

namespace Spectralyzer.WixGen;

internal class Program
{
    [Option('c', "Configuration", Required = true, HelpText = "The configuration.")]
    public string Configuration { get; set; } = null!;

    [Option('d', "Debug", Required = false, HelpText = "Launch the JIT debugger.")]
    public bool Debug { get; set; }

    [Option('o', "OutputDir", Required = true, HelpText = "The output directory to scan.")]
    public string OutputDirectory { get; set; } = null!;

    public static void Main(string[] args)
    {
        Parser.Default
              .ParseArguments<Program>(args)
              .WithParsed(options =>
              {
                  if (options.Debug)
                  {
                      Debugger.Launch();
                  }
                  
                  var outputDirectory = Path.GetFullPath(options.OutputDirectory);
                  Console.WriteLine($"Output directory: {outputDirectory}");

                  SourceGenerator.GenerateFolders(outputDirectory);
              });
    }
}