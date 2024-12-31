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
    private readonly Lazy<DocumentViewModel> _document;
    private readonly Lazy<DocumentViewModel> _httpDocument;
    private readonly Lazy<string> _httpView;
    private readonly WebMessage _webMessage;

    public DocumentViewModel Document => _document.Value;
    public IReadOnlyList<WebHeader> Headers => _webMessage.Headers;
    public DocumentViewModel HttpDocument => _httpDocument.Value;
    public Guid Id => _webMessage.Id;
    public Version Version => _webMessage.Version;

    protected WebMessageViewModel(WebMessage webMessage)
    {
        _webMessage = webMessage ?? throw new ArgumentNullException(nameof(webMessage));

        _httpView = new Lazy<string>(GetHttpView);
        _contentType = new Lazy<ContentType?>(GetContentType);
        _document = new Lazy<DocumentViewModel>(GetDocument);
        _httpDocument = new Lazy<DocumentViewModel>(GetHttpDocument);
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

    private static string? GetContent(string? mimeType, string? bodyAsString)
    {
        if (string.IsNullOrEmpty(mimeType) || bodyAsString is null)
        {
            return bodyAsString;
        }

        if (JsonMimeTypes.Contains(mimeType))
        {
            return GetContentAsJson();
        }

        if (XmlMimeTypes.Contains(mimeType))
        {
            return GetContentAsXml();
        }

        return bodyAsString;

        string? GetContentAsJson()
        {
            try
            {
                var jsonObject = JsonSerializer.Deserialize<object?>(bodyAsString);
                if (jsonObject is null)
                {
                    return null;
                }

                return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception)
            {
                return bodyAsString;
            }
        }

        string GetContentAsXml()
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(bodyAsString);

                using var stringWriter = new StringWriter();
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlDocument.WriteTo(xmlTextWriter);
                }

                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return bodyAsString;
            }
        }
    }

    private ContentType? GetContentType()
    {
        (bool IsMatch, ContentType? ContentType)? header = Headers.Select(FindContentType).FirstOrDefault(h => h.IsMatch);
        return header?.ContentType;
    }

    private string? GetDetectedHighlightDefinitionName()
    {
        if (string.IsNullOrEmpty(_contentType.Value?.MimeType))
        {
            return null;
        }

        if (XmlMimeTypes.Contains(_contentType.Value.MimeType))
        {
            return ".xml";
        }

        if (JsonMimeTypes.Contains(_contentType.Value.MimeType))
        {
            return ".json";
        }

        return ".http";
    }

    private DocumentViewModel GetDocument()
    {
        var highlightDefinitionName = GetDetectedHighlightDefinitionName();
        var content = GetContent(_contentType.Value?.MimeType, _webMessage.BodyAsString);
        return new DocumentViewModel(highlightDefinitionName, content);
    }

    private DocumentViewModel GetHttpDocument()
    {
        return new DocumentViewModel(".http", _httpView.Value);
    }

    private string GetHttpView()
    {
        var stringBuilder = new StringBuilder();

        OnGeneratingHttpViewHeaders(stringBuilder);
        OnGeneratingHttpViewBody(stringBuilder);

        return stringBuilder.ToString();
    }
}