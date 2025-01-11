// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Options;

namespace Spectralyzer.Updater.Shared;

public sealed class UpdaterClient : IUpdaterClient
{
    private readonly IOptions<UpdaterOptions> _options;

    public UpdaterClient(IOptions<UpdaterOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new CommandLineOptions
        {
            Debug = _options.Value.Debug
        };
        var args = Parser.Default.FormatCommandLine(options);
        var startInfo = new ProcessStartInfo
        {
            Arguments = args,
            CreateNoWindow = true,
            FileName = Environment.ExpandEnvironmentVariables(_options.Value.HostFilePath),
            WorkingDirectory = Environment.ExpandEnvironmentVariables(_options.Value.WorkingDirectory),
            UseShellExecute = false
        };
        Process.Start(startInfo);

        return Task.CompletedTask;
    }
}