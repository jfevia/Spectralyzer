// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class WebSessionViewModel : ObservableObject
{
    private WebResponseMessageViewModel? _responseMessage;

    public int Index { get; }

    public string Process { get; }

    public WebRequestMessageViewModel RequestMessage { get; }

    public WebResponseMessageViewModel? ResponseMessage
    {
        get => _responseMessage;
        set => SetProperty(ref _responseMessage, value);
    }

    public WebSessionViewModel(int index, Process process, WebRequestMessageViewModel requestMessage)
    {
        ArgumentNullException.ThrowIfNull(process);

        Index = index;
        Process = $"{process.ProcessName}:{process.Id}";
        RequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
    }
}