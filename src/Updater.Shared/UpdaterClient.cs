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

internal sealed class UpdaterClient : IUpdaterClient
{
    private readonly IOptions<UpdaterOptions> _options;
    private readonly IReleaseClient _releaseClient;

    public UpdaterClient(IOptions<UpdaterOptions> options, IReleaseClient releaseClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _releaseClient = releaseClient ?? throw new ArgumentNullException(nameof(releaseClient));
    }

    public async Task<bool> IsUpdateAvailableAsync(CancellationToken cancellationToken)
    {
        return await _releaseClient.IsReleaseAvailableAsync(cancellationToken);
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