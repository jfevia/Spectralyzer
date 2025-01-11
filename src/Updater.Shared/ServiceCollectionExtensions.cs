// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectralyzer.Updater.Shared.GitHub.Releases;

namespace Spectralyzer.Updater.Shared;

public static class ServiceCollectionExtensions
{
    public static void AddReleaseClient(this IServiceCollection services, IConfiguration gitHubClientConfiguration)
    {
        services.AddGitHubClient(gitHubClientConfiguration);

        services.TryAddTransient<IReleaseClient, ReleaseClient>();
    }

    public static void AddUpdaterClient(this IServiceCollection services, IConfiguration updaterClientConfiguration, IConfiguration gitHubClientConfiguration)
    {
        services.AddReleaseClient(gitHubClientConfiguration);

        services.AddOptions<UpdaterOptions>();
        services.Configure<UpdaterOptions>(updaterClientConfiguration);
        services.TryAddTransient<IUpdaterClient, UpdaterClient>();
    }

    private static void AddGitHubClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddTransient<IHttpClient, GitHubHttpClient>();
        services.AddHttpClient<GitHubHttpClient>("Default", httpClient => httpClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GitHubHttpClient)}/1.0.0"));

        services.AddOptions<GitHubClientOptions>();
        services.Configure<GitHubClientOptions>(configuration);
    }
}