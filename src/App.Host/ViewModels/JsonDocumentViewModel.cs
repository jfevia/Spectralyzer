// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonDocumentViewModel : JsonObjectViewModel
{
    private readonly JsonDocument _jsonDocument;
    private readonly Lazy<IEnumerable<JsonObjectViewModel>> _nodeFactory;

    public IEnumerable<JsonObjectViewModel> Nodes => _nodeFactory.Value;

    public JsonDocumentViewModel(JsonDocument jsonDocument)
    {
        _jsonDocument = jsonDocument ?? throw new ArgumentNullException(nameof(jsonDocument));
        _nodeFactory = new Lazy<IEnumerable<JsonObjectViewModel>>(GetNodes);
    }

    private IEnumerable<JsonObjectViewModel> GetNodes()
    {
        return JsonFactory.ParseObject(_jsonDocument.RootElement);
    }
}