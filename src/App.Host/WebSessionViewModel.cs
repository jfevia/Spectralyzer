// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host;

public sealed class WebSessionViewModel : ObservableObject
{
    private WebResponse? _response;

    public int Index { get; }

    public string Process { get; }

    public WebRequest Request { get; }

    public WebResponse? Response
    {
        get => _response;
        set => SetProperty(ref _response, value);
    }

    public WebSessionViewModel(int index, Process process, WebRequest request)
    {
        ArgumentNullException.ThrowIfNull(process);

        Index = index;
        Process = $"{process.ProcessName}:{process.Id}";
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
}