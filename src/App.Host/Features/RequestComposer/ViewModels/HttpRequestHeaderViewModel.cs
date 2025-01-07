// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpRequestHeaderViewModel : ObservableObject
{
    private string? _description;
    private string? _key;
    private string? _value;

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsReadOnly { get; }

    public string? Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
    }

    public string? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public HttpRequestHeaderViewModel(string? key = null, string? value = null, bool isReadOnly = false)
    {
        IsReadOnly = isReadOnly;
        _key = key;
        _value = value;
    }
}