// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public abstract class WebMessage
{
    public string? BodyAsString { get; }
    public IReadOnlyList<WebHeader> Headers { get; }
    public Guid Id { get; }

    protected WebMessage(Guid id, IReadOnlyList<WebHeader> headers, string? bodyAsString)
    {
        Id = id;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        BodyAsString = bodyAsString;
    }
}