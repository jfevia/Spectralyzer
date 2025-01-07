// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Shared.UI;

public interface IExceptionHandler
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}