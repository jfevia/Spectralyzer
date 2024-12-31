﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebResponseMessageViewModel : WebMessageViewModel
{
    private readonly WebResponseMessage _webResponseMessage;

    public int StatusCode => _webResponseMessage.StatusCode;
    public string StatusDescription => _webResponseMessage.StatusDescription;

    public WebResponseMessageViewModel(WebResponseMessage webResponseMessage)
        : base(webResponseMessage)
    {
        _webResponseMessage = webResponseMessage ?? throw new ArgumentNullException(nameof(webResponseMessage));
    }

    protected override void OnGeneratingHttpView(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"HTTP/{Version} {StatusCode} {StatusDescription}");
        base.OnGeneratingHttpView(stringBuilder);
    }
}