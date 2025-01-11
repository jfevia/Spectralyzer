// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using JetBrains.Annotations;

namespace Spectralyzer.Updater.Shared;

[PublicAPI]
public sealed class UpdaterOptions
{
    public bool Debug { get; set; }
    public string HostFilePath { get; set; } = null!;
    public string WorkingDirectory { get; set; } = null!;
}