// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class ResponseDetailsViewModel : ObservableObject
{
    private long? _contentLength;
    private TimeSpan? _elapsed;
    private int? _statusCode;
    private string? _statusDescription;

    public long? ContentLength
    {
        get => _contentLength;
        set => SetProperty(ref _contentLength, value);
    }

    public TimeSpan? Elapsed
    {
        get => _elapsed;
        set => SetProperty(ref _elapsed, value);
    }

    public int? StatusCode
    {
        get => _statusCode;
        set => SetProperty(ref _statusCode, value);
    }

    public string? StatusDescription
    {
        get => _statusDescription;
        set => SetProperty(ref _statusDescription, value);
    }
}