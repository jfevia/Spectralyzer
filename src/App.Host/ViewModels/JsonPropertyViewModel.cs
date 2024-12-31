// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonPropertyViewModel : JsonObjectViewModel
{
    private readonly JsonProperty _jsonProperty;
    private readonly Lazy<IEnumerable<JsonObjectViewModel>> _nodeFactory;

    public string Name => _jsonProperty.Name;
    public IEnumerable<JsonObjectViewModel> Nodes => _nodeFactory.Value;
    public JsonValueKind ValueKind => _jsonProperty.Value.ValueKind;

    public JsonPropertyViewModel(JsonProperty jsonProperty, Func<JsonProperty, IEnumerable<JsonObjectViewModel>> nodeFactory)
    {
        ArgumentNullException.ThrowIfNull(nodeFactory);

        _jsonProperty = jsonProperty;
        _nodeFactory = new Lazy<IEnumerable<JsonObjectViewModel>>(() => nodeFactory(_jsonProperty));
    }
}