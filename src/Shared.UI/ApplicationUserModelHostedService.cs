// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectralyzer.Shared.Core;

namespace Spectralyzer.Shared.UI;

public sealed partial class ApplicationUserModelHostedService : IHostedService
{
    private readonly IOptions<ApplicationOptions> _options;

    public ApplicationUserModelHostedService(IOptions<ApplicationOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SetCurrentProcessExplicitAppUserModelID($"{_options.Value.ManufacturerName}.{_options.Value.ProductName}.{_options.Value.Version}.{_options.Value.Environment}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [LibraryImport("shell32.dll", SetLastError = true)]
    private static partial void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appId);
}