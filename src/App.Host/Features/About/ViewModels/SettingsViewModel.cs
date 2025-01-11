// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.Updater.Shared;

namespace Spectralyzer.App.Host.Features.About.ViewModels;

public sealed class SettingsViewModel
{
    private readonly IUpdaterClient _updaterClient;

    public ICommand UpdateCommand { get; }

    public SettingsViewModel(IUpdaterClient updaterClient)
    {
        _updaterClient = updaterClient ?? throw new ArgumentNullException(nameof(updaterClient));

        UpdateCommand = new AsyncRelayCommand(UpdateAsync);
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        if (await _updaterClient.IsUpdateAvailableAsync(cancellationToken))
        {
            await _updaterClient.StartAsync(cancellationToken);
        }
    }
}