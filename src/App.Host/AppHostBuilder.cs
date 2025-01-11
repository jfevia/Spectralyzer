// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectralyzer.App.Host.Features.About.ViewModels;
using Spectralyzer.App.Host.Features.RequestComposer.ViewModels;
using Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core;
using Spectralyzer.Core.Http;
using Spectralyzer.Shared.Core.Windows.FileSystem;
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

            services.AddSingleton<SettingsViewModel>();

            services.AddUpdaterClient(ctx.Configuration.GetRequiredSection("Updater"), ctx.Configuration.GetRequiredSection("GitHubClient"));

            services.AddFileSystemClient();

            services.AddApplication();
            services.AddDefaultExceptionHandler();
            services.AddContainerLocator();

            services.AddHostedService<MainWindowHostService>();

            services.AddHostedService<EnvironmentHostedService>();
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