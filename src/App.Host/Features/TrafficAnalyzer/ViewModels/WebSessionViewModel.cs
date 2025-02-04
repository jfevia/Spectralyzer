﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class WebSessionViewModel : ObservableObject
{
    public int Index { get; }
    public string Process { get; }
    public WebRequestMessageViewModel RequestMessage { get; }
    public WebResponseMessageViewModel ResponseMessage { get; }

    public WebSessionViewModel(int index, Process process, WebRequestMessageViewModel requestMessage)
    {
        ArgumentNullException.ThrowIfNull(process);

        Index = index;
        Process = $"{process.ProcessName}:{process.Id}";
        RequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
        
        ResponseMessage = new WebResponseMessageViewModel(requestMessage.RequestId);
    }
}