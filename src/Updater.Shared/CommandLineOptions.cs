// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommandLine;
using JetBrains.Annotations;

namespace Spectralyzer.Updater.Shared;

[PublicAPI]
public sealed class CommandLineOptions
{
    [Option('d', "Debug", Required = false, HelpText = "Launch the JIT debugger.")]
    public bool Debug { get; set; }
}