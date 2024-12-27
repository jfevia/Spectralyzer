// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;

namespace Spectralyzer.Core;

public sealed class WebResponse
{
    public string? BodyAsString { get; }

    public IReadOnlyList<WebHeader> Headers { get; }

    public HttpStatusCode HttpStatusCode { get; }

    public Guid Id { get; }

    public Version Version { get; }

    public WebResponse(
        Guid id,
        HttpStatusCode httpStatusCode,
        Version version,
        IReadOnlyList<WebHeader> headers,
        string? bodyAsString)
    {
        Id = id;
        HttpStatusCode = httpStatusCode;
        Version = version;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        BodyAsString = bodyAsString;
    }
}