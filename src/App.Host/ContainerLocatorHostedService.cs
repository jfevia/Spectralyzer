// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Hosting;

namespace Spectralyzer.App.Host;

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