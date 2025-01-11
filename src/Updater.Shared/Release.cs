// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Updater.Shared;

public sealed class Release
{
    public long Id { get; }
    public string Url { get; }
    public Version Version { get; }

    public Release(long id, Version version, string url)
    {
        Id = id;
        Version = version;
        Url = url ?? throw new ArgumentNullException(nameof(url));
    }
}