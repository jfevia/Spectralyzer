// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Xml;
using MimeKit;
using Spectralyzer.Core;
using ContentType = MimeKit.ContentType;

namespace Spectralyzer.App.Host.ViewModels;

public abstract class WebMessageViewModel
{
    private static readonly HashSet<string> JsonMimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        MediaTypeNames.Application.Json,
        MediaTypeNames.Application.JsonPatch,
        MediaTypeNames.Application.JsonSequence,
        MediaTypeNames.Application.ProblemJson
    };

    private static readonly HashSet<string> XmlMimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Application.XmlDtd,
        MediaTypeNames.Application.XmlPatch,
        MediaTypeNames.Application.ProblemXml,
        MediaTypeNames.Application.Soap
    };

    private readonly Lazy<ContentType?> _contentType;
    private readonly Lazy<JsonDocument?> _jsonDocument;
    private readonly Lazy<IEnumerable<JsonDocumentViewModel>> _jsonElements;
    private readonly WebMessage _webMessage;
    private readonly Lazy<XmlDocument?> _xmlDocument;
    private readonly Lazy<IEnumerable<XmlDocumentViewModel>> _xmlElements;

    public string? BodyAsString => _webMessage.BodyAsString;
    public IReadOnlyList<WebHeader> Headers => _webMessage.Headers;
    public Guid Id => _webMessage.Id;
    public IEnumerable<JsonDocumentViewModel> JsonElements => _jsonElements.Value;
    public IEnumerable<XmlDocumentViewModel> XmlElements => _xmlElements.Value;

    protected WebMessageViewModel(WebMessage webMessage)
    {
        _webMessage = webMessage ?? throw new ArgumentNullException(nameof(webMessage));
        _jsonElements = new Lazy<IEnumerable<JsonDocumentViewModel>>(GetJsonElements);
        _xmlElements = new Lazy<IEnumerable<XmlDocumentViewModel>>(GetXmlElements);

        _contentType = new Lazy<ContentType?>(GetContentType);
        _jsonDocument = new Lazy<JsonDocument?>(GetJsonDocument);
        _xmlDocument = new Lazy<XmlDocument?>(GetXmlDocument);
    }

    private static JsonDocumentViewModel CreateJsonDocumentViewModel(JsonDocument jsonDocument)
    {
        return new JsonDocumentViewModel(jsonDocument);
    }

    private static XmlDocumentViewModel CreateXmlDocumentViewModel(XmlDocument xmlDocument)
    {
        return new XmlDocumentViewModel(xmlDocument);
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
        if (_contentType.Value?.MimeType is null || !JsonMimeTypes.Contains(_contentType.Value.MimeType) || string.IsNullOrEmpty(BodyAsString))
        {
            return null;
        }

        return JsonDocument.Parse(BodyAsString);
    }

    private IEnumerable<JsonDocumentViewModel> GetJsonElements()
    {
        if (_jsonDocument.Value is not null)
        {
            yield return CreateJsonDocumentViewModel(_jsonDocument.Value);
        }
    }

    private XmlDocument? GetXmlDocument()
    {
        if (_contentType.Value?.MimeType is null || !XmlMimeTypes.Contains(_contentType.Value.MimeType) || string.IsNullOrEmpty(BodyAsString))
        {
            return null;
        }

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(BodyAsString);
        return xmlDocument;
    }

    private IEnumerable<XmlDocumentViewModel> GetXmlElements()
    {
        if (_xmlDocument.Value is not null)
        {
            yield return CreateXmlDocumentViewModel(_xmlDocument.Value);
        }
    }
}