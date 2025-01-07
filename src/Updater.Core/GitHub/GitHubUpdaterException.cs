// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Updater.Core.GitHub;

public sealed class GitHubUpdaterException : Exception
{
    public GitHubUpdaterException(string? message)
        : base(message)
    {
    }
}