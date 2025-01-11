// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Spectralyzer.Shared.Core;

namespace Spectralyzer.Updater.Shared;

public sealed class ReleaseClient : IReleaseClient
{
    private readonly IHttpClient _httpClient;
    private readonly IOptions<ApplicationOptions> _options;

    public ReleaseClient(IOptions<ApplicationOptions> options, IHttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken)
    {
        return _httpClient.GetLatestReleaseAsync(cancellationToken);
    }

    public Task<ReleaseStream> GetReleaseInstallerAsync(Release release, CancellationToken cancellationToken)
    {
        return _httpClient.GetReleaseAsync(release, cancellationToken);
    }

    public async Task<bool> IsReleaseAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var release = await GetLatestReleaseAsync(cancellationToken);
            return _options.Value.Version.IsUpdateAvailable(release.Version);
        }
        catch (Exception)
        {
            return false;
        }
    }
}