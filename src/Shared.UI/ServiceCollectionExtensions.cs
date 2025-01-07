// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Spectralyzer.Shared.UI;

public static class ServiceCollectionExtensions
{
    public static void AddSharedUI(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IApplication, Application>();

        services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();
        services.AddHostedService<ContainerLocatorHostedService>();
        services.AddHostedService<ExceptionHandlerHostedService>();
    }
}