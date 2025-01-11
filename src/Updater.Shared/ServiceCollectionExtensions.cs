// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectralyzer.Updater.Shared.GitHub.Releases;

namespace Spectralyzer.Updater.Shared;

public static class ServiceCollectionExtensions
{
    public static void AddReleaseClient(this IServiceCollection services)
    {
        services.AddTransient<IReleaseClient, GitHubReleaseClient>();
        services.AddHttpClient<GitHubReleaseClient>("Default", httpClient => httpClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GitHubReleaseClient)}/1.0.0"));

        services.AddOptions<GitHubReleaseClientOptions>();
        services.Configure<GitHubReleaseClientOptions>(options => options.RepositoryUrl = "https://api.github.com/repos/jfevia/Spectralyzer");
    }

    public static void AddUpdaterClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReleaseClient();

        services.AddOptions<UpdaterOptions>();
        services.Configure<UpdaterOptions>(configuration);
        services.AddTransient<IUpdaterClient, UpdaterClient>();
    }
}