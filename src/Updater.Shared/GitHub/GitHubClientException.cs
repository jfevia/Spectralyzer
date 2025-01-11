// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Updater.Shared.GitHub;

public sealed class GitHubClientException : Exception
{
    public GitHubClientException(string? message)
        : base(message)
    {
    }
}