// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text.Json;

namespace Spectralyzer.App.Host.ViewModels;

public static class JsonFactory
{
    public static IEnumerable<JsonObjectViewModel> ParseObject(JsonElement jsonElement)
    {
        return jsonElement.EnumerateObject().Select(CreateJsonPropertyViewModel);
    }

    private static JsonElementViewModel CreateJsonElementViewModel(JsonElement childElement)
    {
        return new JsonElementViewModel(childElement, ParseElement);
    }

    private static JsonPrimitiveViewModel CreateJsonPrimitiveViewModel(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => new JsonPrimitiveViewModel(jsonElement.GetString()),
            JsonValueKind.Number => new JsonPrimitiveViewModel(jsonElement.GetDouble()),
            JsonValueKind.True or JsonValueKind.False => new JsonPrimitiveViewModel(jsonElement.GetBoolean()),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonElement), jsonElement, null)
        };
    }

    private static JsonPropertyViewModel CreateJsonPropertyViewModel(JsonProperty jsonProperty)
    {
        return new JsonPropertyViewModel(jsonProperty, property => ParseElement(property.Value));
    }

    private static IEnumerable<JsonElementViewModel> ParseArray(JsonElement jsonElement)
    {
        return jsonElement.EnumerateArray().Select(CreateJsonElementViewModel);
    }

    private static IEnumerable<JsonObjectViewModel> ParseElement(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (var jsonObject in ParseArray(jsonElement))
                {
                    yield return jsonObject;
                }

                break;
            case JsonValueKind.Object:
                foreach (var jsonObject in ParseObject(jsonElement))
                {
                    yield return jsonObject;
                }

                break;
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True or JsonValueKind.False:
                yield return CreateJsonPrimitiveViewModel(jsonElement);
                break;
            default:
                yield break;
        }
    }
}