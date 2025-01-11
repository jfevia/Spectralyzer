// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Spectralyzer.App.Host;

public sealed class EnvironmentHostedService : IHostedService
{
    private readonly IFileSystem _fileSystem;

    public EnvironmentHostedService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var appContextDirectory = _fileSystem.DirectoryInfo.New(AppContext.BaseDirectory);
        Environment.SetEnvironmentVariable("UpdaterDir", Path.Combine(appContextDirectory.Parent!.FullName, "Updater"), EnvironmentVariableTarget.Process);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}