// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.Shared.Core.Diagnostics;
using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;
using Spectralyzer.Updater.Core;

namespace Spectralyzer.Updater.Host.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IApplication _application;
    private readonly IFileSystem _fileSystem;
    private readonly IProcess _process;
    private readonly TimeProvider _timeProvider;
    private readonly IUpdater _updater;
    private bool _initialized;
    private double _progress;

    public ICommand CancelCommand { get; }
    public ICommand InitializeCommand { get; }

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public MainViewModel(IApplication application, IUpdater updater, IFileSystem fileSystem, TimeProvider timeProvider, IProcess process)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _updater = updater ?? throw new ArgumentNullException(nameof(updater));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _process = process ?? throw new ArgumentNullException(nameof(process));

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Cancel()
    {
        _application.Shutdown();
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        var currentVersion = Version.Parse("1.0.0");
        var latestRelease = await _updater.GetLatestReleaseAsync(cancellationToken);
        var isUpdateAvailable = latestRelease.Version >= currentVersion;

        if (!isUpdateAvailable)
        {
            return;
        }

        var directory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "Spectralyzer");
        _fileSystem.Directory.CreateDirectory(directory);

        var utcNow = _timeProvider.GetUtcNow();
        var releaseExtension = _fileSystem.Path.GetExtension(latestRelease.Url);
        var filePath = _fileSystem.Path.Combine(directory, $"Release-{latestRelease.Version}-{utcNow:yyyy-MM-dd_HH-mm-ss}.{releaseExtension.TrimStart('.')}");
        await using (var fileStream = _fileSystem.FileStream.New(filePath, FileMode.Create, FileAccess.Write))
        {
            var buffer = new byte[8192];
            long bytesRead = 0;
            await using var releaseStream = await _updater.GetReleaseInstallerAsync(latestRelease, cancellationToken);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var chunkBytes = await releaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (chunkBytes == 0)
                {
                    break;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, chunkBytes), cancellationToken).ConfigureAwait(false);
                bytesRead += chunkBytes;
                Progress = bytesRead / (double)releaseStream.Length;
            }
        }

        _process.Start("msiexec.exe", $"/i \"{filePath}\"");
        _application.Shutdown();
    }
}