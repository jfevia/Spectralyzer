// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpResponseHeaderViewModel : ObservableObject
{
    public string Key { get; }

    public string? Value { get; }

    public HttpResponseHeaderViewModel(string key, string? value)
    {
        Key = key;
        Value = value;
    }
}