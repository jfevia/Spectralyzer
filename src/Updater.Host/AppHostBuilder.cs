// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectralyzer.Shared.UI;
using Spectralyzer.Updater.Host.ViewModels;

namespace Spectralyzer.Updater.Host;

public sealed class AppHostBuilder
{
    private readonly IHostBuilder _builder;

    public AppHostBuilder()
    {
        _builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
        _builder.ConfigureServices(ctx =>
        {
            ctx.AddSingleton<MainViewModel>();

            ctx.AddSharedUI();

            ctx.AddHostedService<MainWindowHostService>();
        });
        _builder.ConfigureLogging(LogLevel.Trace);
        _builder.ConfigureEnvironment();
    }

    public AppHost Build()
    {
        var host = _builder.Build();
        return new AppHost(host);
    }
}