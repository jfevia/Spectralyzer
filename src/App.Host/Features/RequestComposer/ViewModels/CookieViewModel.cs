// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class CookieViewModel : ObservableObject
{
    private string? _domain;
    private DateTime? _expires;
    private bool _isHttpOnly;
    private bool _isSecure;
    private string? _key;
    private string? _path;
    private string? _value;

    public string? Domain
    {
        get => _domain;
        set => SetProperty(ref _domain, value);
    }

    public DateTime? Expires
    {
        get => _expires;
        set => SetProperty(ref _expires, value);
    }

    public bool IsHttpOnly
    {
        get => _isHttpOnly;
        set => SetProperty(ref _isHttpOnly, value);
    }

    public bool IsSecure
    {
        get => _isSecure;
        set => SetProperty(ref _isSecure, value);
    }

    public string? Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
    }

    public string? Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }

    public string? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}