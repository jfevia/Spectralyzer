// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Extensions.Hosting;

namespace Spectralyzer.App.Host;

public sealed class AppHost
{
    private readonly IHost _host;

    public IServiceProvider Services => _host.Services;

    public AppHost(IHost host)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
    }

    public void Start()
    {
        _host.Start();
    }

    public void Stop()
    {
        _host.StopAsync().GetAwaiter().GetResult();
    }
}