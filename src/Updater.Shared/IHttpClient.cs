// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Updater.Shared;

public interface IHttpClient
{
    Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken);
    Task<ReleaseStream> GetReleaseAsync(Release release, CancellationToken cancellationToken);
}