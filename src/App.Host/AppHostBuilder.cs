// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectralyzer.App.Host.Features.RequestComposer.ViewModels;
using Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core;
using Spectralyzer.Core.Http;
using Spectralyzer.Shared.UI;

namespace Spectralyzer.App.Host;

public sealed class AppHostBuilder
{
    private readonly IHostBuilder _builder;

    public AppHostBuilder()
    {
        _builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
        _builder.ConfigureServices(ctx =>
        {
            ctx.AddTransient<PerformanceHandler>();
            ctx.AddHttpClient<HttpRequestComposerItem>("Default")
               .AddHttpMessageHandler<PerformanceHandler>();

            ctx.AddTransient<IWebProxyServerFactory, WebProxyServerFactory>();

            ctx.AddSingleton<MainViewModel>();

            ctx.AddSingleton<TrafficAnalyzerItem>();
            ctx.AddSingleton<HttpRequestComposerItem>();

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