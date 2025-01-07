// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Spectralyzer.Shared.UI;

public static class HostBuilderExtensions
{
    public static void ConfigureEnvironment(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

#if ENVIRONMENT_DEVELOPMENT
        hostBuilder.UseEnvironment(Environments.Development);
#elif ENVIRONMENT_STAGING
        hostBuilder.UseEnvironment(Environments.Staging);
#elif ENVIRONMENT_PRODUCTION
        hostBuilder.UseEnvironment(Environments.Production);
#endif
    }

    public static void ConfigureLogging(this IHostBuilder hostBuilder, LogLevel logLevel)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        hostBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddEventLog();
            loggingBuilder.SetMinimumLevel(logLevel);
        });
    }
}