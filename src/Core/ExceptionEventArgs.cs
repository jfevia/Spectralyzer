// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Core;

public sealed class ExceptionEventArgs : EventArgs
{
    public Exception Exception { get; }

    public ExceptionEventArgs(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}