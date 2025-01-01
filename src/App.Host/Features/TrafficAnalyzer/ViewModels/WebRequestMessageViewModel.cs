// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class WebRequestMessageViewModel : WebMessageViewModel
{
    private readonly WebRequestMessage _webRequestMessage;

    public Uri RequestUri => _webRequestMessage.RequestUri;

    public WebRequestMessageViewModel(WebRequestMessage webRequestMessage)
        : base(webRequestMessage)
    {
        _webRequestMessage = webRequestMessage ?? throw new ArgumentNullException(nameof(webRequestMessage));
    }

    protected override void OnGeneratingHttpViewHeaders(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{_webRequestMessage.Method} {_webRequestMessage.RequestUri.PathAndQuery}");
        base.OnGeneratingHttpViewHeaders(stringBuilder);
    }
}