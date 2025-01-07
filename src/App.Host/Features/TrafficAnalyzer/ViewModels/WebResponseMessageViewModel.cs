// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Text;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class WebResponseMessageViewModel : WebMessageViewModel<WebResponseMessage>
{
    private int? _statusCode;
    private string? _statusDescription;

    public int? StatusCode
    {
        get => _statusCode;
        private set => SetProperty(ref _statusCode, value);
    }

    public string? StatusDescription
    {
        get => _statusDescription;
        private set => SetProperty(ref _statusDescription, value);
    }

    public WebResponseMessageViewModel(Guid requestId)
        : base(requestId)
    {
    }

    public override void ProcessMessage(WebResponseMessage message)
    {
        base.ProcessMessage(message);
        StatusCode = message.StatusCode;
        StatusDescription = message.StatusDescription;
    }

    protected override void OnGeneratingHttpViewHeaders(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"HTTP/{Version} {StatusCode} {StatusDescription}");
        base.OnGeneratingHttpViewHeaders(stringBuilder);
    }
}