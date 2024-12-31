// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public static class JsonFactory
{
    public static IEnumerable<JsonElementViewModel> ParseArray(JsonElement jsonElement)
    {
        foreach (var childElement in jsonElement.EnumerateArray())
        {
            yield return new JsonElementViewModel(childElement, ParseValue);
        }
    }

    public static IEnumerable<JsonPropertyViewModel> ParseObject(JsonElement jsonElement)
    {
        foreach (var jsonProperty in jsonElement.EnumerateObject())
        {
            yield return new JsonPropertyViewModel(jsonProperty, property => ParseValue(property.Value));
        }
    }

    public static IEnumerable<JsonObjectViewModel> ParseValue(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.String:
                yield return new JsonPrimitiveViewModel(jsonElement.GetString());
                break;
            case JsonValueKind.Number:
                yield return new JsonPrimitiveViewModel(jsonElement.GetDouble());
                break;
            case JsonValueKind.True or JsonValueKind.False:
                yield return new JsonPrimitiveViewModel(jsonElement.GetBoolean());
                break;
            case JsonValueKind.Array:
                foreach (var element in ParseArray(jsonElement))
                {
                    yield return element;
                }

                break;
            case JsonValueKind.Object:
                foreach (var element in ParseObject(jsonElement))
                {
                    yield return element;
                }

                break;
            default:
                yield break;
        }
    }
}