// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Hosting;

namespace Spectralyzer.App.Host;

public sealed class ExceptionHandlerHostedService : IHostedService
{
    private readonly IExceptionHandler _exceptionHandler;

    public ExceptionHandlerHostedService(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _exceptionHandler.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _exceptionHandler.StopAsync(cancellationToken);
    }
}