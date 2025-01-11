// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectralyzer.Shared.Core;

namespace Spectralyzer.Shared.UI;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddOptions<ApplicationOptions>();
        services.Configure<ApplicationOptions>(options =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyMetadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();
            var manufacturerNameAttribute = assemblyMetadataAttributes.FirstOrDefault(a => a.Key == "ManufacturerName");
            var productNameAttribute = assemblyMetadataAttributes.FirstOrDefault(a => a.Key == "ProductName");
            var productVersionAttribute = assemblyMetadataAttributes.FirstOrDefault(a => a.Key == "ProductVersion");
            var environmentAttribute = assemblyMetadataAttributes.FirstOrDefault(a => a.Key == "Environment");
            options.ManufacturerName = manufacturerNameAttribute?.Value!;
            options.ProductName = productNameAttribute?.Value!;
            options.Version = Version.Parse(productVersionAttribute?.Value!);
            options.Environment = environmentAttribute?.Value!;
        });
        services.TryAddTransient<IApplication, Application>();
        services.AddHostedService<ApplicationInstanceHostedService>();
        services.AddHostedService<ApplicationUserModelHostedService>();
    }

    public static void AddContainerLocator(this IServiceCollection services)
    {
        services.AddHostedService<ContainerLocatorHostedService>();
    }

    public static void AddDefaultExceptionHandler(this IServiceCollection services)
    {
        services.TryAddTransient<IExceptionHandler, DefaultExceptionHandler>();
        services.AddHostedService<ExceptionHandlerHostedService>();
    }
}