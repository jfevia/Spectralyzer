// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Spectralyzer.Shared.UI;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IApplication, Application>();
    }

    public static void AddContainerLocator(this IServiceCollection services)
    {
        services.AddHostedService<ContainerLocatorHostedService>();
    }

    public static void AddDefaultExceptionHandler(this IServiceCollection services)
    {
        services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();
        services.AddHostedService<ExceptionHandlerHostedService>();
    }
}