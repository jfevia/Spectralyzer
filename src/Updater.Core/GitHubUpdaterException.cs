// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Updater.Core;

public sealed class GitHubUpdaterException : Exception
{
    public GitHubUpdaterException(string? message)
        : base(message)
    {
    }
}