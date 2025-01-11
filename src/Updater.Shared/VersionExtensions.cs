// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Updater.Shared;

public static class VersionExtensions
{
    public static bool IsUpdateAvailable(this Version currentVersion, Version newVersion)
    {
        return newVersion > currentVersion;
    }
}