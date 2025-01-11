// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Spectralyzer.Updater.Shared.GitHub;

internal class GitHubHttpClient : IHttpClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IOptions<GitHubClientOptions> _options;

    public GitHubHttpClient(IHttpClientFactory httpClientFactory, IOptions<GitHubClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _httpClient = httpClientFactory.CreateClient("Default");
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken)
    {
        var gitHubRelease = await GetReleaseAsync("latest", cancellationToken);
        return new Release(gitHubRelease.Id, Version.Parse(gitHubRelease.TagName.TrimStart('v')), gitHubRelease.Assets.First().BrowserDownloadUrl);
    }

    public async Task<ReleaseStream> GetReleaseAsync(Release release, CancellationToken cancellationToken)
    {
        var gitHubRelease = await GetReleaseAsync(release.Id.ToString(), cancellationToken);
        var gitHubReleaseAsset = gitHubRelease.Assets.First();
        var response = await _httpClient.GetAsync(gitHubReleaseAsset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return new ReleaseStream(stream, response.Content.Headers.ContentLength!.Value);
    }

    private async Task<GitHubRelease> GetReleaseAsync(string releaseId, CancellationToken cancellationToken)
    {
        var url = $"{_options.Value.RepositoryUrl}/releases/{releaseId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var gitHubRelease = JsonSerializer.Deserialize<GitHubRelease>(content, JsonSerializerOptions);
        if (gitHubRelease?.TagName is null)
        {
            throw new GitHubClientException("Could not parse GitHub release");
        }

        if (gitHubRelease.Assets is null || gitHubRelease.Assets.Length == 0)
        {
            throw new GitHubClientException("No assets found for the specified GitHub release");
        }

        return gitHubRelease;
    }
}