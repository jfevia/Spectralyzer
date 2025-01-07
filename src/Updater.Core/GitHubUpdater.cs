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

namespace Spectralyzer.Updater.Core;

public class GitHubUpdater : IUpdater
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<GitHubUpdaterOptions> _options;

    public GitHubUpdater(IHttpClientFactory httpClientFactory, IOptions<GitHubUpdaterOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken)
    {
        var gitHubRelease = await GetReleaseAsync("latest", cancellationToken);
        return new Release(gitHubRelease.Id, Version.Parse(gitHubRelease.TagName.TrimStart('v')), gitHubRelease.Assets.First().BrowserDownloadUrl);
    }

    public async Task<ReleaseStream> GetReleaseInstallerAsync(Release release, CancellationToken cancellationToken)
    {
        var gitHubRelease = await GetReleaseAsync(release.Id.ToString(), cancellationToken);
        if (gitHubRelease.Assets is null || gitHubRelease.Assets.Length == 0)
        {
            throw new GitHubUpdaterException("No assets found for the specified release.");
        }

        var asset = gitHubRelease.Assets.First();
        var httpClient = _httpClientFactory.CreateClient("Assets");
        var assetResponse = await httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        assetResponse.EnsureSuccessStatusCode();

        var stream = await assetResponse.Content.ReadAsStreamAsync(cancellationToken);
        return new ReleaseStream(assetResponse.Content.Headers.ContentLength!.Value, stream);
    }

    private async Task<GitHubRelease> GetReleaseAsync(string releaseId, CancellationToken cancellationToken)
    {
        var url = $"{_options.Value.RepositoryUrl}/releases/{releaseId}";
        var httpClient = _httpClientFactory.CreateClient("Default");
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var gitHubRelease = JsonSerializer.Deserialize<GitHubRelease>(content, JsonSerializerOptions);
        if (gitHubRelease?.TagName is null)
        {
            throw new GitHubUpdaterException("Could not parse GitHub release.");
        }

        return gitHubRelease;
    }
}