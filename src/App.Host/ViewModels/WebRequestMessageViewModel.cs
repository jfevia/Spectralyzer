// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebRequestMessageViewModel
{
    private readonly Lazy<IEnumerable<JsonDocumentViewModel>> _jsonElements;
    private readonly WebRequestMessage _webRequestMessage;

    public string? BodyAsString => _webRequestMessage.BodyAsString;
    public IReadOnlyList<WebHeader> Headers => _webRequestMessage.Headers;
    public Guid Id => _webRequestMessage.Id;
    public IEnumerable<JsonDocumentViewModel> JsonElements => _jsonElements.Value;
    public Uri RequestUri => _webRequestMessage.RequestUri;

    public WebRequestMessageViewModel(WebRequestMessage webRequestMessage)
    {
        _webRequestMessage = webRequestMessage ?? throw new ArgumentNullException(nameof(webRequestMessage));
        _jsonElements = new Lazy<IEnumerable<JsonDocumentViewModel>>(GetJsonElements);
    }

    private static JsonDocumentViewModel CreateJsonDocumentViewModel(JsonDocument jsonDocument)
    {
        return new JsonDocumentViewModel(jsonDocument);
    }

    private IEnumerable<JsonDocumentViewModel> GetJsonElements()
    {
        if (_webRequestMessage.BodyAsJson is not null)
        {
            yield return CreateJsonDocumentViewModel(_webRequestMessage.BodyAsJson);
        }
    }
}