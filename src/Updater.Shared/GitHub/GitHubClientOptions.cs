// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using JetBrains.Annotations;

namespace Spectralyzer.Updater.Shared.GitHub;

[PublicAPI]
public sealed class GitHubClientOptions
{
    public string RepositoryUrl { get; set; } = null!;
}