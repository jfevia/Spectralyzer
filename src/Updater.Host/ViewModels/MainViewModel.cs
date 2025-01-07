// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.Updater.Host.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IApplication _application;
    private bool _initialized;

    public ICommand CancelCommand { get; }
    public ICommand InitializeCommand { get; }

    public MainViewModel(IApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Cancel()
    {
        _application.Shutdown();
    }

    private Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return Task.CompletedTask;
        }

        _initialized = true;
        return Task.CompletedTask;
    }
}