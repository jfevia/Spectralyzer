﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Core;

public sealed class WebRequestEventArgs : EventArgs
{
    public WebRequestMessage WebRequestMessage { get; }

    public WebRequestEventArgs(WebRequestMessage webRequestMessage)
    {
        WebRequestMessage = webRequestMessage ?? throw new ArgumentNullException(nameof(webRequestMessage));
    }
}