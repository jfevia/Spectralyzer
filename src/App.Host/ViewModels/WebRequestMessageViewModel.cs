// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebRequestMessageViewModel : WebMessageViewModel
{
    private readonly WebRequestMessage _webRequestMessage;

    public Uri RequestUri => _webRequestMessage.RequestUri;

    public WebRequestMessageViewModel(WebRequestMessage webRequestMessage)
        : base(webRequestMessage)
    {
        _webRequestMessage = webRequestMessage ?? throw new ArgumentNullException(nameof(webRequestMessage));
    }

    protected override void OnGeneratingHttpView(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{_webRequestMessage.Method} {_webRequestMessage.RequestUri.PathAndQuery}");
        base.OnGeneratingHttpView(stringBuilder);
    }
}