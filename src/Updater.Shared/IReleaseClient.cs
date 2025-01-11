// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Updater.Shared;

public interface IReleaseClient
{
    Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken);
    Task<ReleaseStream> GetReleaseInstallerAsync(Release release, CancellationToken cancellationToken);
}