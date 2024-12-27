// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.ComponentModel;

namespace Spectralyzer.App.Host;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ContainerLocator
{
    private static IServiceProvider? _current;
    private static Lazy<IServiceProvider>? _lazyContainer;

    public static IServiceProvider? Current => _current ??= _lazyContainer?.Value;

    public static void Set(Func<IServiceProvider> factory)
    {
        _lazyContainer = new Lazy<IServiceProvider>(factory);
    }
}