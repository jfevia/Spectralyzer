// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Spectralyzer.Shared.Core.Windows.FileSystem;

public static class ServiceCollectionExtensions
{
    public static void AddFileSystemClient(this IServiceCollection services)
    {
        services.TryAddTransient<IFileSystem, System.IO.Abstractions.FileSystem>();
        services.TryAddTransient<IFileSystemClient, FileSystemClient>();
    }
}