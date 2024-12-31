// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;
using System.Text.Json;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebResponseMessageViewModel
{
    private readonly Lazy<IEnumerable<JsonDocumentViewModel>> _jsonElements;
    private readonly WebResponseMessage _webResponseMessage;

    public string? BodyAsString => _webResponseMessage.BodyAsString;
    public IReadOnlyList<WebHeader> Headers => _webResponseMessage.Headers;
    public HttpStatusCode HttpStatusCode => _webResponseMessage.HttpStatusCode;
    public IEnumerable<JsonDocumentViewModel> JsonElements => _jsonElements.Value;

    public WebResponseMessageViewModel(WebResponseMessage webResponseMessage)
    {
        _webResponseMessage = webResponseMessage ?? throw new ArgumentNullException(nameof(webResponseMessage));
        _jsonElements = new Lazy<IEnumerable<JsonDocumentViewModel>>(GetJsonElements);
    }

    private static JsonDocumentViewModel CreateJsonDocumentViewModel(JsonDocument bodyAsJson)
    {
        return new JsonDocumentViewModel(bodyAsJson);
    }

    private IEnumerable<JsonDocumentViewModel> GetJsonElements()
    {
        if (_webResponseMessage.BodyAsJson is not null)
        {
            yield return CreateJsonDocumentViewModel(_webResponseMessage.BodyAsJson);
        }
    }
}