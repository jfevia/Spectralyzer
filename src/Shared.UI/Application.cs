// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Shared.UI;

public sealed class Application : IApplication
{
    public void Shutdown()
    {
        if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
        {
            System.Windows.Application.Current.Dispatcher.Invoke(Shutdown);
            return;
        }

        System.Windows.Application.Current.Shutdown();
    }
}