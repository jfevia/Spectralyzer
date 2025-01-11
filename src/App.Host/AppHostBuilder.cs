// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectralyzer.App.Host.Features.RequestComposer.ViewModels;
using Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core;
using Spectralyzer.Core.Http;
using Spectralyzer.Shared.UI;
using Spectralyzer.Updater.Shared;

namespace Spectralyzer.App.Host;

public sealed class AppHostBuilder
{
    private readonly IHostBuilder _builder;

    public AppHostBuilder()
    {
        _builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
        _builder.ConfigureServices((ctx, services) =>
        {
            services.AddTransient<PerformanceHandler>();
            services.AddHttpClient<HttpRequestComposerItem>("Default")
                    .AddHttpMessageHandler<PerformanceHandler>();

            services.AddTransient<IWebProxyServerFactory, WebProxyServerFactory>();

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<TrafficAnalyzerItem>();
            services.AddSingleton<HttpRequestComposerItem>();

            services.AddUpdaterClient(ctx.Configuration.GetRequiredSection("Updater"));

            services.AddApplication();
            services.AddDefaultExceptionHandler();
            services.AddContainerLocator();

            services.AddHostedService<MainWindowHostService>();
        });
        _builder.ConfigureLogging(LogLevel.Trace);
        _builder.ConfigureEnvironment();

        var appContextDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        Environment.SetEnvironmentVariable("UpdaterDir", Path.Combine(appContextDirectory.Parent!.FullName, "Updater"), EnvironmentVariableTarget.Process);
    }

    public AppHost Build()
    {
        var host = _builder.Build();
        return new AppHost(host);
    }
}