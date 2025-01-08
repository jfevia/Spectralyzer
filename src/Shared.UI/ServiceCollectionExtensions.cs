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

        services.AddApplication();
        services.AddDefaultExceptionHandler();
        services.AddContainerLocator();
    }

    private static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IApplication, Application>();
    }

    private static void AddContainerLocator(this IServiceCollection services)
    {
        services.AddHostedService<ContainerLocatorHostedService>();
    }

    private static void AddDefaultExceptionHandler(this IServiceCollection services)
    {
        services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();
        services.AddHostedService<ExceptionHandlerHostedService>();
    }
}