// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Spectralyzer.Shared.UI;

public sealed class ContainerLocatorHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ContainerLocatorHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ContainerLocator.Set(() => _serviceProvider);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ContainerLocator.Reset();
        return Task.CompletedTask;
    }
}