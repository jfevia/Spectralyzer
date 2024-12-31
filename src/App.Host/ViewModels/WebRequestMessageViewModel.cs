// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

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
}