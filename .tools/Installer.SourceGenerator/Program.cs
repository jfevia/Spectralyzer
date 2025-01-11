// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using CommandLine;

namespace Spectralyzer.Installer.SourceGenerator;

internal class Program
{
    [Option('c', "Configuration", Required = true, HelpText = "The configuration.")]
    public string Configuration { get; set; } = null!;

    [Option('d', "Debug", Required = false, HelpText = "Launch the JIT debugger.")]
    public bool Debug { get; set; }

    [Option('m', "Manufacturer", Required = true, HelpText = "The name of the manufacturer.")]
    public string ManufacturerName { get; set; } = null!;

    [Option('o', "OutputDir", Required = true, HelpText = "The output directory to scan.")]
    public string OutputDirectory { get; set; } = null!;

    [Option('p', "Product", Required = true, HelpText = "The name of the product.")]
    public string ProductName { get; set; } = null!;

    [Option('v', "Version", Required = true, HelpText = "The version.")]
    public string Version { get; set; } = null!;

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
                  Console.WriteLine($"{@namespace} -> ManufacturerName: {options.ManufacturerName}");
                  Console.WriteLine($"{@namespace} -> ProductName: {options.ProductName}");
                  Console.WriteLine($"{@namespace} -> Version: {options.Version}");

                  var outputDirectory = Path.GetFullPath(options.OutputDirectory);
                  Console.WriteLine($"{@namespace} -> Resolved absolute path for OutputDirectory: {outputDirectory}");

                  var stopwatch = Stopwatch.StartNew();
                  SourceGenerator.Generate(outputDirectory, options.ManufacturerName, options.ProductName, options.Version);
                  Console.WriteLine($"{@namespace} -> Generated sources in {stopwatch.ElapsedMilliseconds} milliseconds");
              });
    }
}