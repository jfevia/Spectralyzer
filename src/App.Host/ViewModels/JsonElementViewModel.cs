// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonElementViewModel : JsonObjectViewModel
{
    private readonly JsonElement _jsonElement;
    private readonly Lazy<IEnumerable<JsonObjectViewModel>> _valueFactory;

    public IEnumerable<JsonObjectViewModel> Value => _valueFactory.Value;
    public JsonValueKind ValueKind => _jsonElement.ValueKind;

    public JsonElementViewModel(JsonElement jsonElement, Func<JsonElement, IEnumerable<JsonObjectViewModel>> valueFactory)
    {
        _jsonElement = jsonElement;

        _valueFactory = new Lazy<IEnumerable<JsonObjectViewModel>>(() => valueFactory.Invoke(jsonElement));
    }
}