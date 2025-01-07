// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Spectralyzer.Shared.UI;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ContainerLocator
{
    private static IServiceProvider? _current;
    private static Lazy<IServiceProvider>? _lazyContainer;

    public static IServiceProvider? Current => _current ??= _lazyContainer?.Value;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Reset()
    {
        _current = null;
        _lazyContainer = null;
    }

    public static void Set(Func<IServiceProvider> factory)
    {
        _lazyContainer = new Lazy<IServiceProvider>(factory);
    }
}