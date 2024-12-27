// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebProxyServerFactory : IWebProxyServerFactory
{
    public IWebProxyServer Create()
    {
        return new WebProxyServer();
    }
}