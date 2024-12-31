// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebResponseMessage : WebMessage
{
    public int StatusCode { get; }
    public string StatusDescription { get; }

    public WebResponseMessage(Guid id, int statusCode, string statusDescription, Version version, IReadOnlyList<WebHeader> headers, string? bodyAsString)
        : base(id, version, headers, bodyAsString)
    {
        StatusCode = statusCode;
        StatusDescription = statusDescription;
    }
}