// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;
using MimeKit;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class ResponseBodyViewModel : ResponseItemViewModel
{
    private static class Format
    {
        public const string Fallback = "None";
        public const string Json = "JSON";
        public const string Xml = "XML";
    }

    private string? _body;
    private string? _selectedFormat;

    public override string Title => "Preview";

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value);
    }

    public IEnumerable<string> Formats { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value);
    }

    public ResponseBodyViewModel()
    {
        Formats =
        [
            Format.Json,
            Format.Xml,
            Format.Fallback
        ];

        SelectedFormat = Formats.FirstOrDefault();
    }

    public async Task ProcessHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        Body = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        var result = GetContentType(httpResponseMessage);
        if (result.IsMatch)
        {
            SelectedFormat = GetSelectedFormat(result.ContentType);
        }
        else
        {
            SelectedFormat = Format.Fallback;
        }
    }

    private static (bool IsMatch, MimeKit.ContentType? ContentType) GetContentType(HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.Content.Headers.ContentType is null)
        {
            return (false, null);
        }

        return HeaderHelper.FindContentType(httpResponseMessage.Content.Headers.ContentType);
    }

    private static string GetSelectedFormat(ContentType? contentType)
    {
        if (contentType is null)
        {
            return Format.Fallback;
        }

        if (KnownFormats.IsJson(contentType.MimeType))
        {
            return Format.Json;
        }

        if (KnownFormats.IsXml(contentType.MimeType))
        {
            return Format.Xml;
        }

        return Format.Fallback;
    }
}