// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

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

    [SupportedOSPlatform("windows")]
    public async Task<bool> IsUpdateAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Spectralyzer\Spectralyzer", true);
            var registryValue = registryKey?.GetValue("ProductVersion");
            if (registryValue is null)
            {
                return false;
            }

            if (!Version.TryParse(registryValue.ToString(), out var productVersion))
            {
                return false;
            }

            var release = await _releaseClient.GetLatestReleaseAsync(cancellationToken);
            return productVersion.IsUpdateAvailable(release.Version);
        }
        catch (Exception)
        {
            return false;
        }
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