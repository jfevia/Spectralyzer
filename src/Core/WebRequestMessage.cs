// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Spectralyzer.Core;

public sealed class WebRequestMessage : WebMessage
{
    public string Method { get; }
    public int ProcessId { get; }
    public Uri RequestUri { get; }

    public WebRequestMessage(Guid id, string method, Uri requestUri, Version version, IReadOnlyList<WebHeader> headers, string? bodyAsString, int processId)
        : base(id, version, headers, bodyAsString)
    {
        Method = method;
        RequestUri = requestUri;
        ProcessId = processId;
    }
}