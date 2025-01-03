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
            ctx.AddHttpClient<RequestComposerItem>("Default")
               .AddHttpMessageHandler<PerformanceHandler>();

            ctx.AddTransient<IWebProxyServerFactory, WebProxyServerFactory>();

            ctx.AddSingleton<MainViewModel>();
            ctx.AddSingleton<TrafficAnalyzerItem>();
            ctx.AddSingleton<RequestComposerItem>();

            ctx.AddTransient<IExceptionHandler, DefaultExceptionHandler>();

            ctx.AddHostedService<ContainerLocatorHostedService>();
            ctx.AddHostedService<ExceptionHandlerHostedService>();
            ctx.AddHostedService<MainWindowHostService>();
            ctx.AddHostedService<HighlightingDefinitionsHostedService>();
        });
        _builder.ConfigureLogging(ctx =>
        {
            ctx.ClearProviders();
            ctx.AddConsole();
            ctx.AddDebug();
            ctx.AddEventLog();
            ctx.SetMinimumLevel(LogLevel.Trace);
        });
#if ENVIRONMENT_DEVELOPMENT
        _builder.UseEnvironment(Environments.Development);
#elif ENVIRONMENT_STAGING
        _builder.UseEnvironment(Environments.Staging);
#elif ENVIRONMENT_PRODUCTION
        _builder.UseEnvironment(Environments.Production);
#endif
    }

    public AppHost Build()
    {
        var host = _builder.Build();
        return new AppHost(host);
    }
}