// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Shared.UI;

public sealed class Application : IApplication
{
    public void Shutdown()
    {
        System.Windows.Application.Current.Shutdown();
    }
}