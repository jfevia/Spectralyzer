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
using Spectralyzer.Updater.Core.GitHub.Releases;
using Spectralyzer.Updater.Core.Windows.Installer;
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

            ctx.AddTransient<IReleaseClient, GitHubReleaseClient>();
            ctx.AddHttpClient<GitHubReleaseClient>("Default", httpClient => httpClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GitHubReleaseClient)}/1.0.0"));

            ctx.AddOptions<GitHubReleaseClientOptions>();
            ctx.Configure<GitHubReleaseClientOptions>(options => options.RepositoryUrl = "https://api.github.com/repos/jfevia/Spectralyzer");

            ctx.AddTransient<IInstaller, WindowsInstaller>();

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