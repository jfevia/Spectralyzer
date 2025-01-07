// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Updater.Core;

public interface IUpdater
{
    Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken);
    Task<ReleaseStream> GetReleaseInstallerAsync(Release release, CancellationToken cancellationToken);
}