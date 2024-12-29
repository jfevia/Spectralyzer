// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebSessionViewModel : ObservableObject
{
    private WebResponseMessage? _responseMessage;

    public int Index { get; }

    public string Process { get; }

    public WebRequestMessage RequestMessage { get; }

    public WebResponseMessage? ResponseMessage
    {
        get => _responseMessage;
        set => SetProperty(ref _responseMessage, value);
    }

    public WebSessionViewModel(int index, Process process, WebRequestMessage requestMessage)
    {
        ArgumentNullException.ThrowIfNull(process);

        Index = index;
        Process = $"{process.ProcessName}:{process.Id}";
        RequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
    }
}