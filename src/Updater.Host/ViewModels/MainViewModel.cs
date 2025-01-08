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
using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;
using Spectralyzer.Updater.Core;

namespace Spectralyzer.Updater.Host.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IApplication _application;
    private readonly IFileSystem _fileSystem;
    private readonly IInstaller _installer;
    private readonly IReleaseClient _releaseClient;
    private readonly CancellationTokenSource _taskCancellationSource;
    private readonly TimeProvider _timeProvider;
    private bool _initialized;
    private double _progress;

    public ICommand CancelCommand { get; }
    public ICommand InitializeCommand { get; }

    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }

    public MainViewModel(IApplication application, IReleaseClient releaseClient, IFileSystem fileSystem, TimeProvider timeProvider, IInstaller installer)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _releaseClient = releaseClient ?? throw new ArgumentNullException(nameof(releaseClient));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _installer = installer ?? throw new ArgumentNullException(nameof(installer));

        _taskCancellationSource = new CancellationTokenSource();

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Cancel()
    {
        _taskCancellationSource.Cancel();
        _application.Shutdown();
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_taskCancellationSource.Token, cancellationToken);
        var currentVersion = Version.Parse("1.0.0");
        var latestRelease = await _releaseClient.GetLatestReleaseAsync(linkedTokenSource.Token);
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
            await using var releaseStream = await _releaseClient.GetReleaseInstallerAsync(latestRelease, linkedTokenSource.Token);
            while (true)
            {
                linkedTokenSource.Token.ThrowIfCancellationRequested();
                var chunkBytes = await releaseStream.ReadAsync(buffer, linkedTokenSource.Token).ConfigureAwait(false);
                if (chunkBytes == 0)
                {
                    break;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, chunkBytes), linkedTokenSource.Token).ConfigureAwait(false);
                bytesRead += chunkBytes;
                Progress = bytesRead / (double)releaseStream.Length;
            }
        }

        _installer.Install(filePath);
        _application.Shutdown();
    }
}