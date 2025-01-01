// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestParameterViewModel : ObservableObject
{
    private string? _description;
    private string? _key;
    private string? _value;

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

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
}