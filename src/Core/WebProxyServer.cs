// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Spectralyzer.Core;

public sealed class WebProxyServer : IWebProxyServer
{
    public event EventHandler<ExceptionEventArgs>? Error;
    public event EventHandler<WebRequestEventArgs>? Request;
    public event EventHandler<WebResponseEventArgs>? Response;

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

    public void ResetSystemProxy()
    {
        _server.DisableAllSystemProxies();
    }

    public void SetSystemProxy(WebProxyEndpoint endpoint)
    {
        _server.SetAsSystemProxy(endpoint.Object, ProxyProtocolType.AllHttp);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _server.BeforeRequest += OnRequestAsync;
        _server.BeforeResponse += OnResponseAsync;
        _server.ExceptionFunc = OnException;
        await _server.StartAsync(false, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _server.BeforeRequest -= OnRequestAsync;
        _server.BeforeResponse -= OnResponseAsync;
        _server.ExceptionFunc = null;
        _server.Stop();
        return Task.CompletedTask;
    }

    private void OnException(Exception exception)
    {
        Error?.Invoke(this, new ExceptionEventArgs(exception));
    }

    private async Task OnRequestAsync(object sender, SessionEventArgs e)
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

        var webRequestMessage = new WebRequestMessage(
            userData.Id,
            request.Method!,
            request.RequestUri,
            request.HttpVersion,
            request.Headers
                   .GetAllHeaders()
                   .GroupBy(header => header.Name)
                   .Select(header => new WebHeader(header.Key, header.Select(x => x.Value).ToList()))
                   .ToList(),
            bodyString,
            httpClient.ProcessId.Value);
        Request?.Invoke(this, new WebRequestEventArgs(webRequestMessage));
    }

    private async Task OnResponseAsync(object sender, SessionEventArgs e)
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

        var webResponseMessage = new WebResponseMessage(
            userData.Id,
            response.StatusCode,
            response.StatusDescription,
            response.HttpVersion,
            response.Headers
                    .GetAllHeaders()
                    .GroupBy(header => header.Name)
                    .Select(header => new WebHeader(header.Key, header.Select(x => x.Value).ToList()))
                    .ToList(),
            bodyString);
        Response?.Invoke(this, new WebResponseEventArgs(webResponseMessage));
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