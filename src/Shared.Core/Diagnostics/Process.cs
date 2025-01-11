// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;

namespace Spectralyzer.Shared.Core.Diagnostics;

public sealed class Process : IProcess
{
    public void Start(string filePath, string arguments)
    {
        var processStartInfo = new ProcessStartInfo(filePath, arguments);
        System.Diagnostics.Process.Start(processStartInfo);
    }
}