// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectralyzer.Shared.Core;

namespace Spectralyzer.Shared.UI;

public sealed class ApplicationInstanceHostedService : IHostedService
{
    private readonly IOptions<ApplicationOptions> _options;

    public ApplicationInstanceHostedService(IOptions<ApplicationOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ApplicationInstance.ManufacturerName = _options.Value.ManufacturerName;
        ApplicationInstance.ProductName = _options.Value.ProductName;
        ApplicationInstance.ProductVersion = _options.Value.Version.ToString();
        ApplicationInstance.Environment = _options.Value.Environment;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}