// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommandLine;

namespace Spectralyzer.App.Host;

public sealed class AppOptions
{
    [Option('d', "Debug", Required = false, HelpText = "Launch the JIT debugger.")]
    public bool Debug { get; set; }
}