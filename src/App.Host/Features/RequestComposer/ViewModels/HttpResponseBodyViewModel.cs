// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;
using MimeKit;
using Spectralyzer.App.Host.Controllers;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpResponseBodyViewModel : HttpMessageBodyViewModel
{
    public override string Title => "Preview";

    protected override async Task InitializeEditorAsync(MonacoEditorController monacoEditorController, CancellationToken cancellationToken)
    {
        await monacoEditorController.SetIsReadOnlyAsync(true);
        await base.InitializeEditorAsync(monacoEditorController, cancellationToken);
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