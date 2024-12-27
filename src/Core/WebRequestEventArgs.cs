// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebRequestEventArgs : EventArgs
{
    public WebRequest WebRequest { get; }

    public WebRequestEventArgs(WebRequest webRequest)
    {
        WebRequest = webRequest ?? throw new ArgumentNullException(nameof(webRequest));
    }
}