// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonElementViewModel : JsonObjectViewModel
{
    private readonly JsonElement _jsonElement;
    private readonly Lazy<IEnumerable<JsonObjectViewModel>> _nodeFactory;

    public IEnumerable<JsonObjectViewModel> Nodes => _nodeFactory.Value;
    public JsonValueKind ValueKind => _jsonElement.ValueKind;

    public JsonElementViewModel(JsonElement jsonElement, Func<JsonElement, IEnumerable<JsonObjectViewModel>> nodeFactory)
    {
        ArgumentNullException.ThrowIfNull(nodeFactory);

        _jsonElement = jsonElement;
        _nodeFactory = new Lazy<IEnumerable<JsonObjectViewModel>>(() => nodeFactory.Invoke(jsonElement));
    }
}