// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host.ViewModels;

public sealed class JsonPrimitiveViewModel : JsonObjectViewModel
{
    public object? Value { get; }

    public JsonPrimitiveViewModel(object? value)
    {
        Value = value;
    }
}