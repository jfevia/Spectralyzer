// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Spectralyzer.Core;
using ContentType = MimeKit.ContentType;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public abstract class WebMessageViewModel
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
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

    private static TextDocument CreateTextDocument(string? bodyAsString)
    {
        return !string.IsNullOrEmpty(bodyAsString) ? new TextDocument(bodyAsString) : new TextDocument();
    }

    private static string? GetContent(string? mimeType, string? bodyAsString)
    {
        if (string.IsNullOrEmpty(mimeType) || string.IsNullOrEmpty(bodyAsString))
        {
            return bodyAsString;
        }

        if (KnownFormats.IsJson(mimeType))
        {
            return GetContentAsJson();
        }

        if (KnownFormats.IsXml(mimeType))
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

                return JsonSerializer.Serialize(jsonObject, JsonSerializerOptions);
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

    private static IHighlightingDefinition GetHighlightingDefinitionByFileExtension(string name)
    {
        return HighlightingManager.Instance.GetDefinitionByExtension(name);
    }

    private ContentType? GetContentType()
    {
        (bool IsMatch, ContentType? ContentType)? header = Headers.SelectMany(header => header.Values.Select(value => (header.Key, Value: value)))
                                                                  .Select(header => HeaderHelper.FindContentType(header.Key, header.Value))
                                                                  .FirstOrDefault(header => header.IsMatch);
        return header?.ContentType;
    }

    private string GetDetectedHighlightDefinitionName()
    {
        if (string.IsNullOrEmpty(_contentType.Value?.MimeType))
        {
            return FileExtensions.Http;
        }

        if (KnownFormats.IsJson(_contentType.Value.MimeType))
        {
            return FileExtensions.Json;
        }

        if (KnownFormats.IsXml(_contentType.Value.MimeType))
        {
            return FileExtensions.Xml;
        }

        return FileExtensions.Http;
    }

    private DocumentViewModel GetDocument()
    {
        var highlightDefinitionName = GetDetectedHighlightDefinitionName();
        var highlightingDefinition = GetHighlightingDefinitionByFileExtension(highlightDefinitionName);
        var content = GetContent(_contentType.Value?.MimeType, _webMessage.BodyAsString);
        var textDocument = CreateTextDocument(content);
        return new DocumentViewModel(highlightingDefinition, textDocument);
    }

    private DocumentViewModel GetHttpDocument()
    {
        var highlightingDefinition = GetHighlightingDefinitionByFileExtension(FileExtensions.Http);
        var textDocument = CreateTextDocument(_httpView.Value);
        return new DocumentViewModel(highlightingDefinition, textDocument);
    }

    private string GetHttpView()
    {
        var stringBuilder = new StringBuilder();

        OnGeneratingHttpViewHeaders(stringBuilder);
        OnGeneratingHttpViewBody(stringBuilder);

        return stringBuilder.ToString();
    }
}