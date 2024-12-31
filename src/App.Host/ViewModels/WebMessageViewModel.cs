// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO;
using System.Text;
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
        MediaTypeNames.Application.JsonLd,
        MediaTypeNames.Application.JsonPatch,
        MediaTypeNames.Application.JsonApi,
        MediaTypeNames.Application.JsonSequence,
        MediaTypeNames.Application.ProblemJson,
        MediaTypeNames.Application.JsonMergePatch,
        MediaTypeNames.Application.JsonSiren,
        MediaTypeNames.Application.JsonCollection
    };

    private static readonly HashSet<string> XmlMimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Application.Rss,
        MediaTypeNames.Application.Atom,
        MediaTypeNames.Application.MathML,
        MediaTypeNames.Application.Smil,
        MediaTypeNames.Application.Xaml,
        MediaTypeNames.Application.Soap,
        MediaTypeNames.Application.Wsdl,
        MediaTypeNames.Application.Xslt,
        MediaTypeNames.Application.XmlDtd,
        MediaTypeNames.Application.XmlPatch,
        MediaTypeNames.Application.ProblemXml,
        MediaTypeNames.Text.Xml,
        MediaTypeNames.Text.Html,
        MediaTypeNames.Image.Svg,
        MediaTypeNames.Document.OpenXmlWord,
        MediaTypeNames.Document.OpenXmlSpreadsheet,
        MediaTypeNames.Document.OpenXmlPresentation,
        MediaTypeNames.Document.OpenDocumentText,
        MediaTypeNames.Document.OpenDocumentSpreadsheet,
        MediaTypeNames.Ebook.EPUB
    };

    private readonly Lazy<ContentType?> _contentType;
    private readonly Lazy<string> _httpView;
    private readonly Lazy<IEnumerable<JsonDocumentViewModel>> _jsonElements;
    private readonly WebMessage _webMessage;
    private readonly Lazy<IEnumerable<XmlDocumentViewModel>> _xmlElements;
    public IReadOnlyList<WebHeader> Headers => _webMessage.Headers;

    public string HttpView => _httpView.Value;
    public Guid Id => _webMessage.Id;
    public IEnumerable<JsonDocumentViewModel> JsonElements => _jsonElements.Value;
    public Version Version => _webMessage.Version;
    public IEnumerable<XmlDocumentViewModel> XmlElements => _xmlElements.Value;

    protected WebMessageViewModel(WebMessage webMessage)
    {
        _webMessage = webMessage ?? throw new ArgumentNullException(nameof(webMessage));

        _httpView = new Lazy<string>(GetHttpView);
        _contentType = new Lazy<ContentType?>(GetContentType);
        _jsonElements = new Lazy<IEnumerable<JsonDocumentViewModel>>(GetJsonElements);
        _xmlElements = new Lazy<IEnumerable<XmlDocumentViewModel>>(GetXmlElements);
    }

    protected virtual void OnGeneratingHttpViewBody(StringBuilder stringBuilder)
    {
        if (string.IsNullOrEmpty(_webMessage.BodyAsString))
        {
            return;
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine(_webMessage.BodyAsString);
    }

    protected virtual void OnGeneratingHttpViewHeaders(StringBuilder stringBuilder)
    {
        foreach (var header in _webMessage.Headers.OrderBy(header => header.Key))
        {
            foreach (var headerValue in header.Values)
            {
                stringBuilder.AppendLine($"{header.Key}: {headerValue}");
            }
        }
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

    private string GetHttpView()
    {
        var stringBuilder = new StringBuilder();

        OnGeneratingHttpViewHeaders(stringBuilder);
        OnGeneratingHttpViewBody(stringBuilder);

        return stringBuilder.ToString();
    }

    private IEnumerable<JsonDocumentViewModel> GetJsonElements()
    {
        if (_contentType.Value?.MimeType is null || !JsonMimeTypes.Contains(_contentType.Value.MimeType) || string.IsNullOrEmpty(_webMessage.BodyAsString))
        {
            yield break;
        }

        JsonDocumentViewModel? jsonDocumentViewModel = null;
        try
        {
            var jsonDocument = JsonDocument.Parse(_webMessage.BodyAsString);
            jsonDocumentViewModel = CreateJsonDocumentViewModel(jsonDocument);
        }
        catch (Exception)
        {
            // Nothing
        }

        if (jsonDocumentViewModel is not null)
        {
            yield return jsonDocumentViewModel;
        }
    }

    private IEnumerable<XmlDocumentViewModel> GetXmlElements()
    {
        if (_contentType.Value?.MimeType is null || !XmlMimeTypes.Contains(_contentType.Value.MimeType) || string.IsNullOrEmpty(_webMessage.BodyAsString))
        {
            yield break;
        }

        XmlDocumentViewModel? xmlDocumentViewModel = null;
        try
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_webMessage.BodyAsString);
            xmlDocumentViewModel = CreateXmlDocumentViewModel(xmlDocument);
        }
        catch (Exception)
        {
            // Nothing
        }

        if (xmlDocumentViewModel is not null)
        {
            yield return xmlDocumentViewModel;
        }
    }
}