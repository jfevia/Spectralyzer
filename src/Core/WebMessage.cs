// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public abstract class WebMessage
{
    public string? BodyAsString { get; }
    public IReadOnlyList<WebHeader> Headers { get; }
    public Guid Id { get; }
    public Version Version { get; }

    protected WebMessage(Guid id, Version version, IReadOnlyList<WebHeader> headers, string? bodyAsString)
    {
        Id = id;
        Version = version;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        BodyAsString = bodyAsString;
    }
}