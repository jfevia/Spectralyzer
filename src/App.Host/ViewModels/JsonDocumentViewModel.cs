// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonDocumentViewModel : JsonObjectViewModel
{
    private readonly Lazy<IEnumerable<JsonPropertyViewModel>> _elements;
    private readonly JsonDocument _jsonDocument;

    public IEnumerable<JsonPropertyViewModel> Elements => _elements.Value;

    public JsonDocumentViewModel(JsonDocument jsonDocument)
    {
        _jsonDocument = jsonDocument ?? throw new ArgumentNullException(nameof(jsonDocument));
        _elements = new Lazy<IEnumerable<JsonPropertyViewModel>>(GetElements);
    }

    private IEnumerable<JsonPropertyViewModel> GetElements()
    {
        return JsonFactory.ParseObject(_jsonDocument.RootElement);
    }
}