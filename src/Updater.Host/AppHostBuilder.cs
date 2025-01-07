// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectralyzer.Shared.Core.Diagnostics;
using Spectralyzer.Shared.UI;
using Spectralyzer.Updater.Core;
using Spectralyzer.Updater.Core.GitHub;
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
            ctx.AddSingleton<IFileSystem, FileSystem>();
            ctx.AddTransient<IProcess, Process>();
            ctx.AddSingleton(TimeProvider.System);

            ctx.AddTransient<IUpdater, GitHubUpdater>();
            ctx.AddHttpClient<GitHubUpdater>("Default", httpClient => httpClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GitHubUpdater)}/1.0.0"));

            ctx.AddOptions<GitHubUpdaterOptions>();
            ctx.Configure<GitHubUpdaterOptions>(options => options.RepositoryUrl = "https://api.github.com/repos/jfevia/Spectralyzer");

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