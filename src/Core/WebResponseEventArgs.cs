// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebResponseEventArgs : EventArgs
{
    public WebResponse WebResponse { get; }

    public WebResponseEventArgs(WebResponse webResponse)
    {
        WebResponse = webResponse ?? throw new ArgumentNullException(nameof(webResponse));
    }
}