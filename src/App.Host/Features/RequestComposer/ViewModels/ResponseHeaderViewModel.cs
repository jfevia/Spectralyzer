// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class ResponseHeaderViewModel : ObservableObject
{
    public string Key { get; }

    public string? Value { get; }

    public ResponseHeaderViewModel(string key, string? value)
    {
        Key = key;
        Value = value;
    }
}