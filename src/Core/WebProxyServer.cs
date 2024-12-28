// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Spectralyzer.Core;

public sealed class WebProxyServer : IWebProxyServer
{
    public event EventHandler<ExceptionEventArgs>? Error;

    public event EventHandler<WebResponseEventArgs>? ResponseReceived;

    public event EventHandler<WebRequestEventArgs>? SendingRequest;
    private readonly ProxyServer _server = new();

    public WebProxyEndpoint AddEndpoint(IPAddress ipAddress, int port, bool decryptSsl)
    {
        var explicitProxyEndPoint = new ExplicitProxyEndPoint(ipAddress, port, decryptSsl);
        var webProxyEndpoint = new WebProxyEndpoint(explicitProxyEndPoint);
        _server.AddEndPoint(explicitProxyEndPoint);
        return webProxyEndpoint;
    }

    public void RemoveEndpoint(WebProxyEndpoint endpoint)
    {
        _server.RemoveEndPoint(endpoint.Object);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _server.BeforeRequest += OnBeforeRequestAsync;
        _server.BeforeResponse += OnAfterResponseAsync;
        _server.ExceptionFunc = OnException;
        await _server.StartAsync(false, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _server.BeforeRequest -= OnBeforeRequestAsync;
        _server.BeforeResponse -= OnAfterResponseAsync;
        _server.ExceptionFunc = null;
        _server.Stop();
        return Task.CompletedTask;
    }

    private void OnException(Exception exception)
    {
        Error?.Invoke(this, new ExceptionEventArgs(exception));
    }

    private async Task OnAfterResponseAsync(object sender, SessionEventArgs e)
    {
        if (e.UserData is not UserData userData)
        {
            return;
        }

        var httpClient = e.HttpClient;
        var response = httpClient.Response;
        string? bodyString = null;

        if (response.HasBody)
        {
            bodyString = await e.GetResponseBodyAsString();
        }

        var webResponse = new WebResponse(
            userData.Id,
            (HttpStatusCode)response.StatusCode,
            response.HttpVersion,
            response.Headers
                    .GetAllHeaders()
                    .GroupBy(s => s.Name)
                    .Select(s => new WebHeader(s.Key, s.Select(h => h.Value).ToList()))
                    .ToList(),
            bodyString);
        ResponseReceived?.Invoke(this, new WebResponseEventArgs(webResponse));
    }

    private async Task OnBeforeRequestAsync(object sender, SessionEventArgs e)
    {
        var userData = new UserData(Guid.NewGuid());
        e.UserData = userData;

        var httpClient = e.HttpClient;
        var request = httpClient.Request;
        string? bodyString = null;

        if (request.HasBody)
        {
            bodyString = await e.GetRequestBodyAsString();
        }

        var webRequest = new WebRequest(
            userData.Id,
            request.Method,
            request.RequestUri,
            request.HttpVersion,
            request.Headers
                   .GetAllHeaders()
                   .GroupBy(s => s.Name)
                   .Select(s => new WebHeader(s.Key, s.Select(x => x.Value).ToList()))
                   .ToList(),
            bodyString,
            httpClient.ProcessId.Value);
        SendingRequest?.Invoke(this, new WebRequestEventArgs(webRequest));
    }

    private sealed class UserData
    {
        public Guid Id { get; }

        public UserData(Guid id)
        {
            Id = id;
        }
    }
}