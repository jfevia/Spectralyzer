// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Spectralyzer.Shared.Core.Diagnostics;

namespace Spectralyzer.Updater.Core.Windows.Installer;

public sealed class WindowsInstaller : IInstaller
{
    private readonly IProcess _process;

    public WindowsInstaller(IProcess process)
    {
        _process = process ?? throw new ArgumentNullException(nameof(process));
    }

    public void Install(string filePath)
    {
        _process.Start("msiexec.exe", $"/i \"{filePath}\"");
    }
}