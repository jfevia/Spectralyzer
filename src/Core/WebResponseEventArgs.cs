// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebResponseEventArgs : EventArgs
{
    public WebResponseMessage WebResponseMessage { get; }

    public WebResponseEventArgs(WebResponseMessage webResponseMessage)
    {
        WebResponseMessage = webResponseMessage ?? throw new ArgumentNullException(nameof(webResponseMessage));
    }
}