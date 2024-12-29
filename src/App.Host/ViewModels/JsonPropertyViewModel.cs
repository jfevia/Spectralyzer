// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonPropertyViewModel : JsonObjectViewModel
{
    private readonly JsonProperty _jsonProperty;
    private readonly Lazy<IEnumerable<JsonObjectViewModel>> _valueFactory;

    public string Name => _jsonProperty.Name;
    public IEnumerable<JsonObjectViewModel> Value => _valueFactory.Value;
    public JsonValueKind ValueKind => _jsonProperty.Value.ValueKind;

    public JsonPropertyViewModel(JsonProperty jsonProperty, Func<JsonProperty, IEnumerable<JsonObjectViewModel>> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        _jsonProperty = jsonProperty;
        _valueFactory = new Lazy<IEnumerable<JsonObjectViewModel>>(() => valueFactory(_jsonProperty));
    }
}