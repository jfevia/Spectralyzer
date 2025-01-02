// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;
using System.Windows;

namespace Spectralyzer.App.Host;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppHost? host = null;
        try
        {
            var builder = new AppHostBuilder();
            host = builder.Build();
            ContainerLocator.Set(() => host.Services);
            host.Start();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Fatal exception: {ex}");
            host?.Stop();
            throw;
        }

        base.OnStartup(e);
    }
}