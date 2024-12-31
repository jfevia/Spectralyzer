// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;
using MimeKit;
using HeaderId = MimeKit.HeaderId;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using MimeParser = MimeKit.MimeParser;

namespace Spectralyzer.Core;

public abstract class WebMessage
{
    private readonly Lazy<ContentType?> _contentType;
    private readonly Lazy<JsonDocument?> _jsonDocument;

    public JsonDocument? BodyAsJson => _jsonDocument.Value;
    public string? BodyAsString { get; }
    public IReadOnlyList<WebHeader> Headers { get; }
    public Guid Id { get; }

    protected WebMessage(Guid id, IReadOnlyList<WebHeader> headers, string? bodyAsString)
    {
        Id = id;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        BodyAsString = bodyAsString;

        _contentType = new Lazy<ContentType?>(GetContentType);
        _jsonDocument = new Lazy<JsonDocument?>(GetJsonDocument);
    }

    private static (bool IsMatch, ContentType? ContentType) FindContentType(WebHeader webHeader)
    {
        using var stream = new MemoryStream();
        using var textWriter = new StreamWriter(stream);
        textWriter.Write($"{webHeader.Key}: {webHeader.Values[0]}");
        textWriter.Flush();
        stream.Position = 0;

        var mimeParser = new MimeParser(stream, true);
        var headerList = mimeParser.ParseHeaders();

        var contentTypeHeader = headerList.FirstOrDefault(s => s.Id == HeaderId.ContentType);
        if (contentTypeHeader is null)
        {
            return (IsMatch: false, ContentType: null);
        }

        var contentType = ContentType.Parse(contentTypeHeader.Value);
        return (IsMatch: true, ContentType: contentType);
    }

    private ContentType? GetContentType()
    {
        (bool IsMatch, ContentType? ContentType)? header = Headers.Select(FindContentType).FirstOrDefault(h => h.IsMatch);
        return header?.ContentType;
    }

    private JsonDocument? GetJsonDocument()
    {
        if (!MediaTypeNames.Application.Json.Equals(_contentType.Value?.MimeType, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(BodyAsString))
        {
            return null;
        }

        return JsonDocument.Parse(BodyAsString);
    }
}