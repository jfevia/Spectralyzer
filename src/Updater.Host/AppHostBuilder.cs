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
        _builder.ConfigureServices(services =>
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddTransient<IProcess, Process>();
            services.AddSingleton(TimeProvider.System);

            services.AddTransient<IReleaseClient, GitHubReleaseClient>();
            services.AddHttpClient<GitHubReleaseClient>("Default", httpClient => httpClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GitHubReleaseClient)}/1.0.0"));

            services.AddOptions<GitHubReleaseClientOptions>();
            services.Configure<GitHubReleaseClientOptions>(options => options.RepositoryUrl = "https://api.github.com/repos/jfevia/Spectralyzer");

            services.AddTransient<IInstaller, WindowsInstaller>();

            services.AddSingleton<MainViewModel>();

            services.AddApplication();
            services.AddDefaultExceptionHandler();
            services.AddContainerLocator();

            services.AddHostedService<MainWindowHostService>();
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