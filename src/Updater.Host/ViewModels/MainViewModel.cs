// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.Shared.Core.Windows.FileSystem;
using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;
using Spectralyzer.Updater.Core;
using Spectralyzer.Updater.Shared;

namespace Spectralyzer.Updater.Host.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IApplication _application;
    private readonly IFileSystemClient _fileSystemClient;
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

    public MainViewModel(
        IApplication application,
        IReleaseClient releaseClient,
        IFileSystemClient fileSystemClient,
        TimeProvider timeProvider,
        IInstaller installer)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _releaseClient = releaseClient ?? throw new ArgumentNullException(nameof(releaseClient));
        _fileSystemClient = fileSystemClient ?? throw new ArgumentNullException(nameof(fileSystemClient));
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

        if (!await _releaseClient.IsReleaseAvailableAsync(linkedTokenSource.Token))
        {
            return;
        }

        var latestRelease = await _releaseClient.GetLatestReleaseAsync(linkedTokenSource.Token);
        var directory = _fileSystemClient.GetTempPath();
        directory.Create();

        var utcNow = _timeProvider.GetUtcNow();
        var releaseExtension = _fileSystemClient.GetExtension(latestRelease.Url);
        var filePath = _fileSystemClient.Combine(directory, $"Release-{latestRelease.Version}-{utcNow:yyyy-MM-dd_HH-mm-ss}.{releaseExtension.TrimStart('.')}");

        try
        {
            await using (var fileStream = _fileSystemClient.CreateFileStream(filePath, FileMode.Create, FileAccess.Write))
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
        }
        catch (Exception)
        {
            try
            {
                _fileSystemClient.DeleteFile(filePath);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        _application.Shutdown();
    }
}