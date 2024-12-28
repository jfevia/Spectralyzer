// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public sealed class WebRequest
{
    public string? BodyAsString { get; }

    public IReadOnlyList<WebHeader> Headers { get; }

    public Guid Id { get; }

    public string? Method { get; }

    public int ProcessId { get; }

    public Uri RequestUri { get; }

    public Version Version { get; }

    public WebRequest(
        Guid id,
        string? method,
        Uri requestUri,
        Version version,
        IReadOnlyList<WebHeader> headers,
        string? bodyAsString,
        int processId)
    {
        Id = id;
        Method = method;
        RequestUri = requestUri;
        Version = version;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        BodyAsString = bodyAsString;
        ProcessId = processId;
    }
}