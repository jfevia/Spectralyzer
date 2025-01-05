using System.Diagnostics;
using CommandLine;

namespace Spectralyzer.Installer.SourceGenerator;

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

                  var @namespace = typeof(Program).Namespace;
                  Console.WriteLine();
                  Console.WriteLine($"{@namespace} -> Configuration: {options.Configuration}");
                  Console.WriteLine($"{@namespace} -> OutputDirectory: {options.OutputDirectory}");

                  var outputDirectory = Path.GetFullPath(options.OutputDirectory);
                  Console.WriteLine($"{@namespace} -> Resolved absolute path for OutputDirectory: {outputDirectory}");

                  var stopwatch = Stopwatch.StartNew();
                  SourceGenerator.Generate(outputDirectory);
                  Console.WriteLine($"{@namespace} -> Generated sources in {stopwatch.ElapsedMilliseconds} milliseconds");
              });
    }
}