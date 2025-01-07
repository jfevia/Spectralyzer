// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Spectralyzer.Core;

public sealed class WebHeader
{
    public string Key { get; }

    public IReadOnlyList<string> Values { get; }

    public WebHeader(string key, IReadOnlyList<string> values)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}