// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Updater.Core;

public sealed class DownloadReleaseProgressData
{
    public long DownloadedBytes { get; }
    public long TotalBytes { get; }

    public DownloadReleaseProgressData(long downloadedBytes, long totalBytes)
    {
        DownloadedBytes = downloadedBytes;
        TotalBytes = totalBytes;
    }
}