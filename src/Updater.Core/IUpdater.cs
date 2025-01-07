// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Updater.Core;

public interface IUpdater
{
    Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken);
    Task<Stream> GetReleaseInstallerAsync(Release release, CancellationToken cancellationToken);
}