// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Windows;
using Microsoft.Extensions.Hosting;
using Spectralyzer.App.Host.Views;
using Wpf.Ui.Controls;

namespace Spectralyzer.App.Host;

public class MainWindowHostService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (Application.Current.Windows.OfType<MainWindow>().Any())
        {
            return Task.CompletedTask;
        }

        var mainWindow = new MainWindow();
        mainWindow.Loaded += OnMainWindowLoaded;
        mainWindow.Show();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not MainWindow mainWindow)
        {
            return;
        }

        var navigationViewItem = mainWindow.NavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        if (navigationViewItem?.TargetPageType is null)
        {
            return;
        }

        _ = mainWindow.NavigationView.Navigate(navigationViewItem.TargetPageType);
    }
}