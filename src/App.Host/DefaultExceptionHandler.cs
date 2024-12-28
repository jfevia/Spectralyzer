// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Spectralyzer.App.Host;

public sealed class DefaultExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DefaultExceptionHandler> _logger;

    public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
        return Task.CompletedTask;
    }

    private void Handle(Exception ex)
    {
        _logger.LogCritical(ex, "Unhandled exception");
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception exception) return;
        Handle(exception);
    }

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Handle(e.Exception);
    }
}