// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;

namespace Spectralyzer.Core;

public interface IWebProxyServer
{
    event EventHandler<ExceptionEventArgs> Error;
    event EventHandler<WebResponseEventArgs> ResponseReceived;

    event EventHandler<WebRequestEventArgs> SendingRequest;

    WebProxyEndpoint AddEndpoint(IPAddress ipAddress, int port, bool decryptSsl);
    void RemoveEndpoint(WebProxyEndpoint endpoint);
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}