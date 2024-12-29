// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;

namespace Spectralyzer.Core;

public sealed class WebResponseMessage : WebMessage
{
    public HttpStatusCode HttpStatusCode { get; }

    public WebResponseMessage(Guid id, HttpStatusCode httpStatusCode, Version version, IReadOnlyList<WebHeader> headers, string? bodyAsString)
        : base(id, version, headers, bodyAsString)
    {
        HttpStatusCode = httpStatusCode;
    }
}